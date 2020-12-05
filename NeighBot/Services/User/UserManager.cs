using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeighBot
{
    public class UserManager
    {
        readonly static TimeSpan _expiration = TimeSpan.FromHours(1);

        readonly IMemoryCache _cahce;

        public UserManager(IMemoryCache cahce) =>
            _cahce = cahce;

        string GetKey(User user) =>
            string.IsNullOrEmpty(user.Username)
                ? user.Id.ToString()
                : $"{user.Username}[{user.Id}]";

        public UserContext GetContext(TelegramBotClient bot, User user, Chat chat = null) =>
            _cahce.GetOrCreate(GetKey(user), (entry) =>
            {
                entry.SetSlidingExpiration(_expiration);
                return new UserContext(bot, user, chat);
            });
    }
}
