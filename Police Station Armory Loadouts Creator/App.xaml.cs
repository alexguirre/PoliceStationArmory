using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Police_Station_Armory_Loadouts_Creator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void Start()
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
                AppDomain.CurrentDomain.ProcessExit += End;
            else
                AppDomain.CurrentDomain.DomainUnload += End;

            Logger.Init();
        }

        public static bool HasEnded { get; private set; } = false;
        public static void End(object sender, EventArgs e)
        {
            if (HasEnded)
                return;

            Logger.DisposeStaticLogger();

            HasEnded = true;
        }






        protected override void OnStartup(StartupEventArgs e)
        {
            Start();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            End(this, e);
            base.OnExit(e);
        }
    }
}
