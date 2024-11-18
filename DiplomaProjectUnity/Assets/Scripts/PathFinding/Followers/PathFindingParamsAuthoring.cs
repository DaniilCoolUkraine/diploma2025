using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.PathFinding.Followers
{
    public class PathFindingParamsAuthoring : MonoBehaviour
    {
        public int2 StartPosition;
        public int2 EndPosition;
        
        public class Baker : Baker<PathFindingParamsAuthoring>
        {
            public override void Bake(PathFindingParamsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PathFindingParams()
                {
                    StartPosition = authoring.StartPosition,
                    EndPosition = authoring.EndPosition,
                    CurrentProgress = 0
                });
            }
        }
    }
    
    public struct PathFindingParams : IComponentData
    {
        public int2 StartPosition;
        public int2 EndPosition;

        public float CurrentProgress;
    }
}