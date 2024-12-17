using DiplomaProject.General.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DiplomaProject.PathFinding.Followers
{
    public class CurrentPositionAuthoring : MonoBehaviour
    {
        [SerializeField] private Vector2Int currentPosition;
        
        public class Baker : Baker<CurrentPositionAuthoring>
        {
            public override void Bake(CurrentPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CurrentPosition {Position = authoring.currentPosition.ToInt2()});
            }
        }

        public void SetCurrentPosition(Vector2Int position)
        {
            currentPosition = position;
        }
    }
    
    public struct CurrentPosition : IComponentData
    {
        public int2 Position;
    }
}