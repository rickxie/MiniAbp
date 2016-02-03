using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Yooya.Bpm.Framework.Domain.Entity
{
    public class FileInput
    {
        public List<HttpPostedFile> Files { get; set; }
    }
}
