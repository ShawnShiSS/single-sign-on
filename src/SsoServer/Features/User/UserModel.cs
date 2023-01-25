namespace SsoServer.Features.User
{
    /// <summary>
    ///     API model for user 
    /// </summary>
    public class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /// <summary>
        ///     Whether the user is enabled, for soft deleting purpose.
        /// </summary>
        public bool IsEnabled { get; set; }

    }
}
