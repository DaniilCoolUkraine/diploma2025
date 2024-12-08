using Unity.Entities;
using UnityEngine;

namespace DiplomaProject.PathFinding.Followers
{
    public class MovementSpeedAuthoring : MonoBehaviour
    {
        public class Baker : Baker<MovementSpeedAuthoring>
        {
            public override void Bake(MovementSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MovementSpeed(){Speed = 4});
            }
        }
    }

    public struct MovementSpeed : IComponentData
    {
        public float Speed;
    }
}