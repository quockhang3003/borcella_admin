using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UniversityRepository : IUniversityRepository
    {
        private readonly IDbConnectionFactory _dbFactory;

        public UniversityRepository(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }


        public async Task<IEnumerable<University>> GetAllAsync()
        {
            using var conn = _dbFactory.CreateConnection();
            return await conn.QueryAsync<University>("SELECT * FROM University");
        }


        public async Task<IEnumerable<University>> GetByUserIdAsync(int userId)
        {
            using var conn = _dbFactory.CreateConnection();
            return await conn.QueryAsync<University>(
                @"SELECT u.Id, u.UniversityName
          FROM Education e
          INNER JOIN University u ON e.UniversityId = u.Id
          WHERE e.UserId = @UserId
          ORDER BY e.Id",
                new { UserId = userId });
        }



        public async Task<University?> GetByIdAsync(int id)
        {
            using var conn = _dbFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<University>(
                "SELECT * FROM University WHERE Id = @Id",
                new { Id = id });
        }


        public async Task<int> CreateAsync(University university)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = @"
                INSERT INTO University (UserId, UniversityName, Program, Degree, GPA, OutOf, CreatedAt)
                VALUES (@UserId, @UniversityName, @Program, @Degree, @GPA, @OutOf, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await conn.QuerySingleAsync<int>(sql, university);
        }


        public async Task<bool> UpdateAsync(University university)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = @"
                UPDATE University 
                SET UniversityName = @UniversityName, Program = @Program, 
                    Degree = @Degree, GPA = @GPA, OutOf = @OutOf
                WHERE Id = @Id";

            var affectedRows = await conn.ExecuteAsync(sql, university);
            return affectedRows > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = "DELETE FROM University WHERE Id = @Id";

            var affectedRows = await conn.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

    }
}
