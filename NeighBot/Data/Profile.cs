using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NeighBot
{
    public class Profile
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Apartment { get; set; }
        public string Parking { get; set; }
        public string CarNumber { get; set; }

        public Profile()
        {
        }

        public Profile(User user)
        {
            Id = user.Id;
            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }
    }
}
