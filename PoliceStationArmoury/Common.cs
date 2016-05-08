namespace PoliceStationArmory
{
    // System
    using System.Drawing;
    using System.Runtime.InteropServices;

    // RPH
    using Rage;
    using Rage.Native;

    // PSArmoury
    using PoliceStationArmory.Types;

    internal static class Common
    {
        public static string CurrentVersion { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        //public static Vector3 GetGameplayCameraDirection()
        //{
        //    Vector3 rot = GetGameplayCameraRotation();
        //    double rotX = rot.X / 57.295779513082320876798154814105;
        //    double rotZ = rot.Z / 57.295779513082320876798154814105;
        //    double multXY = System.Math.Abs(System.Math.Cos(rotX));

        //    return new Vector3((float)(-System.Math.Sin(rotZ) * multXY), (float)(System.Math.Cos(rotZ) * multXY), (float)(System.Math.Sin(rotX)));
        //}

        public static Vector3 GetGameplayCameraPosition()
        {
            return NativeFunction.CallByName<Vector3>("GET_GAMEPLAY_CAM_COORD");
        }

        public static Vector3 GetGameplayCameraRotation()
        {
            return NativeFunction.CallByName<Vector3>("GET_GAMEPLAY_CAM_ROT", 2);
        }

        //public static bool IsUsingController { get { return !NativeFunction.CallByHash<bool>(0xa571d46727e2b718, 2); } }

        public static void DisplayHud(bool toggle)
        {
            NativeFunction.CallByName<uint>("DISPLAY_HUD", toggle);
        }

        public static void DisplayRadar(bool toggle)
        {
            NativeFunction.CallByName<uint>("DISPLAY_RADAR", toggle);
        }

        public static void DisableGameControlsGroup(GameControlsGroup controlGroup)
        {
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS((int)controlGroup);
        }

        public static void EnableGameControlsGroup(GameControlsGroup controlGroup)
        {
            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS((int)controlGroup);
        }

        public static bool IsDisabledControlPressed(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_PRESSED", index, (int)control);
        }

        public static bool IsDisabledControlJustPressed(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_JUST_PRESSED", index, (int)control);
        }

        //public static bool IsDisabledControlJustReleased(int index, GameControl control)
        //{
        //    return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_JUST_RELEASED", index, (int)control);
        //}

#if DEBUG
        public static void DrawMarker(MarkerType type, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, Color color)
        {
            DrawMarker(type, pos, dir, rot, scale, color, false, false, 2, false, null, null, false);
        }
        public static void DrawMarker(MarkerType type, Vector3 pos, Vector3 dir, Vector3 rot, Vector3 scale, Color color, bool bobUpAndDown, bool faceCamY, int unk2, bool rotateY, string textueDict, string textureName, bool drawOnEnt)
        {
            dynamic dict = 0;
            dynamic name = 0;

            if (textueDict != null && textureName != null)
            {
                if (textueDict.Length > 0 && textureName.Length > 0)
                {
                    dict = textueDict;
                    name = textureName;
                }
            }
            NativeFunction.CallByName<uint>("DRAW_MARKER", (int)type, pos.X, pos.Y, pos.Z, dir.X, dir.Y, dir.Z, rot.X, rot.Y, rot.Z, scale.X, scale.Y, scale.Z, (int)color.R, (int)color.G, (int)color.B, (int)color.A, bobUpAndDown, faceCamY, unk2, rotateY, dict, name, drawOnEnt);
        }
#endif

        //public static uint CreatePickup(PickupType type, Vector3 position, Rotator rotation, Model customModel, int value)
        //{
        //    customModel.LoadAndWait();
        //    uint handle = NativeFunction.CallByName<uint>("CREATE_PICKUP_ROTATE", (int)type, position.X, position.Y, position.Z, rotation.Pitch, rotation.Roll, rotation.Yaw, 0, value, 2, true, customModel.Hash);

        //    if (handle == 0)
        //    {
        //        return 0;
        //    }

        //    return handle;
        //}
        public static uint CreatePickup(PickupType type, Vector3 position, Rotator rotation, int value)
        {
            uint handle = NativeFunction.CallByName<uint>("CREATE_PICKUP_ROTATE", (int)type, position.X, position.Y, position.Z, rotation.Pitch, rotation.Roll, rotation.Yaw, 0, value, 2, true, 0);

            if (handle == 0)
            {
                return 0;
            }

            return handle;
        }
    }
}

