using System.Linq;
using DiplomaProject.Pool;
using UnityEngine;

namespace DiplomaProject.AttackUtils.Projectiles
{
    public class Projectile : Poolable, IProjectile
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _maxTime;

        [SerializeField] private Rigidbody _rigidbody;

        [SerializeField] private Transform[] _hitEffects;
        
        private Vector3 _targetPosition;

        public IProjectile SetStartPosition(Vector3 position)
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

        private void OnCollisionEnter(Collision other)
        {
            var contact = other.contacts.FirstOrDefault();
            
            // var hitPoint = contact.point;
            var hitNormal = contact.normal;

            Instantiate(_hitEffects[Random.Range(0, _hitEffects.Length)], transform.position, Quaternion.LookRotation(hitNormal) * Quaternion.Euler(90, 0, 0));
            Destroy(gameObject);
        }
    }
}