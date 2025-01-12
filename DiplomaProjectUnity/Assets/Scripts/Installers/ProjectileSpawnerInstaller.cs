using DiplomaProject.AttackUtils;
using UnityEngine;
using Zenject;

namespace DiplomaProject.Installers
{
    public class ProjectileSpawnerInstaller : MonoInstaller
    {
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        public override void InstallBindings()
        {
            Container.Bind<IProjectileSpawner>().To<ProjectileSpawner>().FromInstance(_projectileSpawner).AsSingle();
        }
    }
}