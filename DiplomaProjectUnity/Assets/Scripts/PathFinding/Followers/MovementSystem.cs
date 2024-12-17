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
            foreach (var (transform, buffer, speed, currentPosition) in 
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

                    var newPosition = transform.ValueRO.Position + new float3(moveVector.x, 0, moveVector.z);
                    transform.ValueRW = transform.ValueRO.WithPosition(newPosition);
                    
                    // transform.ValueRW.Position = transform.ValueRO.Position + new float3(moveVector.x, moveVector.y, 0f);
                    
                    var currentPosition3D = new float3(currentPosition.ValueRO.Position.x, 0, currentPosition.ValueRO.Position.y);
                    var targetPosition3D = new float3(targetPosition.x, 0, targetPosition.y);

                    var directionToTarget = targetPosition3D - currentPosition3D;

                    if (!math.all(directionToTarget == float3.zero))
                    {
                        var lookDirection = math.normalize(directionToTarget);
                        var targetRotation = quaternion.LookRotationSafe(lookDirection, math.up());

                        var currentRotation = transform.ValueRO.Rotation;
                        var smoothedRotation = math.slerp(currentRotation, targetRotation, SystemAPI.Time.DeltaTime * 8);

                        transform.ValueRW = transform.ValueRO.WithRotation(smoothedRotation);
                    }
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