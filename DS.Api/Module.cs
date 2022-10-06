using Autofac;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DS.Api
{
    public class Module :  Autofac.Module
    {
        private readonly ConfigurationManager _configurationManager;
        public Module(ConfigurationManager manager)
        {
            _configurationManager = manager;
        }
        protected override void Load(ContainerBuilder builder)
        {
            //var assemblies=AppDomain.CurrentDomain.GetAssemblies();
            //assemblies = assemblies.Where(x => x.FullName.StartsWith("DS")).ToArray();
            //builder.RegisterAssemblyModules(assemblies);
        }
    }
}
