using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class GenrieService : IGenrieService
    {

        private readonly MovieAppContext db;
        public GenrieService(MovieAppContext _db)
        {
            db = _db;
        }
        public async Task<List<Genrie>> GetAll()
        {
            return await db.Genries.OrderBy(g=> g.GenrieName).ToListAsync();
        }
        public async Task<Genrie> GetById(int id)
        {
            return await db.Genries.FirstOrDefaultAsync(a => a.GenrieID == id);
        }
        public async Task<Genrie> Add(Genrie Genrie)
        {
            await db.Genries.AddAsync(Genrie);
            await db.SaveChangesAsync();

            return Genrie;
        }
        public async Task Update(Genrie Genrie)
        {
            db.Genries.Update(Genrie);
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id)
        {
            db.Genries.Remove(await GetById(id));
            await db.SaveChangesAsync();
        }
    }
}
