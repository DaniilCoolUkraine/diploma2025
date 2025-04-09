using UnityEngine;
using UnityEngine.AI;

namespace DiplomaProject.Goap.Strategies
{
    public class WanderStrategy : IActionStrategy
    {
        private readonly NavMeshAgent _agent;
        private readonly float _wanderRadius;

        public bool CanPerform => !Complete;
        public bool Complete => _agent.remainingDistance <= 1.5f && !_agent.pathPending;
        
        public WanderStrategy(NavMeshAgent navMeshAgent, float radius)
        {
            _agent = navMeshAgent;
            _wanderRadius = radius;
        }

        public void Start()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * _wanderRadius;
                randomDirection.y = 0;

                if (NavMesh.SamplePosition(_agent.transform.position + randomDirection, out var navMeshHit, _wanderRadius, 1))
                {
                    _agent.SetDestination(navMeshHit.position);
                    return;
                }
            }
        }
    }
}