using MiniAbp.Dependency;

namespace MiniAbp.Logging
{
    public class LogHelper
    {
        public static ILogger Logger { get; private set; }
        static LogHelper()
        {
            Logger = IocManager.Instance.IsRegistered(typeof(ILogger))
                ? IocManager.Instance.Resolve<ILogger>()
                : NullLogger.Instance;
        }

        public static void LogException(string msg)
        {
        }

    }
}
