namespace PoliceStationArmory.Types
{
    // RPH
    using Rage;

    internal struct Animation
    {
        public AnimationDictionary Dictionary { get; }
        public string Name { get; }
        public AnimationFlags Flags { get; }

        public float BlendInSpeed { get; }
        public float BlendOutSpeed { get; }

        public float StartPosition { get; }

        public int Timeout { get; }

        public Animation(AnimationDictionary dict, string name, AnimationFlags flags, float blendInSpeed, float blendOutSpeed, float startPosition, int timeout)
        {
            Dictionary = dict;
            Name = name;
            Flags = flags;
            BlendInSpeed = blendInSpeed;
            BlendOutSpeed = blendOutSpeed;
            StartPosition = startPosition;
            Timeout = timeout;
        }

        public Animation(AnimationDictionary dict, string name, AnimationFlags flags, float blendInSpeed, float blendOutSpeed, float startPosition)
            : this(dict, name, flags, blendInSpeed, blendOutSpeed, startPosition, -1)
        {
        }

        public Animation(AnimationDictionary dict, string name, AnimationFlags flags, float blendInSpeed, float blendOutSpeed)
            : this(dict, name, flags, blendInSpeed, blendOutSpeed, 0.0f, -1)
        {
        }

        public Animation(AnimationDictionary dict, string name, AnimationFlags flags, float blendInSpeed)
            : this(dict, name, flags, blendInSpeed, 1.0f, 0.0f, -1)
        {
        }

        public Animation(AnimationDictionary dict, string name, AnimationFlags flags) 
            : this(dict, name, flags, 1.0f, 1.0f, 0.0f, -1)
        {
        }

        public Animation(AnimationDictionary dict, string name)
            : this(dict, name, AnimationFlags.None, 1.0f, 1.0f, 0.0f, -1)
        {
        }


        public AnimationTask PlayOnPed(Ped ped)
        {
            return ped.Tasks.PlayAnimation(Dictionary, Name, Timeout, BlendInSpeed, BlendOutSpeed, StartPosition, Flags);
        }
    }
}
