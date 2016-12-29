namespace PoliceStationArmory
{
    // System
    using System.IO;
    using System.Windows.Forms;

    // RPH
    using Rage;

    internal static class Settings
    {
        public const string FileName = @"Plugins\Police Station Armory.ini";

        public static readonly InitializationFile INIFile;
        public static readonly bool OnTheFlyModeEnabled;
        public static readonly Keys OpenCloseOnTheFlyMenuKey;

        static Settings()
        {
            if (!File.Exists(FileName))
            {
                using (FileStream f = File.Create(FileName))
                using (StreamWriter writer = new StreamWriter(f))
                {
                    writer.Write(INIFileDefaultText);
                }
            }


            INIFile = new InitializationFile(FileName);
            OnTheFlyModeEnabled = INIFile.ReadBoolean("SETTINGS", "On The Fly Mode Enabled", true);
            OpenCloseOnTheFlyMenuKey = INIFile.ReadEnum<Keys>("SETTINGS", "Open/Close On The Fly Menu Key", Keys.F7);
        }

        private const string INIFileDefaultText = @"[SETTINGS]
// If true you can open the weapons menu anywhere using the key below; if false this feature will be disabled
On The Fly Mode Enabled = true
Open/Close On The Fly Menu Key = F7
";
    }
}
