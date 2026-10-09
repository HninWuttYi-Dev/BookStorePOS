using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStorePOS.Database.AppDbContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStorePOS.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SeedController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("RealBooks")]
        public async Task<IActionResult> SeedRealBooks()
        {
            try
            {
                var booksData = new List<(string Title, string Author, string Genre, string Description, string CoverUrl)>
                {
                    ("The Great Gatsby", "F. Scott Fitzgerald", "Classic", "A story of the wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan.", "https://images.unsplash.com/photo-1544947950-fa07a98d237f?q=80&w=150&auto=format&fit=crop"),
                    ("To Kill a Mockingbird", "Harper Lee", "Fiction", "Compassionate, dramatic, and deeply moving, it takes readers to the roots of human behavior.", "https://images.unsplash.com/photo-1512820790803-83ca734da794?q=80&w=150&auto=format&fit=crop"),
                    ("1984", "George Orwell", "Dystopian", "Among the seminal texts of the 20th century.", "https://images.unsplash.com/photo-1532012197267-da84d127e765?q=80&w=150&auto=format&fit=crop"),
                    ("Pride and Prejudice", "Jane Austen", "Romance", "A novel of manners that follows the character development of Elizabeth Bennet.", "https://images.unsplash.com/photo-1589998059171-988d887df646?q=80&w=150&auto=format&fit=crop"),
                    ("The Catcher in the Rye", "J.D. Salinger", "Fiction", "The story of a teenage boy's experiences in New York City.", "https://images.unsplash.com/photo-1526488344653-6114ebfa240a?q=80&w=150&auto=format&fit=crop"),
                    ("The Hobbit", "J.R.R. Tolkien", "Fantasy", "The unforgettable story of Bilbo, a peace-loving hobbit.", "https://images.unsplash.com/photo-1629196914282-e31589b3f305?q=80&w=150&auto=format&fit=crop"),
                    ("Fahrenheit 451", "Ray Bradbury", "Dystopian", "A dystopian novel about a future American society where books are outlawed.", "https://images.unsplash.com/photo-1495640388908-05fa85288e61?q=80&w=150&auto=format&fit=crop"),
                    ("Moby-Dick", "Herman Melville", "Adventure", "The narrative of Captain Ahab's obsessive quest.", "https://images.unsplash.com/photo-1519682337058-a94d519337bc?q=80&w=150&auto=format&fit=crop"),
                    ("War and Peace", "Leo Tolstoy", "Historical Fiction", "A broad panorama of Russian life.", "https://images.unsplash.com/photo-1535905557558-afc4877a26fc?q=80&w=150&auto=format&fit=crop"),
                    ("The Odyssey", "Homer", "Epic Poetry", "An epic poem that follows the Greek hero Odysseus.", "https://images.unsplash.com/photo-1506880018603-83d5b814b5a6?q=80&w=150&auto=format&fit=crop"),
                    ("Crime and Punishment", "Fyodor Dostoevsky", "Psychological Fiction", "The mental anguish and moral dilemmas of Rodion Raskolnikov.", "https://images.unsplash.com/photo-1456513080510-7bf3a84b82f8?q=80&w=150&auto=format&fit=crop"),
                    ("The Brothers Karamazov", "Fyodor Dostoevsky", "Philosophical Fiction", "A passionate philosophical novel.", "https://images.unsplash.com/photo-1550399105-c4eb5fedc3a3?q=80&w=150&auto=format&fit=crop"),
                    ("Brave New World", "Aldous Huxley", "Dystopian", "A dystopian novel exploring a technologically advanced future.", "https://images.unsplash.com/photo-1507721999472-8ed4421c4af2?q=80&w=150&auto=format&fit=crop"),
                    ("Jane Eyre", "Charlotte Brontë", "Romance", "Follows the experiences of its eponymous heroine.", "https://images.unsplash.com/photo-1605369572399-05d8d64a0f6e?q=80&w=150&auto=format&fit=crop"),
                    ("Wuthering Heights", "Emily Brontë", "Gothic Fiction", "The story of the intense and almost demonic love.", "https://images.unsplash.com/photo-1524578948792-506e1cc815d3?q=80&w=150&auto=format&fit=crop"),
                    ("The Lord of the Rings", "J.R.R. Tolkien", "Fantasy", "An epic high-fantasy novel.", "https://images.unsplash.com/photo-1618666012174-83b441c0bc76?q=80&w=150&auto=format&fit=crop"),
                    ("The Alchemist", "Paulo Coelho", "Adventure", "A novel about a young Andalusian shepherd in his journey to the pyramids of Egypt.", "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?q=80&w=150&auto=format&fit=crop"),
                    ("Harry Potter and the Sorcerer's Stone", "J.K. Rowling", "Fantasy", "The first novel in the Harry Potter series.", "https://images.unsplash.com/photo-1598153346810-860daa814c4b?q=80&w=150&auto=format&fit=crop"),
                    ("The Picture of Dorian Gray", "Oscar Wilde", "Philosophical Fiction", "A story about a man who sells his soul for eternal youth.", "https://images.unsplash.com/photo-1551029506-0807df4e2031?q=80&w=150&auto=format&fit=crop"),
                    ("Frankenstein", "Mary Shelley", "Science Fiction", "The story of Victor Frankenstein.", "https://images.unsplash.com/photo-1579370318443-8da816457e3d?q=80&w=150&auto=format&fit=crop"),
                    ("Dracula", "Bram Stoker", "Horror", "The novel tells the story of Dracula's attempt to move from Transylvania to England.", "https://images.unsplash.com/photo-1626279930707-1603598711bd?q=80&w=150&auto=format&fit=crop"),
                    ("The Handmaid's Tale", "Margaret Atwood", "Dystopian", "A dystopian novel set in a near-future New England.", "https://images.unsplash.com/photo-1513001900722-370f803f498d?q=80&w=150&auto=format&fit=crop"),
                    ("A Tale of Two Cities", "Charles Dickens", "Historical Fiction", "Set in London and Paris before and during the French Revolution.", "https://images.unsplash.com/photo-1491841550275-ad7854e35ca6?q=80&w=150&auto=format&fit=crop"),
                    ("The Little Prince", "Antoine de Saint-Exupéry", "Children's Literature", "A novella that follows a young prince who visits various planets in space.", "https://images.unsplash.com/photo-1601669466540-3a362bf784bc?q=80&w=150&auto=format&fit=crop"),
                    ("Les Misérables", "Victor Hugo", "Historical Fiction", "Follows the lives and interactions of several characters in 19th-century France.", "https://images.unsplash.com/photo-1554471900-58ab6286701b?q=80&w=150&auto=format&fit=crop"),
                    ("Anna Karenina", "Leo Tolstoy", "Realist Fiction", "A complex novel in eight parts.", "https://images.unsplash.com/photo-1533749047139-189de3cf06d3?q=80&w=150&auto=format&fit=crop"),
                    ("Don Quixote", "Miguel de Cervantes", "Satire", "Follows the adventures of a noble from La Mancha named Alonso Quixano.", "https://images.unsplash.com/photo-1555448248-2571daf6344b?q=80&w=150&auto=format&fit=crop"),
                    ("The Divine Comedy", "Dante Alighieri", "Epic Poetry", "An Italian narrative poem.", "https://images.unsplash.com/photo-1614728423169-3f65fd722b05?q=80&w=150&auto=format&fit=crop"),
                    ("One Hundred Years of Solitude", "Gabriel García Márquez", "Magic Realism", "Tells the multi-generational story of the Buendía family.", "https://images.unsplash.com/photo-1457369804613-52c61a468e7d?q=80&w=150&auto=format&fit=crop"),
                    ("The Catch-22", "Joseph Heller", "Satire", "Set during World War II, a satirical novel.", "https://images.unsplash.com/photo-1532012197267-da84d127e765?q=80&w=150&auto=format&fit=crop")
                };

                var authorsMap = new Dictionary<string, TblAuthor>();
                var genresMap = new Dictionary<string, TblGenre>();

                var paperback = await _db.TblEditions.FirstOrDefaultAsync(e => e.EditionName == "Paperback");
                if (paperback == null) { paperback = new TblEdition { EditionName = "Paperback", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }; _db.TblEditions.Add(paperback); }

                var hardcover = await _db.TblEditions.FirstOrDefaultAsync(e => e.EditionName == "Hardcover");
                if (hardcover == null) { hardcover = new TblEdition { EditionName = "Hardcover", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }; _db.TblEditions.Add(hardcover); }

                await _db.SaveChangesAsync();

                var r = new Random();
                int idx = 0;
                
                foreach (var b in booksData)
                {
                    idx++;
                    if (!authorsMap.ContainsKey(b.Author))
                    {
                        var a = await _db.TblAuthors.FirstOrDefaultAsync(x => x.AuthorName == b.Author);
                        if (a == null) { a = new TblAuthor { AuthorName = b.Author, CreateAt = DateTime.Now, UpdatedAt = DateTime.Now }; _db.TblAuthors.Add(a); await _db.SaveChangesAsync(); }
                        authorsMap[b.Author] = a;
                    }
                    if (!genresMap.ContainsKey(b.Genre))
                    {
                        var g = await _db.TblGenres.FirstOrDefaultAsync(x => x.GenreName == b.Genre);
                        if (g == null) { g = new TblGenre { GenreName = b.Genre, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }; _db.TblGenres.Add(g); await _db.SaveChangesAsync(); }
                        genresMap[b.Genre] = g;
                    }

                    var book = new TblBook
                    {
                        Title = b.Title,
                        Description = b.Description,
                        AuthorId = authorsMap[b.Author].AuthorId,
                        GenreId = genresMap[b.Genre].GenreId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _db.TblBooks.Add(book);
                    await _db.SaveChangesAsync();

                    // Paperback edition
                    _db.TblBookEditions.Add(new TblBookEdition
                    {
                        BookId = book.BookId,
                        EditionId = paperback.EditionId,
                        Isbn = $"978-0-000000-{idx.ToString("D2")}P",
                        Price = r.Next(10, 25) * 1000,
                        StockQuantity = r.Next(10, 100),
                        PublishDate = DateTime.Now.AddDays(-r.Next(100, 1000)),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CoverImageUrl = b.CoverUrl
                    });

                    // Randomly add hardcover edition
                    if (r.Next(0, 2) == 1)
                    {
                        _db.TblBookEditions.Add(new TblBookEdition
                        {
                            BookId = book.BookId,
                            EditionId = hardcover.EditionId,
                            Isbn = $"978-0-000000-{idx.ToString("D2")}H",
                            Price = r.Next(25, 50) * 1000,
                            StockQuantity = r.Next(5, 40),
                            PublishDate = DateTime.Now.AddDays(-r.Next(100, 1000)),
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            CoverImageUrl = b.CoverUrl
                        });
                    }
                }

                await _db.SaveChangesAsync();
                return Ok("30 real sample books created successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
