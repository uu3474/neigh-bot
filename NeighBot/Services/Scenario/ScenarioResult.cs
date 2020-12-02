using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighBot
{
    public struct ScenarioResult
    {
        public static ScenarioResult ContinueCurrent { get; } = new ScenarioResult(ScenarioAction.ContinueCurrent);
        public static ScenarioResult Done { get; } = new ScenarioResult(ScenarioAction.Done);

        public static ScenarioResult NewScenario(IScenario scenario) =>
            new ScenarioResult(ScenarioAction.NewScenario, scenario);

        public ScenarioAction Action { get; }
        public IScenario Scenario { get; }

        public ScenarioResult(ScenarioAction action, IScenario scenario = null)
        {
            Action = action;
            Scenario = scenario;
        }
    }
}
