namespace PoliceStationArmory
{
    // System
    using System;

    // RPH
    using Rage;

    internal static class Globals
    {
        public static Armory MainArmouryInstance { get; set; }

        public static Random Random { get; } = new Random();

        public static StaticFinalizer Finalizer = new StaticFinalizer(delegate { GlobalCleanUp(); });

        public static void GlobalCleanUp()
        {
            if(MainArmouryInstance != null)
                MainArmouryInstance.CleanUp();
        }
    }
}
