using DiplomaProject.AttackUtils.Projectiles;
using UnityEngine;

namespace DiplomaProject.AttackUtils
{
    public interface IProjectileSpawner
    {
        public IProjectile SpawnProjectile(Vector3 start, Vector3 end);
    }
}