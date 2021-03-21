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
        public int UserID { get; }

        public string CallbackData { get; set; }

        public MessageTrail(TelegramBotClient bot, int userID)
        {
            Bot = bot;
            UserID = userID;
        }

        public async Task ClearLastKeyboard()
        {
            if (_prevMessage == null || _prevMessage.ReplyMarkup == null)
                return;

            var callbackText = (CallbackData == null)
                ? null
                :_prevMessage.ReplyMarkup?.InlineKeyboard?
                    .SelectMany(x => x)
                    .Where(x => x.CallbackData == CallbackData)
                    .Select(x => x.Text)
                    .FirstOrDefault();

            _prevMessage = await Bot.EditMessageTextAsync(UserID, _prevMessage.MessageId, _prevMessage.Text, ParseMode.Html);
            CallbackData = null;
        }

        public async Task<Message> SendTextMessageAsync(
            string text,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
        {
            await ClearLastKeyboard();

            _prevMessage = await Bot.SendTextMessageAsync(
                UserID,
                text,
                ParseMode.Html,
                disableWebPagePreview,
                disableNotification,
                replyToMessageId,
                replyMarkup,
                cancellationToken);

            _prevMessage.Text = text;
            return _prevMessage;
        }

        public async Task<Message> SendTextMessageOutTrailAsync(
            string text,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            CancellationToken cancellationToken = default)
            => await Bot.SendTextMessageAsync(
                UserID,
                text,
                ParseMode.Html,
                disableWebPagePreview,
                disableNotification,
                replyToMessageId,
                null,
                cancellationToken);
    }
}
