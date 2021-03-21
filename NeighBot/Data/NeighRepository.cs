using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Dapper;

namespace NeighBot
{
    public class NeighRepository : INeighRepository
    {
        string _connectionString;

        public NeighRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Neigh");
        }

        async Task<NpgsqlConnection> CreateAndOpenConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        async Task<long> AddOrUpdateUserCore(NpgsqlConnection conection, User user)
            => await conection.QuerySingleAsync<long>(
@"insert into users(id, first_name, last_name, user_name)
values (@Id, @FirstName, @LastName, @Username)
on conflict (id) do
update set first_name = @FirstName, last_name = @LastName, user_name = @Username
returning id"
                , user);

        public async Task<long> AddOrUpdateUser(User user)
        {
            using var connection = await CreateAndOpenConnection();
            return await AddOrUpdateUserCore(connection, user);
        }

        async Task<long> AddOrUpdateUserCore(NpgsqlConnection conection, Contact contact)
            => await conection.QuerySingleAsync<long>(
@"insert into users(id, first_name, last_name, phone)
values (@UserId, @FirstName, @LastName, @PhoneNumber)
on conflict (id) do
update set first_name = @FirstName, last_name = @LastName, phone = @PhoneNumber
returning id"
                , contact);

        public async Task<long> AddOrUpdateUser(Contact contact)
        {
            using var connection = await CreateAndOpenConnection();
            return await AddOrUpdateUserCore(connection, contact);
        }

        async Task<DBReview> AddReviewCore(NpgsqlConnection conection, DBReview review)
            => await conection.QuerySingleAsync<DBReview>(
@"insert into reviews(from_user, to_user, grade, review)
values (@FromUser, @ToUser, @Grade, @Review)
returning
    id as ID,
    create_time as CreateTime,
    from_user as FromUser,
    to_user as ToUser,
    grade as Grade,
    review as Review",
                review);

        public async Task<DBReview> AddReview(DBReview review)
        {
            using var connection = await CreateAndOpenConnection();
            return await AddReviewCore(connection, review);
        }

        public async Task<DBReview> AddReview(User fromUser, Contact toContact, DBReview review)
        {
            using var connection = await CreateAndOpenConnection();
            
            var fromID = await AddOrUpdateUserCore(connection, fromUser);
            review.FromUser = fromID;

            var toID = await AddOrUpdateUserCore(connection, toContact);
            review.ToUser = toID;

            return await AddReviewCore(connection, review);
        }

        public async Task<IEnumerable<DBReview>> GetReviews(long toUserID, int limit = 10)
        {
            using var connection = await CreateAndOpenConnection();
            return await connection.QueryAsync<DBReview>(
@"select 
    id as ID,
    create_time as CreateTime,
    from_user as FromUser,
    to_user as ToUser,
    grade as Grade,
    review as Review
from reviews
where to_user = @ToUserID
order by create_time desc
limit @Limit"
                , new { ToUserID = toUserID, Limit = limit });
        }

        public async Task<float> GetAverageGrade(long toUserID, int limit = 10)
        {
            using var connection = await CreateAndOpenConnection();
            return await connection.QuerySingleAsync<float>(
@"select avg(grade)
from reviews
where to_user = @ToUserID
order by create_time desc
limit @Limit"
                , new { ToUserID = toUserID, Limit = limit });
        }

        public async Task<DBFeedback> AddFeedback(DBFeedback feedback)
        {
            using var connection = await CreateAndOpenConnection();
            return await AddFeedbackCore(connection, feedback);
        }

        public async Task<DBFeedback> AddFeedback(User user, DBFeedback feedback)
        {
            using var connection = await CreateAndOpenConnection();

            var userID = await AddOrUpdateUserCore(connection, user);
            feedback.User = userID;

            return await AddFeedbackCore(connection, feedback);
        }

        async Task<DBFeedback> AddFeedbackCore(NpgsqlConnection conection, DBFeedback feedback)
            => await conection.QuerySingleAsync<DBFeedback>(
@"insert into reviews(user, feedback)
values (@User, @Feedback)
returning
    id as ID,
    create_time as CreateTime,
    user as User,
    feedback as Feedback",
                feedback);
    }
}
