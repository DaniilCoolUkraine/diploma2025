using DiplomaProject.AttackUtils.Projectiles;
using DiplomaProject.Pool;
using UnityEngine;

namespace DiplomaProject.AttackUtils
{
    public class ProjectileSpawner : MonoBehaviour, IProjectileSpawner
    {
        [SerializeField] private Projectile _projectileTemplate;
        [SerializeField] private int _poolSize;
        
        private IPool _projectilePool;
        
        private void Awake()
        {
            _projectilePool = new ProjectilePool(_projectileTemplate, _poolSize);
        }

        public IProjectile SpawnProjectile(Vector3 start, Vector3 end)
        {
            return _projectilePool.Fetch() as IProjectile;
        }
    }
}