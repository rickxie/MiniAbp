using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Ado;
using MiniAbp.DataAccess;
using MiniAbp.Reflection;

namespace MiniAbp.Test
{
    [DependsOn(typeof(AdoModule))]
    public class TestModule : MabpModule
    {
        public override void PreInitialize()
        {
            Configuration.Database.ConnectionString = @"Data Source= shaappt0001.ad.shalu.com;Initial Catalog=Dfyf.Bpm;Persist Security Info=true;User ID=sa;PWD=Passw0rd;Packet Size=4096;";
            Configuration.Database.Dialect = Dialect.SqlServer;
            base.PreInitialize();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly()); 
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
