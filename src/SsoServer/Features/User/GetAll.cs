using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SsoServer.Models;
using System.Security.Claims;

namespace SsoServer.Features.User
{
    /// <summary>
    ///     GetAll related commands, validators, and handlers
    /// </summary>
    public class GetAll
    {
        /// <summary>
        ///     Model to GetAll 
        /// </summary>
        public class GetAllQuery : IRequest<QueryResponse>
        {
            
        }

        /// <summary>
        ///     Query Response
        /// </summary>
        public class QueryResponse
        {
            /// <summary>
            ///     Resource
            /// </summary>
            public IEnumerable<UserModel> Resource { get; set; }
        }

        /// <summary>
        ///     Handler
        /// </summary>
        public class QueryHandler : IRequestHandler<GetAllQuery, QueryResponse>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            /// <summary>
            ///     Ctor
            /// </summary>
            /// <param name="userManager"></param>
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
            public async Task<QueryResponse> Handle(GetAllQuery query, CancellationToken cancellationToken)
            {
                QueryResponse response = new QueryResponse();
                List<UserModel> models = new List<UserModel>();

                IEnumerable<ApplicationUser> users = await _userManager.Users
                                                                       .ToListAsync();

                foreach (ApplicationUser user in users)
                {

                    UserModel model = new UserModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
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
                        // we only support one user having one role for now
                        model.Role = roles[0];
                    }

                    models.Add(model);
                }

                response.Resource = models.OrderBy(m => m.UserName);

                return response;
            }
        }
    }
}
