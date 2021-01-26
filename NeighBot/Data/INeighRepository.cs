using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NeighBot
{
    interface INeighRepository
    {
        Task<long> AddOrUpdateUser(User toUser);
        Task<long> AddOrUpdateUser(Contact toContact);
        Task<long> AddReview(DBReview review);
        Task<long> AddReview(User fromUser, Contact toUser, DBReview review);
        Task<IEnumerable<DBReview>> GetReviews(int toUserID);
    }
}
