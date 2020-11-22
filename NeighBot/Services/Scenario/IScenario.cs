using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NeighBot
{
    public interface IScenario
    {
        ScenarioResult Init(TelegramBotClient bot, User user);
        ScenarioResult OnMessage(TelegramBotClient bot, object sender, MessageEventArgs e);
        ScenarioResult OnCallbackQuery(TelegramBotClient bot, object sender, CallbackQueryEventArgs e);
    }
}
