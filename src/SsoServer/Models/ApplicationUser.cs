// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace SsoServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add profile data for application users by adding properties to the ApplicationUser class, see example below.

        /// <summary>
        ///     Whether the account is enabled/disabled.  
        ///     This is different than the user being locked out due to using an incorrect password too many times.
        /// </summary>
        //public bool IsEnabled { get; set; }

    }
}