using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Database
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DbInitializer(IDbConnectionFactory dbConnectionFactory)
        {
                _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task InitializeAsync()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();

            //添加电影表 movies
            await connection.ExecuteAsync("""
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'movies')
                BEGIN
                    CREATE TABLE movies (
                        id UNIQUEIDENTIFIER PRIMARY KEY,
                        slug NVARCHAR(50) NOT NULL,
                        title NVARCHAR(50) NOT NULL,
                        yearofrelease INT NOT NULL
                    );
                END
                """);

            //添加唯一索引,保证slug是唯一的 
            await connection.ExecuteAsync("""
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'movies_slug_idx' AND object_id = OBJECT_ID('movies'))
                BEGIN
                    CREATE UNIQUE INDEX movies_slug_idx 
                    ON movies(slug);
                END
                """);

            //添加类别表 genres
            await connection.ExecuteAsync("""
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'genres')
                BEGIN
                    CREATE TABLE genres (
                        movieId UNIQUEIDENTIFIER REFERENCES movies(Id),
                        name NVARCHAR(50) NOT NULL
                    );
                END
                """);

            //添加评分表 ratings
            await connection.ExecuteAsync("""
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'ratings')
                BEGIN
                    CREATE TABLE ratings (
                        userId UNIQUEIDENTIFIER,
                        movieId UNIQUEIDENTIFIER REFERENCES movies(Id),
                        rating FLOAT,
                        primary key(userId,movieId)
                    );
                END
                """);
        }
    }
}
