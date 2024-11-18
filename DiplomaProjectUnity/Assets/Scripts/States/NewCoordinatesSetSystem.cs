using DiplomaProject.PathFinding.Followers;
using Unity.Entities;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace DiplomaProject.States
{
    public partial struct NewCoordinatesSetSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            
            foreach (var (pathFindingParams, entity) 
                     in SystemAPI.Query<RefRW<PathFindingParams>>().WithAll<WaitForCoordinates>().WithEntityAccess())
            {
                pathFindingParams.ValueRW.StartPosition = pathFindingParams.ValueRO.EndPosition;
                pathFindingParams.ValueRW.EndPosition = new int2(Random.Range(0, 5), Random.Range(0, 5));

                entityManager.SetComponentEnabled<WaitForCoordinates>(entity, false);
            }
        }
    }
}