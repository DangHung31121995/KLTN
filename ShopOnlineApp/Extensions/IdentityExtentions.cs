using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopOnlineApp.Extensions
{
    public static class IdentityExtentions
    {
        public static string GetSpecificDefault(this ClaimsPrincipal claimPrincipal, string typeClaim)
        {
            var claim = claimPrincipal.Claims.FirstOrDefault(x => x.Type == typeClaim);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}