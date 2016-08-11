using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MiniAbp.Identity.Model.Table;

namespace MiniAbp.Identity.Model.LoginModel
{
    public class LoginViewModel
    {
        [Required]
        public string UsernameOrEmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [DefaultValue(true)]
        public bool RememberMe { get; set; }

        public string Error { get; set; }

        public bool Success { get; set; }


        public dynamic Identity { get; set; }

        //public TUser TUserModel { get; set; }

    }
}
