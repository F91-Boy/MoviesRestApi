using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public RatingRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        //删除评分
        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.ExecuteAsync(new CommandDefinition("""
                delete from ratings
                where movieId = @movieId and userId = @userId
                """, new {userId,movieId},cancellationToken:token));

            return result > 0;
        }

        //获取电影评分
        public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
                select round(avg(r.rating),1) from ratings r
                where movieId = @movieId
                """,new {movieId},cancellationToken:token));
        }

        //获取该用户打的电影评分
        public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await connection.QuerySingleOrDefaultAsync<(float?,int?)>(new CommandDefinition("""
                select round(avg(rating),1),
                        (select top 1 rating
                        from ratings
                        where movieId = @movieId
                          and userId = @userId )
                        from ratings
                        where movieId = @movieId
                """, new { movieId , userId }, cancellationToken: token));
        }

        //获取所有评分
        public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
                select r.rating,r.movieId,m.slug
                from ratings r
                inner join movies m on r.movieId = m.id
                where userId = @userId
                """, new { userId }, cancellationToken: token));
        }

        //给电影打评分
        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
        {
            //insert into ratings(userId,movieId,rating)
            //values(@userId,@movieId,@rating)
            //on conflict(userId,movieId) do update
            //    set rating = @rating
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
             var result  = await connection.ExecuteAsync(new CommandDefinition("""
                MERGE INTO ratings AS Target
                USING (VALUES (@userId, @movieId, @rating)) AS Source (userId, movieId, rating)
                ON Target.userId = Source.userId AND Target.movieId = Source.movieId
                WHEN MATCHED THEN
                    UPDATE SET Target.rating = Source.rating
                WHEN NOT MATCHED THEN
                    INSERT (userId, movieId, rating)
                    VALUES (Source.userId, Source.movieId, Source.rating);
                """, new { userId,movieId, rating }, cancellationToken: token));
            return result > 0;
        }
    }
}
