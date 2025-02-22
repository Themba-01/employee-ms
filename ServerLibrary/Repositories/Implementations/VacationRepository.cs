using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class VacationRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Vacation>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Vacations.FirstOrDefaultAsync(v => v.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Vacations.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Vacation>> GetAll()
        {
            return await appDbContext
                .Vacations.AsNoTracking()
                .Include(v => v.VacationType)
                .ToListAsync();
        }

        public async Task<Vacation?> GetById(int id)
        {
            return await appDbContext.Vacations.FirstOrDefaultAsync(v => v.EmployeeId == id);
        }

        public async Task<GeneralResponse> Insert(Vacation item)
        {
            appDbContext.Vacations.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Vacation item)
        {
            var obj = await appDbContext.Vacations.FirstOrDefaultAsync(v => v.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();
            obj.StartDate = item.StartDate;
            obj.NumberOfDays = item.NumberOfDays;
            obj.VacationTypeId = item.VacationTypeId; 
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, vacation record not found");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process completed");

        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}