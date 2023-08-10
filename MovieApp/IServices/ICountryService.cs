using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface ICountryService
    {
        public Task<string> GetNameById(int id);
        public Task<SelectList> GetAllAsSelectList();
        public Task<List<Country>> GetAll();
        public Task<Country> GetById(int id);
        public Task<Country> Add(Country Country);
        public Task Update(Country Country);
        public Task DeleteById(int id);
    }
}
