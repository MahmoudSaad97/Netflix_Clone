using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;

namespace MovieApp.Services
{
    public class PaymentService : IPaymentService
    {
        MovieAppContext db;
        private readonly ILoggedDataService loggedDataService;

        public PaymentService(MovieAppContext _db, ILoggedDataService _loggedDataService)
        {
            db = _db;
            loggedDataService = _loggedDataService;
        }

        public async Task<DateTime> GetDateTimeExpireByLoggedUserId()
        {
            int userid = loggedDataService.LoggedUserId();
            var payments = await GetAll();
            var payment = payments.OrderByDescending(a => a.PaymentDate).FirstOrDefault(a => a.UserID == userid);
            if (payment == null) return DateTime.MinValue;
            var expireDate = payment.PaymentDate.AddDays(payment.Subscribe.DurationDays);

            return expireDate;
        }

        public async Task<List<Payment>> GetAllByLoggedUserId()
        {
            int userid = loggedDataService.LoggedUserId();
            var payments = await GetAll();
            var payment = payments.Where(a=> a.UserID == userid).ToList();

            return  payment;
        }

        public async Task<List<Payment>> GetAll()
        {
            return await db.Payments.OrderByDescending(a => a.PaymentDate).Include(a=> a.User).Include(a=> a.Subscribe).ToListAsync();
        }
        public async Task<Payment> GetById(int id)
        {
            return await db.Payments.OrderByDescending(a=> a.PaymentDate).Include(a => a.User).Include(a => a.Subscribe)
                .FirstOrDefaultAsync(a => a.PaymentID == id);
        }
        public async Task<Payment> Add(Payment Payment)
        {
            await db.Payments.AddAsync(Payment);
            await db.SaveChangesAsync();

            return Payment;
        }
        public async Task Update(Payment Payment)
        {
            db.Payments.Update(Payment);
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id)
        {
            db.Payments.Remove(await GetById(id));
            await db.SaveChangesAsync();
        }
    }
}
