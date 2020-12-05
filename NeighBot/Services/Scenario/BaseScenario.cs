using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NeighBot
{
    public abstract class BaseScenario : IScenario
    {
        public virtual Task<ScenarioResult> Init(MessageTrail trail) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);

        public virtual Task<ScenarioResult> OnMessage(MessageTrail trail, MessageEventArgs args) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);

        public virtual Task<ScenarioResult> OnCallbackQuery(MessageTrail trail, CallbackQueryEventArgs args) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);    

        protected virtual async Task<ScenarioResult> NewScenarioInit(MessageTrail trail, IScenario scenario)
        {
            await scenario.Init(trail);
            return ScenarioResult.NewScenario(scenario);
        }
    }
}
