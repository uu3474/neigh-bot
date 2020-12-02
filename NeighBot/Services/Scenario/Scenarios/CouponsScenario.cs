using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class CouponsScenario : BaseScenario
    {
        const string BackAction = "Coupons.Back";

        async Task PrintMenu(TelegramBotClient bot, User user, Chat chat = null)
        {
            var text = new StringBuilder()
                .AppendLine("Вам доступны следующие награды:")
                .AppendLine("1) Чашка кофе в 'ББ Кофе'")
                .AppendLine("2) Чашка кофе в 'ББ Кофе'")
                .AppendLine("3) Чашка кофе с круасаном в 'ББ Кофе'")
                .AppendLine()
                .AppendLine($"Условия для получения наград:")
                .AppendLine($"1) Оставьте ещё 3 отзыв(а) для получения награды от 'ББ Кофе'!")
                .AppendLine($"2) Получите ещё 4 отзыв(а) для получения награды от 'ББ Кофе'!")
                .AppendLine($"2) Пригласите ещё 4 человек(а) для получения награды от 'ББ Кофе'!")
                .ToString();

            var keyboard = new[] { InlineKeyboardButton.WithCallbackData($"Назад", BackAction) };
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
