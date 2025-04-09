using System;
using UnityEngine;

namespace DiplomaProject.Goap
{
    public class AgentBelief
    {
        public string Name { get; }
        public Vector3 Location => _observedLocation();

        private Func<bool> _condition = () => false;
        private Func<Vector3> _observedLocation = () => Vector3.zero;

        AgentBelief(string name)
        {
            Name = name;
        }

        public bool Evaluate() => _condition();
        
        public class Builder
        {
            private AgentBelief _agentBelief;

            public Builder(string name)
            {
                _agentBelief = new AgentBelief(name);
            }

            public Builder WithCondition(Func<bool> condition)
            {
                _agentBelief._condition = condition;
                return this;
            }

            public Builder WithLocation(Func<Vector3> location)
            {
                _agentBelief._observedLocation = location;
                return this;
            }

            public AgentBelief Build()
            {
                return _agentBelief;
            }
        }
    }
}