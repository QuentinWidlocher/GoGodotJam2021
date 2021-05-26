using Godot;

namespace Helpers
{
    public static class NumericExtensions
    {
        public static float Turn(this float instance) => instance * Mathf.Tau;
        public static float Turn(this int instance) => instance * Mathf.Tau;
        public static double Turn(this double instance) => instance * Mathf.Tau;
    }
}