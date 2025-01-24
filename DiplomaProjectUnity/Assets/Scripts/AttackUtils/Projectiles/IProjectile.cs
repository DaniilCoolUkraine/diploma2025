using System;
using UnityEngine;

namespace DiplomaProject.AttackUtils.Projectiles
{
    public interface IProjectile
    {
        public IProjectile SetStartPosition(Vector3 position);
        public IProjectile SetTarget(Vector3 target);
        public IProjectile Run();
    }
}