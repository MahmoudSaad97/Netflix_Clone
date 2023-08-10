using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class UserService: IUserService
    {
        MovieAppContext db;

        public UserService(MovieAppContext _db)
        {
            db = _db;
        }
        public async Task<List<User>> GetAll()
        {
            return await db.Users.Include(a => a.ProfileUsers)
                .Include(a => a.Subscribe).Include(a => a.Payment).ToListAsync();
        }
        public async Task<User> GetById(int id)
        {
            User User = await db.Users.Include(a => a.ProfileUsers)
                .Include(a => a.Subscribe).Include(a => a.Payment).FirstOrDefaultAsync(a => a.Id == id);
            return User;
        }
        public async Task<User> Add(User User)
        {
            int userId = User.Id;

            await db.Users.AddAsync(User);
            await db.SaveChangesAsync();

            return User;
        }
        public async Task Update(User User)
        {
            //User.NormalizedEmail = User.Email.ToUpper();
            //User.UserName = User.Email;
            //User.NormalizedUserName = User.UserName.ToUpper();

            db.Users.Update(User);
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id)
        {
            db.Users.Remove(await GetById(id));
            await db.SaveChangesAsync();
        }
    }
}
