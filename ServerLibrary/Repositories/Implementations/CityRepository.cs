using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Repositories.Implementations
{
    public class CityRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<City>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var city = await appDbContext.Cities.FindAsync(id);
            if (city is null) return NotFound();

            appDbContext.Cities.Remove(city);
            await Commit();
            return Success();
        }

        
        public async Task<List<City>> GetAll()
        {
            return await appDbContext
            .Cities.AsNoTracking()
            .Include(c => c.Country)
            .ToListAsync();
        }

        public async Task<City?> GetById(int id)
        {
            return await appDbContext.Cities.FindAsync(id);
        }

       
        public async Task<GeneralResponse> Insert(City item)
        {
           
            var checkIfNull = await CheckName(item.Name);
            if (!checkIfNull) 
                return new GeneralResponse(false, "City already added");
            
            appDbContext.Cities.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(City item)
        {
            var city = await appDbContext.Cities.FindAsync(item.Id);
            if (city is null) return NotFound();
            city.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, city not found");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process completed");

        private async Task Commit() => await appDbContext.SaveChangesAsync();

      
         private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Cities.FirstOrDefaultAsync(x => x.Name!.ToLower() == name.ToLower());
            return item is null ? true:false;
        }
    }
}