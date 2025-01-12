using System;
using DiplomaProject.Pool;
using UnityEngine;

namespace DiplomaProject.AttackUtils.Projectiles
{
    public class Projectile : Poolable, IProjectile
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _maxTime;

        [SerializeField] private Rigidbody _rigidbody;

        private Vector3 _targetPosition;

        public IProjectile SetPosition(Vector3 position)
        {
            transform.position = position;
            return this;
        }

        public IProjectile SetTarget(Vector3 target)
        {
            _targetPosition = target;
            return this;
        }

        public IProjectile Run()
        {
            _rigidbody.velocity = (_targetPosition - transform.position).normalized * _speed;
            return this;
        }

        // private void OnCollisionEnter(Collision other)
        // {
        //     Destroy(gameObject);
        // }
    }
}