using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NeighBot
{
    public interface IScenario
    {
        Task<ScenarioResult> Init(MessageTrail trail);
        Task<ScenarioResult> OnMessage(MessageTrail trail, MessageEventArgs args);
        Task<ScenarioResult> OnCallbackQuery(MessageTrail trail, CallbackQueryEventArgs args);
    }
}
