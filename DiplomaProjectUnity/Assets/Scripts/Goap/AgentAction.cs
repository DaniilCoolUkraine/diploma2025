using System.Collections.Generic;
using DiplomaProject.Goap.Strategies;

namespace DiplomaProject.Goap
{
    public class AgentAction
    {
        public string Name { get; }
        public float Cost { get; private set; }

        public HashSet<AgentBelief> Preconditions { get; } = new();
        public HashSet<AgentBelief> Effects { get; } = new();

        public bool Complete => _strategy.Complete;
        
        private IActionStrategy _strategy;

        private AgentAction(string name)
        {
            Name = name;
        }
        
        public void Start()
        {
            _strategy.Start();
        }

        public void Stop()
        {
            _strategy.Stop();
        }

        public void Update(float deltaTime)
        {
            if (_strategy.CanPerform)
                _strategy.Update(deltaTime);

            if (!_strategy.Complete)
                return;

            foreach (var effect in Effects)
                effect.Evaluate();
        }
        
        public class Builder
        {
            private readonly AgentAction _action;

            public Builder(string name)
            {
                _action = new AgentAction(name)
                {
                    Cost = 1
                };
            }

            public Builder WithCost(float cost)
            {
                _action.Cost = cost;
                return this;
            }

            public Builder WithStrategy(IActionStrategy strategy)
            {
                _action._strategy = strategy;
                return this;
            }

            public Builder AddPrecondition(IEnumerable<AgentBelief> preconditions)
            {
                foreach (var precondition in preconditions)
                    AddPrecondition(precondition);
                return this;
            }
            
            public Builder AddPrecondition(AgentBelief preconditions)
            {
                _action.Preconditions.Add(preconditions);
                return this;
            }

            public Builder AddEffect(IEnumerable<AgentBelief> effects)
            {
                foreach (var effect in effects)
                    AddEffect(effect);
                return this;
            }
            
            public Builder AddEffect(AgentBelief effect)
            {
                _action.Effects.Add(effect);
                return this;
            }

            public AgentAction Build()
            {
                return _action;
            }
        }
    }
}