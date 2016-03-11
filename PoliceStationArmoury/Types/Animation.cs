namespace PoliceStationArmoury.Types
{
    // RPH
    using Rage;

    internal struct Animation
    {
        public AnimationDictionary Dictionary { get; set; }
        public string Name { get; set; }
        public AnimationFlags Flags { get; set; }

        public Animation(AnimationDictionary dict, string name)
        {
            Dictionary = dict;
            Name = name;
            Flags = AnimationFlags.None;
        }

        public Animation(AnimationDictionary dict, string name, AnimationFlags flags)
        {
            Dictionary = dict;
            Name = name;
            Flags = flags;
        }
    }
}
