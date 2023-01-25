using MediatR;
using Microsoft.AspNetCore.Identity;
using SsoServer.Models;
using System.ComponentModel.DataAnnotations;

namespace SsoServer.Features.User
{
    /// <summary>
    ///     Delete a user
    /// </summary>
    public class Delete
    {
        public class DeleteUserCommand : IRequest<DeleteUserCommandResponse>, IValidatableObject
        {
            /// <summary>
            ///     User Id
            /// </summary>
            [Required]
            public string Id { get; set; }


            public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
            {

                var validationErrors = new List<ValidationResult>();

                if (!string.IsNullOrWhiteSpace(this.Id))
                {
                    var userManager = (UserManager<ApplicationUser>)validationContext.GetService(typeof(UserManager<ApplicationUser>));

                    ApplicationUser existingUser = userManager.FindByIdAsync(this.Id).Result;

                    if (existingUser == null)
                    {
                        validationErrors.Add(new ValidationResult("User not found."));

                    }
                }

                return validationErrors;
            }
        }

        public class DeleteUserCommandResponse
        {
            public bool NotFound { get; set; }
            public IdentityResult IdentityResult { get; set; }
        }

        public class CommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserCommandResponse>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public CommandHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            }

            public async Task<DeleteUserCommandResponse> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
            {
                var response = new DeleteUserCommandResponse();

                var user = await _userManager.FindByIdAsync(command.Id);

                // Validation should have already handled this. Just in case...
                if (user == null)
                {
                    response.NotFound = true;
                    return response;
                }

                // Delete the record in the identity db
                IdentityResult result = await _userManager.DeleteAsync(user);
                response.IdentityResult = result;

                return response;
            }
        }
    }
}
