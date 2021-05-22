using Godot;

namespace Helpers
{
    public static class TransformExtensions
    {
        public static Transform LookingAtWithY(this Transform instance, Vector3 newY)
        {
            var newInstance = instance;
            newInstance.basis.y = newY;
            newInstance.basis.x = -newInstance.basis.z.Cross(newY);
            newInstance.basis = newInstance.basis.Orthonormalized();
            return newInstance;
        }
    }
}