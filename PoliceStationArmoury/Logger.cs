namespace PoliceStationArmoury
{
    using Rage;
    using System;

    /// <summary>
    /// Class for improved logging
    /// </summary>
    internal static class Logger
    {
        public const string PluginName = "Police Station Armoury";

        public static void LogTrivial(string text)
        {
            Game.LogTrivial("[" + PluginName + "] " + text);
        }

        public static void LogTrivial(string specific, string text)
        {
            Game.LogTrivial("[" + PluginName + " | " + specific + "] " + text);
        }


        public static void LogDebug(string text)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + "]<DEBUG> " + text);
#endif
        }

        public static void LogDebug(string specific, string text)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + " | " + specific + "]<DEBUG> " + text);
#endif
        }


        public static void LogException(Exception ex)
        {
            Game.LogTrivial("[" + PluginName + "]<EXCEPTION> " + ex.Message + " :: " + ex.StackTrace);
        }

        public static void LogException(string specific, Exception ex)
        {
            Game.LogTrivial("[" + PluginName + " | " + specific + "]<EXCEPTION> " + ex.Message + " :: " + ex.StackTrace);
        }


        public static void LogExceptionDebug(Exception ex)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + "]<DEBUG | EXCEPTION> " + ex.Message + " :: " + ex.StackTrace);
#endif
        }

        public static void LogExceptionDebug(string specific, Exception ex)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + " | " + specific + "]<DEBUG | EXCEPTION> " + ex.Message + " :: " + ex.StackTrace);
#endif
        }


        public static void LogWelcome()
        {
            Game.Console.Print("================================================ " + PluginName + " ================================================");
            Game.Console.Print("Created by:  alexguirre");
            Game.Console.Print("Version:  " + Common.CurrentVersion);
            Game.Console.Print();
            Game.Console.Print("Report any issues you have in the NOOSE Tactical Response Unit topic or in the comments section and include the RagePluginHook.log");
            Game.Console.Print("Enjoy!");
            Game.Console.Print("================================================ " + PluginName + " ================================================");
        }
    }
}
