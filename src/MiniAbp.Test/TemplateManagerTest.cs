using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniAbp.DataAccess;
using MiniAbp.Razor;

namespace MiniAbp.Test
{
    [TestClass]
    public class TemplateManagerTest : TestBase
    {
        public string Connection = @"Data Source=shaappt0001;Initial Catalog=Shalu_Bpm_m;Persist Security Info=true;User ID=sa;PWD=Passw0rd;Packet Size=4096;";

        public TemplateManagerTest()
        {
           Initialize(Connection, Dialect.SqlServer);
        }

        [TestMethod]
        public void TestTpl()
        {
        }
    }
}
