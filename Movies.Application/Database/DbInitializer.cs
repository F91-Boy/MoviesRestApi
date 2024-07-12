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

            await connection.ExecuteAsync("""
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'movies_slug_idx' AND object_id = OBJECT_ID('movies'))
                BEGIN
                    CREATE UNIQUE INDEX movies_slug_idx 
                    ON movies(slug);
                END
                """);

            await connection.ExecuteAsync("""
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'genres')
                BEGIN
                    CREATE TABLE genres (
                        movieId UNIQUEIDENTIFIER REFERENCES movies(Id),
                        name NVARCHAR(50) NOT NULL
                    );
                END
                """);
        }
    }
}
