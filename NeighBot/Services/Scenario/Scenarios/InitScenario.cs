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
        const string ProfileAction = "Init.Profile";
        const string ReviewsAction = "Init.Reviews";
        const string CouponswAction = "Init.Coupons";
        const string AddReviewAction = "Init.AddReview";

        async Task PrintMenu(MessageTrail trail)
        {
            var text = "Добро пожаловать в добрососедский бот, пожалуйста, выберите действие:";
            var keyboard = new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData($"Профиль", ProfileAction),
                    InlineKeyboardButton.WithCallbackData($"Отзывы", ReviewsAction),
                    InlineKeyboardButton.WithCallbackData($"Купоны", CouponswAction)
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData($"Добавить отзыв", AddReviewAction)
                }
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
                ProfileAction => await NewScenarioInit(trail, new ProfileScenario()),
                ReviewsAction => await NewScenarioInit(trail, new ReviewsScenario()),
                CouponswAction => await NewScenarioInit(trail, new CouponsScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
