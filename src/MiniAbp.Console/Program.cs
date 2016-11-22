using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Castle.Components.DictionaryAdapter;
using Castle.DynamicProxy;
using MiniAbp.DataAccess;
using MiniAbp.Dependency;
using MiniAbp.Domain;
using MiniAbp.Extension;
using MiniAbp.Logging;
using Newtonsoft.Json;

namespace MiniAbp.Console
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) =>
                {
                    IocManager.Instance.Resolve<ILogger>()
                        .Error(sender.ToString() + eventArgs.ExceptionObject.ToString());
                };
            MiniAbp.StartWithSqlServer("Default");
        }
    }

}
