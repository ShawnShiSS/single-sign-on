using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace SsoServer.Features.User
{
    /// <summary>
    ///     User API related actions.
    /// </summary>
    //[Authorize(LocalApi.PolicyName)]
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
            var response = await _mediator.Send(new GetAll.GetAllQuery(){});
            return response.Resource;
        }


        // GET: api/user/5
        /// <summary>
        ///     Get a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetUser")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> Get(string id)
        {
            var response = await _mediator.Send(new Get.GetQuery() { Id = id });

            if (response.Resource == null)
            {
                return NotFound();
            }

            return Ok(response.Resource);
        }

        // POST: api/user
        /// <summary>
        ///     Create a user
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult<Features.User.Create.CreateUserResponse>> Create([FromBody] Create.CreateUserCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response.IdentityResult.Succeeded)
            {
                return BadRequest();
            }

            return CreatedAtRoute("GetUser", new { id = response.Id }, new { Id = response.Id });
        }

        // PUT: api/user/5
        /// <summary>
        ///     Update a user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Update))]
        public async Task<IActionResult> Update(string id, [FromBody] Update.UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            var response = await _mediator.Send(command);

            if (response.NotFound)
            {
                return NotFound();
            }

            if (!response.IdentityResult.Succeeded)
            {
                return BadRequest();
            }

            // Return success code
            return NoContent();
        }

        // DELETE: api/user/5
        /// <summary>
        ///     Deactivate/Soft-delete a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _mediator.Send(new Delete.DeleteUserCommand { Id = id });

            if (response.NotFound)
            {
                return NotFound();
            }

            if (!response.IdentityResult.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
