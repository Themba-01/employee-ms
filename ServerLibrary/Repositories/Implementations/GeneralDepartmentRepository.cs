using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class GeneralDepartmentRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<GeneralDepartment>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var generalDepartment = await appDbContext.GeneralDepartments.FindAsync(id);
            if (generalDepartment is null) return NotFound();

            appDbContext.GeneralDepartments.Remove(generalDepartment);
            await Commit();
            return Success();
        }

        public async Task<List<GeneralDepartment>> GetAll()
        {
            return await appDbContext.GeneralDepartments.ToListAsync();
        }

        public async Task<GeneralDepartment?> GetById(int id)
        {
            return await appDbContext.GeneralDepartments.FindAsync(id);
        }

        public async Task<GeneralResponse> Insert(GeneralDepartment item)
        {
            var checkIfNull = await CheckName(item.Name);
            if (!checkIfNull) 
                return new GeneralResponse(false, "General Department already added");
            
            appDbContext.GeneralDepartments.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(GeneralDepartment item)
        {
            var generalDepartment = await appDbContext.GeneralDepartments.FindAsync(item.Id);
            if (generalDepartment is null) return NotFound();
            generalDepartment.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, general department not found");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process completed");

        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.GeneralDepartments.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
            return item is null ? true:false;
        }
    }
}