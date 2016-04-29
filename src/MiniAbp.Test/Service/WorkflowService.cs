using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Test.Service
{
    public class WorkflowService
    {
        public string CategoryRepository { get; set; }
        public string WorkflowRepository { get; set; }

        public WorkflowService()
        {
        }

        public List<string> GetAllCategory(string pageInput)
        {
         throw new NotImplementedException();
        }

        public void EditCategory(string m)
        {
            throw new NotImplementedException();
        }
 
}
}
