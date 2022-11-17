using Castle.DynamicProxy;
using System.Diagnostics;

namespace DS.Api.Base
{
    public class LoggingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            DateTime sdt=DateTime.Now;
            Debug.WriteLine(string.Format("{0} start at {1:g}",invocation.Method.Name,sdt));
            invocation.Proceed();
            DateTime edt = DateTime.Now;
            Debug.WriteLine(string.Format("{0} end at {1:g}, duration: {2:f3} ms", invocation.Method.Name, edt,(edt-sdt).TotalMilliseconds));
        }
    }
}
