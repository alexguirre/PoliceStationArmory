namespace Police_Station_Armory_Loadouts_Creator
{
    // System
    using System;

    internal static class Common
    {
        public static Version GetCurrentVerion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
