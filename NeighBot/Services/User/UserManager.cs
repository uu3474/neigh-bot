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

        public (bool isNew, UserContext context) GetContext(TelegramBotClient bot, int userID)
        {
            bool isNew = false;
            var context = _cahce.GetOrCreate(userID, (entry) =>
            {
                isNew = true;
                entry.SetSlidingExpiration(_expiration);
                return new UserContext(bot, userID);
            });
            return (isNew, context);
        }
    }
}
