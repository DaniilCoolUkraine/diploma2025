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
        
        private BehaviourTree _ai;
        
        private void Awake()
        {
            _ai = new BehaviourTree("Main Tree");

            var sequence = new LoopSequence("Follow target sequence")
                .AddChild(new Leaf("Check target", new ConditionStrategy(() => _target.gameObject.activeSelf)))
                .AddChild(new Leaf("Follow target", new FollowTransformStrategy(_agent, _target, _animator, _speed)))
                .AddChild(new Leaf("Log message", new ActionStrategy(() => Debug.Log("sequence end"))))
                ;

            _ai.AddChild(sequence);
        }

        private void Update()
        {
            _ai.Process();
        }
    }
}