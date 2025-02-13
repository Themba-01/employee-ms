using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class OvertimeTypeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<OvertimeType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.OvertimeTypes.FindAsync(id);
            if (item is null) return NotFound();

            appDbContext.OvertimeTypes.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<OvertimeType>> GetAll()
        {
            return await appDbContext
                .OvertimeTypes.AsNoTracking()
                .ToListAsync();
        }

        public async Task<OvertimeType> GetById(int id)
        {
            return await appDbContext.OvertimeTypes.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(OvertimeType item)
        {
           
            var checkIfNull = await CheckName(item.Name);
            if (!checkIfNull) 
                return new GeneralResponse(false, "Overtime Type already added");
            
            appDbContext.OvertimeTypes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(OvertimeType item)
        {
            var obj = await appDbContext.OvertimeTypes.FirstOrDefaultAsync(ot => ot.Id == item.Id);
            if (obj is null) return NotFound();
            // Since OvertimeType doesn't have additional properties in your model, 
            // this update might just be about updating relations or could be extended if more properties are added.
            obj.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, overtime type record not found");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process completed");

        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.OvertimeTypes.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
            return item is null ? true:false;
        }
    }
}