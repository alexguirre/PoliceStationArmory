namespace PoliceStationArmory
{
    // RPH
    using Rage;

    internal static class EntryPoint
    {
        public static void Main()
        {
            Logger.LogWelcome();

            Globals.MainArmouryInstance = new Armory();

            while (true)
            {
                GameFiber.Yield();

                Globals.MainArmouryInstance.Update();
            }
        }
    }
}
