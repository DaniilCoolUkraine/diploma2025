using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.General.Extensions
{
    public static class Vector3Extensions
    {
        public static float3 ToFloat3(this Vector3 vector3)
        {
            return new float3(vector3.x, vector3.y, vector3.z);
        }
    }
}