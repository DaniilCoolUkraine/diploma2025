using System;
using DiplomaProject.BehTree;
using DiplomaProject.BehTree.Strategies;
using UnityEngine;
using UnityEngine.AI;

namespace DiplomaProject.EnemyAI
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform[] _points;
        [SerializeField] private float _speed;
        
        private BehaviourTree _testTree;

        private void Awake()
        {
            _testTree = new BehaviourTree("Test Tree");
            _testTree.AddChild(new Leaf("Patrol", new PatrolStrategy(_agent, _points, _speed)));
        }

        private void Update()
        {
            _testTree.Process();
        }
    }
}