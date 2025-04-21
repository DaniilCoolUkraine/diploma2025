using System;
using System.Runtime.InteropServices;
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
        
        public AgentBeliefStruct ToStruct()
        {
            return new AgentBeliefStruct
            {
                NameHash = Name.GetHashCode(),
                Location = Location,
                Condition = Evaluate() ? 1 : 0
            };
        }
    }
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct AgentBeliefStruct
    {
        public int NameHash;
        public Vector3 Location;
        public int Condition;
    }
}