using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.PathFinding.Followers
{
    public class PathFindingParamsAuthoring : MonoBehaviour
    {
        public class PathFindingBufferBaker : Baker<PathFindingParamsAuthoring>
        {
            public override void Bake(PathFindingParamsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<PathFindingParams>(entity);
            }
        }
    }
    
    public struct PathFindingParams : IBufferElementData
    {
        public int2 StartPosition;
        public int2 EndPosition;
    }
}