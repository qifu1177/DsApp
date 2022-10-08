using System.Reflection;

namespace DS.Api
{
    public class AssemblyProvider
    {
        public static Assembly[] GetAssembliesFromProjects(string presfix)
        {
            presfix = presfix.ToLower();
            var assemblies = AppDomain
                            .CurrentDomain
                            .GetAssemblies();
            assemblies= assemblies.Where(x => x.FullName.ToLower().StartsWith(presfix))
                            .ToArray();
            return assemblies;
            //List<Assembly> assemblies = new List<Assembly>();
            //var fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            //var dirInfo=new DirectoryInfo(fileInfo.DirectoryName);
            //var files = dirInfo.GetFiles();
            //foreach (var file in files)
            //{
            //    if(file.Name.ToLower().StartsWith(presfix) && file.Name.EndsWith(".dll"))
            //    {
            //        assemblies.Add(Assembly.LoadFile(file.FullName));
            //    }
            //}
            //return assemblies.ToArray();
        }
    }
}
