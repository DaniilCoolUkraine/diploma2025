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
                AddComponent(entity, new MovementSpeed() { Speed = Random.Range(3, 5) + Random.Range(0.1f, 0.5f) });
            }
        }
    }

    public struct MovementSpeed : IComponentData
    {
        public float Speed;
    }
}