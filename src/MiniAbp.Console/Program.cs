using System.Collections.Generic;
using System.Collections.Specialized;
using Castle.Components.DictionaryAdapter;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace MiniAbp.Console
{
    class Program
    {
        private static void Main(string[] args)
        {
            Dictionary<string, List<NameVal>> nv = new Dictionary<string, List<NameVal>>();
            nv.Add("admin05", new EditableList<NameVal>() {new NameVal() {Name = "Name", Value = "xiaoming"},new NameVal() {Name = "Sex", Value = "0"} });
            nv.Add("admin01", new EditableList<NameVal>() {new NameVal() {Name = "Name", Value = "XX"},new NameVal() {Name = "Sex", Value = "1"} });
            var jsong = JsonConvert.SerializeObject(nv);
            System.Console.Read();

        }
    }

    public class NameVal
    {
        public string Name { get; set; }
        public string Value { get; set; }
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
