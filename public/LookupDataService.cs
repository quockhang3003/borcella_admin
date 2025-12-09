using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class LookupDataService<T> where T : class
    {
        protected readonly ILookupDataRepository<T> _repo;

        public LookupDataService(ILookupDataRepository<T> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _repo.GetAllAsync();
    }

    // Concrete Services
    public class CityService : LookupDataService<City>
    {
        public CityService(ICityRepository repo) : base(repo) { }
    }

    public class DegreeService : LookupDataService<Degree>
    {
        public DegreeService(IDegreeRepository repo) : base(repo) { }
    }

    public class MajorService : LookupDataService<Major>
    {
        public MajorService(IMajorRepository repo) : base(repo) { }
    }

    public class LevelSkillService : LookupDataService<LevelSkill>
    {
        public LevelSkillService(ILevelSkillRepository repo) : base(repo) { }
    }

    public class QualificationService : LookupDataService<Qualification>
    {
        public QualificationService(IQualificationRepository repo) : base(repo) { }
    }

    public class QualificationStatusService : LookupDataService<QualificationStatus>
    {
        public QualificationStatusService(IQualificationStatusRepository repo) : base(repo) { }
    }

    public class LocationOfficeService : LookupDataService<LocationOffice>
    {
        public LocationOfficeService(ILocationOfficeRepository repo) : base(repo) { }
    }

    public class EmploymentTypeService : LookupDataService<EmploymentType>
    {
        public EmploymentTypeService(IEmploymentTypeRepository repo) : base(repo) { }
    }

    public class PreferenceService : LookupDataService<Preference>
    {
        public PreferenceService(IPreferenceRepository repo) : base(repo) { }
    }

}
