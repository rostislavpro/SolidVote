using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;

namespace VoterApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IContainer IocContainer;

        protected override void OnStartup(StartupEventArgs e)
        {
            IocContainer = ContainerConfiguration.GetConfiguredContainer();

            try
            {
                var mainView = this.IocContainer.Resolve<MainWindow>();
                mainView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                this.Shutdown();
            }
        }
    }
}
