using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class AddReviewScenario : BaseScenario
    {
        enum Step : int
        {
            None = 0,
            EnterName = 1,
            EnterGrade = 2,
            EnterReview = 3,
            ProvideContact = 4,
            Preview = 5,
        }

        const byte _minGrage = 1;
        const byte _maxGrade = 5;

        const string PublishAction = "AddReview.Publish";
        const string BackAction = "AddReview.Back";
        const string CancelAction = "AddReview.Cancel";

        readonly Dictionary<Step, Func<MessageTrail, Task>> _invationByStep;
        readonly Dictionary<Step, Func<MessageEventArgs, CallbackQueryEventArgs, Task>> _actionByStep;
        Step _step;
        string _name;
        byte _grade;
        string _review;
        Contact _contact;

        public AddReviewScenario()
        {
            _invationByStep = new Dictionary<Step, Func<MessageTrail, Task>>
            {
                { Step.EnterName, InviteEnterName },
                { Step.EnterGrade, InviteEnterGrade },
                { Step.EnterReview, InviteEnterReview },
                { Step.ProvideContact, InviteProvideContact },
                { Step.Preview, Preview },
            };
            _actionByStep = new Dictionary<Step, Func<MessageEventArgs, CallbackQueryEventArgs, Task>>
            {
                { Step.EnterName, ParseName },
                { Step.EnterGrade, ParseGrade },
                { Step.EnterReview, ParseReview },
                { Step.ProvideContact, ParseContact },
            };
        }

        async Task InviteEnterName(MessageTrail trail)
        {
            _step = Step.EnterName;

            var text = "Хорошо!\nСначала выберем человека, которому ты хочешь оставить обратную связь и поставить оценку.\nНапиши мне его имя:";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        Task ParseName(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (messageArgs != null)
                _name = messageArgs.Message.Text;

            if (callbackArgs != null)
                _name = callbackArgs.CallbackQuery.Message.Text;

            if (string.IsNullOrEmpty(_name))
                    throw new Exception($"Invalid name");

            return Task.CompletedTask;
        }

        async Task InviteEnterGrade(MessageTrail trail)
        {
            _step = Step.EnterGrade;

            var text = $"Отлично!\nТеперь вспомни последний опыт взаимодействия с <b>{_name}</b> и оцени ваше взаимодействие.\nНапиши мне цифру от <i>1 до 5, где 1 - это низкая оценка, а 5 - высокая</i>.";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        Task ParseGrade(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (messageArgs != null)
                _grade = byte.Parse(messageArgs.Message.Text);

            if (callbackArgs != null)
                _grade = byte.Parse(callbackArgs.CallbackQuery.Message.Text);

            if (_grade < _minGrage || _grade > _maxGrade)
                throw new Exception($"Invalid grade '{_grade}', valid range [{_minGrage}, {_maxGrade}]");

            return Task.CompletedTask;
        }

        async Task InviteEnterReview(MessageTrail trail)
        {
            _step = Step.EnterReview;

            var text = $"👌\nНапиши свой фидбек о взаимодействии с <b>{_name}</b>. Например:\n\n<i>Спасибо, что одолжил мне PS4 на выходные!</i>\nили\n<i>Никогда не здоровается в лифте</i>";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        Task ParseReview(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (messageArgs != null)
                _review = messageArgs.Message.Text;

            if (callbackArgs != null)
                _review = callbackArgs.CallbackQuery.Message.Text;

            if (_review.Length < 3)
                throw new Exception($"Invalid review");

            return Task.CompletedTask;
        }

        async Task InviteProvideContact(MessageTrail trail)
        {
            _step = Step.ProvideContact;

            var text = $"Почти готово!\nОтправь мне контакт Telegram пользователя <b>{_name}</b>. Это нужно для того, чтобы связать пользователя в системе с другими его оценками.";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        Task ParseContact(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (messageArgs?.Message?.Contact == null)
                throw new Exception("Empty contact");

            _contact = messageArgs.Message.Contact;

            return Task.CompletedTask;
        }

        async Task Preview(MessageTrail trail)
        {
            _step = Step.Preview;

            var contactCaption = (string.IsNullOrEmpty(_contact.LastName))
                ? _contact.FirstName
                : $"{_contact.FirstName}_{_contact.LastName}";

            var text = $"Проверь свой отзыв:\n\nОтзыв для <b>{_name} ({contactCaption})</b>:\n<b>{_review}</b>\n\nC оценкой: <b>{_grade} / 5</b>";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"⏺️ Опубликовать", PublishAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        async Task<ScenarioResult> Publish(MessageTrail trail)
        {
            var text = $"Великолепно! Отзыв отправлен.";
            await trail.SendTextMessageAsync(text);
            return await NewScenarioInit(trail, new InitScenario());
        }

        async Task<ScenarioResult> Forward(MessageTrail trail, MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (_step == Step.Preview)
                return await Publish(trail);

            try
            {
                await _actionByStep[_step].Invoke(messageArgs, callbackArgs);
                _step = (Step)((int)_step + 1);
            }
            catch
            {
                // log mb?
            }

            await _invationByStep[_step].Invoke(trail);
            return ScenarioResult.ContinueCurrent;
        }

        async Task<ScenarioResult> Back(MessageTrail trail)
        {
            if (_step == Step.EnterName)
                return await NewScenarioInit(trail, new InitScenario());

            _step = (Step)((int)_step - 1);
            await _invationByStep[_step].Invoke(trail);
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> Init(MessageTrail trail)
        {
            await InviteEnterName(trail);
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> OnMessage(MessageTrail trail, MessageEventArgs args) =>
            await Forward(trail, args, null);

        public override async Task<ScenarioResult> OnCallbackQuery(MessageTrail trail, CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                PublishAction => await Publish(trail),
                BackAction => await Back(trail),
                CancelAction => await NewScenarioInit(trail, new InitScenario()),
                _ => ScenarioResult.ContinueCurrent
            };

    }
}
