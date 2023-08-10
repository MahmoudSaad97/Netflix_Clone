using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface ISubscribesService
    {
        public Task<List<Subscribe>> GetAll();
        public Task<Subscribe> GetById(int id);
        public Task<Subscribe> Add(Subscribe Subscribe);
        public Task Update(Subscribe Subscribe);
        public Task DeleteById(int id);
        public Task<List<SelectListItem>> GetAllSubscriptionsWithRole();
        public Task<string> GetRoleBySubscribeId(int SubId);
        public Task<string> GetLastSubscriptionWithPrice();
    }
}
