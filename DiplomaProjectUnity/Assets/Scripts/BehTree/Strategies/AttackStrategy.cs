using UnityEngine;

namespace DiplomaProject.BehTree.Strategies
{
    public class AttackStrategy : IStrategy
    {
        private Transform _target;
        private Transform _transform;
        private Animator _animator;

        private int _animIDThrowingLayer = 0;
        private int _animIDThrow = Animator.StringToHash("Throw");
        
        private bool _animStarted = false;
        private const float ANIMATION_TIME = 1.8f;
        private float _currentAnimationTime = 0;
        
        public AttackStrategy(Transform target, Transform transform, Animator animator)
        {
            _target = target;
            _animator = animator;
            _transform = transform;

            _animIDThrowingLayer = _animator.GetLayerIndex("Throw Layer");
        }

        public Node.Status Process()
        {
            _animator.SetLayerWeight(_animIDThrowingLayer, 1);
            _transform.LookAt(_target);
            _animator.transform.localRotation = Quaternion.identity;

            if (!_animStarted)
            {
                _animator.SetTrigger(_animIDThrow);
                _animStarted = true;
            }

            if (_animStarted)
            {
                if (_currentAnimationTime >= ANIMATION_TIME)
                {
                    return Node.Status.Success;
                }

                _currentAnimationTime += Time.deltaTime;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            _animator.SetLayerWeight(_animIDThrowingLayer, 0);
            _animator.ResetTrigger(_animIDThrow);
            _animStarted = false;
            _currentAnimationTime = 0;
        }
    }
}