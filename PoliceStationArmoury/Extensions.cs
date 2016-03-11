namespace PoliceStationArmory
{
    // System
    using System.Drawing;

    // RPH
    using Rage;
    using Rage.Native;

    // PSArmoury
    using PoliceStationArmory.Types;

    internal static class Extensions
    {
        public static bool IsInRangeOf(this Vector3 s, Vector3 position, float distance)
        {
            return s.DistanceTo(position) < distance;
        }
        public static bool IsInRangeOf(this ISpatial s, Vector3 position, float distance)
        {
            return s.DistanceTo(position) < distance;
        }


        public static bool IsInRangeOf(this Vector3 s, ISpatial spatial, float distance)
        {
            return s.DistanceTo(spatial) < distance;
        }
        public static bool IsInRangeOf(this ISpatial s, ISpatial spatial, float distance)
        {
            return s.DistanceTo(spatial) < distance;
        }


        public static bool IsInRangeOf2D(this Vector3 s, Vector3 position, float distance)
        {
            return s.DistanceTo2D(position) < distance;
        }
        public static bool IsInRangeOf2D(this ISpatial s, Vector3 position, float distance)
        {
            return s.DistanceTo2D(position) < distance;
        }

        public static bool IsInRangeOf2D(this Vector3 s, ISpatial spatial, float distance)
        {
            return s.DistanceTo2D(spatial) < distance;
        }
        public static bool IsInRangeOf2D(this ISpatial s, ISpatial spatial, float distance)
        {
            return s.DistanceTo2D(spatial) < distance;
        }


        public static Task PlayAnimation(this Ped ped, Animation animation, float blendInSpeed)
        {
            return ped.Tasks.PlayAnimation(animation.Dictionary, animation.Name, blendInSpeed, animation.Flags);
        }
        public static Task PlayAnimation(this Ped ped, Animation animation, int timeout, float blendInSpeed, float blendOutSpeed, float startPosition)
        {
            return ped.Tasks.PlayAnimation(animation.Dictionary, animation.Name, timeout, blendInSpeed, blendOutSpeed, startPosition, animation.Flags);
        }


        public static PointF Add(this PointF left, PointF right)
        {
            return new PointF(left.X + right.X, left.Y + right.Y);
        }
        public static PointF Subtract(this PointF left, PointF right)
        {
            return new PointF(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// Transition from the <see paramref="from"/> camera to the <see paramref="to"/> camera. Activates the <see paramref="to"/> camera.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time">The time that the interpolation takes</param>
        /// <param name="easeLocation">Accelerate/decelerate cam speed during movement</param>
        /// <param name="easeRotation"></param>
        /// <param name="waitForCompletion">If true it will wait until the interpolation finishes</param>
        public static void Interpolate(this Camera from, Camera to, int time, bool easeLocation, bool easeRotation, bool waitForCompletion)
        {
            //SET_CAM_ACTIVE_WITH_INTERP(Cam camTo, Cam camFrom, int duration, BOOL easeLocation, BOOL easeRotation)
            NativeFunction.Natives.SET_CAM_ACTIVE_WITH_INTERP(to, from, time, easeLocation, easeRotation);
            if (waitForCompletion)
                GameFiber.Sleep(time);
        }

        /// <summary>
        /// Transition from the <see paramref="from"/> camera to the <see paramref="to"/> position. Returns and activates the created camera at the <see paramref="to"/> position.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time">The time that the interpolation takes</param>
        /// <param name="easeLocation">Accelerate/decelerate cam speed during movement</param>
        /// <param name="easeRotation"></param>
        /// <param name="waitForCompletion">If true it will wait until the interpolation finishes</param>
        /// <returns>Returns the created camera at the <see paramref="to"/> position.</returns>
        public static Camera Interpolate(this Camera from, Vector3 to, int time, bool easeLocation, bool easeRotation, bool waitForCompletion)
        {
            Camera toCam = new Camera(false);
            toCam.Position = to;

            //SET_CAM_ACTIVE_WITH_INTERP(Cam camTo, Cam camFrom, int duration, BOOL easeLocation, BOOL easeRotation)
            NativeFunction.Natives.SET_CAM_ACTIVE_WITH_INTERP(toCam, from, time, easeLocation, easeRotation);
            if (waitForCompletion)
                GameFiber.Sleep(time);
            return toCam;
        }

        /// <summary>
        /// Transition from the <see paramref="from"/> camera to the <see paramref="to"/> position. Returns and activates the created camera at the <see paramref="to"/> position.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toPos">The position where create the new camera</param>
        /// <param name="toRot">The rotation of the new camera</param>
        /// <param name="time">The time that the interpolation takes</param>
        /// <param name="easeLocation">Accelerate/decelerate cam speed during movement</param>
        /// <param name="easeRotation"></param>
        /// <param name="waitForCompletion">If true it will wait until the interpolation finishes</param>
        /// <returns>Returns the created camera at the <see paramref="to"/> position.</returns>
        public static Camera Interpolate(this Camera from, Vector3 toPos, Rotator toRot, int time, bool easeLocation, bool easeRotation, bool waitForCompletion)
        {
            Camera toCam = new Camera(false);
            toCam.Position = toPos;
            toCam.Rotation = toRot;

            //SET_CAM_ACTIVE_WITH_INTERP(Cam camTo, Cam camFrom, int duration, BOOL easeLocation, BOOL easeRotation)
            NativeFunction.Natives.SET_CAM_ACTIVE_WITH_INTERP(toCam, from, time, easeLocation, easeRotation);
            if (waitForCompletion)
                GameFiber.Sleep(time);
            return toCam;
        }
    }
}
