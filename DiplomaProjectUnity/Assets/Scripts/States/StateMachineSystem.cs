using DiplomaProject.PathFinding.Followers;
using Unity.Entities;
using UnityEngine;

namespace DiplomaProject.States
{
    public partial struct StateMachineSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            
            foreach (var (pathFindingParams, idleState, entity) 
                     in SystemAPI.Query<RefRW<PathFindingParams>, RefRW<IdleState>>()
                         .WithEntityAccess()
                         .WithOptions(EntityQueryOptions.IncludeDisabledEntities))
            {
                if (pathFindingParams.ValueRO.CurrentProgress >= 1)
                {
                    entityManager.SetComponentEnabled<IdleState>(entity, true);
                    entityManager.SetComponentData(entity, new IdleState() { IdleTime = Random.Range(1f, 2f) });
                    pathFindingParams.ValueRW.CurrentProgress = 0;
                }

                if (idleState.ValueRO.IdleTime >= 0)
                {
                    idleState.ValueRW.IdleTime -= SystemAPI.Time.DeltaTime;
                }
                else
                {
                    entityManager.SetComponentEnabled<IdleState>(entity, false);
                    entityManager.SetComponentEnabled<WaitForCoordinates>(entity, true);
                }
            }
        }
    }
}