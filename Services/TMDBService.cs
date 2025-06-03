using MovieTime.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MovieTime.Services
{
    public class TMDBService
    {
       
        private readonly HttpClient _http;
        
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    
        public TMDBService(HttpClient http, IConfiguration config)
        {
            _http = http; 
         
            string? tmdbKey = config["TMDBAccessKey"];
          
            if (!string.IsNullOrWhiteSpace(tmdbKey))
            {
                _http.BaseAddress = new Uri("https://api.themoviedb.org/3/");
                _http.DefaultRequestHeaders.Authorization = new("Bearer", tmdbKey);
            }
            else
            {
                _http.BaseAddress = new Uri(_http.BaseAddress + "tmdb/");
            }
        }

        public async Task<MovieListResponse> GetNowPlayingMovies()
        {
   
            string url = "movie/now_playing?region=US&language=en-US";
            
            MovieListResponse response = await _http.GetFromJsonAsync<MovieListResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Now Playing Movies could not be loaded");

           
            foreach (Movie movie in response.Results)
            {
                if (string.IsNullOrWhiteSpace(movie.PosterPath))
                {
                    movie.PosterPath = "/images/poster.png";
                }
                else
                {
                    movie.PosterPath = $"http://image.tmdb.org/t/p/w500{movie.PosterPath}";
                }
            }

            return response;
        }

        public async Task<MovieListResponse> GetPopularMovies()
        {
            string url = "https://api.themoviedb.org/3/movie/popular?region=US&language=en-US";

            MovieListResponse response = await _http.GetFromJsonAsync<MovieListResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Popular Movies could not be loaded");

            foreach (Movie movie in response.Results)
            {
                if (string.IsNullOrWhiteSpace(movie.PosterPath))
                {
                    movie.PosterPath = "/images/poster.png";
                }
                else
                {
                    movie.PosterPath = $"http://image.tmdb.org/t/p/w500{movie.PosterPath}";
                }
            }

            return response;
        }
    }
}
