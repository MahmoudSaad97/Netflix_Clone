using Microsoft.EntityFrameworkCore;
using MovieApp.IServices;
using MovieApp.Models;
using System.Collections.Generic;

namespace MovieApp.Services
{
    public class SeriesActorsService : ISeriesActorsService
    {
        private readonly MovieAppContext db;
        private readonly ISeriesService seriesService;

        public SeriesActorsService(MovieAppContext _db, ISeriesService _seriesService)
        {
            db = _db;
            seriesService = _seriesService;
        }
        public async Task<List<Actor>> GetActorIn(int id)
        {
            var serie = await seriesService.GetById(id);
            return serie.Actors.OrderBy(a => a.ActorName).ToList();
        }

        public async Task<List<Actor>> GetActorOut(int id)
        {
            var actors = await GetActorIn(id);
            var allActors = await db.Actors.ToListAsync();
            return allActors.Except(actors).OrderBy(a => a.ActorName).ToList();
        }

        public async Task RemoveActor(int id, int[] ActorIds)
        {
            var serie = await seriesService.GetById(id);
            foreach (var item in ActorIds)
            {
                var actor = await db.Actors.FirstOrDefaultAsync(g => g.ActorID == item);
                serie.Actors.Remove(actor);
            }
            await db.SaveChangesAsync();
        }

        public async Task UpdateActor(int id, int[] ActorIds)
        {
            var serie = await seriesService.GetById(id);
            foreach (var item in ActorIds)
            {
                var actor = await db.Actors.FirstOrDefaultAsync(g => g.ActorID == item);
                serie.Actors.Add(actor);
            }
            await db.SaveChangesAsync();
        }
    }
}
