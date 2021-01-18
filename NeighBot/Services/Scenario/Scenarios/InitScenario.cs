using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class InitScenario : BaseScenario
    {
        const string AddReviewAction = "Init.AddReview";
        const string ShareAction = "Init.Share";
        const string PromotionsAction = "Init.Promotions";
        const string ProfileAction = "Init.Profile";
        const string HelpAction = "Init.Help";
        
        async Task PrintMenu(MessageTrail trail)
        {
            var text = "Добро пожаловать в Бумеранг Бот, главное меню:";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"📝 Поставить оценку", AddReviewAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"📢 Поделиться ботом", ShareAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🎁 Посмотреть действующие акции", PromotionsAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"📈 Мой рейтинг и оценки", ProfileAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"❓ Что я умею", HelpAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        public override async Task<ScenarioResult> Init(MessageTrail trail)
        {
            await PrintMenu(trail);
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> OnCallbackQuery(MessageTrail trail, CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                AddReviewAction => await NewScenarioInit(trail, new AddReviewScenario()),
                ShareAction => await NewScenarioInit(trail, new ShareScenario()),
                PromotionsAction => await NewScenarioInit(trail, new PromotionsScenario()),
                ProfileAction => await NewScenarioInit(trail, new ProfileScenario()),
                HelpAction => await NewScenarioInit(trail, new HelpScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
