using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace NeighBot
{
    public interface IScenario
    {
        Task<ScenarioResult> Init(UserManager userManager, INeighRepository repository, MessageTrail trail);
        Task<ScenarioResult> OnMessage(MessageEventArgs args);
        Task<ScenarioResult> OnCallbackQuery(CallbackQueryEventArgs args);
    }
}
