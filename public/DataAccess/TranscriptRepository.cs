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
    public class TranscriptRepository : ITranscriptRepository
    {
        private readonly IDbConnectionFactory _dbFactory;

        public TranscriptRepository(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }


        public async Task<IEnumerable<Transcript>> GetAllAsync()
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = @"
                SELECT t.Id, t.UserId, t.UniversityId, t.FileName, t.FileData, t.ContentType, t.UploadedAt,
                       u.Id, u.UniversityName, u.Program, u.Degree, u.GPA, u.OutOf, u.CreatedAt
                FROM Transcript t
                INNER JOIN University u ON t.UniversityId = u.Id";

            return await conn.QueryAsync<Transcript, University, Transcript>(
                sql,
                (transcript, university) =>
                {
                    transcript.University = university;
                    return transcript;
                },
                splitOn: "Id");
        }


        public async Task<IEnumerable<Transcript>> GetByUserIdAsync(int userId)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = @"SELECT t.Id, t.UserId, t.UniversityId, t.FileName, t.FileData, t.ContentType, t.UploadedAt,u.Id AS UniversityId, u.UniversityName
                                FROM Transcript t
                                INNER JOIN University u ON t.UniversityId = u.Id
                                WHERE t.UserId = @UserId
                                ORDER BY t.Id";

            return await conn.QueryAsync<Transcript, University, Transcript>(
                sql,
                (transcript, university) =>
                {
                    transcript.University = university;
                    return transcript;
                },
                new { UserId = userId },
                splitOn: "UniversityId");
        }



        public async Task<Transcript?> GetByIdAsync(int id)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = @"SELECT 
                                        t.Id, 
                                        t.UserId, 
                                        t.UniversityId, 
                                        t.FileName, 
                                        t.FileData, 
                                        t.ContentType, 
                                        t.UploadedAt,
                                        u.Id AS UniversityId, 
                                        u.UniversityName
                                FROM Transcript t
                                INNER JOIN University u ON t.UniversityId = u.Id
                                WHERE t.Id = @Id";

            var result = await conn.QueryAsync<Transcript, University, Transcript>(
                sql,
                (transcript, university) =>
                {
                    transcript.University = university;
                    return transcript;
                },
                new { Id = id },
                splitOn: "UniversityId" 
            );

            return result.FirstOrDefault();
        }



        public async Task<int> CreateAsync(Transcript transcript)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Transcript (UserId, UniversityId, FileName, FileData, ContentType, UploadedAt)
                VALUES (@UserId, @UniversityId, @FileName, @FileData, @ContentType, @UploadedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await conn.QuerySingleAsync<int>(sql, transcript);
        }


        public async Task<bool> UpdateAsync(Transcript transcript)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = @"
                UPDATE Transcript 
                SET FileName = @FileName, FileData = @FileData, ContentType = @ContentType
                WHERE Id = @Id";

            var affectedRows = await conn.ExecuteAsync(sql, transcript);
            return affectedRows > 0;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _dbFactory.CreateConnection();
            const string sql = "DELETE FROM Transcript WHERE Id = @Id";

            var affectedRows = await conn.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

    }
}
