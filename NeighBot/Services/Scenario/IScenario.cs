using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NeighBot
{
    public interface IScenario
    {
        Task<ScenarioResult> Init(TelegramBotClient bot, User user, Chat chat) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);

        Task<ScenarioResult> OnMessage(TelegramBotClient bot, object sender, MessageEventArgs args) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);

        Task<ScenarioResult> OnCallbackQuery(TelegramBotClient bot, object sender, CallbackQueryEventArgs args) =>
            Task.FromResult(ScenarioResult.ContinueCurrent);
    }
}
