using MovieApp.Models;

namespace MovieApp.IServices
{
    public interface ISeriesActorsService
    {
        public Task<List<Actor>> GetActorIn(int id);
        public Task<List<Actor>> GetActorOut(int id);
        public Task UpdateActor(int id, int[] AdctorIds);
        public Task RemoveActor(int id, int[] ActorIds);
    }
}
