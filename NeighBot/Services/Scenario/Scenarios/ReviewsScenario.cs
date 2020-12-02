using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
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

        async Task PrintMenu(TelegramBotClient bot, User user, Chat chat = null)
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
            await bot.SendTextMessageAsync(chat?.Id ?? user.Id, text, replyMarkup: markup);
        }

        public async Task<ScenarioResult> Init(TelegramBotClient bot, User user, Chat chat = null)
        {
            await PrintMenu(bot, user, chat);
            return ScenarioResult.ContinueCurrent;
        }
    }
}
