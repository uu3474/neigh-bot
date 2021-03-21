using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class FeedbackScenario : BaseScenario
    {
        const string BackAction = "Feedback.Back";

        async Task PrintMenu(string text)
        {
            var keyboard = new[] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) };
            var markup = new InlineKeyboardMarkup(keyboard);
            await Trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        public override async Task<ScenarioResult> Init(UserManager userManager, INeighRepository repository, MessageTrail trail)
        {
            await base.Init(userManager, repository, trail);
            await PrintMenu("Оставьте сообщение для разработчкиов:");
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> OnMessage(MessageEventArgs args)
        {
            var dbFeedback = new DBFeedback(args.Message.Text);
            await Repository.AddFeedback(args.Message.ForwardFrom, dbFeedback);
            await PrintMenu("Ваше сообщение отправлено разработчикам, можете написать ещё что-нибудь:");
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> OnCallbackQuery(CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                BackAction => await NewScenarioInit(new InitScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
