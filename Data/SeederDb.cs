using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoundScape.Data;
using SoundScape.Models;
using System;
using System.Linq;
using System.Collections.Generic;

public static class Seeder
{
    public static void SeedTrack(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            var track = new Track
            {
                Title = "Lover",
                Artist = "Taylor Swift",
                Album = "Album Name",
                Genre = "Chill",
                Duration = "4:00",
                FilePath = "/tracks/testtrack2.mp3",
                UploadDate = DateTime.UtcNow,
                ImageUrl = "/images/TSF.png"
            };

            context.MusicTracks.Add(track);

            var tracks = new List<Track>
            {
                new Track { Title = "Espresso", Artist = "Sabrina Carpenter", FilePath = "/music/espresso.mp3", ImageUrl = "/images/songs/esc.png" },
                new Track { Title = "Beautiful Things", Artist = "Benson Boone", FilePath = "/music/beautiful-things.mp3", ImageUrl = "/images/songs/btbb.png" },
                new Track { Title = "Birds of a Feather", Artist = "Billie Eilish", FilePath = "/music/birds-of-a-feather.mp3", ImageUrl = "/images/songs/boafbe.png" },
                new Track { Title = "Gata Only", Artist = "FloyyMenor, Cris MJ", FilePath = "/music/gata-only.mp3", ImageUrl = "/images/songs/gofmcm.png" },
                new Track { Title = "Lose Control", Artist = "Teddy Swims", FilePath = "/music/lose-control.mp3", ImageUrl = "/images/songs/lcts.png" },
                new Track { Title = "End of Beginning", Artist = "Djo", FilePath = "/music/end-of-beginning.mp3", ImageUrl = "/images/songs/eobd.png" },
                new Track { Title = "Too Sweet", Artist = "Hozier", FilePath = "/music/too-sweet.mp3", ImageUrl = "/images/songs/tsh.png" },
                new Track { Title = "One of the Girls", Artist = "The Weeknd (feat. Jennie, Lily-Rose Depp)", FilePath = "/music/one-of-the-girls.mp3", ImageUrl = "/images/songs/ootgtw.png" },
                new Track { Title = "Cruel Summer", Artist = "Taylor Swift", FilePath = "/music/cruel-summer.mp3", ImageUrl = "/images/songs/csts.png" },
                new Track { Title = "Die with a Smile", Artist = "Lady Gaga, Bruno Mars", FilePath = "/music/die-with-a-smile.mp3", ImageUrl = "/images/songs/dwas.png" }
            };

            context.MusicTracks.AddRange(tracks);
            context.SaveChanges();
            Console.WriteLine("Tracks added successfully.");
        }
    }

    public static void SeedAlbum(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            var artist = context.Artists.FirstOrDefault(a => a.Id == 13);
            if (artist != null)
            {
                if (!context.Albums.Any(a => a.ArtistId == 13))
                {
                    var albums = new[]
                    {
                        new Album { Title = "The tortured poets department: the anthology", ReleaseDate = DateTime.UtcNow, ArtistId = 13, ImageUrl = "/images/ALTS1.png" },
                        new Album { Title = "Album 2", ReleaseDate = DateTime.UtcNow, ArtistId = 13, ImageUrl = "/images/ALTSOD.png" },
                        new Album { Title = "Album 3", ReleaseDate = DateTime.UtcNow, ArtistId = 13, ImageUrl = "/images/ALTSS.png" },
                        new Album { Title = "Album 4", ReleaseDate = DateTime.UtcNow, ArtistId = 13, ImageUrl = "/images/ALTS1.png" }
                    };

                    context.Albums.AddRange(albums);
                }

                var taylorSwift = context.Artists.FirstOrDefault(a => a.Name == "Taylor Swift");
                var billieEilish = context.Artists.FirstOrDefault(a => a.Name == "Billie Eilish");
                var sabrinaCarpenter = context.Artists.FirstOrDefault(a => a.Name == "Sabrina Carpenter");
                var karolG = context.Artists.FirstOrDefault(a => a.Name == "Karol G");

                var additionalAlbums = new List<Album>
                {
                    new Album { Title = "The Tortured Poets Department", ArtistId = taylorSwift?.Id ?? 0, ImageUrl = "/images/albums/alts1.png" },
                    new Album { Title = "Hit Me Hard and Soft", ArtistId = billieEilish?.Id ?? 0, ImageUrl = "/images/albums/boafbe.png" },
                    new Album { Title = "Short n' Sweet", ArtistId = sabrinaCarpenter?.Id ?? 0, ImageUrl = "/images/albums/alsc.png" },
                    new Album { Title = "Mañana Será Bonito", ArtistId = karolG?.Id ?? 0, ImageUrl = "/images/albums/alkg.png" }
                };

                context.Albums.AddRange(additionalAlbums);
                context.SaveChanges();
                Console.WriteLine("Albums added successfully.");
            }
        }
    }

    public static void SeedSingle(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            var artist = context.Artists.FirstOrDefault(a => a.Id == 13);
            if (artist != null)
            {
                if (!context.Singles.Any(s => s.ArtistId == 13))
                {
                    var singles = new[]
                    {
                        new Single { Title = "Single 1", ReleaseDate = DateTime.UtcNow.AddDays(-10), ArtistId = 13, Genre = "Pop", Duration = "3:30", FilePath = "/path/to/single1.mp3", ImageUrl = "/images/STSI.png" },
                        new Single { Title = "Single 2", ReleaseDate = DateTime.UtcNow.AddDays(-20), ArtistId = 13, Genre = "Rock", Duration = "4:00", FilePath = "/path/to/single2.mp3", ImageUrl = "/images/STSI2.png" },
                        new Single { Title = "Single 3", ReleaseDate = DateTime.UtcNow.AddDays(-30), ArtistId = 13, Genre = "Jazz", Duration = "5:00", FilePath = "/path/to/single3.mp3", ImageUrl = "/images/STSI.png" },
                        new Single { Title = "Single 4", ReleaseDate = DateTime.UtcNow.AddDays(-40), ArtistId = 13, Genre = "Hip-Hop", Duration = "3:45", FilePath = "/path/to/single4.mp3", ImageUrl = "/images/STSI2.png" },
                        new Single { Title = "Single 5", ReleaseDate = DateTime.UtcNow.AddDays(-50), ArtistId = 13, Genre = "Classical", Duration = "6:00", FilePath = "/path/to/single5.mp3", ImageUrl = "/images/STSI.png" }
                    };

                    context.Singles.AddRange(singles);
                    context.SaveChanges();
                    Console.WriteLine("Singles added successfully.");
                }
            }
        }
    }
}
