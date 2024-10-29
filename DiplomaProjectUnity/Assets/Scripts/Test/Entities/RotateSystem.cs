using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DiplomaProject.Test.Entities
{
    public partial struct RotateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RotateSpeed>();
        }
        
        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = SystemAPI.QueryBuilder().WithAll<RotateSpeed>().WithAll<LocalTransform>().Build();
            var job = new RotateJob{DeltaTime = SystemAPI.Time.DeltaTime};

            job.ScheduleParallel(entities);
        }
    }
    
    [BurstCompile]
    partial struct RotateJob : IJobEntity
    {
        public float DeltaTime;
        public void Execute(ref LocalTransform transform, in RotateSpeed speed)
        {
            transform = transform.RotateY(speed.Speed * DeltaTime);
        }
    }
}