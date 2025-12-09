using Dapper;
using Domain.DTO;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CandidateSearchRepository : ICandidateSearchRepository
    {
        private readonly IDbConnectionFactory _dbFactory;

        public CandidateSearchRepository(IDbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<CandidateSearchResponse> SearchCandidatesAsync(CandidateSearchFilter filter)
        {
            using var conn = _dbFactory.CreateConnection();

            var whereClauses = new List<string>();
            var parameters = new DynamicParameters();

            System.Diagnostics.Debug.WriteLine($"[REPO] Received Status: '{filter.Status ?? "NULL"}'");
            System.Diagnostics.Debug.WriteLine($"[REPO] IsNullOrWhiteSpace: {string.IsNullOrWhiteSpace(filter.Status)}");

            var fromClause = new StringBuilder(@"
                 FROM [Users] u
                 LEFT JOIN Education e ON u.ID = e.UserID AND e.DeletedAt IS NULL
                 LEFT JOIN University uni ON e.UniversityID = uni.Id
                 LEFT JOIN Major maj ON e.MajorID = maj.Id
                 LEFT JOIN LocationOffice lo ON u.PreferableOfficeLocationID = lo.Id
                 INNER JOIN HeardAbout ha ON u.ID = ha.UserId AND u.StatusID = ha.StatusID
                 LEFT JOIN Preference fp ON u.FirstPreferenceID = fp.Id
                 LEFT JOIN Preference sp ON u.SecondPreferenceID = sp.Id
                 WHERE ha.CreatedAt IS NOT NULL 
            ");

            bool statusHandled = false;

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var trimmedStatus = filter.Status.Trim();

                if (trimmedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    whereClauses.Add("u.StatusID = 1");
                    whereClauses.Add("ha.StatusID = 1");
                    statusHandled = true;
                }
                else if (trimmedStatus.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                {
                    whereClauses.Add("u.StatusID = -1");
                    whereClauses.Add("ha.StatusID = -1");
                    statusHandled = true;
                }
                else if (trimmedStatus.Equals("Exported", StringComparison.OrdinalIgnoreCase) ||
                         trimmedStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) ||
                         trimmedStatus.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    whereClauses.Add("u.StatusID = 1");
                    whereClauses.Add("ha.StatusID = 1");
                    whereClauses.Add("u.ApplicationStatus = @ApplicationStatus");
                    parameters.Add("ApplicationStatus", trimmedStatus);
                    statusHandled = true;
                }
            }

            if (!statusHandled)
            {
                whereClauses.Add("u.StatusID = 1");
                whereClauses.Add("ha.StatusID = 1");
            }
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                whereClauses.Add("(u.FirstName LIKE @Name OR u.LastName LIKE @Name OR u.VietnameseName LIKE @Name OR CONCAT(u.FirstName, ' ', u.LastName) LIKE @Name)");
                parameters.Add("Name", $"%{filter.Name.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                whereClauses.Add("u.Email LIKE @Email");
                parameters.Add("Email", $"%{filter.Email.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.University))
            {
                whereClauses.Add("uni.UniversityName LIKE @University");
                parameters.Add("University", $"%{filter.University.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.Major))
            {
                whereClauses.Add("maj.MajorName LIKE @Major");
                parameters.Add("Major", $"%{filter.Major.Trim()}%");
            }

            if (filter.FromDate.HasValue)
            {
                whereClauses.Add("ha.CreatedAt >= @FromDate");
                parameters.Add("FromDate", filter.FromDate.Value.Date);
            }

            if (filter.ToDate.HasValue)
            {
                whereClauses.Add("ha.CreatedAt < @ToDate");
                parameters.Add("ToDate", filter.ToDate.Value.Date.AddDays(1));
            }

            if (!string.IsNullOrWhiteSpace(filter.Office))
            {
                whereClauses.Add("lo.LocationName LIKE @Office");
                parameters.Add("Office", $"%{filter.Office}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.Gender))
            {
                whereClauses.Add("u.Gender = @Gender");
                parameters.Add("Gender", filter.Gender);
            }

            if (!string.IsNullOrWhiteSpace(filter.FirstPreference))
            {
                whereClauses.Add("fp.PreferenceName LIKE @FirstPreference");
                parameters.Add("FirstPreference", $"%{filter.FirstPreference}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.SecondPreference))
            {
                whereClauses.Add("sp.PreferenceName LIKE @SecondPreference");
                parameters.Add("SecondPreference", $"%{filter.SecondPreference}%");
            }

            if (filter.ProgramId.HasValue && filter.ProgramId > 0)
            {
                whereClauses.Add("ha.RecruitmentProgramId = @ProgramId");
                parameters.Add("ProgramId", filter.ProgramId.Value);
            }

            var whereClause = string.Empty;
            if (whereClauses.Any())
            {
                whereClause = " AND " + string.Join(" AND ", whereClauses);
            }

            // COUNT QUERY
            var countSql = new StringBuilder("SELECT COUNT(DISTINCT u.ID) ");
            countSql.Append(fromClause);
            countSql.Append(whereClause);

            System.Diagnostics.Debug.WriteLine("=== COUNT SQL ===");
            System.Diagnostics.Debug.WriteLine(countSql.ToString());
            System.Diagnostics.Debug.WriteLine("=== PARAMETERS ===");
            foreach (var paramName in parameters.ParameterNames)
            {
                System.Diagnostics.Debug.WriteLine($"{paramName} = {parameters.Get<object>(paramName)}");
            }
            System.Diagnostics.Debug.WriteLine("==================");

            var totalCount = await conn.ExecuteScalarAsync<int>(countSql.ToString(), parameters);

            if (totalCount == 0)
            {
                return new CandidateSearchResponse
                {
                    Candidates = new List<CandidateSearchResult>(),
                    TotalCount = 0,
                    CurrentPage = filter.Page,
                    PageSize = filter.PageSize
                };
            }

            var dataSql = new StringBuilder(@"
            SELECT 
                u.ID as UserId,
                CONCAT(u.FirstName, ' ', u.LastName) as FullName,
                u.DateOfBirth,
                u.Gender,
                CASE 
                    WHEN u.StatusID = 1 THEN 'Active'
                    WHEN u.StatusID = -1 THEN 'Inactive'
                    ELSE 'Unknown'
                END as Status,
                ISNULL(lo.LocationName, '') as Office,
                u.Mobile,
                u.Email,
                ISNULL(u.IDCardNoEncrypted, '') as IDCardNo,
                ISNULL(uni.UniversityName, '') as University,
                ISNULL(maj.MajorName, '') as Major,
                ISNULL(CAST(e.GPA as VARCHAR(10)), '') as GPA,
                ISNULL(ha.CreatedAt, u.CreatedAt) as SubmittedOn");

            dataSql.Append(fromClause);
            dataSql.Append(whereClause);
            dataSql.AppendLine(" ORDER BY ISNULL(ha.CreatedAt, u.CreatedAt) DESC");
            dataSql.AppendLine(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

            parameters.Add("Offset", (filter.Page - 1) * filter.PageSize);
            parameters.Add("PageSize", filter.PageSize);

            var candidates = await conn.QueryAsync<CandidateSearchResult>(dataSql.ToString(), parameters);

            return new CandidateSearchResponse
            {
                Candidates = candidates.ToList(),
                TotalCount = totalCount,
                CurrentPage = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<bool> UpdateCandidateStatusAsync(int userId, int statusId)
        {
            using var conn = _dbFactory.CreateConnection();

            var sql = @"
                UPDATE Users 
                SET StatusID = @StatusId,
                    UpdatedAt = GETDATE()
                WHERE ID = @UserId";

            var rowsAffected = await conn.ExecuteAsync(sql, new { UserId = userId, StatusId = statusId });
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateHeardAboutStatusAsync(int userId, int statusId)
        {
            using var conn = _dbFactory.CreateConnection();

            var sql = @"
                UPDATE HeardAbout 
                SET StatusID = @StatusId,
                    UpdatedAt = GETDATE()
                WHERE UserId = @UserId";

            var rowsAffected = await conn.ExecuteAsync(sql, new { UserId = userId, StatusId = statusId });
            return rowsAffected > 0;
        }
    }
}