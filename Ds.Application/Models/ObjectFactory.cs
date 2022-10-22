using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Application.Models
{
    public class ObjectFactory<T> where T : class
    {
        private static T _instance;
        static ObjectFactory()
        {
            _instance = System.Activator.CreateInstance<T>();
        }
        public static T Create()
        {
            Type type = typeof(object);
            var clone = type.GetMethod("MemberwiseClone",BindingFlags.Instance | BindingFlags.NonPublic);
            T o= (T)clone.Invoke(_instance,null);
            return o;
        }
    }
}
