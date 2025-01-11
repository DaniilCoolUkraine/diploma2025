using Unity.Entities;
using UnityEngine;

namespace DiplomaProject.States
{
    public class WaitForCoordinatesAuthoring : MonoBehaviour
    {
        public class Baker : Baker<WaitForCoordinatesAuthoring>
        {
            public override void Bake(WaitForCoordinatesAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new WaitForCoordinates());
            }
        }
    }
    
    public struct WaitForCoordinates : IComponentData, IEnableableComponent
    {
    }
}