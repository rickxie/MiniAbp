using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Console = System.Console;

namespace MiniAbp.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            //给Car类生成代理   
            ProxyGenerator generator = new ProxyGenerator();
            IInterceptor handler = new SimpleInterceptor();
            Type[] interfaces = { typeof(ICar) };
            Car car = new Car();
            //ICar carA = (generator.CreateInterfaceProxyWithTarget(interfaces, handler, car) as ICar;

            //ICar car = generator.CreateClassProxy<Car>(new IInterceptor[] { interceptor });
            //var start = typeof (Car).GetMethod("Start");
            //start.Invoke(car, null);
            //carA.Start();
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
