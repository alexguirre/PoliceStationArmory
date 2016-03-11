namespace PoliceStationArmoury
{
    // RPH
    using Rage;

    internal static class EntryPoint
    {
        public static void Main()
        {
            Logger.LogWelcome();

            Globals.MainArmoury = new Armoury();

            while (true)
            {
                GameFiber.Yield();

                Globals.MainArmoury.Update();
            }
        }

    }
}
