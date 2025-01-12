using UnityEngine;

namespace DiplomaProject.Pool
{
    public class ProjectilePool : IPool
    {
        private Poolable _template;
        private int _initialCapacity;

        public ProjectilePool(Poolable template, int initialCapacity)
        {
            _template = template;
            _initialCapacity = initialCapacity;
        }

        public Poolable Fetch()
        {
            return Object.Instantiate(_template);
        }
    }
}