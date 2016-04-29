using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniAbp.DataAccess;

namespace MiniAbp.Test
{
    [TestClass]
    public class DbTest : TestBase
    {
        public DbTest()
        {
           
        }
        [TestMethod]
        public void RunDataTableSqlTest()
        {
            var table = DbDapper.RunDataTableSql("Select * from wfdworkflow");
            Assert.IsNotNull(table);
        }
    }
}
