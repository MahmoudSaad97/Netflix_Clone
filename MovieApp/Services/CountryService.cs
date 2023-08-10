using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class CountryService : ICountryService
    {
        private readonly MovieAppContext db;
        public CountryService(MovieAppContext _db)
        {
            db = _db;
        }
        public async Task<string> GetNameById(int id)
        {
            var country = await db.Countries.FirstOrDefaultAsync(a => a.Id == id);
            return country.Name;
        }
        public async Task<SelectList> GetAllAsSelectList()
        {
            return new SelectList(await GetAll(), "Id", "Name");
        }
        public async Task<List<Country>> GetAll()
        {
            return await db.Countries.ToListAsync();
        }
        public async Task<Country> GetById(int id)
        {
            return await db.Countries.FirstOrDefaultAsync(a=> a.Id == id);
        }
        public async Task<Country> Add(Country Country)
        {
            await db.Countries.AddAsync(Country);
            await db.SaveChangesAsync();

            return Country;
        }
        public async Task Update(Country Country)
        {
            db.Countries.Update(Country);
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id)
        {
            db.Countries.Remove(await GetById(id));
            await db.SaveChangesAsync();
        }
    }
}
