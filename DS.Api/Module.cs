using Autofac;
using Autofac.Extras.DynamicProxy;
using DS.Api.Base;
using DS.Api.Services;
using DS.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DS.Api
{
    public class Module :  Autofac.Module
    {
       
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggingInterceptor>();
            builder.RegisterType<FileService>().As<IFileService>().SingleInstance()
                .EnableClassInterceptors().InterceptedBy(typeof(LoggingInterceptor));
            builder.RegisterModule(new Ds.Application.Module());
        }
    }
}
