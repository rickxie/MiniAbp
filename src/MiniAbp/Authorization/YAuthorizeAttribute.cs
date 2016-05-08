
using System;

namespace MiniAbp.Authorization
{
    public class MabpAuthorizeAttribute : Attribute, IMabpAuthorizeAttribute
    {
        public MabpAuthorizeAttribute()
        {
        }

        public string[] Permissions { get; }
        public bool RequireAllPermissions { get; set; }
    }
}
