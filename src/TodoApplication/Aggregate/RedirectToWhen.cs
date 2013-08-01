using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Aggregate
{
    // snippet is for http://abdullin.com/journal/2011/6/26/event-sourcing-a-la-lokad.html

    // simple helper, that looks up and calls the proper overload of
    // When(SpecificEventType event). Reflection information is cached statically
    // once per type. 
    public static class RedirectToWhen
    {
        static class Cache<T>
        {
            public static readonly IDictionary<Type, MethodInfo> Dict = typeof(T)
            .GetMethods()
            .Where(m => m.Name == "When")
            .Where(m => m.GetParameters().Length == 1)
            .ToDictionary(m => m.GetParameters().First().ParameterType, m => m);
        }

        //public static void InvokeEvent<T>(T instance, IEvent command)
        public static void InvokeEvent<T>(T instance, object command)
        {
            MethodInfo info;
            var type = command.GetType();
            if (!Cache<T>.Dict.TryGetValue(type, out info))
            {
                var s = string.Format("Failed to locate {0}.When({1})", typeof(T).Name, type.Name);
                throw new InvalidOperationException(s);
            }
            info.Invoke(instance, new[] { command });

        }
        
        //Commands are not used in this project.
        //public static void InvokeCommand<T>(T instance, ICommand command)
        //{
        //    MethodInfo info;
        //    var type = command.GetType();
        //    if (!Cache<T>.Dict.TryGetValue(type, out info))
        //    {
        //        var s = string.Format("Failed to locate {0}.When({1})", typeof(T).Name, type.Name);
        //        throw new InvalidOperationException(s);
        //    }
        //    info.Invoke(instance, new[] { command });
        //}
    }
}
