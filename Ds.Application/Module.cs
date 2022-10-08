using Autofac;
using Ds.Application.Services;
using Ds.Infrastructure.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Application
{
    public class Module : Autofac.Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<FileService>().As<FileService>().InstancePerLifetimeScope();
        }
        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<IDbService>().As<DbServices>().InstancePerLifetimeScope();
        }
    }
}
