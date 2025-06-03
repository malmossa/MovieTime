using System.Text.Json;
using Microsoft.JSInterop;
using MovieTime.Models;

namespace MovieTime.Services
{
    public class FavoritesService(IJSRuntime jsRuntime)
    {
        private readonly string _localStorageKey = "favorites";

        public async Task<List<Movie>> GetFavorites()
        {
            List<Movie> movies = new();

            try
            {
                var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", _localStorageKey);
                movies = JsonSerializer.Deserialize<List<Movie>>(json) ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return movies;
        }

        public async Task SaveFavorites(List<Movie> movies)
        {
            try
            {
                var json = JsonSerializer.Serialize(movies);
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", _localStorageKey, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task AddFavorite(Movie movie)
        {
            var currentMovies = await GetFavorites();

            if (currentMovies.All(m => m.Id != movie.Id))
            {
                currentMovies.Add(movie);
                await SaveFavorites(currentMovies);
            }
        }

        public async Task RemoveFavorite(Movie movie)
        {
            var currentMovies = await GetFavorites();
            currentMovies = currentMovies.Where(f => f.Id != movie.Id).ToList();
            await SaveFavorites(currentMovies);
        }

        public async Task<bool> IsFavorite(int id)
        {
            var currentMovies = await GetFavorites();
            bool IsFavorite = currentMovies.Any(f => f.Id == id);

            return IsFavorite;
        }
    }
}
