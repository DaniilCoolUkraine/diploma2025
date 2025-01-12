using UnityEngine;

namespace DiplomaProject.AttackUtils.Projectiles
{
    public interface IProjectile
    {
        public IProjectile SetPosition(Vector3 position);
        public IProjectile SetTarget(Vector3 target);
        public IProjectile Run();
    }
}