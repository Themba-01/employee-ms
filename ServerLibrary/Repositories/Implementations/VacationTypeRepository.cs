using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class VacationTypeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<VacationType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.VacationTypes.FindAsync(id);
            if (item is null) return NotFound();

            appDbContext.VacationTypes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<VacationType>> GetAll()
        {
            return await appDbContext
                .VacationTypes.AsNoTracking()
                .ToListAsync();
        }

        public async Task<VacationType?> GetById(int id)
        {
            return await appDbContext.VacationTypes.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(VacationType item)
        {
            var checkIfNull = await CheckName(item.Name);
            if (!checkIfNull) 
                return new GeneralResponse(false, "Vacation Type already added");
            
            appDbContext.VacationTypes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(VacationType item)
        {
            var obj = await appDbContext.VacationTypes.FindAsync(item.Id);
            if (obj is null) return NotFound();
            obj.Name = item.Name; // Assuming VacationType has a Name property like SanctionType
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, vacation type record not found");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process completed");

        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.VacationTypes.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
            return item is null;
        }
    }
}