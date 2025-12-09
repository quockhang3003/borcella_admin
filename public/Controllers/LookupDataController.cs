using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class LookupDataController<T> : ControllerBase where T : class
    {
        protected readonly LookupDataService<T> _service;

        protected LookupDataController(LookupDataService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }

    public class CityController : LookupDataController<City>
    {
        public CityController(CityService service) : base(service) { }
    }

    public class DegreeController : LookupDataController<Degree>
    {
        public DegreeController(DegreeService service) : base(service) { }
    }

    public class MajorController : LookupDataController<Major>
    {
        public MajorController(MajorService service) : base(service) { }
    }

    public class LevelSkillController : LookupDataController<LevelSkill>
    {
        public LevelSkillController(LevelSkillService service) : base(service) { }
    }

    public class QualificationController : LookupDataController<Qualification>
    {
        public QualificationController(QualificationService service) : base(service) { }
    }

    public class QualificationStatusController : LookupDataController<QualificationStatus>
    {
        public QualificationStatusController(QualificationStatusService service) : base(service) { }
    }

    public class LocationOfficeController : LookupDataController<LocationOffice>
    {
        public LocationOfficeController(LocationOfficeService service) : base(service) { }
    }

    public class EmploymentTypeController : LookupDataController<EmploymentType>
    {
        public EmploymentTypeController(EmploymentTypeService service) : base(service) { }
    }

    public class PreferenceController : LookupDataController<Preference>
    {
        public PreferenceController(PreferenceService service) : base(service) { }
    }

}
