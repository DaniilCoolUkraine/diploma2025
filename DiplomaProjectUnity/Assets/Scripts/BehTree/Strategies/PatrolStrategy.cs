using System.Collections.Generic;
using System.Linq;
using DiplomaProject.General;
using UnityEngine;
using UnityEngine.AI;

namespace DiplomaProject.BehTree.Strategies
{
    public class PatrolStrategy : IStrategy
    {
        private NavMeshAgent _agent;
        private List<Transform> _waypoints;
        private float _speed;

        private int _currentIndex;
        private bool _isPathCalculated;

        public PatrolStrategy(NavMeshAgent agent, IEnumerable<Transform> waypoints, float speed = 3)
        {
            _agent = agent;
            _waypoints = waypoints.ToList();
            _speed = speed;
        }

        public Node.Status Process()
        {
            if (_currentIndex >= _waypoints.Count) return Node.Status.Success;
            
            var target = _waypoints[_currentIndex];
            _agent.SetDestination(target.position);
            _agent.speed = _speed;

            if (_isPathCalculated && _agent.remainingDistance < Constants.AGENT_STOP_DISTANCE)
            {
                _currentIndex++;
                _isPathCalculated = false;
            }

            if (_agent.pathPending)
            {
                _isPathCalculated = true;
            }
            
            return Node.Status.Running;
        }

        public void Reset()
        {
            _currentIndex = 0;
            _isPathCalculated = false;
        }
    }
}