using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using MiniAbp.Ado.Uow;
using MiniAbp.Configuration;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;

namespace MiniAbp.Ado.Dependency
{
    public class AdoConventionalRegisterer : IConventionalDependencyRegistrar
    {
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<AdoDbContext>()
                    .WithServiceSelf()
                    .LifestyleTransient()
                    .Configure(c => c.DynamicParameters((kernel, dynamicParams) =>
                    {
                        var dbSetting = context.IocManager.Resolve<DatabaseSetting>();
                        if (!string.IsNullOrWhiteSpace(dbSetting.ConnectionString))
                        {
                            dynamicParams["connectionString"] = dbSetting.ConnectionString;
                            dynamicParams["dialect"] = dbSetting.Dialect;
                        }
                    })));
        }


    }
}
