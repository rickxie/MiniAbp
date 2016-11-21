using System.Collections.Generic;
using System.Collections.Specialized;
using Castle.Components.DictionaryAdapter;
using Castle.DynamicProxy;
using MiniAbp.Extension;
using Newtonsoft.Json;

namespace MiniAbp.Console
{
    class Program
    {
        private static void Main(string[] args)
        {

            var className = "public class A<A,BC>:".FileString_GetCsharpClassName();
//            var listNV = new Genric<NameVal>();
//            var nameVal = new NameVal();
//            nameVal.Name = "xiaoming";
//            nameVal.Value = "happy";
//            listNV.Model = nameVal;
//            var listNV2 = new Genric<NameVal>();
//            var nameVal2 = new NameVal();
//            nameVal2.Name = "333";
//            nameVal2.Value = "333";
//            listNV2.Model = nameVal;
//
//            var newN = listNV.MapTo(listNV2);
//            newN.Model.Name = "444";
        }

        private void AopTest()
        {
            Dictionary<string, List<NameVal>> nv = new Dictionary<string, List<NameVal>>();
            nv.Add("admin05", new EditableList<NameVal>() { new NameVal() { Name = "Name", Value = "xiaoming" }, new NameVal() { Name = "Sex", Value = "0" } });
            nv.Add("admin01", new EditableList<NameVal>() { new NameVal() { Name = "Name", Value = "XX" }, new NameVal() { Name = "Sex", Value = "1" } });
            var jsong = JsonConvert.SerializeObject(nv);
            System.Console.Read();
        }
    }

    public class Genric<T>
    {
        public T Model { get; set; }
    }
    public class NameVal
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class NameA
    {
        public string Name { get; set; }
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
