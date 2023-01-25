namespace SsoServer.Constants
{
    public class UserRoles
    {
        /// <summary>
        /// Administrator role
        /// </summary>
        public static readonly string Administrator = "Administrator";
        /// <summary>
        /// Finance role
        /// </summary>
        public static readonly string Finance = "Finance";

        /// <summary>
        /// The above roles represented in an array
        /// </summary>
        public static readonly string[] SupportedRoles = new string[] { Administrator, Finance };
    }
}
