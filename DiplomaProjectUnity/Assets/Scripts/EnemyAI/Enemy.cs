using DiplomaProject.BehTree;
using DiplomaProject.BehTree.Strategies;
using UnityEngine;
using UnityEngine.AI;

namespace DiplomaProject.EnemyAI
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        
        [SerializeField] private Transform[] _points;
        [SerializeField] private float _speed;

        [SerializeField] private Transform _target;
        
        private BehaviourTree _testTree;
        
        private void Awake()
        {
            _testTree = new BehaviourTree("Test Tree");

            // _testTree.AddChild(new Leaf("Patrol", new PatrolStrategy(_agent, _points, _speed)));

            var sequence = new Sequence("Go to treasure seq")
                .AddChild(new Leaf("Check target", new ConditionStrategy(() => _target.gameObject.activeSelf)))
                .AddChild(new Leaf("Follow target", new FollowTransformStrategy(_agent, _target, _animator, _speed)))
                ;

            _testTree.AddChild(sequence);
        }

        private void Update()
        {
            _testTree.Process();
        }
    }
}