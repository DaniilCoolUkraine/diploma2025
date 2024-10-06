using UnityEngine;

namespace DiplomaProject.ThirdPersonController
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _speed = 25;
        
        [SerializeField] private Transform _hitParticles;
        
        private void Start()
        {
            _rigidbody.velocity = transform.forward * _speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            var particles = Instantiate(_hitParticles, transform.position, Quaternion.identity);
            Destroy(particles.gameObject, 3);
            Destroy(gameObject);
        }
    }
}