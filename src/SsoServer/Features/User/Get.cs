using MediatR;
using Microsoft.AspNetCore.Identity;
using SsoServer.Models;
using System.ComponentModel.DataAnnotations;

namespace SsoServer.Features.User
{
    /// <summary>
    ///     Get related query, validators, and handlers
    /// </summary>
    public class Get
    {
        /// <summary>
        ///     Model to Get 
        /// </summary>
        public class GetQuery : IRequest<QueryResponse>
        {
            /// <summary>
            ///     Id
            /// </summary>
            [Required]
            public string Id { get; set; }

        }

        /// <summary>
        ///     Query Response
        /// </summary>
        public class QueryResponse
        {
            /// <summary>
            ///     Resource
            /// </summary>
            public UserModel Resource { get; set; }
        }

        /// <summary>
        ///     Handler
        /// </summary>
        public class QueryHandler : IRequestHandler<GetQuery, QueryResponse>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            /// <summary>
            ///     Ctor
            /// </summary>
            /// <param name="userService"></param>
            public QueryHandler(UserManager<ApplicationUser> userManager)
            {
                this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            }

            /// <summary>
            ///     Handle
            /// </summary>
            /// <param name="query"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public async Task<QueryResponse> Handle(GetQuery query, CancellationToken cancellationToken)
            {
                QueryResponse response = new QueryResponse();

                ApplicationUser user = await _userManager.FindByIdAsync(query.Id);

                if (user == null)
                {
                    return response;
                    //throw new EntityNotFoundException(nameof(ApplicationUser), query.Id);
                }

                // Get user info
                UserModel model = new UserModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsEnabled = user.IsEnabled
                };

                // Get user claims
                var currentClaims = await _userManager.GetClaimsAsync(user);
                var firstNameClaim = currentClaims.FirstOrDefault(x => x.Type == IdentityModel.JwtClaimTypes.GivenName);
                if (firstNameClaim != null)
                {
                    model.FirstName = firstNameClaim.Value;
                }

                var lastNameClaim = currentClaims.FirstOrDefault(x => x.Type == IdentityModel.JwtClaimTypes.FamilyName);
                if (lastNameClaim != null)
                {
                    model.LastName = lastNameClaim.Value;
                }

                // Get role
                IList<string> roles = await _userManager.GetRolesAsync(user);
                if (roles != null && roles.Count > 0)
                {
                    // Support 1 role for now
                    model.Role = roles[0];
                }

                response.Resource = model;
                return response;
            }

        }


    }
}
