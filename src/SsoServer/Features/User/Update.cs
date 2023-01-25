using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SsoServer.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SsoServer.Features.User
{
    public class Update
    {
        public class UpdateUserCommand : IRequest<UpdateUserCommandResponse>, IValidatableObject
        {
            [HiddenInput]
            public string Id { get; set; }

            /// <summary>
            ///     A unique email that the user uses to sign in with.
            /// </summary>
            [Required]
            [StringLength(255)]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     First name
            /// </summary>
            [Required]
            [StringLength(100)]
            public string FirstName { get; set; }

            /// <summary>
            ///     Last name
            /// </summary>
            [Required]
            [StringLength(100)]
            public string LastName { get; set; }

            /// <summary>
            ///     The Role that the user is being assigned.
            /// </summary>
            [Required]
            public string Role { get; set; }

            public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
            {

                var validationErrors = new List<ValidationResult>();

                if (!string.IsNullOrWhiteSpace(this.Email))
                {
                    var userManager = (UserManager<ApplicationUser>)validationContext.GetService(typeof(UserManager<ApplicationUser>));

                    ApplicationUser existingUser = userManager.FindByEmailAsync(this.Email).Result;

                    var emailAlreadyUsed = existingUser.Id != this.Id; // is there another user already using the email address

                    if (emailAlreadyUsed)
                        validationErrors.Add(new ValidationResult("The email address is already associated to a different user.  Please use a different email address."));
                }

                // Check role is valid
                if (!SsoServer.Constants.UserRoles.SupportedRoles.Contains(this.Role))
                {
                    validationErrors.Add(new ValidationResult($"{this.Role} is not a valid role."));
                }

                return validationErrors;
            }
        }

        public class UpdateUserCommandResponse
        {
            public bool NotFound { get; set; }
            public IdentityResult IdentityResult { get; set; }
        }

        public class CommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserCommandResponse>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public CommandHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            }

            public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
            {
                var response = new UpdateUserCommandResponse();

                var user = await _userManager.FindByIdAsync(command.Id);

                if (user == null)
                {
                    response.NotFound = true;
                    return response;
                }

                // Update the record in the identity db
                // We are auto-confirming email addresses regardless of account being created or edited
                user.UserName = command.Email;
                user.Email = command.Email;
                user.EmailConfirmed = true;

                IdentityResult result = await _userManager.UpdateAsync(user);
                response.IdentityResult = result;

                if (result.Succeeded)
                {
                    var currentClaims = await _userManager.GetClaimsAsync(user);
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    // We only support a user having one role 
                    string currentRole = (currentRoles != null) ? currentRoles[0] : "";

                    // update claims/roles in identity db
                    var firstNameClaim = currentClaims.FirstOrDefault(x => x.Type == IdentityModel.JwtClaimTypes.GivenName);
                    if (firstNameClaim == null)
                    {
                        await _userManager.AddClaimAsync(user, new Claim(IdentityModel.JwtClaimTypes.GivenName, command.FirstName));
                    }
                    else if (command.FirstName != firstNameClaim.Value)
                    {
                        await _userManager.ReplaceClaimAsync(user, firstNameClaim, new Claim(IdentityModel.JwtClaimTypes.GivenName, command.FirstName));
                    }

                    var lastNameClaim = currentClaims.FirstOrDefault(x => x.Type == IdentityModel.JwtClaimTypes.FamilyName);
                    if (lastNameClaim == null)
                    {
                        await _userManager.AddClaimAsync(user, new Claim(IdentityModel.JwtClaimTypes.FamilyName, command.LastName));
                    }
                    else if (command.LastName != lastNameClaim.Value)
                    {
                        await _userManager.ReplaceClaimAsync(user, lastNameClaim, new Claim(IdentityModel.JwtClaimTypes.FamilyName, command.LastName));
                    }

                    // Update user role if needed
                    if (!string.Equals(currentRole, command.Role, StringComparison.OrdinalIgnoreCase))
                    {
                        // Remove role from identity db
                        await _userManager.RemoveFromRoleAsync(user, currentRole);
                        // Add new role
                        await _userManager.AddToRoleAsync(user, command.Role);
                    }

                }

                return response;
            }
        }
    }
}
