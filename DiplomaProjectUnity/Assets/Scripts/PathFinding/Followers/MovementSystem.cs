using DiplomaProject.General.Extensions;
using DiplomaProject.States;
using DiplomaProject.TileMap;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DiplomaProject.PathFinding.Followers
{
    public partial struct MovementSystem : ISystem
    {
        private const float THRESHOLD = 0.1f;
        
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, buffer, speed, currentPosition)in 
                     SystemAPI.Query<RefRW<LocalTransform>, DynamicBuffer<PathFindingParams>, RefRO<MovementSpeed>, RefRW<CurrentPosition>>()
                         .WithNone<IdleState>())
            {
                if (buffer.IsEmpty)
                    continue;
                
                var targetPosition = buffer[0].EndPosition;
                var worldTargetPosition = TileMapUtils.TileToWorldPosition(targetPosition.x, targetPosition.y).ToFloat3();

                var moveVector = math.normalize(worldTargetPosition - transform.ValueRO.Position) 
                                 * SystemAPI.Time.DeltaTime * speed.ValueRO.Speed;
                
                if (math.distance(worldTargetPosition, transform.ValueRO.Position) <= THRESHOLD)
                {
                    currentPosition.ValueRW.Position = targetPosition;
                    buffer.RemoveAt(0);
                }
                else
                {
                    // var job = new MoveJob { MoveVector = new float3(moveVector.x, moveVector.y, 0f) };
                    // job.Schedule();

                    var newPosition = transform.ValueRO.Position + new float3(moveVector.x, 0, moveVector.y);
                    transform.ValueRW = transform.ValueRO.WithPosition(newPosition);

                    // transform.ValueRW.Position = transform.ValueRO.Position + new float3(moveVector.x, moveVector.y, 0f);
                }
            }
        }
    }
    
    [BurstCompile]
    partial struct MoveJob : IJobEntity
    {
        public float3 MoveVector;

        public void Execute(ref LocalTransform transform)
        {
            transform = transform.WithPosition(transform.Position + MoveVector);
        }
    }
}