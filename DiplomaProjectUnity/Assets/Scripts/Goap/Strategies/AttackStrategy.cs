using ImprovedTimers;
using UnityEngine;

namespace DiplomaProject.Goap.Strategies
{
    public class AttackStrategy : IActionStrategy
    {
        private Animator _animator;

        private readonly CountdownTimer _timer;
        
        public AttackStrategy(Animator animator)
        {
            _animator = animator;

            _timer = new CountdownTimer(2f);
            _timer.OnTimerStart += () => Complete = false;
            _timer.OnTimerStop += () => Complete = true;
        }

        public bool CanPerform => true;
        public bool Complete { get; private set; }

        public void Start()
        {
            _timer.Start();
        }

        public void Update(float deltaTime)
        {
            _timer.Tick();
        }
    }
}