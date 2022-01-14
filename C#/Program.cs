using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlbumList
{
    class Program
    {
        static async Task Main(string[] args)
        {
            List<CSVObject> csvObjects = await DataTableHelper.ParseCSVFilePath<CSVObject>(@"C:\Users\mccu4157\OneDrive Corp\OneDrive - Atkins Ltd\Desktop\album_list.csv");

            StringBuilder output = new StringBuilder();

            output.AppendLine("Number, Year, Album, Artist, Genre, SubGenre, Thumbnail, Cover");

            foreach(CSVObject csvObject in csvObjects)
            {
                output.AppendLine(csvObject.ToString());
            }

            await File.WriteAllTextAsync(@"C:\Users\mccu4157\OneDrive Corp\OneDrive - Atkins Ltd\Desktop\album_list_updated.csv", output.ToString());
        }
    }

    public interface IParsable
    {
        Task BuildAsync(string[] csvData);
    }

    public class CSVObject : IParsable
    {
        public int Number { get; set; }
        public int Year { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string SubGenre { get; set; }
        public string ThumbnailUrl { get; set; }
        public string CoverUrl { get; set; }

        public async Task BuildAsync(string[] csvData)
        {
            Number = int.Parse(csvData[0]);
            Year = int.Parse(csvData[1]);
            Album = csvData[2];
            Artist = csvData[3];
            Genre = csvData[4];
            SubGenre = csvData[5];

            string[] imageUrls;

            try
            {
                imageUrls = await APIHelper.DiscogsRequestAsync(Artist + " " + Album);
            }
            catch
            {
                imageUrls = new string[] { "Error", "Error" };
            }

            ThumbnailUrl = imageUrls[0];
            CoverUrl = imageUrls[1];
        }

        public override string ToString()
        {
            return $"{Number}, {Year}, {Album}, {Artist}, {Genre}, {SubGenre}, {ThumbnailUrl}, {CoverUrl}";
        }
    }

    public static class DataTableHelper
    {
        private static async Task<List<T>> ParseCSVAsync<T>(string file) where T : IParsable
        {
            List<T> entities = new List<T>();

            using (StringReader reader = new StringReader(file))
            {
                string line;

                reader.ReadLine(); //clearing the headers in CSV file:

                while((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(",");

                    T entity = (T)Activator.CreateInstance(typeof(T));

                    await entity.BuildAsync(values);

                    Thread.Sleep(1500);

                    entities.Add(entity);

                    Console.WriteLine($"Fetched { values[0] }");
                }
            }

            return entities;
        }

        public static async Task<List<T>> ParseCSVFilePath<T>(string filePath) where T: IParsable
        {
            string contents = File.ReadAllText(filePath);

            return await ParseCSVAsync<T>(contents);
        }
    }

    public static class APIHelper
    {
        private static readonly HttpClient client;

        private const string baseUrl = "https://api.discogs.com/database/search";

        private const string token = "rtkixvdeOIHpKwrdKBzQxQPECJzXpsmUMOAKMlig"; //from Discogs

        static APIHelper()
        {
            client = new HttpClient();

            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Pete's app");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Discogs", "token="+token);
        }

        public static async Task<string[]> DiscogsRequestAsync(string query)
        {
            string url = baseUrl + "?q=" + query;

            HttpResponseMessage response = await client.GetAsync(url);

            string content = await response.Content.ReadAsStringAsync();

            JToken results = JsonConvert.DeserializeObject<JToken>(content);

            JToken firstResult = results["results"].First;

            string thumbnail = firstResult["thumb"].ToString();

            string cover = firstResult["cover_image"].ToString();

            return new string[] { thumbnail, cover };
        }
    }
}

