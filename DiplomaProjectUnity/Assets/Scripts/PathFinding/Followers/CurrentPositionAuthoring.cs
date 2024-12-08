using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.PathFinding.Followers
{
    public class CurrentPositionAuthoring : MonoBehaviour
    {
        public class Baker : Baker<CurrentPositionAuthoring>
        {
            public override void Bake(CurrentPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CurrentPosition());
            }
        }
    }
    
    public struct CurrentPosition : IComponentData
    {
        public int2 Position;
    }
}