using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeighBot
{
    public class UserContext
    {
        public User User { get; }
        public SemaphoreSlim Lock { get; }
        public MessageTrail Trail { get; }
        public IScenario CurrentScenario { get; set; }

        public UserContext(TelegramBotClient bot, User user, Chat chat = null, IScenario scenario = null)
        {
            User = user;
            Lock = new SemaphoreSlim(1, 1);
            Trail = new MessageTrail(bot, user, chat);
            CurrentScenario = scenario;
        }
    }
}
