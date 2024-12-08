using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.General.Extensions
{
    public static class VectorExtensions
    {
        public static float3 ToFloat3(this Vector3 vector3)
        {
            return new float3(vector3.x, vector3.y, vector3.z);
        }

        public static int2 ToInt2(this Vector2Int vector2)
        {
            return new int2(vector2.x, vector2.y);
        }
    }
}