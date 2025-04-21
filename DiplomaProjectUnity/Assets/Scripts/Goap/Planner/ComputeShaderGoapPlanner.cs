using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DiplomaProject.Goap.Planner
{
    public class ComputeShaderGoapPlanner : IGoapPlanner
    {
        private const int PLAN_ACTIONS_LENGTH = 3;

        private static readonly int _goals = Shader.PropertyToID("Goals");
        private static readonly int _actions = Shader.PropertyToID("Actions");
        private static readonly int _plan = Shader.PropertyToID("PlannedActions");
        private static readonly int _actionsCount = Shader.PropertyToID("ActionsCount");
        private static readonly int _succeededGoalIndex = Shader.PropertyToID("SucceededGoalIndex");
        private static readonly int _goalsCount = Shader.PropertyToID("GoalsCount");

        private readonly int _kernel;

        private ComputeShader _plannerShader;

        private ComputeBuffer _goalBuffer;
        private ComputeBuffer _actionsBuffer;
        private ComputeBuffer _actionPlanBuffer;
        private ComputeBuffer _succeededGoalBuffer;

        public ComputeShaderGoapPlanner(ComputeShader plannerShader)
        {
            _plannerShader = plannerShader;
            _kernel = _plannerShader.FindKernel("Plan");
        }

        public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
        {
            Debug.Log("======= Compute Shader Planner =======");
            
            PrepareArrays(agent, goals, mostRecentGoal, out AgentGoalStruct[] goalsStructs, out var actionsStructs);
            PrepareComputeBuffers(goalsStructs, actionsStructs);

            _plannerShader.Dispatch(_kernel, 1, 1, 1);

            // get the plan out of _actionPlanBuffer

            AgentActionStruct[] actionPlanStructs = new AgentActionStruct[PLAN_ACTIONS_LENGTH];
            int[] succeededGoal = new int[] { -1 };
            _actionPlanBuffer.GetData(actionPlanStructs);
            _succeededGoalBuffer.GetData(succeededGoal);

            CleanupBuffers();

            // now we can return plan
            if (succeededGoal[0] == -1)
            {
                Debug.LogError("No plan found");
                return null;
            }
            
            Stack<AgentAction> actionsStack = new Stack<AgentAction>();
            foreach (var action in agent.Actions)
            {
                if (actionPlanStructs.Select(a => a.NameHash).Contains(action.Name.GetHashCode()))
                    actionsStack.Push(action);
            }
            
            actionsStack = new Stack<AgentAction>(actionsStack.Reverse());

            var goalStruct = goalsStructs[succeededGoal[0]];
            var goal = goals.First(g => g.Name.GetHashCode() == goalStruct.NameHash);

            Debug.Log("Goal: " + goal.Name);
            
            foreach (var action in actionsStack)
            {
                Debug.Log("Action: " + action.Name);
            }
            
            var actionPlan = new ActionPlan(
                goal,
                actionsStack,
                actionPlanStructs.Sum(a => a.Cost));

            return actionPlan;
        }

        private void PrepareArrays(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal, out AgentGoalStruct[] goalsStructs, out AgentActionStruct[] actionsStructs)
        {
            var newGoals = new HashSet<AgentGoal>(goals);
            newGoals.Remove(mostRecentGoal);
            var orderedGoals = newGoals
                .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
                .OrderByDescending(g => g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
                .ToList();
            var orderedActions = agent.Actions
                .OrderBy(a => a.Cost);

            goalsStructs = orderedGoals
                .Select(g => g.ToStruct())
                .ToArray();
            actionsStructs = orderedActions
                .Select(a => a.ToStruct())
                .ToArray();
        }

        private void PrepareComputeBuffers(AgentGoalStruct[] goalsStructs, AgentActionStruct[] actionsStructs)
        {
            _goalBuffer = new ComputeBuffer(goalsStructs.Length, Marshal.SizeOf<AgentGoalStruct>());
            _actionsBuffer = new ComputeBuffer(actionsStructs.Length, Marshal.SizeOf<AgentActionStruct>());
            _actionPlanBuffer = new ComputeBuffer(PLAN_ACTIONS_LENGTH, Marshal.SizeOf<AgentActionStruct>());
            _succeededGoalBuffer = new ComputeBuffer(1, sizeof(int));

            _goalBuffer.SetData(goalsStructs);
            _actionsBuffer.SetData(actionsStructs);
            _actionPlanBuffer.SetData(new AgentActionStruct[PLAN_ACTIONS_LENGTH]);
            _succeededGoalBuffer.SetData(new int[] {-1 });

            
            _plannerShader.SetBuffer(_kernel, _goals, _goalBuffer);
            _plannerShader.SetInt(_goalsCount, goalsStructs.Length);
            
            _plannerShader.SetBuffer(_kernel, _actions, _actionsBuffer);
            _plannerShader.SetInt(_actionsCount, actionsStructs.Length);
            
            _plannerShader.SetBuffer(_kernel, _plan, _actionPlanBuffer);
            _plannerShader.SetBuffer(_kernel, _succeededGoalIndex, _succeededGoalBuffer);
        }

        private void CleanupBuffers()
        {
            _actionsBuffer.Release();
            _goalBuffer.Release();
            _actionPlanBuffer.Release();
            _succeededGoalBuffer.Release();
        }
    }
}