using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class SanctionTypeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<SanctionType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.SanctionTypes.FindAsync(id);
            if (item is null) return NotFound();

            appDbContext.SanctionTypes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<SanctionType>> GetAll()
        {
            return await appDbContext
                .SanctionTypes.AsNoTracking()
                .ToListAsync();
        }

        public async Task<SanctionType?> GetById(int id)
        {
            return await appDbContext.SanctionTypes.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(SanctionType item)
        {
            var checkIfNull = await CheckName(item.Name);
            if (!checkIfNull) 
                return new GeneralResponse(false, "Sanction Type already added");
            
            appDbContext.SanctionTypes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(SanctionType item)
        {
            var obj = await appDbContext.SanctionTypes.FindAsync(item.Id);
            if (obj is null) return NotFound();
            obj.Name = item.Name; // Assuming SanctionType has a Name property like OvertimeType
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, sanction type record not found");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process completed");

        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.SanctionTypes.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
            return item is null;
        }
    }
}