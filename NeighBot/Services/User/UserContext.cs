using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NeighBot
{
    public class UserContext
    {
        public User User { get; }
        public SemaphoreSlim Lock { get; }
        public IScenario CurrentScenario { get; set; }

        public UserContext(User user, IScenario scenario)
        {
            User = user;
            Lock = new SemaphoreSlim(1, 1);
            CurrentScenario = scenario;
        }
    }
}
