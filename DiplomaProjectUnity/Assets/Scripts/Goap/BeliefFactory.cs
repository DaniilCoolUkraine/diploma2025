using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiplomaProject.Goap
{
    public class BeliefFactory
    {
        private readonly GoapAgent _agent;
        // ToDo add hash save instead of string
        private readonly Dictionary<string, AgentBelief> _beliefs;

        public BeliefFactory(GoapAgent agent, Dictionary<string, AgentBelief> beliefs)
        {
            _agent = agent;
            _beliefs = beliefs;
        }

        public void AddBelief(string key, Func<bool> condition)
        {
            _beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(condition)
                .Build());
        }

        public void AddBelief(string key, Transform locationCondition, float distance)
        {
            AddBelief(key, locationCondition.position, distance);
        }

        public void AddBelief(string key, Vector3 locationCondition, float distance)
        {
            _beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(() => InRangeOf(locationCondition, distance))
                .WithLocation(() => locationCondition)
                .Build());
        }

        public void AddBelief(string key, Sensor sensor)
        {
            _beliefs.Add(key, new AgentBelief.Builder(key)
                .WithCondition(() => sensor.IsInRange)
                .WithLocation(() => sensor.TargetPosition)
                .Build());
        }
        
        private bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(_agent.transform.position, pos) <= range;
    }
}