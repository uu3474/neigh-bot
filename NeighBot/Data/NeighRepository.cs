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

        async Task<long> AddOrUpdateUserCore(NpgsqlConnection conection, User toUser)
            => await conection.QuerySingleAsync<long>(
@"insert into users(id, first_name, last_name, user_name)
values (@Id, @FirstName, @LastName, @Username)
on conflict do
update set first_name = @FirstName, last_name = @LastName, user_name = @Username
returning id"
                , toUser);

        public async Task<long> AddOrUpdateUser(User toUser)
        {
            using var connection = await CreateAndOpenConnection();
            return await AddOrUpdateUserCore(connection, toUser);
        }

        async Task<long> AddOrUpdateUserCore(NpgsqlConnection conection, Contact toContact)
            => await conection.QuerySingleAsync<long>(
@"insert into users(id, first_name, last_name, phone)
values (@UserId, @FirstName, @LastName, @PhoneNumber)
on conflict do
update set first_name = @FirstName, last_name = @LastName, phone = @PhoneNumber
returning id"
                , toContact);

        public async Task<long> AddOrUpdateUser(Contact toContact)
        {
            using var connection = await CreateAndOpenConnection();
            return await AddOrUpdateUserCore(connection, toContact);
        }

        async Task<long> AddReviewCore(NpgsqlConnection conection, DBReview review)
            => await conection.QuerySingleAsync<long>(
@"insert into reviews(from_user, to_user, grade, review)
values (@FromUser, @ToUser, @Grade, @Review)
returning id",
                review);

        public async Task<long> AddReview(DBReview review)
        {
            using var connection = await CreateAndOpenConnection();
            return await AddReviewCore(connection, review);
        }

        public async Task<long> AddReview(User fromUser, Contact toContact, DBReview review)
        {
            using var connection = await CreateAndOpenConnection();
            
            var fromID = await AddOrUpdateUserCore(connection, fromUser);
            review.FromUser = fromID;

            var toID = await AddOrUpdateUserCore(connection, toContact);
            review.ToUser = toID;

            var reviewID = await AddReviewCore(connection, review);
            review.ID = reviewID;
            
            return reviewID;
        }

        public Task<IEnumerable<DBReview>> GetReviews(int toUserID)
        {
            throw new NotImplementedException();
        }
    }
}
