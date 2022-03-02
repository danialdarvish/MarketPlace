﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace MarketPlace.Web.PresentationExtensions
{
    public static class IdentityExtensions
    {
        public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal != null)
            {
                var data = claimsPrincipal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (data != null) return Convert.ToInt64(data.Value);
            }

            return default;
        }

        public static long GetUserId(this IPrincipal principal)
        {
            var user = (ClaimsPrincipal)principal;
            return user.GetUserId();
        }
    }
}
