using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }


        //创建
        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();//事务开启

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                insert into movies(id,slug,title,yearofrelease)
                values (@Id,@Slug,@Title,@YearOfRelease)
                """, parameters: movie, transaction: transaction,cancellationToken: token));

            //插入副表(gender)
            if (result > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition("""
                        insert into genres (movieId,name)
                        values(@MovieId,@Name)
                        """, 
                        parameters: new { MovieId = movie.Id, Name = genre }, 
                        transaction: transaction, cancellationToken: token));
                }
            }

            transaction.Commit();//事务结束
            return result > 0;
        }

        //删除
        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();//事务开启

            await connection.ExecuteAsync(new CommandDefinition("""
                delete from genres where movieid = @id
                """, parameters: new { id }, transaction: transaction, cancellationToken: token));
          

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                delete from movies where id = @id
                """, parameters: new { id }, transaction: transaction, cancellationToken: token));

            transaction.Commit();
            return result > 0;
        }

        //检查存在
        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                select count(1) from movies where id = @id
                """,parameters: new {id}, cancellationToken: token));
        }

        //获取所有
        public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.QueryAsync(new CommandDefinition("""
                select m.*,string_agg(g.name,',') as genres
                from movies m 
                left join 
                genres g 
                on m.id = g.movieId
                group by m.id,m.slug,m.title,m.yearofrelease
                """, cancellationToken: token));
            return result.Select(x =>new Movie 
            { 
                Id = x.id,
                Title = x.title,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(','))
            });
        }

        //通过id获取单个
        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition("""
                    select * from movies where id = @id
                    """, parameters: new { id }, cancellationToken: token));

            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition("""
                    select name from genres where movieid = @id
                    """, parameters: new { id }, cancellationToken: token));

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }

            return movie;
        }

        //通过slug获取单个
        public async Task<Movie?> GetBySlugAsync(string slug,CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition("""
                    select * from movies where slug = @slug
                    """, parameters: new { slug }, cancellationToken: token));

            if (movie is null)
            {
                return null;
            }

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition("""
                    select name from genres where movieid = @Id
                    """, parameters: new { movie.Id }, cancellationToken: token));

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }

            return movie;
        }

        //更新
        public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();//事务开启

            await connection.ExecuteAsync(new CommandDefinition("""
                delete from genres where movieid = @id
                """, parameters: new {id=movie.Id}, transaction: transaction, cancellationToken: token));

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId,name)
                    values (@MovieId,@Name)
                    """,parameters:new { MovieId = movie.Id, Name = genre},
                    transaction: transaction, cancellationToken: token));
            }

            var result = await connection.ExecuteAsync(new CommandDefinition("""
                update movies set slug = @Slug,title = @Title,yearofrelease = @YearOfRelease
                where id = @Id
                """, parameters: movie,
                transaction: transaction, cancellationToken: token));

            transaction.Commit();
            return result > 0;
        }
    }
}
