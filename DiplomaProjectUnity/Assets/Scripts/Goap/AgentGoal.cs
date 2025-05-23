﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DiplomaProject.General;

namespace DiplomaProject.Goap
{
    public class AgentGoal
    {
        public string Name { get; }
        public int Priority { get; private set; }

        public HashSet<AgentBelief> DesiredEffects { get; } = new();
        
        private AgentGoal(string name)
        {
            Name = name;
        }   
        
        public class Builder
        {
            private readonly AgentGoal _goal;

            public Builder(string name)
            {
                _goal = new AgentGoal(name);
            }

            public Builder WithPriority(int priority)
            {
                _goal.Priority = priority;
                return this;
            }

            public Builder AddDesiredEffect(IEnumerable<AgentBelief> effects)
            {
                foreach (var effect in effects)
                    AddDesiredEffect(effect);
                return this;
            }
            
            public Builder AddDesiredEffect(AgentBelief effect)
            {
                _goal.DesiredEffects.Add(effect);
                return this;
            }

            public AgentGoal Build()
            {
                return _goal;
            }
        }
        
        public AgentGoalStruct ToStruct()
        {
            return new AgentGoalStruct
            {
                NameHash = Name.GetHashCode(),
                Priority = Priority,
                DesiredEffect = DesiredEffects.Select(e => e.ToStruct()).FirstOrDefault()
            };
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct AgentGoalStruct
    {
        public int NameHash;
        public int Priority;
        public AgentBeliefStruct DesiredEffect;
    }
}