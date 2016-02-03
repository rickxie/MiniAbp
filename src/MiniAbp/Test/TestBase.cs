using Yooya.Bpm.Framework;

namespace MiniAbp.Test
{
    public class TestBase
    {
        public YBootstrapper Bootstrapper;
        public TestBase()
        {
            Bootstrapper = new YBootstrapper();
            Initialize();
            Bootstrapper.Initialize();
        }

        public virtual void Initialize()
        {
        }
    }
}
