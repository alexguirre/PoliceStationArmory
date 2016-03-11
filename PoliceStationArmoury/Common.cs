namespace PoliceStationArmoury
{
    internal static class Common
    {
        public static string CurrentVersion { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
    }
}
