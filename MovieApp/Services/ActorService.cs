using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class ActorService : IActorService
    {
        private readonly MovieAppContext db;
        public ActorService(MovieAppContext _db)
        {
            db = _db;
        }
        public async Task<List<Actor>> GetAll()
        {
            return await db.Actors.ToListAsync();
        }
        public async Task<Actor> GetById(int id)
        {
            return await db.Actors.FirstOrDefaultAsync(a => a.ActorID == id);
        }
        public async Task<Actor> Add(Actor Actor)
        {
            await db.Actors.AddAsync(Actor);
            await db.SaveChangesAsync();

            return Actor;
        }
        public async Task Update(Actor Actor)
        {
            db.Actors.Update(Actor);
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id)
        {
            db.Actors.Remove(await GetById(id));
            await db.SaveChangesAsync();
        }
    }
}
