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
    public class ProfileScenario : BaseScenario
    {
        const string BackAction = "Profile.Back";

        async Task PrintMenu()
        {
            var reviews = (await Repository.GetReviews(Trail.UserID)).ToList();

            string text = null;
            if (reviews.Count == 0)
            {
                text = "К сожалению, вы ещё не получали отзывы, поделитесь ботом с соседями и пусть они вас оценят!";
            }
            else
            {
                var builder = new StringBuilder();

                foreach (var review in reviews)
                    builder
                        .AppendLine($"<i>Отзыв от {review.CreateTime}:</i>\n{review.Review}\n<i>C оценкой:</i> <b>{review.Grade} / 5</b>")
                        .AppendLine();

                builder.AppendLine($"<b>Ваш рейтинг: {Math.Round(reviews.Average(x => x.Grade), 2)} / 5</b>");

                text = builder.ToString();
            }

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
