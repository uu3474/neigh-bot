using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class PromotionsScenario : BaseScenario
    {
        const string BackAction = "Coupons.Back";

        async Task PrintMenu()
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

            var keyboard = new[] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) };
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
                BackAction => await NewScenarioInit(new InitScenario()),
                _ => ScenarioResult.ContinueCurrent
            };
    }
}
