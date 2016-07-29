using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Domain.Entitys;

namespace MiniAbp.Test
{
    [TestClass]
    public class TemplateManagerTest : TestBase
    {
        

        public TemplateManagerTest()
        {
            //Connection, Dialect.SqlServer
            Initialize();
        }

        [TestMethod]
        public void TestTpl()
        {
            var a = DbDapper.RunDataTableSql(@"With a as (select * from  appuser)
SELECT * FROM a", new PageInput()
            {
                Ascending = false,
                CurrentPage = 2, Filters = null, OrderByProperty = "CreationTime", PageSize = 10
            });
            var b = DbDapper.Query<UserDto>(@"With a as (select * from  appuser)
SELECT * FROM a", new PageInput()
            {
                Ascending = false,
                CurrentPage = 2, Filters = null, OrderByProperty = "CreationTime", PageSize = 10
            });
        }
        
    }

    public class UserDto
    {
        public string Id { get; set; }
    }
}
