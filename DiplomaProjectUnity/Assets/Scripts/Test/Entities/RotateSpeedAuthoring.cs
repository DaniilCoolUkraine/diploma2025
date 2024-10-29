using Unity.Entities;
using UnityEngine;

namespace DiplomaProject.Test.Entities
{
    public class RotateSpeedAuthoring : MonoBehaviour
    {
        private class Baker : Baker<RotateSpeedAuthoring>
        {
            public override void Bake(RotateSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RotateSpeed { Speed = Random.Range(-5, 5) });
            }
        }
    }
    
    public struct RotateSpeed : IComponentData
    {
        public float Speed;
    }
}