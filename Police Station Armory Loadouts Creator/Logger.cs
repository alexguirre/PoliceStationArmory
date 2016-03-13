namespace Police_Station_Armory_Loadouts_Creator
{
    // System
    using System;
    using System.IO;
    using System.Globalization;

    internal sealed class Logger : IDisposable
    {
        internal static string GetCurrentTime()
        {
            return DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }


        //private StreamWriter fileWriter;

        //public bool HasBeenInitialized { get; private set; } = false;
        public bool HasBeenDisposed { get; private set; } = false;

        public string Path { get; private set; }

#if DEBUG
        /// <summary>
        /// Determines wheter <see cref="LogType.Debug"/> will log or not. By default it depends on the build type.
        /// </summary>
        public bool IsDebugBuild { get; set; } = true;
#else
        /// <summary>
        /// Determines wheter <see cref="LogType.Debug"/> will log or not. By default it depends on the build type.
        /// </summary>
        public bool IsDebugBuild { get; set; } = false;
#endif

        /// <summary>
        /// The Logger contructor
        /// </summary>
        /// <param name="fileName">The name of the file that will be created.</param>
        /// <param name="createConsole">If true a <see cref="XEngine.Core.XConsole"/> will be created.</param>
        /// <remarks>Do not set <paramref name="createConsole"/> to true if there isn't a <see cref="System.Windows.Forms.Form">Windows Forms</see> running</remarks>
        public Logger(string fileName)
        {
            string path = @"Police Station Armory Loadouts Creator Logs\" + fileName + "_" + DateTime.UtcNow.ToShortDateString().Replace("/", "-") + "_" + DateTime.UtcNow.ToLongTimeString().Replace(":", "") + ".log";
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            File.Create(path).Dispose();
            Path = path;
            logInit();
        }

        public void LogTrivialInstance(object o)
        {
            LogInstance(o, LogType.Trivial, null);
        }
        public void LogTrivialInstance<T>(object o)
        {
            LogInstance("[" + typeof(T).Name + "]", LogType.Trivial, null);
            LogInstance(o, LogType.Trivial, null);
        }

        public void LogDebugInstance(object o)
        {
            LogInstance(o, LogType.Debug, null);
        }
        public void LogDebugInstance<T>(object o)
        {
            LogInstance("[" + typeof(T).Name + "]", LogType.Debug, null);
            LogInstance(o, LogType.Debug, null);
        }

        public void LogWarningInstance(object o)
        {
            LogInstance(o, LogType.Warning, null);
        }
        public void LogWarningInstance<T>(object o)
        {
            LogInstance("[" + typeof(T).Name + "]", LogType.Warning, null);
            LogInstance(o, LogType.Warning, null);
        }

        public void LogExceptionInstance(object o, Exception exception)
        {
            LogInstance(o, LogType.Exception, exception);
        }
        public void LogExceptionInstance<T>(object o, Exception exception)
        {
            LogInstance("[" + typeof(T).Name + "]", LogType.Exception, exception);
            LogInstance(o, LogType.Exception, exception);
        }

        public void LogInstance(object o, LogType type, Exception exception)
        {
            if (HasBeenDisposed)
                return;
            string textToLog = String.Empty;
            switch (type)
            {
                case LogType.Trivial:
                    textToLog = "[" + GetCurrentTime() + "] " + o;
                    break;
                case LogType.Debug:
                    if (!IsDebugBuild)
                        return;
                    textToLog = "[" + GetCurrentTime() + "]<DEBUG> " + o;
                    break;
                case LogType.Warning:
                    textToLog = "[" + GetCurrentTime() + "]<WARNING> " + o;
                    break;
                case LogType.Exception:
                    if (exception == null)
                    {
                        Logger.LogWarning<Logger>("Parameter 'exception' is null, when using LogType.Exception 'exception' can't be null. Throwing ArgumentNullException.");
                        throw new ArgumentNullException("exception", "To use LogType.Exception the parameter 'exception' can't be null");
                    }
                    textToLog = "[" + GetCurrentTime() + "]<EXCEPTION> " + o + " - " + exception.ToString();
                    break;
                default:
                    Logger.LogWarning<Logger>("Invalid LogType in the Logger with path " + Path + ". Throwing ArgumentOutOfRangeException.");
                    throw new ArgumentOutOfRangeException("type", "Invalid LogType");
            }

            using (StreamWriter writer = new StreamWriter(Path, true))
                writer.WriteLine(textToLog);
        }
        public void LogInstance<T>(object o, LogType type, Exception exception)
        {
            Log("[" + typeof(T).Name + "] ", type, exception);
            Log(o, type, exception);
        }

        public void LogInstance()
        {
            string textToLog = "[" + GetCurrentTime() + "]";
            using (StreamWriter writer = new StreamWriter(Path, true))
                writer.WriteLine(textToLog);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposeManagedResources)
        {
            if (HasBeenDisposed)
                return;

            if (disposeManagedResources)
            {
                logEnd();
            }

            // Free any unmanaged objects here.
            //
            HasBeenDisposed = true;
        }

        /// <summary>
        /// Performs object finalization.
        /// </summary>
        ~Logger()
        {
            Dispose(false);
        }

        private void logInit()
        {
            LogTrivialInstance("Started new log on " + GetCurrentTime());
            LogTrivialInstance("====================================================================================================");
            LogTrivialInstance("Log path: " + System.IO.Path.GetFullPath(Path));
        }

        private void logEnd()
        {
            LogTrivialInstance("Finished log on " + GetCurrentTime());
            LogTrivialInstance("====================================================================================================");
            LogTrivialInstance("Log disposed");
        }


        private static Logger current;
        public static bool IsStaticLoggerInitialized { get; private set; } = false;
        public static bool HasStaticLoggerBeenDisposed { get { return current == null || current.HasBeenDisposed; } }
        public static string StaticLoggerPath { get { return current.Path; } }
        static Logger()
        {
            if (!IsStaticLoggerInitialized)
                Logger.Init();
        }

        public static void Init()
        {
            if (IsStaticLoggerInitialized)
                return;

            current = new Logger(@"PoliceStationArmoryLoadoutsCreator_" + Common.GetCurrentVerion());

            IsStaticLoggerInitialized = true;
        }

        public static void LogTrivial(object o)
        {
            current.LogTrivialInstance(o);
        }
        public static void LogTrivial<T>(object o)
        {
            current.LogTrivialInstance<T>(o);
        }

        public static void LogDebug(object o)
        {
            current.LogDebugInstance(o);
        }
        public static void LogDebug<T>(object o)
        {
            current.LogDebugInstance<T>(o);
        }

        public static void LogWarning(object o)
        {
            current.LogWarningInstance(o);
        }
        public static void LogWarning<T>(object o)
        {
            current.LogWarningInstance<T>(o);
        }

        public static void LogException(object o, Exception exception)
        {
            current.LogExceptionInstance(o, exception);
        }
        public static void LogException<T>(object o, Exception exception)
        {
            current.LogExceptionInstance<T>(o, exception);
        }


        public static void Log(object o, LogType type, Exception exception)
        {
            current.LogInstance(o, type, exception);
        }
        public static void Log<T>(object o, LogType type, Exception exception)
        {
            current.LogInstance<T>(o, type, exception);
        }

        public static void Log()
        {
            current.LogInstance();
        }

        public static void DisposeStaticLogger()
        {
            if (HasStaticLoggerBeenDisposed)
                return;

            current.Dispose();
        }
    }

    public enum LogType
    {
        Trivial,
        Debug,
        Warning,
        Exception,
    }
}
