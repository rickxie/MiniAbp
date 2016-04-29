using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using RazorEngine.Compilation.ImpromptuInterface.Dynamic;

namespace MiniAbp.Extension
{
    public static class ObjectExtension
    {
        public static TDestination MapTo<TDestination>(this object source) where TDestination : class, new()
        {
            var targetInstance = new TDestination();
            return source.MapTo(targetInstance);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        ///             There must be a mapping between objects before calling this method.
        /// 
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam><typeparam name="TDestination">Destination type</typeparam><param name="source">Source object</param><param name="destination">Destination object</param>
        /// <returns/>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) where TDestination : class, new()
        {
            var sourceType = source.GetType();
            var sourceProps = sourceType.GetProperties();
            var targetType = typeof(TDestination);
            var targetProps = targetType.GetProperties();
            var targetInstance = destination;

            foreach (var p in sourceProps)
            {
                var targetValueType = targetProps.FirstOrDefault(r => r.Name == p.Name);
                if (targetValueType != null)
                {
                    var valueType = sourceType.GetProperty(p.Name);
                    var value = valueType.GetValue(source);
                    if (value != null)
                    {
                        targetValueType.SetValue(targetInstance, value);
                    }
                }
            }
            return targetInstance;
        }

       
    }
}
