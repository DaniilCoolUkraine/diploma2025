using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DiplomaProject.Goap.Planner;
using DiplomaProject.Goap.Strategies;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace DiplomaProject.Goap
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class GoapAgent : MonoBehaviour
    {
        private const string NOTHING_KEY = "Nothing";
        private const string AGENT_MOVING_KEY = "AgentMoving";
        private const string AGENT_IDLE_KEY = "AgentIdle";
        private const string AGENT_STAMINA_LOW_KEY = "AgentStaminaLow";
        private const string AGENT_HEALTH_LOW_KEY = "AgentHealthLow";
        private const string AGENT_HEALTHY_KEY = "AgentHealthy";
        private const string AGENT_RESTED_KEY = "AgentRested";
        private const string AGENT_AT_DOOR_ONE_KEY = "AgentAtDoorOne";
        private const string AGENT_AT_DOOR_TWO_KEY = "AgentAtDoorTwo";
        private const string AGENT_AT_RESTING_POINT_KEY = "AgentAtRestingPoint";
        private const string AGENT_AT_FOOD_POINT_KEY = "AgentAtFoodPoint";
        private const string ENEMY_IN_CHASE_RANGE_KEY = "EnemyInChaseRange";
        private const string ENEMY_IN_ATTACK_RANGE_KEY = "EnemyInAttackRange";
        private const string ATTACKING_ENEMY_KEY = "AttackingEnemy";

        [Header("Sensors")]
        [SerializeField, Required] private Sensor _chaseSensor;
        [SerializeField, Required] private Sensor _attackSensor;

        [Header("Points of interest")]
        [SerializeField, Required] private Transform _restingPosition;
        [SerializeField, Required] private Transform _foodCourt;
        [SerializeField, Required] private Transform _doorOne;
        [SerializeField, Required] private Transform _doorTwo;
        
        [Header("Components")]
        [SerializeField, Required] private NavMeshAgent _navMeshAgent;
        [SerializeField, Required] private Animator _animator;
        [SerializeField, Required] private Rigidbody _rb;

        [Header("Stats")] 
        [SerializeField] private float _health;
        [SerializeField] private float _stamina;

        private CountdownTimer _statsTimer;
        
        private GameObject _target;
        private Vector3 _destination;

        private AgentGoal _lastGoal;
        private AgentGoal _currentGoal;
        
        private ActionPlan _actionPlan;
        private AgentAction _currentAction;
        
        private Dictionary<string, AgentBelief> _beliefs;
        private HashSet<AgentAction> _actions;
        private HashSet<AgentGoal> _goals;

        private IGoapPlanner _planner;

        public HashSet<AgentAction> Actions => _actions;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            if (_rb != null)
                _rb.freezeRotation = true;
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        private void Awake()
        {
            _planner = new GoapPlanner();
        }

        private void OnEnable()
        {
            _chaseSensor.OnTargetChanged += OnTargetChanged;
        }

        private void Start()
        {
            SetupTimers();
            SetupBeliefs();
            SetupActions();
            SetupGoals();
        }

        private void Update()
        {
            _statsTimer.Tick();

            if (_currentAction == null)
            {
                CalculatePlan();

                if (_actionPlan != null && _actionPlan.Actions.Count > 0)
                {
                    _navMeshAgent.ResetPath();
                
                    _currentGoal = _actionPlan.ActionGoal;
                    _currentAction = _actionPlan.Actions.Pop();

                    if (_currentAction.Preconditions.All(b => b.Evaluate()))
                    {
                        _currentAction.Start();
                    }
                    else
                    {
                        _currentAction = null;
                        _currentGoal = null;
                    }                    
                }
            }
            
            if (_actionPlan != null && _currentAction != null)
            {
                _currentAction.Update(Time.deltaTime);

                if (_currentAction.Complete)
                {
                    _currentAction.Stop();
                    _currentAction = null;

                    if (_actionPlan.Actions.Count == 0)
                    {
                        _lastGoal = _currentGoal;
                        _currentGoal = null;
                    }
                }
            }
        }

        private void OnDisable()
        {
            _chaseSensor.OnTargetChanged -= OnTargetChanged;

            _currentAction = null;
            _currentGoal = null;
        }

        private void OnTargetChanged()
        {
            _currentAction = null;
            _currentGoal = null;
        }

        private void SetupTimers()
        {
            _statsTimer = new CountdownTimer(5f);
            _statsTimer.OnTimerStop += () =>
            {
                UpdateStats();
                _statsTimer.Start();
            };
            
            _statsTimer.Start();
        }
        
        private void SetupBeliefs()
        {
            _beliefs = new Dictionary<string, AgentBelief>();
            var factory = new BeliefFactory(this, _beliefs);
            
            factory.AddBelief(NOTHING_KEY, () => false);
            factory.AddBelief(AGENT_IDLE_KEY, () => !_navMeshAgent.hasPath);
            factory.AddBelief(AGENT_MOVING_KEY, () => _navMeshAgent.hasPath);
            factory.AddBelief(AGENT_HEALTH_LOW_KEY, () => _health < 30);
            factory.AddBelief(AGENT_HEALTHY_KEY, () => _health >= 60);
            factory.AddBelief(AGENT_STAMINA_LOW_KEY, () => _stamina < 30);
            factory.AddBelief(AGENT_RESTED_KEY, () => _stamina >= 80);
            
            factory.AddBelief(AGENT_AT_DOOR_ONE_KEY, _doorOne, 3f);
            factory.AddBelief(AGENT_AT_DOOR_TWO_KEY, _doorTwo, 3f);
            factory.AddBelief(AGENT_AT_RESTING_POINT_KEY, _restingPosition, 3f);
            factory.AddBelief(AGENT_AT_FOOD_POINT_KEY, _foodCourt, 3f);
            
            factory.AddBelief(ENEMY_IN_CHASE_RANGE_KEY, _chaseSensor);
            factory.AddBelief(ENEMY_IN_ATTACK_RANGE_KEY, _attackSensor);
            factory.AddBelief(ATTACKING_ENEMY_KEY, () => false);
        }

        private void SetupActions()
        {
            _actions = new HashSet<AgentAction>();

            _actions.Add(new AgentAction.Builder("Relax")
                .WithStrategy(new IdleStrategy(5))
                .AddEffect(_beliefs[NOTHING_KEY])
                .Build());

            _actions.Add(new AgentAction.Builder("Wander")
                .WithStrategy(new WanderStrategy(_navMeshAgent, 10))
                .AddEffect(_beliefs[AGENT_MOVING_KEY])
                .Build());

            // Health
            _actions.Add(new AgentAction.Builder("MoveToEatingPosition")
                .WithStrategy(new MoveStrategy(_navMeshAgent, () => _foodCourt.position))
                .AddEffect(_beliefs[AGENT_AT_FOOD_POINT_KEY])
                .Build());
            
            _actions.Add(new AgentAction.Builder("Eat")
                .WithStrategy(new IdleStrategy(5))
                .AddPrecondition(_beliefs[AGENT_AT_FOOD_POINT_KEY])
                .AddEffect(_beliefs[AGENT_HEALTHY_KEY])
                .Build());

            // Rest
            _actions.Add(new AgentAction.Builder("MoveToRestingPosition")
                .WithStrategy(new MoveStrategy(_navMeshAgent, () => _restingPosition.position))
                .AddEffect(_beliefs[AGENT_AT_RESTING_POINT_KEY])
                .Build());
            
            _actions.Add(new AgentAction.Builder("Rest")
                .WithStrategy(new IdleStrategy(5))
                .AddPrecondition(_beliefs[AGENT_AT_RESTING_POINT_KEY])
                .AddEffect(_beliefs[AGENT_RESTED_KEY])
                .Build());
            
            // Doors
            _actions.Add(new AgentAction.Builder("MoveToDoorOne")
                .WithStrategy(new MoveStrategy(_navMeshAgent, () => _doorOne.position))
                .AddEffect(_beliefs[AGENT_AT_DOOR_ONE_KEY])
                .Build());

            _actions.Add(new AgentAction.Builder("MoveToDoorTwo")
                .WithStrategy(new MoveStrategy(_navMeshAgent, () => _doorTwo.position))
                .AddEffect(_beliefs[AGENT_AT_DOOR_TWO_KEY])
                .Build());
            
            _actions.Add(new AgentAction.Builder("FromDoorOneMoveToRestingPosition")
                .WithStrategy(new MoveStrategy(_navMeshAgent, () => _restingPosition.position))
                .AddPrecondition(_beliefs[AGENT_AT_DOOR_ONE_KEY])
                .AddEffect(_beliefs[AGENT_AT_RESTING_POINT_KEY])
                .Build());

            _actions.Add(new AgentAction.Builder("FromDoorTwoMoveToRestingPosition")
                .WithStrategy(new MoveStrategy(_navMeshAgent, () => _restingPosition.position))
                .WithCost(2)
                .AddPrecondition(_beliefs[AGENT_AT_DOOR_TWO_KEY])
                .AddEffect(_beliefs[AGENT_AT_RESTING_POINT_KEY])
                .Build());
            
            // Chase and attack
            _actions.Add(new AgentAction.Builder("ChaseEnemy")
                .WithStrategy(new MoveStrategy(_navMeshAgent, () => _beliefs[ENEMY_IN_CHASE_RANGE_KEY].Location))
                .AddPrecondition(_beliefs[ENEMY_IN_CHASE_RANGE_KEY])
                .AddEffect(_beliefs[ENEMY_IN_ATTACK_RANGE_KEY])
                .Build());
            
            _actions.Add(new AgentAction.Builder("AttackEnemy")
                .WithStrategy(new AttackStrategy(_animator))
                .AddPrecondition(_beliefs[ENEMY_IN_ATTACK_RANGE_KEY])
                .AddEffect(_beliefs[ATTACKING_ENEMY_KEY])
                .Build());
        }

        private void SetupGoals()
        {
            _goals = new HashSet<AgentGoal>();

            _goals.Add(new AgentGoal.Builder("Relax")
                .WithPriority(1)
                .AddDesiredEffect(_beliefs[NOTHING_KEY])
                .Build());

            _goals.Add(new AgentGoal.Builder("Wander")
                .WithPriority(1)
                .AddDesiredEffect(_beliefs[AGENT_MOVING_KEY])
                .Build());

            _goals.Add(new AgentGoal.Builder("Healthy")
                .WithPriority(2)
                .AddDesiredEffect(_beliefs[AGENT_HEALTHY_KEY])
                .Build());
            
            _goals.Add(new AgentGoal.Builder("Rested")
                .WithPriority(2)
                .AddDesiredEffect(_beliefs[AGENT_RESTED_KEY])
                .Build());

            _goals.Add(new AgentGoal.Builder("Rested")
                .WithPriority(3)
                .AddDesiredEffect(_beliefs[ATTACKING_ENEMY_KEY])
                .Build());
        }
        
        private void CalculatePlan()
        {
            var priorityLevel = _currentGoal?.Priority ?? 0;
            var goalsToCheck = _goals;

            if (_currentGoal != null)
                goalsToCheck = new HashSet<AgentGoal>(_goals.Where(g => g.Priority >= priorityLevel));

            var potentialPlan = _planner.Plan(this, goalsToCheck, _lastGoal);
            if (potentialPlan != null)
                _actionPlan = potentialPlan;
        }
        
        private void UpdateStats()
        {
            _stamina += InRangeOf(_restingPosition.position, 3f) ? 20 : -3;
            _health += InRangeOf(_foodCourt.position, 3f) ? 20 : -3;
            
            _stamina = Mathf.Clamp(_stamina, 0f, 100);
            _health = Mathf.Clamp(_health, 0f, 100);
        }

        private bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(transform.position, pos) <= range;
    }
}