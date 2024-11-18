using DiplomaProject.States;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DiplomaProject.PathFinding.Followers
{
    public partial struct MovementSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, pathFindingParams, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PathFindingParams>, RefRO<MovementSpeed>>().WithNone<IdleState>())
            {
                pathFindingParams.ValueRW.CurrentProgress += SystemAPI.Time.DeltaTime * speed.ValueRO.Speed;

                var pathRo = pathFindingParams.ValueRO;
                var moveVector = math.lerp(pathRo.StartPosition, pathRo.EndPosition, pathRo.CurrentProgress);

                transform.ValueRW.Position = new float3(moveVector.x, moveVector.y, 0);
            }
        }
    }
    
    [BurstCompile]
    partial struct MoveJob : IJobEntity
    {
        public float DeltaTime;
        public float3 MoveVector;

        public void Execute(ref LocalTransform transform)
        {
            transform = transform.WithPosition(transform.Position + MoveVector);
        }
    }
}