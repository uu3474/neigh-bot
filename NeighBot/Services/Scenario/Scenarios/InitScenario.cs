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
    public class InitScenario : IScenario
    {
        public async Task<ScenarioResult> Init(TelegramBotClient bot, User user, Chat chat = null)
        {
            var text = "Добро пожаловать в добрососедский бот, пожалуйста, выберите действие:";
            var keyboard = new []
            {
                new [] 
                { 
                    InlineKeyboardButton.WithCallbackData($"Профиль"),
                    InlineKeyboardButton.WithCallbackData($"Отзывы"),
                    InlineKeyboardButton.WithCallbackData($"Купоны")
                },
                new [] 
                { 
                    InlineKeyboardButton.WithCallbackData($"Добавить отзыв")
                }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await bot.SendTextMessageAsync(chat?.Id ?? user.Id, text, replyMarkup: markup);
            return ScenarioResult.ContinueCurrent;
        }
    }
}
