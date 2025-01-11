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
        private bool _attacked = false;

        private const float ANIMATION_TIME = 2.05f;
        private const float ANIMATION_THROW_TIME = 0.24f;
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
                    return Node.Status.Success;

                _currentAnimationTime += Time.deltaTime;
            }

            if (_currentAnimationTime >= ANIMATION_THROW_TIME && !_attacked)
            {
                _attacked = true;
                
            }
            
            return Node.Status.Running;
        }

        public void Reset()
        {
            _animator.SetLayerWeight(_animIDThrowingLayer, 0);
            _animator.ResetTrigger(_animIDThrow);
            _currentAnimationTime = 0;

            _animStarted = false;
        }
    }
}