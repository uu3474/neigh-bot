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

        async Task PrintMenu(TelegramBotClient bot, User user, Chat chat = null)
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
            await bot.SendTextMessageAsync(chat?.Id ?? user.Id, text, replyMarkup: markup);
        }

        public async Task<ScenarioResult> Init(TelegramBotClient bot, User user, Chat chat = null)
        {
            await PrintMenu(bot, user, chat);
            return ScenarioResult.ContinueCurrent;
        }

        public async Task<ScenarioResult> OnCallbackQuery(TelegramBotClient bot, object sender, CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                ProfileAction => await NewScenarioInit(bot, args, new ProfileScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
