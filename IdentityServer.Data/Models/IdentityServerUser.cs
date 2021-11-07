namespace IdentityServer.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    
    public class IdentityServerUser: IdentityUser
    {
        public string Nickname { get; set; }
    }
}
