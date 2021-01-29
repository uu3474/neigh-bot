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

        readonly Dictionary<Step, Func<Task>> _invationByStep;
        readonly Dictionary<Step, Func<MessageEventArgs, CallbackQueryEventArgs, Task>> _actionByStep;
        Step _step;
        string _name;
        byte _grade;
        string _review;
        Contact _toContact;
        User _fromUser;

        public AddReviewScenario()
        {
            _invationByStep = new Dictionary<Step, Func<Task>>
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
                { Step.ProvideContact, ParseToContact },
            };
        }

        public override async Task<ScenarioResult> Init(UserManager userManager, INeighRepository repository, MessageTrail trail)
        {
            await base.Init(userManager, repository, trail);
            await InviteEnterName();
            return ScenarioResult.ContinueCurrent;
        }

        async Task InviteEnterName()
        {
            _step = Step.EnterName;

            var text = "Хорошо!\nСначала выберем человека, которому ты хочешь оставить обратную связь и поставить оценку.\nНапиши мне его имя:";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await Trail.SendTextMessageAsync(text, replyMarkup: markup);
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

        async Task InviteEnterGrade()
        {
            _step = Step.EnterGrade;

            var text = $"Отлично!\nТеперь вспомни последний опыт взаимодействия с <b>{_name}</b> и оцени ваше взаимодействие.\nНапиши мне цифру от <i>1 до 5, где 1 - это низкая оценка, а 5 - высокая</i>.";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await Trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        void ValidateGrade()
        {
            if (_grade < _minGrage || _grade > _maxGrade)
                throw new Exception($"Invalid grade '{_grade}', valid range [{_minGrage}, {_maxGrade}]");
        }

        Task ParseGrade(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (messageArgs != null)
                _grade = byte.Parse(messageArgs.Message.Text);

            if (callbackArgs != null)
                _grade = byte.Parse(callbackArgs.CallbackQuery.Message.Text);

            ValidateGrade();

            return Task.CompletedTask;
        }

        async Task InviteEnterReview()
        {
            _step = Step.EnterReview;

            var text = $"👌\nНапиши свой фидбек о взаимодействии с <b>{_name}</b>. Например:\n\n<i>Спасибо, что одолжил мне PS4 на выходные!</i>\nили\n<i>Никогда не здоровается в лифте</i>";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await Trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        void ValidateReview()
        {
            if (_review.Length < 3)
                throw new Exception($"Invalid review");
        }

        Task ParseReview(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (messageArgs != null)
                _review = messageArgs.Message.Text;

            if (callbackArgs != null)
                _review = callbackArgs.CallbackQuery.Message.Text;

            ValidateReview();

            return Task.CompletedTask;
        }

        async Task InviteProvideContact()
        {
            _step = Step.ProvideContact;

            var text = $"Почти готово!\nОтправь мне контакт Telegram пользователя <b>{_name}</b>. Это нужно для того, чтобы связать пользователя в системе с другими его оценками.";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await Trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        void ValidateToContact()
        {
            if (_toContact == null)
                throw new Exception("Empty contact (to)");

            if (_toContact.UserId == 0)
                throw new Exception("Contact (to) do not contain user id");
        }

        Task ParseToContact(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            _toContact = messageArgs?.Message?.Contact;

            ValidateToContact();

            return Task.CompletedTask;
        }

        async Task Preview()
        {
            _step = Step.Preview;

            var contactCaption = (string.IsNullOrEmpty(_toContact.LastName))
                ? _toContact.FirstName
                : $"{_toContact.FirstName}_{_toContact.LastName}";

            var text = $"Проверь свой отзыв:\n\nОтзыв для <b>{_name} ({contactCaption})</b>:\n<b>{_review}</b>\n\nC оценкой: <b>{_grade} / 5</b>";
            var keyboard = new[]
            {
                new [] { InlineKeyboardButton.WithCallbackData($"⏺️ Опубликовать", PublishAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔙 Назад", BackAction) },
                new [] { InlineKeyboardButton.WithCallbackData($"🔝 Отмена", CancelAction) }
            };
            var markup = new InlineKeyboardMarkup(keyboard);
            await Trail.SendTextMessageAsync(text, replyMarkup: markup);
        }

        void SetFromUser(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (messageArgs != null)
                _fromUser = messageArgs.Message.From;

            if (callbackArgs != null)
                _fromUser = callbackArgs.CallbackQuery.From;

            ValidateFromUser();
        }

        void ValidateFromUser()
        {
            if (_fromUser == null)
                throw new Exception("Empty user (from)");
        }

        void EnsureReview()
        {
            ValidateGrade();
            ValidateReview();
            ValidateToContact();
            ValidateFromUser();
        }

        async Task SentNotificationToContact(DBReview review)
        {
            var (_, contactContext) = Users.GetContext(Trail.Bot, _toContact.UserId);

            var text = $"<i>Полуен отзыв:</i>\n{review.Review}\n<i>C оценкой:</i> <b>{review.Grade} / 5</b>";
            await contactContext.Trail.SendTextMessageOutTrailAsync(text);
        }

        async Task<ScenarioResult> Publish(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            SetFromUser(messageArgs, callbackArgs);

            EnsureReview();

            var dbReview = await Repository.AddReview(_fromUser, _toContact, new DBReview(_grade, _review));
            await SentNotificationToContact(dbReview);

            var text = $"Великолепно! Отзыв отправлен.";
            await Trail.SendTextMessageAsync(text);
            return await NewScenarioInit(new InitScenario());
        }

        async Task<ScenarioResult> Forward(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            if (_step == Step.Preview)
                return await Publish(messageArgs, callbackArgs);

            try
            {
                await _actionByStep[_step].Invoke(messageArgs, callbackArgs);
                _step = (Step)((int)_step + 1);
            }
            catch
            {
                // log mb?
            }

            await _invationByStep[_step].Invoke();
            return ScenarioResult.ContinueCurrent;
        }

        async Task<ScenarioResult> Back()
        {
            if (_step == Step.EnterName)
                return await NewScenarioInit(new InitScenario());

            _step = (Step)((int)_step - 1);
            await _invationByStep[_step].Invoke();
            return ScenarioResult.ContinueCurrent;
        }

        public override async Task<ScenarioResult> OnMessage(MessageEventArgs args) =>
            await Forward(args, null);

        public override async Task<ScenarioResult> OnCallbackQuery(CallbackQueryEventArgs args) =>
            args.CallbackQuery.Data switch
            {
                PublishAction => await Publish(null, args),
                BackAction => await Back(),
                CancelAction => await NewScenarioInit(new InitScenario()),
                _ => ScenarioResult.ContinueCurrent
            };

    }
}
