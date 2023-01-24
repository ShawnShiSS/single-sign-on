using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SsoServer.Features.User
{
    /// <summary>
    ///     User API related actions.
    /// </summary>
    //[Authorize(LocalApi.PolicyName)]
    //[AllowAnonymous]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        
        private readonly IMediator _mediator;

        /// <summary>
        ///     ctor
        /// </summary>
        /// <param name="mediator">Using mediator pattern, this should be the ONLY dependency that a controller has</param>
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/user
        /// <summary>
        ///     Get all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IEnumerable<UserModel>> GetAll()
        {
            var response = await _mediator.Send(new GetAll.GetAllQuery()
            {
                IsEnabled = true
            });
            return response.Resource;
        }

    }
}
