using System.Collections.Generic;

namespace DiplomaProject.Goap.Planner
{
    public interface IGoapPlanner
    {
        ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null);
    }

}