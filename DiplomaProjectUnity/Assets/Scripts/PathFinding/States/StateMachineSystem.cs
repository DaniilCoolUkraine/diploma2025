using DiplomaProject.PathFinding.Followers;
using DiplomaProject.States;
using Unity.Entities;
using UnityEngine;

namespace DiplomaProject.PathFinding.States
{
    public partial struct StateMachineSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;

            foreach (var (buffer, entity)
                     in SystemAPI.Query<DynamicBuffer<PathFindingParams>>().WithEntityAccess())
            {
                if (buffer.IsEmpty)
                {
                    entityManager.SetComponentEnabled<IdleState>(entity, true);
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