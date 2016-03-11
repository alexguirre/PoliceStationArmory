namespace PoliceStationArmoury
{
    // System
    using System;

    // RPH
    using Rage;

    internal static class Globals
    {
        public static Armoury MainArmoury { get; set; }

        public static Random Random { get; } = new Random();

        public static StaticFinalizer Finalizer = new StaticFinalizer(delegate { GlobalCleanUp(); });

        public static void GlobalCleanUp()
        {
            if(MainArmoury != null)
                MainArmoury.CleanUp();
        }
    }
}
