using Godot;

namespace KongrooTools
{
    public class Utils
    {
        // https://victorkarp.com/godot-engine-how-to-remap-a-range-of-numbers/
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax) =>
        outMin + ((value - inMin) / (inMax - inMin) * (outMax - outMin));
    }

    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public override string ToString()
        {
            return $"Origin: {Origin}, Direction: {Direction}";
        }

    }
}