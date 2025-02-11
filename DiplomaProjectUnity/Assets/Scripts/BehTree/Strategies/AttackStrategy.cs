using DiplomaProject.AttackUtils;
using DiplomaProject.EventSystem.Core;
using DiplomaProject.EventSystem.Extendables;
using UnityEngine;
using Zenject;

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
        private const float ANIMATION_THROW_TIME = 0.7f;
        private float _currentAnimationTime = 0;

        private int _maxAmmoCount = 0;
        private int _currentAmmoCount = 0;
        private Transform _projectileSpawn;
        [Inject] private IProjectileSpawner _projectileSpawner;
        
        public AttackStrategy(Transform target, Transform transform, Animator animator, Transform projectileSpawn, int maxAmmoCount,
            DiContainer container)
        {
            _target = target;
            _animator = animator;
            _transform = transform;
            _projectileSpawn = projectileSpawn;

            _maxAmmoCount = maxAmmoCount;
            _currentAmmoCount = maxAmmoCount;

            _animIDThrowingLayer = _animator.GetLayerIndex("Throw Layer");
            
            container.Inject(this);
            GlobalEvents.AddListener<RefillEvent>(OnRefill);
        }

        public Node.Status Process()
        {
            if (_currentAmmoCount <= 0)
                return Node.Status.Failure;
            
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
                SetupProjectile();
                _currentAmmoCount--;
                GlobalEvents.Publish<FireEvent>(new FireEvent(_currentAmmoCount));
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            _animator.SetLayerWeight(_animIDThrowingLayer, 0);
            _animator.ResetTrigger(_animIDThrow);
            _currentAnimationTime = 0;

            _animStarted = false;
            _attacked = false;
        }

        private void OnRefill(RefillEvent @event)
        {
            if (@event.EntityToRefill == _transform)
            {
                _currentAmmoCount = _maxAmmoCount;
            }
        }
        
        private void SetupProjectile()
        {
            var projectile = _projectileSpawner.SpawnProjectile(_transform.position, _target.position);

            projectile.SetStartPosition(_projectileSpawn.position);
            projectile.SetTarget(_target.position);
            projectile.Run();
        }
    }
}