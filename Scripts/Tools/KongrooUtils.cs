namespace KongrooTools
{
    public class Utils
    {
        // https://victorkarp.com/godot-engine-how-to-remap-a-range-of-numbers/
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax) =>
        outMin + ((value - inMin) / (inMax - inMin) * (outMax - outMin));
    }
}