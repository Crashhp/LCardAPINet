using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using LCard.API.Modules;
using LCard.API.Interfaces;
using LCard.Core.Interfaces;
using LCard.Core.Services;

namespace LCard.E2010GUI.Startup
{
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IContainer> container = new Lazy<IContainer>(() =>
        {
            var container = new ContainerBuilder();
            RegisterTypes(container);
            return container.Build();
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(ContainerBuilder container)
        {
            container.RegisterType<E2010>().As<IE2010>().SingleInstance();
            container.RegisterType<DataService>().As<IDataService>();
            
        }
    }
}
