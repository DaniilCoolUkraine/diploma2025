using System;
using DiplomaProject.BehTree;
using DiplomaProject.BehTree.Strategies;
using DiplomaProject.EventSystem.Core;
using DiplomaProject.EventSystem.Extendables;
using DiplomaProject.Interactable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace DiplomaProject.EnemyAI
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _runSpeed;
        [SerializeField] private int _ammoCount = 5;

        [SerializeField, Required] private Transform _target;
        [SerializeField, Required] private Transform _refillPoint;

        [SerializeField, Required] private Transform _projectileSpawnPoint;
        
        [Inject] private DiContainer _container;
        
        private BehaviourTree _ai;

        private int _currentAmmo;

        private void Awake()
        {
            _currentAmmo = _ammoCount;
            
            SetupBrain();

            GlobalEvents.AddListener<FireEvent>(OnEnemyFire);
            GlobalEvents.AddListener<RefillEvent>(OnRefill);
        }
        
        private void Update()
        {
            _ai.Process();
        }

        private void OnDestroy()
        {
            GlobalEvents.RemoveListener<FireEvent>(OnEnemyFire);
            GlobalEvents.RemoveListener<RefillEvent>(OnRefill);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact(transform);
            }
        }

        private void SetupBrain()
        {
            _ai = new BehaviourTree("Main Tree");

            var attackSequence = new LoopSequence("Follow target sequence")
                    .AddChild(new Leaf("Check target", new ConditionStrategy(() => _target.gameObject.activeSelf)))
                    .AddChild(new Leaf("Follow target", new FollowTransformStrategy(_agent, _target, _animator, _walkSpeed, _runSpeed)))
                    .AddChild(new Leaf("Attack", new AttackStrategy(_target, transform, _animator, _projectileSpawnPoint, _ammoCount, _container)))
                ;
            var checkAmmoSequence = new Sequence("Check Ammo sequence")
                .AddChild(new Leaf("Check Ammo", new ConditionStrategy(CheckAmmo())))
                .AddChild(new Leaf("Go to refill", new PatrolStrategy(_agent, new[] { _refillPoint }, _runSpeed)));

            var mainLoop = new LoopSelector("Main Loop selector")
                .AddChild(checkAmmoSequence)
                .AddChild(attackSequence);

            _ai.AddChild(mainLoop)
                .AddChild(new Leaf("End log", new ActionStrategy(() => Debug.Log("End log"))));
        }

        private Func<bool> CheckAmmo()
        {
            Debug.Log($"Checking Ammo {_currentAmmo}");
            return () => _currentAmmo <= 0;
        }

        private void OnEnemyFire(FireEvent ev)
        {
            _currentAmmo = ev.AmmoCount;
        }
        
        private void OnRefill(RefillEvent @event)
        {
            if (@event.EntityToRefill == transform)
            {
                _currentAmmo = _ammoCount;
            }
        }
    }
}