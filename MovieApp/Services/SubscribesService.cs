using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class SubscribesService : ISubscribesService
    {
        MovieAppContext db;
        private readonly IPaymentService paymentService;

        public SubscribesService(MovieAppContext _db, IPaymentService _paymentService)
        {
            db = _db;
            paymentService = _paymentService;
        }

        public async Task<string> GetLastSubscriptionWithPrice()
        {
            var prevPayment = await paymentService.GetAllByLoggedUserId();
            if (prevPayment != null)
            {
                var prevSubscriptionId = prevPayment.Select(a => a.Subscribe.SubscribeID).FirstOrDefault();
                if (prevSubscriptionId > 0)
                {
                    var subscribe = await GetById(prevSubscriptionId);
                    if (subscribe != null)
                    {
                        return subscribe.Role.Name + " - " + subscribe.Price + "$";
                    }
                }
            }
            return "";
        }

        public async Task<string> GetRoleBySubscribeId(int SubId)
        {
            Subscribe subscribe = await GetById(SubId);

            return subscribe.Role.Name;
        }

        public async Task<List<SelectListItem>> GetAllSubscriptionsWithRole()
        {
            List<Subscribe> subscribes = await GetAll();

            var subList = subscribes.Select(u => new SelectListItem
            {
                Text = u.Role.Name + " - " + u.Price + "$",
                Value = u.SubscribeID.ToString()
            });

            return subList.ToList();
        }

        public async Task<List<Subscribe>> GetAll()
        {
            return await db.Subscribes.Include(a=> a.Role).ToListAsync();
        }
        public async Task<Subscribe> GetById(int id)
        {
            return await db.Subscribes.Include(a => a.Role).FirstOrDefaultAsync(a => a.SubscribeID == id);
        }
        public async Task<Subscribe> Add(Subscribe Subscribe)
        {
            await db.Subscribes.AddAsync(Subscribe);
            await db.SaveChangesAsync();

            return Subscribe;
        }
        public async Task Update(Subscribe Subscribe)
        {
            db.Subscribes.Update(Subscribe);
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id)
        {
            db.Subscribes.Remove(await GetById(id));
            await db.SaveChangesAsync();
        }
    }
}
