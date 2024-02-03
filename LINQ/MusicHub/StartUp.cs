namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            int producerId = int.Parse(Console.ReadLine());

            Console.WriteLine(ExportAlbumsInfo(context, producerId));

            int songDuration = int.Parse(Console.ReadLine());
            Console.WriteLine(ExportSongsAboveDuration(context, songDuration));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var producer = context.Producers
                .Where(p => p.Id == producerId)
                .AsEnumerable()
                .Select(x => new
                {
                    Albums = x.Albums.OrderByDescending(a => a.Price).Select(x => new
                    {
                        AlbumName = x.Name,
                        ReleaseDate = x.ReleaseDate.ToString("MM/dd/yyyy"),
                        ProducerName = x.Producer.Name,
                        Songs = x.Songs.OrderByDescending(s => s.Name).ThenBy(w => w.Writer.Name).ToList(),
                        AlbumPrice = x.Price
                    })
                }).FirstOrDefault();

            var sb = new StringBuilder();

            foreach (var album in producer.Albums)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");
                int songCounter = 0;

                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{++songCounter}");
                    sb.AppendLine($"---SongName: {song.Name}");
                    sb.AppendLine($"---Price: {song.Price:F2}");
                    sb.AppendLine($"---Writer: {song.Writer.Name}");
                }

                sb.AppendLine($"-AlbumPrice {album.AlbumPrice:F2}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songs = context.Songs
                .Where(s => s.Duration > TimeSpan.FromSeconds(duration))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Writer)
                .ThenBy(x => x.SongPerformers.Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName).FirstOrDefault())
                .ToList()
                .Select(x => new
                {
                    x.Name,
                    WriterName = x.Writer.Name,
                    Performers = x.SongPerformers.Select(x => x.Performer.FirstName + " " + x.Performer.LastName),
                    AlbumProducer = x.Album.Producer.Name,
                    Duration = x.Duration.ToString("c")
                });

            var sb = new StringBuilder();
            int songCounter = 0;

            foreach (var song in songs)
            {
                sb.AppendLine($"Song #{++songCounter}");
                sb.AppendLine($"---SongName: {song.Name}");
                sb.AppendLine($"---Writer: {song.WriterName}");
                sb.AppendLine($"---Performer {string.Join(", ", song.Performers)}");
                sb.AppendLine($"---AlbumProducer: {song.AlbumProducer}");
                sb.AppendLine($"---Duration: {song.Duration}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
