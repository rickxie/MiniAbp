using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniAbp.DataAccess;

namespace MiniAbp.Test
{
    [TestClass]
    public class DbTest : TestBase
    {
        public DbTest()
        {
            DbDapper.ConnectionString =
                "Data Source=shaappt0001;Initial Catalog=Yooya.bpm.designer-dev;Persist Security Info=true;User ID=sa;PWD=Passw0rd;Packet Size=4096;";
            DbDapper.DatabaseType = DatabaseType.Sql;
        }
        [TestMethod]
        public void RunDataTableSqlTest()
        {
            var table = DbDapper.RunDataTableSql("Select * from wfdworkflow");
            Assert.IsNotNull(table);
        }
    }
}
