using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ProfessionalQualificationService
    {
        private readonly IProfessionalQualificationRepository _repo;
        public ProfessionalQualificationService(IProfessionalQualificationRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<ProfessionalQualification>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<IEnumerable<ProfessionalQualification>> GetByUserIdAsync(int userId) => await _repo.GetByUserIdAsync(userId);

        public async Task AddProfessionalQualification(ProfessionalQualificationDTO dto, int userId)
        {
            var professionalQualification = new ProfessionalQualification
            {
                UserID = userId,
                QualificationID = dto.QualificationID.Value,
                Paper = dto.Paper,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                StatusID = dto.StatusID.Value,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(professionalQualification);
        }
        public async Task UpdateProfessionalQualification(ProfessionalQualificationDTO dto, int id)
        {
            var professionalQualification = new ProfessionalQualification
            {
                Id = id,
                QualificationID = dto.QualificationID.Value,
                Paper = dto.Paper,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                StatusID = dto.StatusID.Value,
                Notes = dto.Notes,
                UpdatedAt = DateTime.UtcNow
            };
            await _repo.UpdateAsync(professionalQualification);
        }
        public async Task DeleteProfessionalQualification(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
