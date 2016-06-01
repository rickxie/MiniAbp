using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Console = System.Console;

namespace MiniAbp.Console
{
    class Program
    {
        private static void Main(string[] args)
        {
            List<string> a = new List<string>();
            a.Add("1");
            var b = a.Where(r => r == "3").ToList();
            WindsorContainer iocContainer = new WindsorContainer();
            iocContainer.Register(Component.For<SimpleInterceptor>().LifestyleTransient());
            iocContainer.Kernel.ComponentRegistered += (key, handler1) =>
            {
                handler1.ComponentModel.Interceptors.Add(new InterceptorReference(typeof (SimpleInterceptor)));
            };
            iocContainer.Register(Component.For<ICar>().ImplementedBy<Car>().LifestyleTransient());
            //给Car类生成代理   
            //ProxyGenerator generator = new ProxyGenerator();
            //IInterceptor handler = new SimpleInterceptor();
            //Type[] interfaces = {typeof (ICar)};
            //ICar car = new Car();
            //ICar carA = (generator.CreateInterfaceProxyWithTarget(interfaces, handler, car) as ICar;

            //ICar car = generator.CreateClassProxy<Car>(new IInterceptor[] { interceptor });
            var ct = typeof (ICar);
            var car = iocContainer.Resolve(ct);
            var start = car.GetType().GetMethod("Start");
            start.Invoke(car, null);
//            car.Start();
            System.Console.Read();

        }
    }

    public interface ICar
    {
        void Start();
    }
    public class Car : ICar
    {
        public  void Start()
        {
            System.Console.WriteLine("Car Starting");
        }
    }

    public class SimpleInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            System.Console.WriteLine("begin invoke");
            invocation.Proceed();
            System.Console.WriteLine("end invoke");
        }
    }
}
