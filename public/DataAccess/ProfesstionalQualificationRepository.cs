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
    public class ProfesstionalQualificationRepository : IProfessionalQualificationRepository
    {
        private readonly IDbConnectionFactory _dbFactory;
        public ProfesstionalQualificationRepository(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task AddAsync(ProfessionalQualification professionalQualification)
        {
            using var conn = _dbFactory.CreateConnection();
            var sql = @"INSERT INTO ProfessionalQualification(UserID, QualificationID, Paper, FromDate, ToDate, StatusID, Notes, CreatedAt)
                                VALUES(@UserID, @QualificationID, @Paper, @FromDate, @ToDate, @StatusID, @Notes, @CreatedAt)";
            await conn.ExecuteAsync(sql, professionalQualification);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _dbFactory.CreateConnection();
            var sql = "UPDATE ProfessionalQualification SET DeletedAt = @DeletedAt WHERE Id = @Id";
            await conn.ExecuteAsync(sql, new {Id = id, DeletedAt = DateTime.UtcNow});
        }

        public async Task<IEnumerable<ProfessionalQualification>> GetAllAsync()
        {
            using var conn = _dbFactory.CreateConnection();
            return await conn.QueryAsync<ProfessionalQualification>("SELCT * FROM ProfessionalQualification");
        }

        public async Task<IEnumerable<ProfessionalQualification>> GetByUserIdAsync(int userId)
        {
            using var conn = _dbFactory.CreateConnection();
            var sql = "SELECT * FROM ProfessionalQualification WHERE UserID = @UserID AND DeletedAt IS NULL ORDER BY CreatedAt DESC";
            return await conn.QueryAsync<ProfessionalQualification>(sql, new { UserID = userId });
        }

        public async Task UpdateAsync(ProfessionalQualification professionalQualification)
        {
            using var conn = _dbFactory.CreateConnection();
            var sql = @"UPDATE ProfessionalQualification SET
                        QualificationID = @QualificationID,
                        Paper = @Paper,
                        FromDate = @FromDate,
                        ToDate = @ToDate,
                        StatusID = @StatusID,
                        Notes = @Notes,
                        UpdatedAt = @UpdatedAt
                        WHERE Id = @Id AND DeletedAt IS NULL";
            await conn.ExecuteAsync(sql, professionalQualification);
        }
    }
}
