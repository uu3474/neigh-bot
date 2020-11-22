using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NeighBot
{
    public class InitScenario : IScenario
    {
        public ScenarioResult Init(TelegramBotClient bot, User user)
        {
            throw new NotImplementedException();
        }

        public ScenarioResult OnCallbackQuery(TelegramBotClient bot, object sender, CallbackQueryEventArgs e)
        {
            throw new NotImplementedException();
        }

        public ScenarioResult OnMessage(TelegramBotClient bot, object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
