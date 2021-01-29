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
        protected UserManager Users { get; set; }
        protected INeighRepository Repository { get; set; }
        protected MessageTrail Trail { get; set; }

        public virtual Task<ScenarioResult> Init(UserManager userManager, INeighRepository repository, MessageTrail trail)
        {
            Users = userManager;
            Repository = repository;
            Trail = trail;
            return Task.FromResult(ScenarioResult.ContinueCurrent);
        }

        public virtual Task<ScenarioResult> OnMessage(MessageEventArgs args) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);

        public virtual Task<ScenarioResult> OnCallbackQuery(CallbackQueryEventArgs args) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);    

        protected virtual async Task<ScenarioResult> NewScenarioInit(IScenario scenario)
        {
            await scenario.Init(Users, Repository, Trail);
            return ScenarioResult.NewScenario(scenario);
        }
    }
}
