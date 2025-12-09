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
    public class LookupDataRepository<T> : ILookupDataRepository<T> where T : class
    {
        private readonly IDbConnectionFactory _dbFactory;
        private readonly string _tableName;

        public LookupDataRepository(IDbConnectionFactory dbFactory, string tableName)
        {
            _dbFactory = dbFactory;
            _tableName = tableName;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var conn = _dbFactory.CreateConnection();
            return await conn.QueryAsync<T>($"SELECT * FROM {_tableName}");
        }
    }

    // Concrete Repositories
    public class CityRepository : LookupDataRepository<City>, ICityRepository
    {
        public CityRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "City") { }
    }

    public class DegreeRepository : LookupDataRepository<Degree>, IDegreeRepository
    {
        public DegreeRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "Degree") { }
    }

    public class MajorRepository : LookupDataRepository<Major>, IMajorRepository
    {
        public MajorRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "Major") { }
    }

    public class LevelSkillRepository : LookupDataRepository<LevelSkill>, ILevelSkillRepository
    {
        public LevelSkillRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "LevelSkill") { }
    }

    public class QualificationRepository : LookupDataRepository<Qualification>, IQualificationRepository
    {
        public QualificationRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "Qualification") { }
    }

    public class QualificationStatusRepository : LookupDataRepository<QualificationStatus>, IQualificationStatusRepository
    {
        public QualificationStatusRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "QualificationStatus") { }
    }

    public class LocationOfficeRepository : LookupDataRepository<LocationOffice>, ILocationOfficeRepository
    {
        public LocationOfficeRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "LocationOffice") { }
    }

    public class EmploymentTypeRepository : LookupDataRepository<EmploymentType>, IEmploymentTypeRepository
    {
        public EmploymentTypeRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "EmploymentType") { }
    }

    public class PreferenceRepository : LookupDataRepository<Preference>, IPreferenceRepository
    {
        public PreferenceRepository(IDbConnectionFactory dbFactory) : base(dbFactory, "Preference") { }
    }

}
