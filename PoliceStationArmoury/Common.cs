namespace PoliceStationArmoury
{
    // RPH
    using Rage;
    using Rage.Native;

    // PSArmoury
    using PoliceStationArmoury.Types;

    internal static class Common
    {
        public static string CurrentVersion { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public static Vector3 GetGameplayCameraDirection()
        {
            Rotator rot = GetGameplayCameraRotation();
            double rotX = rot.Pitch / 57.295779513082320876798154814105;
            double rotZ = rot.Yaw / 57.295779513082320876798154814105;
            double multXY = System.Math.Abs(System.Math.Cos(rotX));

            return new Vector3((float)(-System.Math.Sin(rotZ) * multXY), (float)(System.Math.Cos(rotZ) * multXY), (float)(System.Math.Sin(rotX)));
        }

        public static Vector3 GetGameplayCameraPosition()
        {
            return NativeFunction.CallByName<Vector3>("GET_GAMEPLAY_CAM_COORD");
        }

        public static Rotator GetGameplayCameraRotation()
        {
            return NativeFunction.CallByName<Rotator>("GET_GAMEPLAY_CAM_ROT", 2);
        }

        public static bool IsUsingController { get { return !NativeFunction.CallByHash<bool>(0xa571d46727e2b718, 2); } }

        public static void DisplayHud(bool toggle)
        {
            NativeFunction.CallByName<uint>("DISPLAY_HUD", toggle);
        }

        public static void DisplayRadar(bool toggle)
        {
            NativeFunction.CallByName<uint>("DISPLAY_RADAR", toggle);
        }

        public static void DisableAllGameControls(GameControlGroup controlGroup)
        {
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS((int)controlGroup);
        }

        public static void EnableAllGameControls(GameControlGroup controlGroup)
        {
            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS((int)controlGroup);
        }
    }
}
