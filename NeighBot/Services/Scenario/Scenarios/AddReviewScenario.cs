using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class AddReviewScenario : BaseScenario
    {
        enum Step
        {
            None = 0,
            EnterName = 1,
            EnterGrade = 2,
            EnterReview = 3,
            ProvideContact = 4,
        }

        const string BackAction = "AddReview.Back";
        const string CancelAction = "AddReview.Cancel";

        Step _step;
        string _name;
        byte _grade;
        string _review;

        public override async Task<ScenarioResult> Init(MessageTrail trail)
        {
            _step = Step.EnterName;

            var text = "Хорошо!\n\nСначала выберем человека, которому ты хочешь оставить обратную связь и поставить оценку.Напиши мне его имя";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"❌ Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
            return ScenarioResult.ContinueCurrent;
        }

    }
}
