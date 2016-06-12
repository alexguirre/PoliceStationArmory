namespace PoliceStationArmory
{
    // RPH
    using Rage;

    internal static class EntryPoint
    {
        public static void Main()
        {
            while (Game.IsLoading)
                GameFiber.Yield();

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
