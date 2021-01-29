using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class HelpScenario : BaseScenario
    {
        const string BackAction = "Help.Back";

        async Task PrintMenu()
        {
            var text = new StringBuilder()
                .AppendLine("Делай добрые дела и будь вежлив к окружающим. Взамен получай высокие оценки, меняй их на бесплатный кофе и другие приятные бонусы 🤗")
                .AppendLine()
                .AppendLine("Зачем писать комментарии и ставить оценки другим людям?")
                .AppendLine("...")
                .AppendLine("Комментарии и оценки анонимны?")
                .AppendLine("...")
                .AppendLine("Другие люди могут увидеть то, что обо мне тут пишут?")
                .AppendLine("...")
                .AppendLine("Как формируется рейтинг?")
                .AppendLine("...")
                .AppendLine("На что он влияет?")
                .AppendLine("...")
                .AppendLine("Что считается добрым делом?")
                .AppendLine("...")
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
