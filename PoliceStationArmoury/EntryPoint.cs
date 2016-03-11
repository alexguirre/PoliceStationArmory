namespace PoliceStationArmoury
{
    // RPH
    using Rage;

    internal static class EntryPoint
    {
        public static Armoury MainArmoury;

        public static void Main()
        {
            Logger.LogWelcome();

            MainArmoury = new Armoury();

            while (true)
            {
                GameFiber.Yield();

                MainArmoury.Update();
            }
        }

    }
}
