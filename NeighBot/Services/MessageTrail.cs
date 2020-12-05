using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeighBot
{
    public class MessageTrail
    {
        Message _prevMessage;

        public TelegramBotClient Bot { get; }
        public User User { get; }
        public Chat Chat { get; }

        public string CallbackData { get; set; }

        public MessageTrail(TelegramBotClient bot, User user, Chat chat = null)
        {
            Bot = bot;
            User = user;
            Chat = chat;
        }

        async Task TryEditPrevMessage()
        {
            if (_prevMessage == null || CallbackData == null)
                return;

            var callbackText = _prevMessage.ReplyMarkup?.InlineKeyboard?
                .SelectMany(x => x)
                .Where(x => x.CallbackData == CallbackData)
                .Select(x => x.Text)
                .FirstOrDefault();

            var prevText = _prevMessage.Text;
            if (callbackText != null)
                prevText += $"\n\nБыла нажата кнопка: [{callbackText}]";

            _prevMessage = await Bot.EditMessageTextAsync(Chat?.Id ?? User.Id, _prevMessage.MessageId, prevText);
            CallbackData = null;
        }

        public async Task<Message> SendTextMessageAsync(
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            await TryEditPrevMessage();

            _prevMessage = await Bot.SendTextMessageAsync(
                Chat?.Id ?? User.Id,
                text,
                parseMode,
                disableWebPagePreview,
                disableNotification,
                replyToMessageId,
                replyMarkup,
                cancellationToken);

            return _prevMessage;
        }

    }
}
