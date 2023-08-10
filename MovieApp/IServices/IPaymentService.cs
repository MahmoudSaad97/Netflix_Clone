using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface IPaymentService
    {
        public Task<List<Payment>> GetAll();
        public Task<Payment> GetById(int id);
        public Task<Payment> Add(Payment Payment);
        public Task Update(Payment Payment);
        public Task DeleteById(int id);
        public Task<DateTime> GetDateTimeExpireByLoggedUserId();
        public Task<List<Payment>> GetAllByLoggedUserId();
    }
}
