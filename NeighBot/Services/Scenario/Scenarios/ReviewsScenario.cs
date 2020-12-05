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
    public class ReviewsScenario : BaseScenario
    {
        const string ReviewsAboutMeAction = "Reviews.AboutMe";
        const string MyReviewsAction = "Reviews.My";
        const string AddReviewAction = "Reviews.Add";
        const string BackAction = "Reviews.Back";

        async Task PrintMenu(MessageTrail trail)
        {
            var text = "Отзывы:";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"Отзывы обо мне", ReviewsAboutMeAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"Мои отзывы", MyReviewsAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"Добавить отзыв", AddReviewAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"Назад", BackAction) }
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
                BackAction => await NewScenarioInit(trail, new InitScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
