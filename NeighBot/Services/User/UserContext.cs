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
        public SemaphoreSlim Lock { get; }
        public MessageTrail Trail { get; }
        public IScenario CurrentScenario { get; set; }

        public int UserID => Trail.UserID;

        public UserContext(TelegramBotClient bot, int userID)
        {
            Lock = new SemaphoreSlim(1, 1);
            Trail = new MessageTrail(bot, userID);
        }
    }
}
