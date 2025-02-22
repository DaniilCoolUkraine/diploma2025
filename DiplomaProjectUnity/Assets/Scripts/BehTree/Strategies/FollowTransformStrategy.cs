﻿using DiplomaProject.General;
using UnityEngine;
using UnityEngine.AI;

namespace DiplomaProject.BehTree.Strategies
{
    public class FollowTransformStrategy : IStrategy
    {
        private const int FRAMES_BETWEEN_UPDATE = 10;
        private const int RUN_DISTANCE = 10;
        
        private NavMeshAgent _agent;
        private Transform _target;
        private Animator _animator;

        private float _walkSpeed;
        private float _runSpeed;

        private int _framesWaiting = FRAMES_BETWEEN_UPDATE;
        
        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        
        public FollowTransformStrategy(NavMeshAgent agent, Transform target, Animator animator, float walkSpeed, float runSpeed)
        {
            _agent = agent;
            _target = target;
            _animator = animator;
            _walkSpeed = walkSpeed;
            _runSpeed = runSpeed;
        }
        
        public Node.Status Process()
        {
            UpdateTarget();

            if (_agent.hasPath && _agent.remainingDistance <= Constants.AGENT_FOLLOW_DISTANCE)
            {
                Reset();
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            _agent.ResetPath();
            _framesWaiting = FRAMES_BETWEEN_UPDATE;
            _animator.SetFloat(_animIDSpeed, 0);
            _animator.SetFloat(_animIDMotionSpeed, 0);
        }
        
        private void UpdateTarget()
        {
            if (_framesWaiting >= FRAMES_BETWEEN_UPDATE)
            {
                _framesWaiting = 0;
                
                _agent.SetDestination(_target.position);
                _agent.speed = Vector3.Distance(_target.position, _agent.transform.position) >= RUN_DISTANCE ? _runSpeed : _walkSpeed;
                _animator.SetFloat(_animIDSpeed, _walkSpeed);
                _animator.SetFloat(_animIDMotionSpeed, 1);
            }

            _framesWaiting++;
        }
    }
}