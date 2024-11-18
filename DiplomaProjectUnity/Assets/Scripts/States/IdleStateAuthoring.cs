using Unity.Entities;
using UnityEngine;

namespace DiplomaProject.States
{
    public class IdleStateAuthoring : MonoBehaviour
    {
        public class Baker : Baker<IdleStateAuthoring>
        {
            public override void Bake(IdleStateAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new IdleState()
                {
                    IdleTime = Random.Range(1, 2)
                });
            }
        }
    }

    public struct IdleState : IComponentData, IEnableableComponent
    {
        public float IdleTime;
    }
}