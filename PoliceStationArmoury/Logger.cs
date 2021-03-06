﻿namespace PoliceStationArmory
{
    // System
    using System;
    using System.Diagnostics;

    // RPH
    using Rage;

    /// <summary>
    /// Class for improved logging
    /// </summary>
    internal static class Logger
    {
        public const string PluginName = "Police Station Armory";

        public static void LogTrivial(object o)
        {
            Game.LogTrivial("[" + PluginName + "] " + o);
        }

        public static void LogTrivial(string specific, object o)
        {
            Game.LogTrivial("[" + PluginName + " | " + specific + "] " + o);
        }

        [Conditional("DEBUG")]
        public static void LogDebug(object o)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + "]<DEBUG> " + o);
#endif
        }
        [Conditional("DEBUG")]
        public static void LogDebug(string specific, object o)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + " | " + specific + "]<DEBUG> " + o);
#endif
        }


        public static void LogException(Exception ex)
        {
            Game.LogTrivial("[" + PluginName + "]<EXCEPTION> " + ex.ToString());
        }

        public static void LogException(string specific, Exception ex)
        {
            Game.LogTrivial("[" + PluginName + " | " + specific + "]<EXCEPTION> " + ex.ToString());
        }


        [Conditional("DEBUG")]
        public static void LogExceptionDebug(Exception ex)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + "]<DEBUG | EXCEPTION> " + ex.ToString());
#endif
        }
        [Conditional("DEBUG")]
        public static void LogExceptionDebug(string specific, Exception ex)
        {
#if DEBUG
            Game.LogTrivial("[" + PluginName + " | " + specific + "]<DEBUG | EXCEPTION> " + ex.ToString());
#endif
        }


        public static void LogWelcome()
        {
            Game.Console.Print("================================================ " + PluginName + " ================================================");
            Game.Console.Print("Created by:  alexguirre");
            Game.Console.Print("Version:  " + Common.CurrentVersion);
            Game.Console.Print("================================================ " + PluginName + " ================================================");
        }
    }
}
