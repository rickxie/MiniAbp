using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Identity.Model.Table;

namespace MiniAbp.Identity.Model
{
    public class AccountModel
    {

        public dynamic Identity { get; set; }

        public User UserModel { get; set; }
    }
}
