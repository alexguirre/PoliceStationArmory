namespace PoliceStationArmoury
{
    // System
    using System.Drawing;

    // RPH
    using Rage;

    // PSArmoury
    using PoliceStationArmoury.Types;

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
    }
}
