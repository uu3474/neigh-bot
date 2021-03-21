using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NeighBot
{
    public interface INeighRepository
    {
        Task<long> AddOrUpdateUser(User user);
        Task<long> AddOrUpdateUser(Contact contact);
        
        Task<DBReview> AddReview(DBReview review);
        Task<DBReview> AddReview(User fromUser, Contact toUser, DBReview review);
        Task<IEnumerable<DBReview>> GetReviews(long toUserID, int limit = 10);
        Task<float> GetAverageGrade(long toUserID, int limit = 10);

        Task<DBFeedback> AddFeedback(DBFeedback feedback);
        Task<DBFeedback> AddFeedback(User user, DBFeedback feedback);
    }
}
