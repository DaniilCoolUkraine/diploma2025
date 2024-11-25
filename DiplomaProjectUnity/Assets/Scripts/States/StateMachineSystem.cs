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

            foreach (var (pathFindingParams, entity)
                     in SystemAPI.Query<RefRW<PathFindingParams>>().WithEntityAccess())
            {
                if (pathFindingParams.ValueRO.CurrentProgress >= 1)
                {
                    entityManager.SetComponentEnabled<IdleState>(entity, true);
                    pathFindingParams.ValueRW.CurrentProgress = 0;
                }
            }

            foreach (var (idleState, entity)
                     in SystemAPI.Query<RefRW<IdleState>>().WithEntityAccess())
            {
                if (idleState.ValueRO.IdleTime >= 0)
                {
                    idleState.ValueRW.IdleTime -= SystemAPI.Time.DeltaTime;
                }
                else
                {
                    idleState.ValueRW.IdleTime = Random.Range(2, 4);
                    entityManager.SetComponentEnabled<IdleState>(entity, false);
                    entityManager.SetComponentEnabled<WaitForCoordinates>(entity, true);
                }
            }
        }
    }
}