using System;
using Autofac;
using LCard.API.Interfaces;
using LCard.API.Modules;
using LCard.Core.Interfaces;
using LCard.Core.Services;

namespace LCard.Manager.Startup
{
    public class UnityConfig
    {
        #region Unity Container
        private static readonly Lazy<IContainer> Container = new Lazy<IContainer>(() =>
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
            return Container.Value;
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
            container.RegisterType<DialogService>().As<IDialogService>();
            container.RegisterType<DeviceManager>().As<IDeviceManager>().SingleInstance();
        }
    }
}
