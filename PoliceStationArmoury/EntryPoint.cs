namespace PoliceStationArmory
{
    // RPH
    using Rage;

    internal static class EntryPoint
    {
        public static void Main()
        {
            Logger.LogWelcome();

            Globals.MainArmoury = new Armory();

            while (true)
            {
                GameFiber.Yield();

                Globals.MainArmoury.Update();
            }
        }
    }
}
