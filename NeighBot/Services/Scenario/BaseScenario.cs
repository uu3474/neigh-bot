using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace NeighBot
{
    public abstract class BaseScenario : IScenario
    {
        protected async Task<ScenarioResult> NewScenarioInit(TelegramBotClient bot, MessageEventArgs args, IScenario scenario)
        {
            await scenario.Init(bot, args.Message.From, args.Message.Chat);
            return ScenarioResult.NewScenario(scenario);
        }

        protected async Task<ScenarioResult> NewScenarioInit(TelegramBotClient bot, CallbackQueryEventArgs args, IScenario scenario)
        {
            await scenario.Init(bot, args.CallbackQuery.Message.From, args.CallbackQuery.Message.Chat);
            return ScenarioResult.NewScenario(scenario);
        }
    }
}
