namespace PoliceStationArmory
{
    // System
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;

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


        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this ISpatial spatial, ISpatial towardsSpatial)
        {
            return GetHeadingTowards(spatial, towardsSpatial.Position);
        }


        /// <summary>
        /// Gets the heading towards a position
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsPosition">Position to face to</param>
        /// <returns>the heading towards a position</returns>
        public static float GetHeadingTowards(this ISpatial spatial, Vector3 towardsPosition)
        {
            return GetHeadingTowards(spatial.Position, towardsPosition);
        }


        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this Vector3 position, Vector3 towardsPosition)
        {
            Vector3 directionFromEntityToPosition = (towardsPosition - position);
            directionFromEntityToPosition.Normalize();

            float heading = MathHelper.ConvertDirectionToHeading(directionFromEntityToPosition);
            return heading;
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this Vector3 position, ISpatial towardsSpatial)
        {
            return GetHeadingTowards(position, towardsSpatial.Position);
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

        ///// <summary>
        ///// Transition from the <see paramref="from"/> camera to the <see paramref="to"/> position. Returns and activates the created camera at the <see paramref="to"/> position.
        ///// </summary>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <param name="time">The time that the interpolation takes</param>
        ///// <param name="easeLocation">Accelerate/decelerate cam speed during movement</param>
        ///// <param name="easeRotation"></param>
        ///// <param name="waitForCompletion">If true it will wait until the interpolation finishes</param>
        ///// <returns>Returns the created camera at the <see paramref="to"/> position.</returns>
        //public static Camera Interpolate(this Camera from, Vector3 to, int time, bool easeLocation, bool easeRotation, bool waitForCompletion)
        //{
        //    Camera toCam = new Camera(false);
        //    toCam.Position = to;

        //    //SET_CAM_ACTIVE_WITH_INTERP(Cam camTo, Cam camFrom, int duration, BOOL easeLocation, BOOL easeRotation)
        //    NativeFunction.Natives.SET_CAM_ACTIVE_WITH_INTERP(toCam, from, time, easeLocation, easeRotation);
        //    if (waitForCompletion)
        //        GameFiber.Sleep(time);
        //    return toCam;
        //}

        ///// <summary>
        ///// Transition from the <see paramref="from"/> camera to the <see paramref="to"/> position. Returns and activates the created camera at the <see paramref="to"/> position.
        ///// </summary>
        ///// <param name="from"></param>
        ///// <param name="toPos">The position where create the new camera</param>
        ///// <param name="toRot">The rotation of the new camera</param>
        ///// <param name="time">The time that the interpolation takes</param>
        ///// <param name="easeLocation">Accelerate/decelerate cam speed during movement</param>
        ///// <param name="easeRotation"></param>
        ///// <param name="waitForCompletion">If true it will wait until the interpolation finishes</param>
        ///// <returns>Returns the created camera at the <see paramref="to"/> position.</returns>
        //public static Camera Interpolate(this Camera from, Vector3 toPos, Rotator toRot, int time, bool easeLocation, bool easeRotation, bool waitForCompletion)
        //{
        //    Camera toCam = new Camera(false);
        //    toCam.Position = toPos;
        //    toCam.Rotation = toRot;

        //    //SET_CAM_ACTIVE_WITH_INTERP(Cam camTo, Cam camFrom, int duration, BOOL easeLocation, BOOL easeRotation)
        //    NativeFunction.Natives.SET_CAM_ACTIVE_WITH_INTERP(toCam, from, time, easeLocation, easeRotation);
        //    if (waitForCompletion)
        //        GameFiber.Sleep(time);
        //    return toCam;
        //}

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = Globals.Random.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

        public static T GetRandomElement<T>(this IList<T> list, bool shuffle = false)
        {
            if (list == null || list.Count <= 0)
                return default(T);

            if (shuffle) list.Shuffle();
            return list[Globals.Random.Next(list.Count)];
        }

        //public static T GetRandomElement<T>(this IEnumerable<T> enumarable, bool shuffle = false)
        //{
        //    if (enumarable == null || enumarable.Count() <= 0)
        //        return default(T);

        //    T[] array = enumarable.ToArray();
        //    return GetRandomElement(array, shuffle);
        //}
    }
}
