using Autofac;
using DS.Api.Services;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DS.Api
{
    public class Module :  Autofac.Module
    {
       
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileService>().As<FileService>().InstancePerLifetimeScope();
            builder.RegisterModule(new Ds.Application.Module());
        }
    }
}
