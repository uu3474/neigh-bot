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
        const string FeedbackAction = "Init.Feedback";
        const string HelpAction = "Init.Help";
        
        async Task PrintMenu()
        {
            var text = "Добро пожаловать в Бумеранг Бот, главное меню:";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"📝 Поставить оценку", AddReviewAction) },
                new [] { InlineKeyboardButton.WithSwitchInlineQuery($"📢 Поделиться ботом", "Привет, поставь мне оценку 😀") },
                new [] { InlineKeyboardButton.WithCallbackData($"🎁 Посмотреть действующие акции", PromotionsAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"📈 Мой рейтинг и оценки", ProfileAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"💌 Связаться с разработчиками", FeedbackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"❓ Что я умею", HelpAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await Trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        public override async Task<ScenarioResult> Init(UserManager userManager, INeighRepository repository, MessageTrail trail)
        {
            await base.Init(userManager, repository, trail);
            await PrintMenu();
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> OnCallbackQuery(CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                AddReviewAction => await NewScenarioInit(new ReviewScenario()),
                PromotionsAction => await NewScenarioInit(new PromotionsScenario()),
                ProfileAction => await NewScenarioInit(new ProfileScenario()),
                FeedbackAction => await NewScenarioInit(new FeedbackScenario()),
                HelpAction => await NewScenarioInit(new HelpScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
