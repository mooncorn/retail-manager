﻿namespace Portal.Models
{
    /// <summary>
    /// Response from the server after providing valid authentication credentials.
    /// </summary>
    public class AuthenticatedUserModel
    {
        public string Access_Token { get; set; }
        public string UserName { get; set; }
    }
}
