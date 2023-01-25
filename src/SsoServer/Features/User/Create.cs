using MediatR;
using Microsoft.AspNetCore.Identity;
using SsoServer.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SsoServer.Features.User
{
    /// <summary>
    ///     Create related commands, validators and handlers
    /// </summary>
    public class Create
    {
        /// <summary>
        ///     Model to create a new User
        /// </summary>
        public class CreateUserCommand : IRequest<CreateUserResponse>, IValidatableObject
        {
            
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

            // Register model validation
            public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
            {
                var validationErrors = new List<ValidationResult>();
                // Add extra validation here if needed

                if (string.IsNullOrWhiteSpace(this.Role))
                {
                    validationErrors.Add(new ValidationResult("Role can not be empty."));
                }

                // Check role is valid
                if (!Constants.UserRoles.SupportedRoles.Contains(this.Role))
                {
                    validationErrors.Add(new ValidationResult($"{this.Role} is not a valid role."));
                }

                // Check if email is used by active user.
                // Note if the email is used by an inactive user, we will reactivate that user in the handler below.
                if (!string.IsNullOrWhiteSpace(this.Email))
                {
                    var userManager = (UserManager<ApplicationUser>)validationContext.GetService(typeof(UserManager<ApplicationUser>));

                    ApplicationUser existingUser = userManager.FindByEmailAsync(this.Email).Result;

                    if (existingUser != null && existingUser.IsEnabled)
                    {
                        validationErrors.Add(new ValidationResult($"Email {this.Email} is already being used by an active user."));
                    }
                }

                return validationErrors;
            }
        }

        /// <summary>
        ///     Command Response
        /// </summary>
        public class CreateUserResponse
        {
            /// <summary>
            ///     Item Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            ///     Identity result communicating with user store.
            /// </summary>
            public IdentityResult IdentityResult { get; set; }
        }

        /// <summary>
        ///     Handler
        /// </summary>
        public class CommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            // Resolve a dependency on email service in order to send emails, if necessary. Skipped here.
            //private readonly IEmailService _emailService;

            private readonly IMediator _mediator;

            public CommandHandler(UserManager<ApplicationUser> userManager,
                                  IMediator mediator)
            {
                this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
                this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            }

            /// <summary>
            ///     Handle
            /// </summary>
            /// <param name="command"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public async Task<CreateUserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
            {
                CreateUserResponse response = new CreateUserResponse();

                ApplicationUser existingUser = await _userManager.FindByEmailAsync(command.Email);

                // Optional logic : if user exists and is just soft-deleted, just reactivate the user
                if (existingUser != null && !existingUser.IsEnabled)
                {
                    // Send Update command using Mediatr
                    var updateUserCommand = new Features.User.Update.UpdateUserCommand()
                    {
                        Id = existingUser.Id,
                        Email = command.Email,
                        FirstName = command.FirstName,
                        LastName = command.LastName,
                        Role = command.Role,
                        IsEnabled = true
                    };

                    var updateUserCommandResponse = await _mediator.Send(updateUserCommand);

                    // Update response
                    response.Id = existingUser.Id;
                    response.IdentityResult = updateUserCommandResponse.IdentityResult;

                    // We will skip sending invitation email, since the user has got it before...
                    // If the user has forgotten their password, they can use the Forgot Password page.
                    return response;

                }

                // Create brand new user
                ApplicationUser user = new ApplicationUser();
                user.UserName = command.Email;
                user.Email = command.Email;
                user.EmailConfirmed = true;
                user.IsEnabled = true;
                string initialPassword = $"{Guid.NewGuid().ToString("N")}9aA#";

                IdentityResult result = await _userManager.CreateAsync(user, initialPassword);
                response.IdentityResult = result;
                if (!result.Succeeded)
                {
                    return response;
                }

                // Add additional claims to user if needed.
                var claims = new List<Claim>();

                claims.Add(new Claim(IdentityModel.JwtClaimTypes.GivenName, command.FirstName));
                claims.Add(new Claim(IdentityModel.JwtClaimTypes.FamilyName, command.LastName));

                await _userManager.AddClaimsAsync(user, claims);

                // Update role
                List<string> roles = new List<string>();
                roles.Add(command.Role);
                await _userManager.AddToRolesAsync(user, roles);

                // Skipped: send invitation email to user
                
                // Prepare response
                ApplicationUser userCreated = await _userManager.FindByEmailAsync(user.Email);

                response.Id = userCreated.Id;
                return response;
            }
        }
    }
}
