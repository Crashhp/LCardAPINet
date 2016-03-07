using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using LCard.API.Interfaces;
using LCard.Core.Logger;
using LCard.Manager.Startup;

namespace LCard.Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            var e = args.Exception;
            Logger.Current.Error("Inner", e.InnerException);
            Logger.Current.Error("Excep", e);
            MessageBox.Show(e.ToString());
            args.Handled = true;
        }

        private static volatile object _lockObj = new object();
        public static bool IsDevicePluggedIn
        {
            get
            {
                lock (_lockObj)
                {
                    return UnityConfig.GetConfiguredContainer().Resolve<IE2010>().IsDevicePluggedIn;
                }
            }
        }

    }
}
