using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class SanctionRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Sanction>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Sanctions.FirstOrDefaultAsync(s => s.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Sanctions.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Sanction>> GetAll()
        {
            return await appDbContext
                .Sanctions.AsNoTracking()
                .Include(s => s.SanctionType)
                .ToListAsync();
        }

        public async Task<Sanction?> GetById(int id)
        {
            return await appDbContext.Sanctions.FirstOrDefaultAsync(s => s.EmployeeId == id);
        }

        public async Task<GeneralResponse> Insert(Sanction item)
        {
            appDbContext.Sanctions.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Sanction item)
        {
            var obj = await appDbContext.Sanctions.FirstOrDefaultAsync(s => s.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();
            obj.Date = item.Date;
            obj.Punishment = item.Punishment;
            obj.PunishmentDate = item.PunishmentDate;
            // Note: SanctionType is not directly updated here as it's a navigation property. 
            // If you need to update the SanctionType, you would do this through the SanctionTypeId.
            obj.SanctionType = item.SanctionType;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, sanction record not found");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process completed");

        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}