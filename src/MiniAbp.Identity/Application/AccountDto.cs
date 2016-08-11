using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Identity.Application
{
    public class AccountDto
    {
        public string Phone { get; set; }

        public int Step { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }

        public int Type { get; set; }

        public string UserId { get; set; }

        public dynamic Identity { get; set; }

        public UserDto UserModel { get; set; }
    }
}
