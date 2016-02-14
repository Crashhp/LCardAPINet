using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using LCard.API.Interfaces;
using LCard.Core.Interfaces;
using LCard.E2010GUI.Startup;

namespace LCard.E2010GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var module = UnityConfig.GetConfiguredContainer().Resolve<IE2010>();
            if (!module.OpenLDevice())
            {
                MessageBox.Show("Устройство не подключено");
                this.Shutdown();
            }
        }
    }
}
