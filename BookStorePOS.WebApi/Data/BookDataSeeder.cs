using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BookStorePOS.Database.AppDbContextModels;

namespace BookStorePOS.WebApi.Data;

public class BookSeedItem
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public string EditionName { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public int PageCount { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string PlotSummary { get; set; } = string.Empty;
    public string EditionNotes { get; set; } = string.Empty;
}

public static class BookDataSeeder
{
    public static readonly List<BookSeedItem> TargetBooks = new()
    {
            new BookSeedItem
            {
                Title = "Harry Potter and the Philosopher's Stone",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Young Adult",
                Isbn = "9781408855652",
                Price = 18500m,
                StockQuantity = 22,
                ReorderLevel = 5,
                EditionName = "Bloomsbury 20th Anniversary Edition (Paperback)",
                PublishDate = DateTime.Parse("2014-09-01"),
                PageCount = 352,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408855652-L.jpg",
                PlotSummary = "Harry Potter is an orphaned boy living under the stairs at the home of his cruel aunt and uncle. On his eleventh birthday, he discovers his true magical heritage when the half-giant Hagrid delivers an acceptance letter to Hogwarts School of Witchcraft and Wizardry. Accompanied by new friends Ron Weasley and Hermione Granger, Harry navigates magical classes, enchanted staircases, and Quidditch matches while learning of the dark wizard Voldemort who murdered his parents. When mysterious happenings point toward the legendary Philosopher's Stone hidden deep beneath the castle, Harry must confront ultimate evil to protect the wizarding realm.",
                EditionNotes = "The Bloomsbury 20th Anniversary paperback edition features Jonny Duddle's vivid reimagined cover art, designed to captivate a modern generation of young readers. It features updated typography set on sustainably sourced cream paper for an easy, comfortable reading experience. Inside, readers will find renewed character profiles, an updated map of Hogwarts grounds, and fascinating trivia notes detailing the initial publication history of J. K. Rowling's legendary debut. This edition offers the complete British text with authoritative grammatical corrections approved by the author, making it a reliable staple for collector shelves and everyday reading alike."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Philosopher's Stone",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Illustrated Classics",
                Isbn = "9781408845646",
                Price = 38000m,
                StockQuantity = 12,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Deluxe Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2015-10-06"),
                PageCount = 256,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408845646-L.jpg",
                PlotSummary = "On his eleventh birthday, young Harry Potter learns that he is no ordinary boy but a wizard with an extraordinary past. Leaving behind the abusive Dursley household, he travels to Hogwarts School of Witchcraft and Wizardry aboard the Hogwarts Express. There, he finds lifelong friends in Ron Weasley and Hermione Granger, learns the art of spellcasting, and unravels the secret behind a magical artifact hidden within the third-floor corridor. When dark forces threaten to seize the stone to restore Lord Voldemort to power, Harry steps up to protect his newfound sanctuary with unyielding courage.",
                EditionNotes = "The Bloomsbury Deluxe Illustrated Edition is an opulent full-color hardcover masterpiece brilliantly brought to life by award-winning artist Jim Kay. Spanning 256 oversized pages on heavy glossy art stock, it features over one hundred breathtaking painted plates, pencil sketches, and architectural cross-sections of Diagon Alley and Hogwarts. The book is bound in durable royal red cloth with foil-stamped gilded lettering, featuring head and tail bands, patterned endpapers, and a satin ribbon placeholder. A magnificent showcase volume intended for library centerpieces, discerning collectors, and avid fantasy enthusiasts."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Chamber of Secrets",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Young Adult",
                Isbn = "9781408855669",
                Price = 18500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Children's Paperback Edition",
                PublishDate = DateTime.Parse("2014-09-01"),
                PageCount = 384,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408855669-L.jpg",
                PlotSummary = "Returning to Hogwarts for his second year, Harry Potter ignores warnings from Dobby the house-elf about grave dangers ahead. Soon, ominous whispers echo through stone corridors and students are found petrified as ancient writing in blood proclaims that the Chamber of Secrets has been opened. Suspicions fall upon Harry when he reveals his rare ability to speak Parseltongue. Accompanied by Ron and Hermione, Harry investigates a fifty-year-old mystery through a magical diary left by former student Tom Riddle, culminating in a perilous descent into the underground labyrinth beneath the school.",
                EditionNotes = "Published by Bloomsbury with Jonny Duddle's striking cover illustration depicting the enchanted flying Ford Anglia swooping over Hogwarts, this standard paperback remains the quintessential edition for school reading. Printed on high-opacity light-cream paper to ensure maximum visual contrast and reduce eye strain, it includes author biographical notes and updated Hogwarts crest insignia. Its lightweight binding and flexible spine make it resilient against repeated reading sessions, offering young readers and adults a smooth portal back into the wizarding world."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Chamber of Secrets",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Illustrated Classics",
                Isbn = "9781408845653",
                Price = 38000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Deluxe Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2016-10-04"),
                PageCount = 272,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408845653-L.jpg",
                PlotSummary = "Whispers of venomous serpents and ancient malice haunt Hogwarts as students, ghosts, and the caretaker's cat are turned to stone. Harry, Ron, and Hermione brew forbidden Polyjuice Potion to uncover the heir of Slytherin, but their investigations take an alarming turn when Ginny Weasley is kidnapped into the depths of the Chamber. Armed with the sword of Gryffindor and the aid of Fawkes the phoenix, Harry must defeat the lethal basilisk and destroy a cursed diary before the dark lord's past self achieves living form.",
                EditionNotes = "This luxury illustrated hardcover showcases Jim Kay's astounding visual artistry across 272 lavish full-color pages. Kay's detailed depictions of the Burrow, the sinister Mandrake root gardens, Aragog's lair in the Forbidden Forest, and the terrifying visage of the Basilisk breathe fresh depth into the narrative. Bound in emerald cloth with iridescent metallic foil blocking on the cover and spine, this edition features premium archival paper, custom illustrated endpapers, and a silk page marker, crafted specifically as a timeless heirloom gift."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Prisoner of Azkaban",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Young Adult",
                Isbn = "9781408855676",
                Price = 19500m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Children's Paperback Edition",
                PublishDate = DateTime.Parse("2014-09-01"),
                PageCount = 464,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408855676-L.jpg",
                PlotSummary = "In his third year at Hogwarts, Harry faces grave peril as notorious murderer Sirius Black escapes from the wizarding fortress Azkaban, reportedly seeking to kill Harry. Soul-draining Dementors patrol the school perimeters, inflicting dread upon Harry until Defense Against the Dark Arts teacher Remus Lupin teaches him the Patronus charm. Using the mystical Marauder's Map, Harry discovers painful truths about his parents' final hours and learns that the true traitor was never who everyone believed, embarking on a daring race against time to rescue an innocent soul.",
                EditionNotes = "This widely appreciated Bloomsbury paperback edition features Jonny Duddle's dynamic cover art portraying Harry and Hermione soaring aboard Buckbeak the Hippogriff. Set in readable Caslon font on acid-free paper, it incorporates an introductory preface on the Marauder's Map and details about magical creatures. The cover boasts soft-touch matte lamination with spot-varnish accents on the title text, providing enhanced durability for daily reading while honoring J. K. Rowling's acclaimed third installment."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Prisoner of Azkaban",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Illustrated Classics",
                Isbn = "9781408845660",
                Price = 39000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Deluxe Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2017-10-03"),
                PageCount = 336,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408845660-L.jpg",
                PlotSummary = "The shadow of Sirius Black hangs over Hogwarts, bringing terrifying Dementor sentinels that threaten Harry's fragile peace of mind. As dark secrets of the past are unraveled in the Shrieking Shack, Harry, Ron, and Hermione confront werewolf transformations, time manipulation with Hermione's Time-Turner, and the devastating revelations surrounding the betrayal of James and Lily Potter.",
                EditionNotes = "Jim Kay delivers an awe-inspiring pictorial rendering of Rowling's most critically acclaimed volume in this monumental 336-page hardcover edition. His hauntingly beautiful depictions of the Knight Bus, the Grim, the Shrieking Shack, and the silver stag Patronus illuminating the dark lake represent a crowning achievement in contemporary fantasy illustration. Enclosed in royal purple cloth binding with embossed gold foil details and packaged with ribbon markers, this edition is an exquisite addition to any premier library."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Goblet of Fire",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Young Adult",
                Isbn = "9781408855683",
                Price = 22000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Children's Paperback Edition",
                PublishDate = DateTime.Parse("2014-09-01"),
                PageCount = 640,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408855683-L.jpg",
                PlotSummary = "Hogwarts hosts the legendary Triwizard Tournament, a grueling magical competition between European wizarding academies. Although underage, Harry's name mysteriously emerges from the enchanted Goblet of Fire, binding him to compete against older champions in deadly trials involving fierce dragons, lake merpeople, and an enchanted hedge maze. The tournament ends in horrific tragedy when the Goblet proves to be a portkey transporting Harry to a dark graveyard, where Lord Voldemort rises again in physical flesh.",
                EditionNotes = "This hefty Bloomsbury paperback features an exhilarating Jonny Duddle illustration of Harry dodging the fire-breathing Hungarian Horntail dragon. Engineered with specialized flexible glue binding to withstand the book's substantial 640-page spine, the volume is typeset in legible Plantin typography on eye-comfort paper. A cornerstone volume marking the crucial shift toward darker, mature themes in the saga."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Goblet of Fire",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Illustrated Classics",
                Isbn = "9781408845677",
                Price = 42000m,
                StockQuantity = 8,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Deluxe Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2019-10-08"),
                PageCount = 464,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408845677-L.jpg",
                PlotSummary = "The Triwizard Tournament arrives at Hogwarts, drawing rival schools Beauxbatons and Durmstrang into high-stakes competition. Harry finds himself thrust into perilous tasks against his will, culminating in a terrifying resurrection in Little Hangleton graveyard. Escaping by the slimmest margin with Cedric Diggory's body, Harry brings grave tidings of the Dark Lord's return to an unbelieving Ministry of Magic.",
                EditionNotes = "Jim Kay spent over two years creating the extraordinary full-color artwork for this grand 464-page hardcover edition. Rich double-page panoramic paintings bring the Quidditch World Cup, the Yule Ball, the Hungarian Horntail, and the sinister graveyard resurrection into breathtaking clarity. Featuring ribbon bookmarks, head and tail bands, heavy archival coated paper, and a royal blue cloth slipcase with gold embellishments, it stands as a collector's tour de force."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Order of the Phoenix",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Young Adult",
                Isbn = "9781408855690",
                Price = 24000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Children's Paperback Edition",
                PublishDate = DateTime.Parse("2014-09-01"),
                PageCount = 800,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408855690-L.jpg",
                PlotSummary = "The Ministry of Magic fiercely denies Voldemort's return, ostracizing Harry and Dumbledore as lunatics. To enforce government propaganda, malicious bureaucrat Dolores Umbridge is appointed High Inquisitor at Hogwarts, outlawing practical defensive magic. In response, Harry forms a secret underground defense club known as Dumbledore's Army. The escalating war reaches a devastating peak inside the Ministry's Department of Mysteries, where a prophecy is sought and deep family tragedy strikes.",
                EditionNotes = "Spanning 800 dense pages, this Bloomsbury paperback edition features cover art by Jonny Duddle showcasing Harry and Dumbledore's Army confronting Thestrals. Constructed with reinforced spine binding to maintain structural integrity across years of intensive reading, it features uniform series typography, a detailed floor map of the Ministry of Magic, and pristine character summaries."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Order of the Phoenix",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Illustrated Classics",
                Isbn = "9781408845684",
                Price = 45000m,
                StockQuantity = 8,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Deluxe Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2022-10-11"),
                PageCount = 576,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408845684-L.jpg",
                PlotSummary = "Harry Potter faces his most tumultuous year as smear campaigns paint him as a liar, while the grim Order of the Phoenix rallies in secret. As Dolores Umbridge dismantles freedom at Hogwarts, Harry trains classmates in practical combat spells. The year concludes with a catastrophic showdown between Death Eaters and the Order in London's subterranean halls, claiming the life of Harry's beloved godfather.",
                EditionNotes = "An extraordinary creative collaboration between Kate Greenaway Medal winner Jim Kay and guest artist Neil Packer, this lavish 576-page hardcover brings Rowling's longest novel to vivid visual life. Packer's distinct graphic style perfectly complements Kay's expressive oil paintings, illustrating Grimmauld Place, the Hall of Prophecy, and Grawp the giant. Complete with textured cloth covers, gilt-edged spine lettering, and fine satin ribbon, it is an essential prestige volume."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Half-Blood Prince",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Young Adult",
                Isbn = "9781408855706",
                Price = 22000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Children's Paperback Edition",
                PublishDate = DateTime.Parse("2014-09-01"),
                PageCount = 560,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408855706-L.jpg",
                PlotSummary = "As war spills into the Muggle world, Dumbledore prepares Harry for the ultimate conflict by exploring the memories of Voldemort's origins through the Pensieve. In Potions class, Harry excels using an annotated second-hand textbook once belonging to the mysterious Half-Blood Prince. Together, Harry and Dumbledore search for Horcruxes—vessels holding fragments of the dark wizard's splintered soul—leading to a fateful betrayal atop the Astronomy Tower that changes Hogwarts forever.",
                EditionNotes = "Featuring Jonny Duddle's evocative cover illustration depicting Harry and Dumbledore engulfed by the firestorm inside the seaside inferi cavern, this Bloomsbury paperback is engineered for lasting appeal. Typeset in clear Baskerville-styled font on lightweight wood-free paper, it incorporates an appendix detailing wizarding genealogies and revised timeline annotations for serious fans of the lore."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Half-Blood Prince",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Adult Fiction",
                Isbn = "9781408835005",
                Price = 25000m,
                StockQuantity = 12,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Adult Cover Edition (Paperback)",
                PublishDate = DateTime.Parse("2013-07-18"),
                PageCount = 560,
                Publisher = "Bloomsbury Publishing",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408835005-L.jpg",
                PlotSummary = "Dark omens cloud Harry's sixth year as he uncovers the sinister truth behind Lord Voldemort's immortality: Horcruxes. Aided by notes penned in an enigmatic potion manual, Harry unlocks memories from Professor Slughorn while watching school rival Draco Malfoy succumb to Death Eater commands. A perilous expedition to retrieve a locket Horcrux culminates in an unimaginable sacrifice atop the highest tower of Hogwarts.",
                EditionNotes = "Designed specifically for mature readers seeking a sophisticated aesthetic on commuter trains and home bookshelves, the Bloomsbury Adult Edition features brooding woodcut-inspired graphic cover art by Andrew Davidson. The cover is printed on textured uncoated cardstock with debossed silver titling. Interior formatting boasts elegant chapter headings, wider margins, and premium paper for a refined reading ambiance."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Deathly Hallows",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Young Adult",
                Isbn = "9781408855713",
                Price = 24000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Children's Paperback Edition",
                PublishDate = DateTime.Parse("2014-09-01"),
                PageCount = 640,
                Publisher = "Bloomsbury Children's Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408855713-L.jpg",
                PlotSummary = "Without the protective guidance of Dumbledore, Harry, Ron, and Hermione leave Hogwarts behind to hunt and destroy Voldemort's remaining Horcruxes across Britain. As the country falls under totalitarian Death Eater rule, the trio uncovers the legendary Deathly Hallows—three ancient objects capable of making their possessor the Master of Death. The epic journey culminates in the monumental Battle of Hogwarts, where secrets are unmasked, sacrifices made, and the Boy Who Lived faces his destiny.",
                EditionNotes = "The concluding novel of the core seven-part saga in Bloomsbury's official children's paperback series. Jonny Duddle's cover captures the climactic duel in the Great Hall between Harry and Voldemort as dawn breaks. Reinforced with heavy-duty PUR binding, it contains complete original British spellings, comprehensive character indices, and the complete 19-years-later epilogue intact."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Deathly Hallows",
                Author = "J. K. Rowling",
                Genre = "Fantasy / Adult Fiction",
                Isbn = "9781408835012",
                Price = 26000m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "Bloomsbury Adult Cover Edition (Paperback)",
                PublishDate = DateTime.Parse("2013-07-18"),
                PageCount = 640,
                Publisher = "Bloomsbury Publishing",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781408835012-L.jpg",
                PlotSummary = "On the run as hunted fugitives, Harry and his companions travel across desolate landscapes searching for dark soul fragments. Facing internal strife, near-fatal ambushes, and the loss of treasured friends, Harry must choose between seeking the legendary Hallows or hunting the Horcruxes. The grand clash at Hogwarts tests every bond of love and loyalty before peace can be reclaimed.",
                EditionNotes = "The Bloomsbury Adult Edition presents the harrowing finale with Andrew Davidson's stark, minimalist woodcut cover art representing the Deathly Hallows insignia set against a grim landscape. Bound in heavy matte card with embossed lettering, this edition is tailored for adult collectors and literary purists who value a mature presentation of Rowling's defining triumph."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Cursed Child",
                Author = "J. K. Rowling, Jack Thorne, John Tiffany",
                Genre = "Fantasy / Stage Play Script",
                Isbn = "9780751565362",
                Price = 18000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "Little, Brown Playscript Paperback Edition",
                PublishDate = DateTime.Parse("2017-07-25"),
                PageCount = 352,
                Publisher = "Little, Brown Book Group",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780751565362-L.jpg",
                PlotSummary = "Nineteen years after the Battle of Hogwarts, Harry Potter is an overworked Ministry of Magic employee, husband, and father of three school-age children. While Harry grapples with a past that refuses to stay buried, his youngest son Albus Severus struggles with the crushing weight of a family legacy he never desired. As past and present fuse ominously, father and son learn uncomfortable truths: sometimes, darkness comes from unexpected places when forbidden Time-Turners alter established history.",
                EditionNotes = "This Little, Brown paperback represents the official Special Rehearsal Playscript edition of the West End theatrical production. Formatted clearly in standard theatrical script layout with stage directions and cast lists, it enables readers to experience Jack Thorne and John Tiffany's acclaimed West End narrative as staged at London's Palace Theatre. Printed on crisp paper with the iconic child-in-a-nest cover graphic."
            },
            new BookSeedItem
            {
                Title = "Harry Potter and the Cursed Child",
                Author = "J. K. Rowling, Jack Thorne, John Tiffany",
                Genre = "Fantasy / Stage Play Script",
                Isbn = "9780751565355",
                Price = 32000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Little, Brown Definitive Collector's Hardcover",
                PublishDate = DateTime.Parse("2017-10-31"),
                PageCount = 352,
                Publisher = "Little, Brown Book Group",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780751565355-L.jpg",
                PlotSummary = "Set nineteen years following Lord Voldemort's fall, Albus Potter befriends Scorpius Malfoy at Hogwarts, both feeling alienated under their parents' immense reputations. Desperate to correct historical wrongs, the boys steal a prototype Time-Turner to save Cedric Diggory, inadvertently plunging the magical timeline into nightmarish alternate realities where dark wizards rule once more.",
                EditionNotes = "The Definitive Edition hardcover features the finalized dialogue and stage directions after the play's official London West End premiere. Bound in rich black boards with shimmering gold-foil heraldic typography and dust jacket, this luxury edition includes an extensive family tree of the Potter and Weasley lineages along with a timeline of wizarding history, designed for lifelong drama and fantasy aficionados."
            },
            new BookSeedItem
            {
                Title = "A Game of Thrones",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Grimdark",
                Isbn = "9780553593716",
                Price = 22000m,
                StockQuantity = 24,
                ReorderLevel = 5,
                EditionName = "Bantam Mass Market Paperback",
                PublishDate = DateTime.Parse("2011-03-22"),
                PageCount = 864,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553593716-L.jpg",
                PlotSummary = "In the mythical continent of Westeros, summers span decades and winter can last a lifetime. Lord Eddard Stark of Winterfell is summoned south by King Robert Baratheon to serve as the Hand of the King following the suspicious demise of his predecessor. As Eddard navigates lethal conspiracies woven by Queen Cersei and House Lannister, exiled princess Daenerys Targaryen weds a barbarian warlord across the Narrow Sea, while mysterious, icy entities known as the Others stir beyond the colossal ancient Wall in the frigid North.",
                EditionNotes = "The bestselling Bantam mass-market paperback edition features the iconic iron throne imagery on a durable foil-embossed cover. Sized conveniently for pocket travel and daily commuting, it features crisp digital typesetting on lightweight pulp paper. This edition includes a complete set of introductory maps delineating the North and South of Westeros, accompanied by an exhaustive appendix cataloging the lineages, sigils, mottos, and retainers of the Great Houses of the Seven Kingdoms."
            },
            new BookSeedItem
            {
                Title = "A Game of Thrones",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Illustrated Classics",
                Isbn = "9780553808049",
                Price = 46000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Bantam 20th Anniversary Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2016-10-18"),
                PageCount = 896,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553808049-L.jpg",
                PlotSummary = "Winter is coming to the Seven Kingdoms. When King Robert Baratheon requests Eddard Stark's assistance in governing an unstable realm, Stark reluctantly travels to King's Landing, unraveling an incestuous treason that fractures the empire. Meanwhile, across the eastern continent of Essos, Daenerys Targaryen survives court intrigues to hatch three petrified dragon eggs in funeral pyres. In the far frozen north, Jon Snow joins the Night's Watch to defend the realm against ancient supernatural horrors awakenings.",
                EditionNotes = "Celebrating two decades of Martin's monumental saga, this deluxe 20th Anniversary hardcover features a specially commissioned introduction by fantasy author John Hodgman. The volume includes seventy-three magnificent full-page illustrations—eight in glorious full color—crafted by celebrated artists including Marc Simonetti, Ted Nasmith, and Gary Gianni. Bound in deep navy simulated leather with metallic copper stamping, gilded page edges, and a ribbon marker, this is an unrivaled collector's centerpiece."
            },
            new BookSeedItem
            {
                Title = "A Clash of Kings",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Grimdark",
                Isbn = "9780553579901",
                Price = 22000m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Bantam Mass Market Paperback",
                PublishDate = DateTime.Parse("2002-05-28"),
                PageCount = 1040,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553579901-L.jpg",
                PlotSummary = "With King Robert dead, five separate factions lay claim to the Iron Throne, thrusting Westeros into the devastating War of the Five Kings. In the capital, Tyrion Lannister acts as Hand to moderate the sadistic whims of boy-king Joffrey. In the North, Robb Stark declares independence as King in the North, while Stannis Baratheon aligns with a fiery sorceress named Melisandre to claim his lawful birthright through dark shadow arts. The war culminates in the cataclysmic Battle of the Blackwater, fought with green wildfire upon raging river currents.",
                EditionNotes = "The standard Bantam mass-market edition packs Martin's 1040-page epic into an accessible, tightly bound format. Equipped with a distinctive gold crown graphic on its foil-stamped jacket, this volume is printed on acid-neutral cream book paper. It contains fully revised maps of the Valyrian Freehold and Westeros, an expanded appendix detailing religious sects including the Lord of Light, and genealogical breakdowns of House Greyjoy of the Iron Islands and House Martell of Dorne."
            },
            new BookSeedItem
            {
                Title = "A Clash of Kings",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Illustrated Classics",
                Isbn = "9780553808056",
                Price = 48000m,
                StockQuantity = 8,
                ReorderLevel = 5,
                EditionName = "Bantam Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2019-11-05"),
                PageCount = 1008,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553808056-L.jpg",
                PlotSummary = "A red comet blazes through the sky as civil war tears the Seven Kingdoms apart. In King's Landing, Tyrion Lannister prepares defensive chain traps for Stannis Baratheon's oncoming naval armada. Across the Red Waste, Daenerys Targaryen seeks shelter in Qarth, surviving the hallucinations of the House of the Undying. Meanwhile, beyond the Wall, the Night's Watch rides north in force to search for missing rangers and confront the King-Beyond-the-Wall.",
                EditionNotes = "This luxury illustrated edition contains over sixty full-page drawings and paintings curated by French fantasy artist Laurent Durieux. Featuring striking depictions of wildfire engulfing the Blackwater estuary, the warlock towers of Qarth, and the rugged shores of Pyke, this volume is bound in heavyweight cloth-textured boards with blind debossing and gold foil ornamentation. It comes complete with two-color printing, custom endpapers, and a burgundy bookmark ribbon."
            },
            new BookSeedItem
            {
                Title = "A Storm of Swords",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Grimdark",
                Isbn = "9780553573428",
                Price = 24000m,
                StockQuantity = 22,
                ReorderLevel = 5,
                EditionName = "Bantam Mass Market Paperback",
                PublishDate = DateTime.Parse("2003-03-04"),
                PageCount = 1216,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553573428-L.jpg",
                PlotSummary = "The War of the Five Kings reaches its most harrowing climax as treacherous alliances upend the balance of power. Robb Stark attempts to reconcile with House Frey, resulting in the infamous, bloody betrayal known as the Red Wedding. In King's Landing, Tyrion Lannister is framed for royal assassination and sentenced to trial by combat, defended by Prince Oberyn Martell against the monstrous Mountain. In Essos, Daenerys conquers slave cities with her growing dragons and Unsullied army.",
                EditionNotes = "Regarded by critics and fans alike as the pinnacle of the series, this single-volume Bantam paperback manages the massive 1216-page story with specialized high-flex binding. Featuring the iconic bronze gauntlet motif on the cover, it is printed in clear micro-type with narrow margins to optimize bulk. The volume includes updated regional charts of the Riverlands and Slaver's Bay alongside an exhaustive character index detailing fallen lords."
            },
            new BookSeedItem
            {
                Title = "A Storm of Swords",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Illustrated Classics",
                Isbn = "9780553808063",
                Price = 52000m,
                StockQuantity = 8,
                ReorderLevel = 5,
                EditionName = "Bantam Illustrated Edition (Hardcover)",
                PublishDate = DateTime.Parse("2020-11-03"),
                PageCount = 1152,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553808063-L.jpg",
                PlotSummary = "Treachery and vengeance consume Westeros. The young northern rebellion collapses under Frey crossbows at the Twins, while Jon Snow defends the Wall from tens of thousands of wildlings and mammoths. Tyrion Lannister escapes execution through dark castle tunnels, taking deadly revenge upon his father Tywin before fleeing into the night. Daenerys cements her role as Breaker of Chains while learning difficult realities of imperial rulership.",
                EditionNotes = "This monumental illustrated hardcover features over twenty-five original black-and-white drawings and sixteen full-color plates by acclaimed fantasy illustrator Gary Gianni. Bound in crimson buckram cloth with intricate gold-stamped dragon and direwolf crests, this collector's item features thick acid-free archival stock, dyed page edges, decorative headbands, and silk ribbon bookmarks, making it the definitive showcase edition of Martin's masterpiece."
            },
            new BookSeedItem
            {
                Title = "A Feast for Crows",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Grimdark",
                Isbn = "9780553582024",
                Price = 22000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Bantam Mass Market Paperback",
                PublishDate = DateTime.Parse("2006-09-26"),
                PageCount = 1060,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553582024-L.jpg",
                PlotSummary = "In the ashes of the civil war, crows feast upon the dead across ruined fields. Queen Regent Cersei Lannister assumes supreme rule in King's Landing, succumbing to paranoia and empowering a militant religious order known as the Sparrows to crush rival House Tyrell. In the Iron Islands, the kingsmoot crowns ruthless pirate Euron Greyjoy, who sets his sights on dragons. Meanwhile, Brienne of Tarth wanders the war-torn Riverlands searching for Lady Catelyn's daughters, confronting broken men and outlaw bands.",
                EditionNotes = "The Bantam mass-market edition presents the aftermath of the five kings' war in a unified series format. Adorned with a metallic goblets and raven illustration, this edition includes Martin's original author's note explaining the geographical split of characters between volumes four and five. Complete with detailed maps of the Iron Islands, Dorne, and Oldtown, as well as an appendix exploring the history of the Kingsguard."
            },
            new BookSeedItem
            {
                Title = "A Feast for Crows",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Hardcover Classics",
                Isbn = "9780002247436",
                Price = 44000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Harper Voyager Hardcover First Edition",
                PublishDate = DateTime.Parse("2005-10-17"),
                PageCount = 784,
                Publisher = "Voyager / HarperCollins",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780002247436-L.jpg",
                PlotSummary = "The realm bleeds as Queen Cersei constructs plots against her son's young queen, Margaery Tyrell, inadvertently trapping herself in the High Septon's inquisitorial clutches. In Braavos, Arya Stark trains in the shadowy temple of the Many-Faced God, casting off her noble identity to master assassination. In Dorne, Princess Arianne Martell attempts a daring political coup to crown Princess Myrcella under Dornish law.",
                EditionNotes = "The British first-edition hardcover published by Voyager features iconic dust jacket cover artwork by Larry Rostant, illustrating a golden goblet entwined by black raven wings. Printed in the UK on thick opaque wove paper, it features durable black cloth binding with metallic silver spine lettering, custom cartographic endpapers drawn by Richard Geist, and crisp typography that provides a superior physical reading feel."
            },
            new BookSeedItem
            {
                Title = "A Dance with Dragons",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Grimdark",
                Isbn = "9780553582017",
                Price = 24000m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Bantam Mass Market Paperback",
                PublishDate = DateTime.Parse("2013-10-29"),
                PageCount = 1152,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553582017-L.jpg",
                PlotSummary = "In the frozen North, Jon Snow serves as Lord Commander of the Night's Watch, attempting a controversial alliance with wildlings as Stannis Baratheon marches upon Winterfell in deep blizzards. In Meereen, Daenerys Targaryen marries a noble ghiscari to secure peace, but the Sons of the Harpy terrorize the city until Drogon descends upon Daznak's Pit. Tyrion Lannister journeys down the river Rhoyne toward Meereen, discovering an unexpected claimant to the Iron Throne claiming to be Aegon Targaryen.",
                EditionNotes = "The mass-market paperback edition of the fifth volume brings together the Northern and Essos storylines. Featuring a gleaming silver shield emblem on the cover, this edition includes two detailed maps of Beyond the Wall and the Free Cities, an exhaustive appendix detailing the sellsword companies of Essos, and a timeline reconciliation note from Martin explaining how the events run concurrently with A Feast for Crows."
            },
            new BookSeedItem
            {
                Title = "A Dance with Dragons",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Hardcover Classics",
                Isbn = "9780553801477",
                Price = 48000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Bantam Trade Hardcover First Edition",
                PublishDate = DateTime.Parse("2011-07-12"),
                PageCount = 1040,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553801477-L.jpg",
                PlotSummary = "As ice and fire collide, rulers discover that governing is far more perilous than conquering. Daenerys is carried away by dragon wings into the Dothraki sea, Jon Snow faces mutiny at Castle Black as old brothers turn daggers upon him, and Cersei Lannister undergoes a humiliating walk of atonement through King's Landing.",
                EditionNotes = "The original North American trade hardcover edition published simultaneously with the novel's worldwide launch. Featuring striking dust jacket art of a dragon-etched curved shield against a white snowy backdrop, this hefty 1040-page hardcover is bound in dark slate boards with metallic silver foil spine titling, emerald-tinted endpapers, and an extensive historical appendix."
            },
            new BookSeedItem
            {
                Title = "Fire & Blood",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Historical Fiction",
                Isbn = "9781524796303",
                Price = 24000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Bantam Trade Paperback Edition",
                PublishDate = DateTime.Parse("2020-05-05"),
                PageCount = 736,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781524796303-L.jpg",
                PlotSummary = "Set three centuries before the events of A Game of Thrones, Fire & Blood chronicles the rise and reign of House Targaryen, the only family of dragonlords to survive the Doom of Valyria. Written from the perspective of Archmaester Gyldayn of the Citadel, the narrative begins with Aegon the Conqueror forging the Iron Throne with dragonfire and continues through the brutal civil war known as the Dance of the Dragons. Filled with epic battles, political betrayal, and tragic dragon duels, the history illuminates the golden and bloody ages of the dragon kings.",
                EditionNotes = "The Bantam trade paperback edition features more than eighty original black-and-white pencil illustrations and character portraits by artist Doug Wheatley. Printed on high-opacity cream paper in a comfortable, readable font, it includes an extensive genealogical chart of the Targaryen dynasty from Aegon I through Aegon III, accompanied by regional maps of Westeros during the Conquest. An essential historical companion for enthusiasts of Martin's fantasy universe."
            },
            new BookSeedItem
            {
                Title = "Fire & Blood",
                Author = "George R. R. Martin",
                Genre = "Epic Fantasy / Illustrated Classics",
                Isbn = "9781524796280",
                Price = 48000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Bantam Illustrated Hardcover First Edition",
                PublishDate = DateTime.Parse("2018-11-20"),
                PageCount = 736,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781524796280-L.jpg",
                PlotSummary = "Three hundred years before A Game of Thrones, dragons ruled Westeros. This definitive chronicle details the legendary Conquest of the Seven Kingdoms by Aegon Targaryen and his sister-wives Visenya and Rhaenys. As subsequent generations struggle for dominance, sibling rivalries erupt into the devastating Dance of the Dragons, pitting dragonrider against dragonrider in an apocalyptic contest that nearly exterminated the magical beasts.",
                EditionNotes = "The original Bantam hardcover first edition is bound in black textured boards with red foil stamping of the three-headed Targaryen dragon sigil on the front and spine. Featuring Doug Wheatley's exquisite interior artwork across 736 dense pages, this volume comes complete with high-gloss pictorial dust jacket, blood-red endpapers depicting dragon anatomy, and a crimson ribbon bookmark, designed as a lasting library centerpiece."
            },
            new BookSeedItem
            {
                Title = "A Knight of the Seven Kingdoms",
                Author = "George R. R. Martin",
                Genre = "Heroic Fantasy / Adventure",
                Isbn = "9780345533487",
                Price = 18000m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Bantam Trade Paperback Edition",
                PublishDate = DateTime.Parse("2020-02-25"),
                PageCount = 368,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780345533487-L.jpg",
                PlotSummary = "A century before A Game of Thrones, two unlikely heroes wandered Westeros: Ser Duncan the Tall, a young, naive but fiercely honorable hedge knight, and his diminutive squire Egg, who is secretly Prince Aegon Targaryen traveling in disguise. Collecting three novellas—The Hedge Knight, The Sworn Sword, and The Mystery Knight—the stories explore chivalry, tourney rivalries, and the Blackfyre Rebellions. Dunk and Egg navigate deadly political machinations, proving that true honor and heroism reside not in bloodlines or titles, but in self-sacrificing deeds.",
                EditionNotes = "The Bantam trade paperback features over 160 magnificent black-and-white illustrations by acclaimed artist Gary Gianni seamlessly woven into the text. Typeset on acid-free book paper with wide margins and clear headings, the volume contains an introduction by George R. R. Martin and heraldic illustrations of early tourney shields, providing a charming and accessible entry point to the world of Westeros."
            },
            new BookSeedItem
            {
                Title = "A Knight of the Seven Kingdoms",
                Author = "George R. R. Martin",
                Genre = "Heroic Fantasy / Illustrated Classics",
                Isbn = "9780345533425",
                Price = 36000m,
                StockQuantity = 12,
                ReorderLevel = 5,
                EditionName = "Bantam Deluxe Hardcover Edition",
                PublishDate = DateTime.Parse("2015-10-06"),
                PageCount = 368,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780345533425-L.jpg",
                PlotSummary = "Journey into the golden age of the Targaryen dynasty with the wandering hedge knight Ser Duncan the Tall and his spirited squire Egg. From the fateful tourney at Ashford Meadow to the drought-stricken Riverlands and a conspiratorial wedding tourney at Whitewalls, the pair confront rogue knights, corrupt lords, and secret rebellions, laying the foundation for Aegon V's eventual ascension to the Iron Throne.",
                EditionNotes = "This luxury hardcover edition is bound in dark brown cloth with copper foil stamping and full-color pictorial dust jacket art by Gary Gianni. With over one hundred detailed pencil sketches and woodcut-style engravings interspersed throughout the narrative, this edition features custom illustrated endpapers, heavyweight opaque text paper, and a sewn binding engineered for durability."
            },
            new BookSeedItem
            {
                Title = "The World of Ice & Fire: The Untold History of Westeros",
                Author = "George R. R. Martin, Elio M. Garcia Jr., Linda Antonsson",
                Genre = "Fantasy Lore / World-Building Reference",
                Isbn = "9780553805444",
                Price = 49000m,
                StockQuantity = 10,
                ReorderLevel = 5,
                EditionName = "Bantam Deluxe Illustrated Hardcover",
                PublishDate = DateTime.Parse("2014-10-28"),
                PageCount = 336,
                Publisher = "Bantam Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553805444-L.jpg",
                PlotSummary = "This comprehensive illustrated history provides a panoramic overview of the Seven Kingdoms and the broader world from the Dawn Age through Robert's Rebellion. Written as an expansive historical treatise by Maester Yandel, the book covers the Age of Heroes, the Long Night, the rise and fall of Valyria, the history of each of the Nine Free Cities, and the exotic, enigmatic lands of Asshai-by-the-Shadow and the Summer Isles, uncovering secrets and legends previously unrevealed in the main novels.",
                EditionNotes = "A lavish coffee-table hardcover volume spanning 336 oversized full-color pages. Featuring more than 170 original full-color artworks, maps, and family tree diagrams painted by prominent fantasy artists including Ted Nasmith, Marc Simonetti, and Justin Sweet. Bound in embossed leatherette boards with gilded lettering, high-gloss coated stock, and full-color cartographic endpapers."
            },
            new BookSeedItem
            {
                Title = "The World of Ice & Fire: The Untold History of Westeros",
                Author = "George R. R. Martin, Elio M. Garcia Jr., Linda Antonsson",
                Genre = "Fantasy Lore / World-Building Reference",
                Isbn = "9780544773486",
                Price = 32000m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "Harper Voyager Trade Paperback Edition",
                PublishDate = DateTime.Parse("2016-11-03"),
                PageCount = 336,
                Publisher = "Harper Voyager",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780544773486-L.jpg",
                PlotSummary = "An encyclopedic companion to the A Song of Ice and Fire universe, exploring ancient civilizations, legendary historical figures, and regional lineages across Westeros, Essos, and Sothoryos. From the construction of the colossal Wall and the mysteries of the Children of the Forest to the reign of the Mad King Aerys II, this volume illuminates the rich tapestry of lore underpinning the epic fantasy saga.",
                EditionNotes = "The Harper Voyager trade edition provides the full text, color maps, and illustrations in a flexible, lightweight format. It includes full-color reproductions of historical battle paintings, comprehensive dynastic trees, and detailed gazetteers, making it an accessible and portable reference guide for dedicated readers and television fans."
            },
            new BookSeedItem
            {
                Title = "1984",
                Author = "George Orwell",
                Genre = "Dystopian / Political Fiction",
                Isbn = "9780451524935",
                Price = 12500m,
                StockQuantity = 25,
                ReorderLevel = 5,
                EditionName = "Signet Classics Paperback Edition",
                PublishDate = DateTime.Parse("1950-07-01"),
                PageCount = 328,
                Publisher = "Signet Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780451524935-L.jpg",
                PlotSummary = "Winston Smith is a low-ranking member of the ruling Party in London, chief city of Airstrip One in the superstate Oceania. In a terrifying totalitarian world dominated by Big Brother, the Party controls every aspect of human life through telescreens, historical revisionism by the Ministry of Truth, and the persecution of thoughtcrime. When Winston begins an illicit romance with Julia and questions the regime, he is snared by the Thought Police and subjected to horrific psychological and physical reconditioning inside Room 101, exposing the chilling depths of totalitarian supremacy.",
                EditionNotes = "The Signet Classics mass-market paperback edition of Orwell's seminal masterpiece is a fixture of modern political literature. This edition includes a widely praised afterword by renowned psychoanalyst and philosopher Erich Fromm, who explores the psychological mechanisms of authoritarian obedience. Set in clean, readable typography with a durable laminated cover, the volume also features an authoritative selected bibliography and a note on the development of Newspeak, making it an indispensable resource for students and serious readers."
            },
            new BookSeedItem
            {
                Title = "Animal Farm",
                Author = "George Orwell",
                Genre = "Political Satire / Classic",
                Isbn = "9780451526342",
                Price = 10500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Signet Classics 50th Anniversary Edition",
                PublishDate = DateTime.Parse("1996-04-01"),
                PageCount = 144,
                Publisher = "Signet Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780451526342-L.jpg",
                PlotSummary = "Tired of the exploitation of their human master Mr. Jones, the animals of Manor Farm mount a successful rebellion to establish a self-governed society based on equality and shared labor. However, idealistic principles gradually erode as the shrewd pigs, led by the ruthless Napoleon and persuasive Squealer, consolidate absolute control. Changing the farm's sacred commandments to justify their privileges, the ruling pigs eliminate dissidents and gradually adopt human vices, culminating in the chilling realization that all animals are equal, but some are more equal than others.",
                EditionNotes = "This 50th Anniversary Signet Classics edition provides the definitive English text of Orwell's legendary fable. It includes an insightful preface by Russell Baker examining the timeless relevance of political allegories, alongside C. M. Woodhouse's historic critical introduction. The book is bound in high-durability coated cardstock with clear interior typesetting, making it a handy and enduring volume suitable for classroom studies and general personal libraries."
            },
            new BookSeedItem
            {
                Title = "To Kill a Mockingbird",
                Author = "Harper Lee",
                Genre = "Southern Gothic / Historical Drama",
                Isbn = "9780061120084",
                Price = 14000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Harper Perennial Modern Classics (Paperback)",
                PublishDate = DateTime.Parse("2006-05-23"),
                PageCount = 336,
                Publisher = "Harper Perennial Modern Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780061120084-L.jpg",
                PlotSummary = "Set during the Great Depression in the fictional town of Maycomb, Alabama, the story is narrated by young Scout Finch, whose father, lawyer Atticus Finch, is appointed to defend Tom Robinson, a Black man falsely accused of raping a white woman. As the town's entrenched racial bigotry erupts into hostility, Scout, her brother Jem, and their friend Dill learn harsh truths about morality, injustice, and human prejudice. Through it all, Atticus instills compassion, demonstrating that it is a sin to kill a mockingbird because they do no harm.",
                EditionNotes = "Published under the Harper Perennial Modern Classics imprint, this trade paperback features the memorable silhouette cover art depicting Scout beneath a spreading oak tree. The text is printed on acid-free, non-yellowing cream paper that ensures high reading comfort. An included P.S. section offers author interviews, archival background essays on 1930s Southern culture, and discussion questions for reading circles and academic study."
            },
            new BookSeedItem
            {
                Title = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Genre = "Literary Fiction / Modern Classic",
                Isbn = "9780743273565",
                Price = 12000m,
                StockQuantity = 22,
                ReorderLevel = 5,
                EditionName = "Scribner Trade Paperback Edition",
                PublishDate = DateTime.Parse("2004-09-30"),
                PageCount = 180,
                Publisher = "Scribner",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780743273565-L.jpg",
                PlotSummary = "Narrated by Nick Carraway, the novel chronicles the roaring summer of 1922 in Long Island, where enigmatic millionaire Jay Gatsby throws extravagant weekend parties in the hope of winning back his lost love, Daisy Buchanan. Now married to wealthy aristocrat Tom Buchanan, Daisy is drawn back into Gatsby's orbit, sparking an escalating tragedy fueled by obsession, deception, and class divide. Fitzgerald's dazzling prose provides a poignant critique of the American Dream, exposing the hollowness beneath the gleaming facade of the Jazz Age.",
                EditionNotes = "The official Scribner trade edition showcases the famous 1925 original jacket artwork 'Celestial Eyes' by Francis Cugat. Edited with textual notes by Matthew J. Bruccoli, this authoritative volume provides readers with the pristine text as Fitzgerald intended. The high-quality matte paper, spacious typography, and concise explanatory footnotes make it the golden standard for literary scholars and casual enthusiasts alike."
            },
            new BookSeedItem
            {
                Title = "Pride and Prejudice",
                Author = "Jane Austen",
                Genre = "Regency Romance / Classic Literature",
                Isbn = "9780141439518",
                Price = 13500m,
                StockQuantity = 19,
                ReorderLevel = 5,
                EditionName = "Penguin Classics Edition (Paperback)",
                PublishDate = DateTime.Parse("2002-12-31"),
                PageCount = 480,
                Publisher = "Penguin Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439518-L.jpg",
                PlotSummary = "Set in rural nineteenth-century England, the novel follows the spirited and intelligent Elizabeth Bennet, the second of five daughters whose mother is desperate to see them married to wealthy gentlemen. When the proud, aristocratic Fitzwilliam Darcy arrives in their neighborhood, an initial clash of personalities sparks fierce prejudice between them. As misunderstandings and societal gossip unfold, Elizabeth and Darcy are compelled to confront their personal flaws, gradually recognizing that genuine love requires overcoming stubborn pride and hasty judgment.",
                EditionNotes = "This Penguin Classics edition is edited with an introduction and notes by Vivien Jones, featuring a preface by Tony Tanner. Bound in Penguin's recognizable black spine with front cover fine art, the book features comprehensive textual notes, a chronology of Austen's life, and an insightful commentary on 19th-century English marriage customs, establishing it as a definitive edition for literature lovers."
            },
            new BookSeedItem
            {
                Title = "The Hobbit",
                Author = "J. R. R. Tolkien",
                Genre = "High Fantasy / Classic",
                Isbn = "9780547928227",
                Price = 16000m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Mariner Books Paperback Edition",
                PublishDate = DateTime.Parse("2012-09-18"),
                PageCount = 320,
                Publisher = "Mariner Books / Houghton Mifflin Harcourt",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780547928227-L.jpg",
                PlotSummary = "Bilbo Baggins is a respectable, comfort-loving hobbit residing in the peaceful Shire, who desires nothing more than second breakfasts and a quiet hearth. His tranquil life is shattered when wizard Gandalf and thirteen dwarves, led by Thorin Oakenshield, recruit him as their burglar for a perilous quest to reclaim the Lonely Mountain and its vast treasure from the fearsome dragon Smaug. Along the way, Bilbo encounters giant spiders, treacherous goblins, and a peculiar creature named Gollum, from whom he unwittingly gains a fateful magic ring.",
                EditionNotes = "The Mariner Books 75th Anniversary edition contains Tolkien's original cover painting, interior black-and-white drawings, and runes. The volume includes Thror's Map and the Map of Wilderland printed cleanly across the front and rear spreads. Printed on opaque cream paper with a soft-touch matte lamination on the cover, it remains the beloved introduction to Middle-earth for readers of all ages."
            },
            new BookSeedItem
            {
                Title = "The Fellowship of the Ring",
                Author = "J. R. R. Tolkien",
                Genre = "Epic Fantasy / High Fantasy",
                Isbn = "9780547928210",
                Price = 17500m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "Mariner Books Trade Paperback",
                PublishDate = DateTime.Parse("2012-09-18"),
                PageCount = 432,
                Publisher = "Mariner Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780547928210-L.jpg",
                PlotSummary = "Frodo Baggins inherits a seemingly simple golden ring from his cousin Bilbo, only to learn from Gandalf that it is the One Ring forged by the Dark Lord Sauron to dominate Middle-earth. To prevent catastrophic tyranny, Frodo must journey to Mount Doom in Mordor to destroy it. Accompanied by a brave fellowship of men, elves, dwarves, and hobbits, the party faces relentless Black Riders, treacherous snowy passes, and subterranean horrors in the mines of Moria as the One Ring begins corrupting their hearts.",
                EditionNotes = "This official Tolkien estate-approved trade paperback presents the 50th Anniversary revised text verified by Wayne G. Hammond and Christina Scull. It features Tolkien's original foldout maps of Middle-earth, Tengwar script inscriptions, and full appendices. Bound in flexible card with foil titling, it provides an exceptional reading experience for fantasy aficionados."
            },
            new BookSeedItem
            {
                Title = "The Two Towers",
                Author = "J. R. R. Tolkien",
                Genre = "Epic Fantasy / High Fantasy",
                Isbn = "9780547928203",
                Price = 17500m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "Mariner Books Trade Paperback",
                PublishDate = DateTime.Parse("2012-09-18"),
                PageCount = 352,
                Publisher = "Mariner Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780547928203-L.jpg",
                PlotSummary = "Following the tragic dissolution of the Fellowship, Aragorn, Legolas, and Gimli track captured hobbits across the plains of Rohan, allying with King Theoden to withstand Saruman's monstrous army at the monumental Battle of Helm's Deep. Meanwhile, Frodo and Sam continue their arduous trek toward Mordor across the dead marshes, guided by the duplicitous Gollum. Suspended between loyalty and treachery, the hobbits brave giant spider Shelob's deadly tunnels, navigating dark testing grounds of sacrifice and perseverance.",
                EditionNotes = "The second installment of the Lord of the Rings trilogy in Mariner's uniform paperback edition. It features verified textual corrections, Christopher Tolkien's hand-drawn maps of the West of Middle-earth, and an expanded synopsis of the preceding volume. Its sturdy binding and clean formatting make it ideal for marathon reads and collector shelves."
            },
            new BookSeedItem
            {
                Title = "The Return of the King",
                Author = "J. R. R. Tolkien",
                Genre = "Epic Fantasy / High Fantasy",
                Isbn = "9780547928197",
                Price = 17500m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "Mariner Books Trade Paperback",
                PublishDate = DateTime.Parse("2012-09-18"),
                PageCount = 464,
                Publisher = "Mariner Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780547928197-L.jpg",
                PlotSummary = "The epic battle for Middle-earth reaches its thunderous finale as Sauron unleashes his armies upon Minas Tirith. While Aragorn claims his rightful lineage as the King of Gondor and leads a desperate diversionary march upon the Black Gate, Frodo and Sam crawl across the scorched wastes of Gorgoroth toward the fiery Cracks of Doom. With the Ring's malevolent power peaking, a final struggle on the volcanic brink determines the fate of all free peoples.",
                EditionNotes = "The concluding volume of Tolkien's epic masterpiece contains the complete historical appendices (A through F), chronicling the annals of kings, the calendar of the Shire, and the pronunciation guides of Elvish languages. Featuring pristine typography and durable trade paper binding, it represents the definitive conclusion to the Middle-earth saga."
            },
            new BookSeedItem
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Genre = "Science Fiction / Space Opera",
                Isbn = "9780441172719",
                Price = 18000m,
                StockQuantity = 22,
                ReorderLevel = 5,
                EditionName = "Ace Books Mass Market Paperback",
                PublishDate = DateTime.Parse("1990-09-01"),
                PageCount = 896,
                Publisher = "Ace Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780441172719-L.jpg",
                PlotSummary = "Set on the harsh desert planet Arrakis, the story follows young Paul Atreides, whose noble family is granted stewardship of the galaxy's sole source of 'spice'—the most valuable substance in the universe. Betrayed by rival House Harkonnen and the galactic Emperor, Paul and his mother Jessica flee into the deep desert among the native Fremen. Fulfilling an ancient prophecy as the messianic Muad'Dib, Paul masterminds a planetary uprising using giant sandworms, altering the fate of human civilization forever.",
                EditionNotes = "The classic Ace Books mass-market paperback edition of Frank Herbert's Hugo and Nebula award-winning masterpiece. Complete with extensive appendices exploring the ecology of Arrakis, the religion of the Fremen, and the Bene Gesserit breeding program, this edition includes a comprehensive terminology glossary of the Imperium and hand-drawn orbital maps of the planet."
            },
            new BookSeedItem
            {
                Title = "Fahrenheit 451",
                Author = "Ray Bradbury",
                Genre = "Dystopian / Sci-Fi Classic",
                Isbn = "9781451673319",
                Price = 13000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Simon & Schuster 60th Anniversary Edition",
                PublishDate = DateTime.Parse("2012-01-10"),
                PageCount = 256,
                Publisher = "Simon & Schuster",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781451673319-L.jpg",
                PlotSummary = "Guy Montag is a fireman in a bleak future where firemen do not extinguish fires but ignite them to burn outlawed books. Society is numbed by wall-sized television screens and sensory sedation, until Montag meets an eccentric young neighbor, Clarisse, who encourages him to observe the beauty of nature and question his hollow existence. When Montag begins stealing and reading hidden books, he rebels against his tyrannical captain and flees into the wilderness to join an underground enclave of scholars who memorize literature to preserve human memory.",
                EditionNotes = "The 60th Anniversary edition published by Simon & Schuster features an insightful introduction by fantasy master Neil Gaiman. Bound with a clean minimalist cover depicting a flaming match, this trade paperback incorporates Ray Bradbury's archival retrospective essays and personal recollections detailing how he typed the original manuscript on a rented typewriter in the basement of UCLA library."
            },
            new BookSeedItem
            {
                Title = "Brave New World",
                Author = "Aldous Huxley",
                Genre = "Dystopian / Speculative Fiction",
                Isbn = "9780060850524",
                Price = 13500m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "Harper Perennial Modern Classics",
                PublishDate = DateTime.Parse("2006-10-17"),
                PageCount = 288,
                Publisher = "Harper Perennial",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780060850524-L.jpg",
                PlotSummary = "In the World State of the twenty-sixth century, human beings are genetically engineered in test tubes, conditioned into rigid castes, and kept in perpetual superficial happiness through consumerism and the calming drug soma. Natural birth, family bonds, and art are outlawed. When an outsider named John the Savage is brought from a primitive reservation into modern London, his moral revulsion at the soulless hedonism clashes violently with the ruling World Controller, sparking an ideological showdown between comfort and genuine human freedom.",
                EditionNotes = "Harper Perennial's Modern Classics edition contains Huxley's provocative 1946 foreword along with a comprehensive essay comparing Brave New World with Orwell's 1984. Printed on fine cream wove paper with a modern graphical cover, this volume provides student guides, biographical essays, and thematic analytical frameworks."
            },
            new BookSeedItem
            {
                Title = "The Catcher in the Rye",
                Author = "J. D. Salinger",
                Genre = "Coming-of-Age / American Classic",
                Isbn = "9780316769488",
                Price = 14500m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "Little, Brown Mass Market Edition",
                PublishDate = DateTime.Parse("1991-05-01"),
                PageCount = 224,
                Publisher = "Little, Brown and Company",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780316769488-L.jpg",
                PlotSummary = "Following his expulsion from Pencey Prep boarding school, sixteen-year-old Holden Caulfield wanders New York City for three lonely, freezing days before returning home. Deeply alienated by the perceived 'phoniness' of adult society and grieving the death of his younger brother Allie, Holden navigates encounters with cab drivers, old girlfriends, and teachers while trying to protect children's innocence. Salinger's raw, iconic first-person vernacular captures the angst, vulnerability, and aching isolation of adolescent youth with enduring brilliance.",
                EditionNotes = "Little, Brown's classic paperback edition preserves J. D. Salinger's strict instructions for a plain, unadorned cover featuring only the title and author in clean type against pure white and magenta cardstock. Printed with historical fidelity to the 1951 original text, this volume is an essential piece of twentieth-century American literature."
            },
            new BookSeedItem
            {
                Title = "The Alchemist",
                Author = "Paulo Coelho",
                Genre = "Philosophical Fiction / Fable",
                Isbn = "9780062315007",
                Price = 12500m,
                StockQuantity = 25,
                ReorderLevel = 5,
                EditionName = "HarperOne 25th Anniversary Edition",
                PublishDate = DateTime.Parse("2014-04-15"),
                PageCount = 208,
                Publisher = "HarperOne",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780062315007-L.jpg",
                PlotSummary = "Santiago, an Andalusian shepherd boy, dreams of finding a worldly treasure hidden near the Egyptian pyramids. Encouraged by a gypsy fortune teller and an ancient king, he sells his flock to embark on a journey across the North African desert. Along the way, he encounters a beautiful desert maiden and a wise alchemist who teaches him to read the omens of nature, listen to his heart, and understand that fulfilling one's Personal Legend is life's ultimate spiritual mission.",
                EditionNotes = "The 25th Anniversary trade paperback edition by HarperOne features an inspiring foreword by Paulo Coelho reflecting on the novel's worldwide journey. Beautifully designed with celestial motifs and gold embellishments on the cover, this edition contains author interview excerpts, reading group guides, and high-opacity interior pages crafted for reflective, peaceful reading."
            },
            new BookSeedItem
            {
                Title = "The Da Vinci Code",
                Author = "Dan Brown",
                Genre = "Mystery / Conspiracy Thriller",
                Isbn = "9780307474278",
                Price = 15000m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Anchor Books Trade Paperback",
                PublishDate = DateTime.Parse("2009-03-31"),
                PageCount = 496,
                Publisher = "Anchor Books / Vintage",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780307474278-L.jpg",
                PlotSummary = "While in Paris on business, Harvard symbologist Robert Langdon receives an urgent midnight call to the Louvre Museum, where elderly curator Jacques Saunière has been murdered, leaving behind baffling anagrams drawn in invisible ink. Aided by French cryptologist Sophie Neveu, Langdon unravels clues hidden in Leonardo da Vinci's celebrated paintings, discovering that Saunière belonged to the Priory of Sion—a secret society guarding a two-thousand-year-old secret about the Holy Grail that could shake the foundations of Western Christianity.",
                EditionNotes = "The Anchor Books trade edition provides Dan Brown's international bestselling mystery in a generous, easy-to-read format. It includes historical diagrams, architectural floorplans of the Louvre and Rosslyn Chapel, and cryptographic puzzles. Printed with a high-gloss embossed cover and durable binding built for fast-paced page-turning."
            },
            new BookSeedItem
            {
                Title = "The Kite Runner",
                Author = "Khaled Hosseini",
                Genre = "Contemporary Drama / Historical Fiction",
                Isbn = "9781594631931",
                Price = 14000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Riverhead Books 10th Anniversary Edition",
                PublishDate = DateTime.Parse("2013-03-05"),
                PageCount = 400,
                Publisher = "Riverhead Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781594631931-L.jpg",
                PlotSummary = "Growing up in Kabul in the 1970s, Amir, the son of a wealthy Pashtun merchant, forms a close bond with Hassan, his Hazara servant's son and gifted kite runner. During a triumphant winter kite tournament, a devastating act of cowardice by Amir shatters their friendship. Years later, having fled to America following the Soviet invasion, Amir receives a fateful call from Pakistan offering him a dangerous path back to Taliban-controlled Afghanistan: a chance to be good again by rescuing Hassan's orphaned son.",
                EditionNotes = "Riverhead's 10th Anniversary paperback edition features an exclusive essay by Khaled Hosseini on how the novel transformed his life, as well as archival photographs of pre-war Kabul. Featuring French flaps, deckled page edges, and premium interior paper stock, this edition provides a deeply moving tactile experience."
            },
            new BookSeedItem
            {
                Title = "A Thousand Splendid Suns",
                Author = "Khaled Hosseini",
                Genre = "Historical Fiction / Women's Drama",
                Isbn = "9781594489501",
                Price = 14500m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "Riverhead Books Trade Paperback",
                PublishDate = DateTime.Parse("2008-05-01"),
                PageCount = 432,
                Publisher = "Riverhead Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781594489501-L.jpg",
                PlotSummary = "Spanning three tumultuous decades of Afghan history, the novel weaves the lives of two women forced into marriage with the same tyrannical shoemaker in Kabul: Mariam, an illegitimate girl bearing deep social shame, and Laila, an educated young woman orphaned by rocket bombardments. Despite initial rivalry, the two women forge an unbreakable sisterly bond of solidarity and love, finding resilience and making extraordinary sacrifices to protect each other and their children against horrific domestic and wartime oppression.",
                EditionNotes = "Published by Riverhead Books in high-quality trade paperback with textured linen-finish covers, this volume contains an appendix with a historical chronology of Afghanistan from the 1970s Soviet intervention through modern rebuilding efforts. Complete with an author conversation and book club discussion points."
            },
            new BookSeedItem
            {
                Title = "Life of Pi",
                Author = "Yann Martel",
                Genre = "Philosophical Adventure / Magical Realism",
                Isbn = "9780156027328",
                Price = 13500m,
                StockQuantity = 19,
                ReorderLevel = 5,
                EditionName = "Harvest / Harcourt Trade Paperback",
                PublishDate = DateTime.Parse("2003-05-01"),
                PageCount = 336,
                Publisher = "Harcourt / Mariner Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780156027328-L.jpg",
                PlotSummary = "Pi Patel, the sixteen-year-old son of a zookeeper in Pondicherry, India, survives the tragic sinking of a Japanese cargo ship in the Pacific Ocean. Cast adrift in a twenty-six-foot lifeboat, Pi finds himself stranded with an injured zebra, an aggressive hyena, an orangutan, and a 450-pound Royal Bengal tiger named Richard Parker. Over the course of 227 days at sea, Pi relies on zoological knowledge, resourcefulness, and profound faith to survive an extraordinary test of human spirit and spiritual perseverance.",
                EditionNotes = "The Harvest trade paperback edition features striking cover artwork depicting Pi in the orange lifeboat facing the tiger across calm azure waves. The book includes an extensive author's note on the inception of the story, accompanied by a reading group guide and discussions on faith, animal behavior, and the power of storytelling."
            },
            new BookSeedItem
            {
                Title = "The Book Thief",
                Author = "Markus Zusak",
                Genre = "Historical Fiction / YA Classic",
                Isbn = "9780375842207",
                Price = 15500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Knopf Books for Young Readers Paperback",
                PublishDate = DateTime.Parse("2007-09-11"),
                PageCount = 576,
                Publisher = "Alfred A. Knopf",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780375842207-L.jpg",
                PlotSummary = "Narrated by Death, the story centers on Liesel Meminger, a foster girl living in Nazi Germany near Munich in 1939. Learning to read with the patient encouragement of her accordion-playing foster father, Liesel steals books from Nazi bonfires and the mayor's library to share stories with neighbors during air raids. Her life is transformed when her foster family hides Max Vandenburg, a young Jewish fist-fighter, in their basement, proving that words carry immense power to feed the soul during times of unimaginable darkness.",
                EditionNotes = "Knopf's definitive paperback edition presents Markus Zusak's beloved international bestseller with Trudy White's original interior linocut sketches and illustrated vignettes intact. Printed on thick, non-glare cream paper with durable binding, it includes an exclusive author interview and reader discussion materials."
            },
            new BookSeedItem
            {
                Title = "Sapiens: A Brief History of Humankind",
                Author = "Yuval Noah Harari",
                Genre = "Non-Fiction / Anthropology & History",
                Isbn = "9780062316097",
                Price = 19500m,
                StockQuantity = 24,
                ReorderLevel = 5,
                EditionName = "Harper Trade Paperback Edition",
                PublishDate = DateTime.Parse("2015-02-10"),
                PageCount = 464,
                Publisher = "Harper",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780062316097-L.jpg",
                PlotSummary = "Dr. Yuval Noah Harari charts the history of our species, exploring how an insignificant ape on the plains of East Africa became the master of planet Earth. Tracing three crucial revolutions—the Cognitive Revolution, the Agricultural Revolution, and the Scientific Revolution—Harari examines how shared mythologies such as money, religion, human rights, and corporations enabled unprecedented mass cooperation, while questioning whether technological mastery has genuinely improved human happiness or ecological stewardship.",
                EditionNotes = "Harper's bestselling trade paperback edition is illustrated with black-and-white historical photographs, timeline charts, and archaeological diagrams. Sized generously in trade paperback format with high-opacity text pages and a sturdy matte-laminated cover, it remains a landmark best-seller in contemporary non-fiction."
            },
            new BookSeedItem
            {
                Title = "Atomic Habits",
                Author = "James Clear",
                Genre = "Self-Help / Personal Productivity",
                Isbn = "9780735211292",
                Price = 18500m,
                StockQuantity = 25,
                ReorderLevel = 5,
                EditionName = "Avery Hardcover Edition",
                PublishDate = DateTime.Parse("2018-10-16"),
                PageCount = 320,
                Publisher = "Avery / Penguin Random House",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780735211292-L.jpg",
                PlotSummary = "Drawing on behavioral science, cognitive psychology, and neuroscience, James Clear presents a proven framework for achieving remarkable results through small, incremental improvements. By breaking down the four laws of behavior change—make it obvious, make it attractive, make it easy, and make it satisfying—Clear provides practical strategies to eliminate bad habits and establish enduring positive routines. Emphasizing systems over goals, the book teaches that identity-based changes lead to lifelong personal transformation.",
                EditionNotes = "The original Avery hardcover edition of the multi-million-copy bestseller features a pristine white dust jacket with embossed golden atom graphics. Typeset in crisp modern typography with clear summary tables and operational habit scorecards at the end of every chapter, it includes ribbon markers and durable cloth spine construction."
            },
            new BookSeedItem
            {
                Title = "Thinking, Fast and Slow",
                Author = "Daniel Kahneman",
                Genre = "Psychology / Behavioral Economics",
                Isbn = "9780374533557",
                Price = 19000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "Farrar, Straus and Giroux Paperback",
                PublishDate = DateTime.Parse("2013-04-02"),
                PageCount = 512,
                Publisher = "Farrar, Straus and Giroux",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780374533557-L.jpg",
                PlotSummary = "Nobel laureate Daniel Kahneman takes readers on a groundbreaking tour of the human mind, explaining the two cognitive systems that drive decision-making: System 1 (fast, intuitive, and emotional) and System 2 (slow, deliberative, and logical). Examining cognitive biases such as loss aversion, anchoring, and overconfidence, Kahneman reveals the predictable errors that cloud human judgment in finance, medicine, and daily life, offering insights into how we can become smarter, more rational thinkers.",
                EditionNotes = "The FSG trade paperback edition includes Kahneman's seminal academic papers co-authored with Amos Tversky reprinted in full within the appendices. Formatted in clean double-spaced layout with charts and experimental questions, this edition is an indispensable modern classic for business leaders, academics, and thinkers."
            },
            new BookSeedItem
            {
                Title = "The Power of Habit",
                Author = "Charles Duhigg",
                Genre = "Psychology / Business & Self-Help",
                Isbn = "9780812981605",
                Price = 16500m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Random House Trade Paperback",
                PublishDate = DateTime.Parse("2014-01-07"),
                PageCount = 416,
                Publisher = "Random House",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780812981605-L.jpg",
                PlotSummary = "Pulitzer Prize-winning reporter Charles Duhigg explains the science behind habit formation and modification. Centered on the neurological 'habit loop'—cue, routine, and reward—the book explores how habits function within individual lives, successful corporations such as Procter & Gamble and Starbucks, and societal movements like the civil rights boycotts. Demonstrating that habits are malleable, Duhigg provides a clear blueprint to diagnose and reprogram unconscious behaviors for personal and professional growth.",
                EditionNotes = "Random House's popular trade paperback includes a practical reader's appendix offering a step-by-step framework to apply the habit loop to everyday challenges. Designed with generous margins and clear chapter breakdowns, it features extensive source notes and interviews with leading neurobiologists."
            },
            new BookSeedItem
            {
                Title = "Man's Search for Meaning",
                Author = "Viktor E. Frankl",
                Genre = "Memoir / Existential Psychology",
                Isbn = "9780807014295",
                Price = 12500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Beacon Press Trade Paperback",
                PublishDate = DateTime.Parse("2006-06-01"),
                PageCount = 184,
                Publisher = "Beacon Press",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780807014295-L.jpg",
                PlotSummary = "Psychiatrist Viktor Frankl recounts his harrowing survival across four Nazi concentration camps, including Auschwitz, while observing how fellow inmates coped with unimaginable suffering. Rather than succumbing to despair, Frankl formulated 'logotherapy'—a therapeutic approach positing that humanity's primary motivational force is not pleasure or power, but the pursuit of meaning. Even in the direst circumstances, Frankl argues, human beings possess the ultimate freedom to choose their spiritual attitude and find purpose in life.",
                EditionNotes = "The Beacon Press trade edition features a foreword by Rabbi Harold S. Kushner and an afterword by William J. Winslade detailing Frankl's later life and clinical impact. Bound in elegant cream cardstock with deep navy typography, it includes Frankl's definitive summary of logotherapy principles."
            },
            new BookSeedItem
            {
                Title = "Norwegian Wood",
                Author = "Haruki Murakami",
                Genre = "Literary Fiction / Coming-of-Age",
                Isbn = "9780307744661",
                Price = 15000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Vintage International Paperback",
                PublishDate = DateTime.Parse("2010-09-01"),
                PageCount = 304,
                Publisher = "Vintage Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780307744661-L.jpg",
                PlotSummary = "Toru Watanabe hears the Beatles song 'Norwegian Wood' upon landing in Hamburg, triggering nostalgic memories of his college days in 1960s Tokyo. Haunted by the tragic suicide of his best friend Kizuki, Toru finds himself caught between two vastly different women: Naoko, Kizuki's fragile, grief-stricken girlfriend who retreats into a mountain sanatorium, and Midori, a vivacious, outspoken classmate full of life. Murakami paints a poignant, delicate portrait of youthful grief, blossoming love, and emotional loss.",
                EditionNotes = "Translated by Jay Rubin, this Vintage International trade paperback features John Gall's minimalist cover art showcasing the red-and-white circle design characteristic of Murakami's uniform catalog. The volume is printed on archival acid-free paper, providing an immersive reading experience."
            },
            new BookSeedItem
            {
                Title = "Kafka on the Shore",
                Author = "Haruki Murakami",
                Genre = "Magical Realism / Surrealist Fiction",
                Isbn = "9781400079278",
                Price = 16000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "Vintage International Paperback",
                PublishDate = DateTime.Parse("2006-01-03"),
                PageCount = 480,
                Publisher = "Vintage Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9781400079278-L.jpg",
                PlotSummary = "Fifteen-year-old runaway Kafka Tamura flees his home to escape a dark Oedipal curse spoken by his estranged father, seeking refuge in a quiet private library in Takamatsu. Concurrently, an aging, intellectually disabled man named Nakata, who can converse with cats, finds himself propelled on a bizarre journey following an unavoidable murder. Their surreal parallel trajectories intertwine with fish raining from the skies, mysterious Colonel Sanders appearances, and entrance stones bridging life and memory.",
                EditionNotes = "Philip Gabriel's acclaimed English translation in the Vintage International series features eye-catching cover design by John Gall. It includes an introductory author Q&A exploring Murakami's inspirations from Greek mythology and Japanese folk tales. Complete with generous margins and high-quality binding."
            },
            new BookSeedItem
            {
                Title = "The Little Prince",
                Author = "Antoine de Saint-Exupery",
                Genre = "Philosophical Fable / Classic",
                Isbn = "9780156012195",
                Price = 11000m,
                StockQuantity = 25,
                ReorderLevel = 5,
                EditionName = "Harcourt Trade Paperback Edition",
                PublishDate = DateTime.Parse("2000-05-15"),
                PageCount = 96,
                Publisher = "Houghton Mifflin Harcourt",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780156012195-L.jpg",
                PlotSummary = "An aviator stranded in the Sahara desert following an engine failure meets a mysterious, golden-haired boy who hails from Asteroid B-612. As the little prince recounts his travels across planets inhabited by foolish grown-ups and his love for a proud rose left behind, he meets a desert fox who imparts the story's profound lesson: 'It is only with the heart that one can see rightly; what is essential is invisible to the eye.'",
                EditionNotes = "Translated into English by Richard Howard, this classic trade edition restores the author's original delicate watercolor illustrations in soft, luminous full color. Printed on heavy matte paper with high color fidelity, this book remains a cherished heirloom across generations."
            },
            new BookSeedItem
            {
                Title = "The Picture of Dorian Gray",
                Author = "Oscar Wilde",
                Genre = "Gothic Fiction / Philosophical Drama",
                Isbn = "9780141439570",
                Price = 12500m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "Penguin Classics Revised Edition",
                PublishDate = DateTime.Parse("2003-01-30"),
                PageCount = 304,
                Publisher = "Penguin Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439570-L.jpg",
                PlotSummary = "Entranced by artist Basil Hallward's portrait and seduced by Lord Henry Wotton's hedonistic philosophy, youthful Dorian Gray makes a Faustian wish that the painting would age while he retains youthful beauty. As Dorian descends into a life of secret debauchery, cruelty, and murder, his physical visage remains angelic while the hidden canvas transforms into a hideous reflection of his corrupt, rotting soul, leading to a fateful reckoning with his conscience.",
                EditionNotes = "Edited by Robert Mighall, this Penguin Classics volume includes the complete 1891 text alongside Wilde's famous defensive preface on aestheticism. Complete with historical notes on Victorian censorship, contemporary reviews, and an essay analyzing Decadent literature."
            },
            new BookSeedItem
            {
                Title = "Crime and Punishment",
                Author = "Fyodor Dostoevsky",
                Genre = "Psychological Fiction / Russian Classic",
                Isbn = "9780140449136",
                Price = 16500m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "Penguin Classics Deluxe Paperback",
                PublishDate = DateTime.Parse("2003-12-30"),
                PageCount = 592,
                Publisher = "Penguin Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780140449136-L.jpg",
                PlotSummary = "In the sweltering slums of Saint Petersburg, impoverished former student Rodion Raskolnikov conceives a theory dividing humanity into ordinary people and 'extraordinary' men permitted to transgress moral laws. To test his theory, he murders a despicable pawnbroker with an axe, but is immediately plunged into frantic fever, guilt, and paranoia. Pursued by clever detective Porfiry Petrovich and comforted by the pious, self-sacrificing prostitute Sonya, Raskolnikov undergoes an agonizing spiritual journey toward repentance.",
                EditionNotes = "Translated by David McDuff, this Penguin Classics volume provides a vivid, faithful rendition of Dostoevsky's Russian prose. Features a detailed critical introduction, a map of 1860s St. Petersburg, and an explanatory guide to Russian patronymics and social ranks."
            },
            new BookSeedItem
            {
                Title = "The Brothers Karamazov",
                Author = "Fyodor Dostoevsky",
                Genre = "Philosophical Fiction / Russian Masterpiece",
                Isbn = "9780374528379",
                Price = 19500m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "Farrar, Straus and Giroux Paperback",
                PublishDate = DateTime.Parse("2002-06-14"),
                PageCount = 824,
                Publisher = "Farrar, Straus and Giroux",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780374528379-L.jpg",
                PlotSummary = "Dostoevsky's crowning masterpiece revolves around the murder of the vulgar patriarch Fyodor Pavlovich Karamazov and the ensuing trial of his eldest son Dmitri. The novel explores the conflicting worldviews of the three Karamazov brothers: passionate soldier Dmitri, intellectual atheist Ivan, and compassionate novice monk Alyosha. Incorporating the famous chapter 'The Grand Inquisitor,' the book conducts a profound examination of faith, free will, morality, and redemption.",
                EditionNotes = "Translated by the award-winning duo Richard Pevear and Larissa Volokhonsky, this edition is celebrated worldwide for restoring the authentic cadence, humor, and psychological intensity of Dostoevsky's Russian voice. Includes an introduction, textual notes, and a character index."
            },
            new BookSeedItem
            {
                Title = "Jane Eyre",
                Author = "Charlotte Bronte",
                Genre = "Gothic Romance / Victorian Classic",
                Isbn = "9780141441146",
                Price = 13000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Penguin Classics Paperback Edition",
                PublishDate = DateTime.Parse("2006-04-25"),
                PageCount = 576,
                Publisher = "Penguin Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780141441146-L.jpg",
                PlotSummary = "Orphaned as a child and mistreated by her cruel aunt and harsh boarding school, resilient Jane Eyre takes a position as governess at the secluded manor of Thornfield Hall. There, she falls in love with her brooding, enigmatic employer, Edward Rochester. On what should be their wedding day, a terrible secret from Rochester's past is revealed—a madwoman locked in the attic—forcing Jane to choose between passion and her moral self-respect.",
                EditionNotes = "This Penguin Classics edition, edited by Stevie Davies, features an authoritative introduction exploring Bronte's revolutionary treatment of female agency, alongside comprehensive explanatory notes and an extensive glossary of Victorian phrasing."
            },
            new BookSeedItem
            {
                Title = "Wuthering Heights",
                Author = "Emily Bronte",
                Genre = "Gothic Romance / Tragedy",
                Isbn = "9780141439556",
                Price = 13000m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "Penguin Classics Paperback Edition",
                PublishDate = DateTime.Parse("2003-01-30"),
                PageCount = 448,
                Publisher = "Penguin Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439556-L.jpg",
                PlotSummary = "Set upon the tempestuous Yorkshire moors, the story chronicles the fierce, destructive passion between Heathcliff, a mistreated orphan brought to Wuthering Heights, and Catherine Earnshaw. When Catherine chooses to marry the wealthy Edgar Linton for social standing, Heathcliff vows vengeance against both families, spending his life orchestrating a brutal campaign of retribution that spans generations.",
                EditionNotes = "Edited by Pauline Nestor with a preface by Lucasta Miller, this edition provides the authentic 1847 first edition text, complete with a family tree, map of the moors, and critical analysis of Emily Bronte's revolutionary narrative structure."
            },
            new BookSeedItem
            {
                Title = "Frankenstein",
                Author = "Mary Shelley",
                Genre = "Gothic Horror / Sci-Fi Origin",
                Isbn = "9780141439471",
                Price = 12000m,
                StockQuantity = 19,
                ReorderLevel = 5,
                EditionName = "Penguin Classics 1818 Text",
                PublishDate = DateTime.Parse("2003-04-29"),
                PageCount = 352,
                Publisher = "Penguin Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439471-L.jpg",
                PlotSummary = "Obsessed with unlocking the secrets of life and death, young scientist Victor Frankenstein reanimates a giant humanoid composed of grave parts. Terrified by the grotesque appearance of his creation, Victor abandons him. Shuttered by society and longing for companionship, the intelligent creature turns to monstrous vengeance against his creator, pursuing Victor to the desolate ice floes of the Arctic.",
                EditionNotes = "This Penguin Classics edition restores the raw 1818 original text published when Mary Shelley was only twenty years old. Edited by Maurice Hindle, it includes Shelley's 1831 introduction, contemporary reception notes, and a critical chronology."
            },
            new BookSeedItem
            {
                Title = "Dracula",
                Author = "Bram Stoker",
                Genre = "Gothic Horror / Epistolary",
                Isbn = "9780141439846",
                Price = 13500m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Penguin Classics Paperback",
                PublishDate = DateTime.Parse("2003-10-30"),
                PageCount = 496,
                Publisher = "Penguin Classics",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780141439846-L.jpg",
                PlotSummary = "Told through journal entries, letters, and telegraphs, the story begins with English solicitor Jonathan Harker visiting the remote Transylvanian castle of Count Dracula. Escaping back to England, Harker learns the vampire nobleman has arrived in London to feed upon his fiancee Mina's friend Lucy. Led by Dutch doctor Abraham Van Helsing, a determined band of companions pursues Dracula back to his mountain lair in a desperate battle to save Mina's soul.",
                EditionNotes = "Edited by Maurice Hindle, this edition includes extensive notes on Victorian sexual anxieties, folklore origins of vampirism, and Stoker's original theatrical notes, wrapped in Penguin's durable black-jacket aesthetic."
            },
            new BookSeedItem
            {
                Title = "The Name of the Wind",
                Author = "Patrick Rothfuss",
                Genre = "Heroic Fantasy / Coming-of-Age",
                Isbn = "9780756404741",
                Price = 19000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "DAW Books Trade Paperback",
                PublishDate = DateTime.Parse("2008-04-01"),
                PageCount = 672,
                Publisher = "DAW Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780756404741-L.jpg",
                PlotSummary = "In a remote inn, the legendary adventurer and musician Kvothe tells his real life story to a traveling chronicler. Beginning with his childhood among a troupe of traveling performers slaughtered by the mythical Chandrian, Kvothe survives as a feral street urchin before gaining admittance to the prestigious University to study magic, alchemy, and the secret names of the elements.",
                EditionNotes = "DAW Books trade paperback edition featuring lyrical prose and world-building diagrams. Includes the map of the Four Corners of Civilization and an appendix exploring the magical sympathy systems."
            },
            new BookSeedItem
            {
                Title = "The Way of Kings",
                Author = "Brandon Sanderson",
                Genre = "Epic Fantasy / The Stormlight Archive",
                Isbn = "9780765365279",
                Price = 24000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "Tor Books Mass Market Paperback",
                PublishDate = DateTime.Parse("2011-05-24"),
                PageCount = 1280,
                Publisher = "Tor Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780765365279-L.jpg",
                PlotSummary = "On the storm-ravaged world of Roshar, an endless war rages on the Shattered Plains. Kaladin, a disgraced surgeon turned slave bridge-runner, fights to protect his comrades; Brightlord Dalinar Kholin experiences disturbing visions of ancient Radiants; and young scholar Shallan seeks to save her family from bankruptcy through dangerous academic theft as the ancient Voidbringers awaken.",
                EditionNotes = "Tor Books mass-market edition contains dozens of original interior illustrations, botanical sketches, and maps drawn by Michael Whelan and Isaac Stewart, bound with reinforced spine construction to support its 1280-page length."
            },
            new BookSeedItem
            {
                Title = "Mistborn: The Final Empire",
                Author = "Brandon Sanderson",
                Genre = "Epic Fantasy / Heist & Magic",
                Isbn = "9780765350381",
                Price = 16500m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "Tor Books Mass Market Paperback",
                PublishDate = DateTime.Parse("2007-07-24"),
                PageCount = 672,
                Publisher = "Tor Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780765350381-L.jpg",
                PlotSummary = "For a thousand years, ash has fallen from the sky and the immortal Lord Ruler has enslaved the Skaa people. Kelsier, a charismatic thief and 'Mistborn' capable of ingesting and burning metals to gain superhuman powers, recruits young street urchin Vin into an audacious plan to rob the empire's treasury and spark a revolution to overthrow a god.",
                EditionNotes = "The introductory volume of the Cosmere universe in Tor's mass-market edition. Includes complete Allomantic metal tables, architectural plans of Luthadel, and Sanderson's magic system diagrams."
            },
            new BookSeedItem
            {
                Title = "Neuromancer",
                Author = "William Gibson",
                Genre = "Cyberpunk / Sci-Fi Classic",
                Isbn = "9780441569595",
                Price = 14000m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "Ace Books Paperback Edition",
                PublishDate = DateTime.Parse("2000-07-01"),
                PageCount = 288,
                Publisher = "Ace Books",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780441569595-L.jpg",
                PlotSummary = "Henry Dorsett Case is a washed-up computer hacker in the underworld of Chiba City whose nervous system was sabotaged by angry employers. Recruited by an enigmatic operative named Armitage and paired with razor-fingered mercenary Molly Millions, Case's neural pathways are repaired for a final mission: hacking an omnipotent artificial intelligence owned by the Tessier-Ashpool corporate dynasty.",
                EditionNotes = "The groundbreaking novel that coined the term 'cyberspace' and swept the Hugo, Nebula, and Philip K. Dick awards. Ace's edition includes a celebratory author's note examining the evolution of the Internet and digital culture."
            },
            new BookSeedItem
            {
                Title = "Foundation",
                Author = "Isaac Asimov",
                Genre = "Science Fiction / Space Opera Classic",
                Isbn = "9780553293357",
                Price = 13500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "Spectra / Bantam Paperback",
                PublishDate = DateTime.Parse("1991-06-01"),
                PageCount = 256,
                Publisher = "Spectra / Bantam",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780553293357-L.jpg",
                PlotSummary = "Mathematician Hari Seldon develops 'psychohistory'—a science capable of predicting the behavior of massive populations over millennia. Foreseeing the collapse of the 12,000-year-old Galactic Empire and a subsequent 30,000-year dark age, Seldon establishes the Foundation on the remote world Terminus to preserve human knowledge and shorten the coming barbarism to a single millennium.",
                EditionNotes = "The cornerstone of Isaac Asimov's celebrated Hugo Award-winning series for Best All-Time Series. Bantam Spectra's edition provides the original text with an introductory preface by Asimov detailing the novel's origins inspired by Gibbon's Decline and Fall of the Roman Empire."
            },
            new BookSeedItem
            {
                Title = "Lord of the Flies",
                Author = "William Golding",
                Genre = "Allegorical Fiction / Classic",
                Isbn = "9780399501487",
                Price = 12500m,
                StockQuantity = 22,
                ReorderLevel = 5,
                EditionName = "Perigee Books Paperback Edition",
                PublishDate = DateTime.Parse("2003-12-16"),
                PageCount = 224,
                Publisher = "Perigee / Penguin Group",
                CoverImageUrl = "https://covers.openlibrary.org/b/isbn/9780399501487-L.jpg",
                PlotSummary = "When a plane crashes on an uninhabited tropical island during an evacuation, a group of British schoolboys are stranded without adult supervision. Initially electing Ralph as leader and using a conch shell to establish order, the boys gradually fracture under the influence of Jack, who forms a savage hunting tribe. Fear of an imaginary beast leads to paranoia, superstition, and ritualistic violence, exposing the inherent cruelty beneath civilized human veneer.",
                EditionNotes = "Perigee's classic trade paperback edition includes an insightful critical essay by E. L. Epstein and a biographical note on Nobel laureate William Golding. Printed on acid-neutral paper with the haunting painted face cover graphic."
            },
            new BookSeedItem
            {
                Title = "မအိမ်ကံ",
                Author = "ခင်ခင်ထူး",
                Genre = "သမိုင်းနောက်ခံ ဝတ္ထုရှည် / ကျေးလက်ဓလေ့",
                Isbn = "978-9997101235",
                Price = 9500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "စတုတ္ထအကြိမ် ပုံနှိပ်ထုတ်ဝေခြင်း (စက္ကူချော အထူးထုတ်)",
                PublishDate = DateTime.Parse("2018-05-10"),
                PageCount = 360,
                Publisher = "စိတ်ကူးချိုချို စာပေတိုက်",
                CoverImageUrl = "",
                PlotSummary = "အထက်မြန်မာပြည် အညာဒေသ ကျေးလက်လူနေမှုဘဝနှင့် ရိုးရာဓလေ့ထုံးတမ်းစဉ်လာများကို နောက်ခံပြု၍ မြန်မာအမျိုးသမီးတစ်ဦးဖြစ်သူ 'မအိမ်ကံ' ၏ ဘဝကြမ္မာလှိုင်းတံပိုးများကို သရုပ်ဖော်ထားသည့် ဂန္ထဝင်ဝတ္ထုကြီးဖြစ်ပါသည်။ မိသားစုတာဝန်၊ အသိုင်းအဝိုင်း၏ ရိုးရာယဉ်ကျေးမှု ချည်နှောင်မှုများနှင့် ခေတ်ကာလအပြောင်းအလဲကြားတွင် မိန်းမသားတစ်ဦး၏ မဆုတ်မနစ်သော ဇွဲလုံ့လ၊ သည်းခံကြံ့ကြံ့ခံနိုင်မှုနှင့် စွန့်လွှတ်အနစ်နာခံမှုများကို အညာဒေသစကားဝဲဝဲ၊ ရသမြောက် စာပန်းချီများဖြင့် အသက်ဝင်စွာ ဖွဲ့ဆိုထားပါသည်။",
                EditionNotes = "ဤစတုတ္ထအကြိမ်ထုတ်ဝေမှုတွင် စာရေးဆရာမကြီးကိုယ်တိုင် ယူနီကုဒ်စာလုံးပေါင်းသတ်ပုံများကို တိုက်ဆိုင်စစ်ဆေးပြင်ဆင်ထားပါသည်။ စာဖတ်သူ မျက်စိမညောင်းစေရန် ဂျပန်နိုင်ငံထုတ် 70gsm မျက်စိအေးစက္ကူ (Cream Paper) ဖြင့် ပုံနှိပ်ထားပြီး၊ အညာရိုးရာ ရက်ကန်းလုပ်ငန်းနှင့် ပတ်သက်သည့် မှတ်တမ်းဓာတ်ပုံများ၊ စာရေးသူ၏ အထူးအမှာစာတို့ ပါဝင်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ရေနံ့သာခင်ခင်ကြီး",
                Author = "မောင်သိန်းဆိုင်",
                Genre = "ဂန္ထဝင်ဒရာမာ / အချစ်ဝတ္ထုရှည်",
                Isbn = "978-9997104526",
                Price = 12000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "စ+ဆုံး ပေါင်းချုပ် မာတိကာပြည့်စုံထုတ်",
                PublishDate = DateTime.Parse("2016-11-01"),
                PageCount = 512,
                Publisher = "ပန်းဆက်လမ်း စာပေ",
                CoverImageUrl = "",
                PlotSummary = "ကိုလိုနီခေတ် ရေနံချောင်း ရေနံတွင်းလုပ်ငန်းများ အထွတ်အထိပ်ရောက်ချိန်ကို ဇာတ်အိမ်တည်ထားပြီး မာနကြီးပြီး ချောမောလှပလွန်းလှသည့် 'ခင်ခင်ကြီး' အမည်ရှိ အမျိုးသမီးတစ်ဦး၏ အချစ်၊ အာဏာနှင့် အရှုံးအနိုင်ပြိုင်ဆိုင်မှုများကို ရေးဖွဲ့ထားပါသည်။ ခေတ်ဟောင်း မြန်မာ့ရိုးရာ အသိုင်းအဝိုင်းနှင့် နယ်ချဲ့အရင်းရှင်တို့၏ ရေနံလောကကြား ရင်ဆိုင်တွေ့ကြုံရသည့် လူသားတို့၏ မာနတရား၊ လောဘနှင့် မေတ္တာတရားတို့ ရောပြွမ်းနေပုံကို ဇာတ်ကွက်ဇာတ်လမ်း အလှည့်အပြောင်းများစွာဖြင့် ရင်နင့်ဖွယ်ရာ ဖတ်ရှုရမည်ဖြစ်ပါသည်။",
                EditionNotes = "မူရင်းအပိုင်း (၁) နှင့် (၂) တို့ကို အပိုအလိုမရှိ ပြန်လည်ပေါင်းစပ်ကာ စာအုပ်တစ်အုပ်တည်းအဖြစ် ထုတ်ဝေထားသော အပြည့်အစုံထုတ်ဖြစ်ပါသည်။ မူရင်းပန်းချီလက်ရာ မျက်နှာဖုံးကို ပြန်လည်အသက်သွင်းထားပြီး၊ စာအုပ်ချုပ်လုပ်မှုတွင် ခိုင်ခံ့သော အထူးချုပ်ရိုးစနစ်ကို အသုံးပြုထားသည့်အတွက် စာအုပ်ချစ်သူများ သိမ်းဆည်းထားရန် အထူးသင့်လျော်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မြလေးရုံ",
                Author = "ခင်ခင်ထူး",
                Genre = "မြန်မာ့ရိုးရာ / လူမှုဘဝသရုပ်ဖော်",
                Isbn = "978-9997102140",
                Price = 8500m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပြင်ဆင်ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2019-02-15"),
                PageCount = 280,
                Publisher = "စိတ်ကူးချိုချို စာပေ",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာ့ရိုးရာ ဆိုင်းဝိုင်း၊ သဘင်ပညာနှင့် အနုပညာသည်တို့၏ ဘဝသရုပ်ဖော် ဝတ္ထုရှည်ဖြစ်ပါသည်။ ရိုးရာအနုပညာကို အသက်တမျှ ချစ်မြတ်နိုးကြသော ဇာတ်သဘင်လောကသားတို့၏ ရုန်းကန်လှုပ်ရှားမှု၊ စင်ပေါ်က အလင်းရောင်နှင့် စင်နောက်ကွယ်က မျက်ရည်၊ အချစ်နှင့် မာနတရားတို့၏ ရိုက်ခတ်မှုများကို ရေးဖွဲ့ထားသည်။ ပရိသတ်လက်ခုပ်သံများအောက်တွင် ဝှက်ထားရသော သဘင်သည်တို့၏ ဘဝအမှန်ကို အနုပညာရသ မြောက်စွာ ဖော်ကျူးထားပါသည်။",
                EditionNotes = "ဤအကြိမ်ထုတ်ဝေမှုတွင် စာရေးဆရာမ ခင်ခင်ထူး၏ မြန်မာ့ဇာတ်သဘင် ဝေါဟာရအသုံးအနှုန်း ရှင်းလင်းချက်များ သီးခြားပါဝင်ပြီး၊ စာဖတ်သူတို့ ဖတ်ရှုရ လွယ်ကူစေရန် စာလုံးအရွယ်အစားနှင့် စာမျက်နှာ အပြင်အဆင်ကို ခေတ်မီစွာ ပြင်ဆင်ဖန်တီးထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ကြာဆူး",
                Author = "ဂျူး",
                Genre = "ရသဝတ္ထုရှည် / ခေတ်ပေါ်ဒရာမာ",
                Isbn = "978-9997103116",
                Price = 10500m,
                StockQuantity = 19,
                ReorderLevel = 5,
                EditionName = "ပဉ္စမအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2017-08-20"),
                PageCount = 310,
                Publisher = "ဆည်းဆာစာပေ",
                CoverImageUrl = "",
                PlotSummary = "လူငယ်တို့၏ အတွေးအမြင်၊ အချစ်နှင့် သဘာဝပတ်ဝန်းကျင် ချစ်မြတ်နိုးမှုတို့ကို ပေါင်းစပ်တင်ပြထားသည့် ဆရာမဂျူး၏ ထင်ရှားသော ဝတ္ထုတစ်ပုဒ်ဖြစ်ပါသည်။ အချစ်ဆိုသည်မှာ နူးညံ့လှပသော ကြာပန်းကဲ့သို့ပင် ဖြစ်သော်လည်း ရံဖန်ရံခါတွင် စူးရှနာကျင်စေတတ်သော ဆူးခက်သဖွယ် တည်ရှိနေပုံကို သဘာဝတရား၏ အလှတရားများကြားမှတစ်ဆင့် ခံစားတင်ပြထားပါသည်။ ဇာတ်ကောင်တို့၏ အတွင်းစိတ် ခံစားချက်များကို ရိုးသားပွင့်လင်းစွာ ဖော်ထုတ်ထားသည်။",
                EditionNotes = "စာဖတ်ပရိသတ် လက်ခံအားပေးမှု အများဆုံးရရှိခဲ့သော ဆရာမဂျူး၏ လက်ရာမွန်ဖြစ်ပြီး မျက်နှာဖုံးဒီဇိုင်းအသစ်ဖြင့် အဆင့်မြင့် ပုံနှိပ်စက္ကူအသုံးပြုကာ ပြန်လည်ထုတ်ဝေထားသော စာအုပ်ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မျှော်လင့်ခြင်းခရီးသည်",
                Author = "မောင်စိန်ဝင်း(ပုတီးကုန်း)",
                Genre = "ရသဝတ္ထု / လူငယ်ဘဝဒဿန",
                Isbn = "978-9997105233",
                Price = 8000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2015-06-12"),
                PageCount = 240,
                Publisher = "လောကသစ်စာပေ",
                CoverImageUrl = "",
                PlotSummary = "တက္ကသိုလ်ကျောင်းသားဘဝ၏ ချစ်ခြင်းမေတ္တာ၊ သစ္စာတရားနှင့် ဘဝရည်မှန်းချက်များအတွက် မျှော်လင့်ချက်မပျက် ကြိုးစားအားထုတ်ပုံကို ကဗျာဆန်ဆန် စာသားများဖြင့် ဖွဲ့ဆိုထားသည့် ဝတ္ထုဖြစ်ပါသည်။ ဘဝခရီးလမ်းတွင် အခက်အခဲ မုန်တိုင်းများ မည်မျှပင် တွေ့ကြုံရစေကာမူ မလျှော့သော ဇွဲလုံ့လဖြင့် မိမိတို့၏ အိပ်မက်ဆီသို့ ဆက်လက်လျှောက်လှမ်းသင့်ပုံကို အားတက်ဖွယ် သွန်သင်ထားပါသည်။",
                EditionNotes = "ဆရာမောင်စိန်ဝင်း(ပုတီးကုန်း)၏ လက်ရွေးစင် ကဗျာဆန်ဆန် အသုံးအနှုန်းများဖြင့် စီမံထားပြီး ကျစ်လျစ်ခိုင်မာသော စာအုပ်ချုပ်စနစ်နှင့် စာဖတ်သူ အကြိုက်တွေ့စေမည့် စာမျက်နှာ အရည်အသွေးတို့ ပါဝင်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "စိန်ခေါ်နေတဲ့ အလွမ်းများ",
                Author = "ဂျူး",
                Genre = "အချစ်ဝတ္ထုရှည် / ခေတ်ပေါ်ရသ",
                Isbn = "978-9997103253",
                Price = 10000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "စတုတ္ထအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2019-10-05"),
                PageCount = 290,
                Publisher = "ကံ့ကော်ဝတ်ရည်စာပေ",
                CoverImageUrl = "",
                PlotSummary = "ခေတ်သစ်လူငယ်တို့၏ အချစ်၊ သံယောဇဉ်နှင့် စွန့်လွှတ်ခြင်းတို့ကို ရေးဖွဲ့ထားသည့် နာမည်ကျော် ဝတ္ထုဖြစ်ပါသည်။ အချိန်နှင့် နေရာဒေသ ကွာဝေးမှုများကြားတွင် အလွမ်းတရားတို့က မည်သို့ စိန်ခေါ်နေပုံ၊ လူသားတို့၏ မေတ္တာတရားသည် အခက်အခဲအတားအဆီးများကို ကျော်လွှားနိုင်ပုံတို့ကို နက်ရှိုင်းသော စကားလုံးများဖြင့် သရုပ်ဖော်ထားပါသည်။",
                EditionNotes = "ဆရာမဂျူး၏ စာချစ်သူများ နှစ်သက်ကြသည့် စာအုပ်ဖြစ်ပြီး မျက်နှာဖုံးတွင် အနုပညာမြောက် ပန်းချီဒီဇိုင်းဖြင့် ပုံနှိပ်ကာ စာမျက်နှာ အထားအသို ပြေပြစ်စေရန် သေသပ်စွာ ပြင်ဆင်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "အရိုင်းစံပယ်",
                Author = "ခင်ခင်ထူး",
                Genre = "ကျေးလက်ဓလေ့ / လူမှုဘဝ",
                Isbn = "978-9997102188",
                Price = 9000m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2020-01-10"),
                PageCount = 260,
                Publisher = "စိတ်ကူးချိုချို စာပေတိုက်",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာ့ကျေးလက် ရိုးရာဓလေ့နှင့် မြစ်ရိုးဒေသ လူတို့၏ ရိုးသားဖြူစင်မှု၊ ဘဝမာနတို့ကို နောက်ခံပြုထားသော ဝတ္ထုဖြစ်ပါသည်။ တောရိုင်းပန်းလေးတစ်ပွင့်ကဲ့သို့ မာနနှင့် သိက္ခာကို တန်ဖိုးထားသော ကျေးလက်သူလေးတစ်ဦး၏ ဘဝရုန်းကန်မှုနှင့် အချစ်ဇာတ်လမ်းကို အညာသံ အညာဝေါဟာရများဖြင့် အလှဆင်ထားပါသည်။",
                EditionNotes = "အညာဒေသ လူမှုဘဝ ဓာတ်ပုံမှတ်တမ်းတိုများနှင့်အတူ စာရေးသူ၏ အမှာစာတို့ ပါဝင်ပြီး စက္ကူချောဖြင့် သေသပ်စွာ ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မိုးနှင့်အတူ",
                Author = "မိုးမိုး(အင်းလျား)",
                Genre = "ရသဝတ္ထုရှည် / မိန်းမသားဘဝသရုပ်ဖော်",
                Isbn = "978-9997106018",
                Price = 8500m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "အကြိမ်ကြိမ် ပုံနှိပ်ထုတ်ဝေခြင်း (ဂန္ထဝင်ထုတ်)",
                PublishDate = DateTime.Parse("2016-04-18"),
                PageCount = 272,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာစာပေလောကတွင် မိုးမိုး(အင်းလျား)၏ အထင်ကရ လက်ရာတစ်ခုဖြစ်ပြီး မိုးရာသီနှင့်အတူ ရောက်ရှိလာတတ်သော အမှတ်တရများ၊ မိန်းမသားတစ်ဦး၏ မိသားစုနှင့် ပတ်ဝန်းကျင်အပေါ် ထားရှိသော မေတ္တာ၊ အနစ်နာခံမှုများကို ရိုးရှင်းသော်လည်း ရင်ထဲစွဲငြိစေမည့် စာဖွဲ့မှုဖြင့် တင်ပြထားသော ဝတ္ထုကောင်းဖြစ်ပါသည်။",
                EditionNotes = "ဆရာမကြီး မိုးမိုး(အင်းလျား) ကွယ်လွန်ခြင်း နှစ်ပတ်လည် အထိမ်းအမှတ် ဂုဏ်ပြုထုတ်ဝေမှုဖြစ်ပြီး စာရေးသူ၏ ဘဝမှတ်တမ်းအကျဉ်းနှင့် ဓာတ်ပုံတို့ ပါဝင်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "လွမ်းသူ့အိမ်မက် လမင်းမျက်နှာ",
                Author = "တက္ကသိုလ်ဘုန်းနိုင်",
                Genre = "ဂန္ထဝင်အချစ်ဝတ္ထုရှည် / ဒဿနရသ",
                Isbn = "978-9997107022",
                Price = 11500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "ရာပြည့် အထူးထုတ် ပေါင်းချုပ်",
                PublishDate = DateTime.Parse("2017-02-14"),
                PageCount = 440,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "ဆရာကြီး တက္ကသိုလ်ဘုန်းနိုင်၏ ထိပ်တန်းဂန္ထဝင် အချစ်ဝတ္ထုကြီးဖြစ်ပါသည်။ ချစ်ခြင်းမေတ္တာ၏ နက်ရှိုင်းမှု၊ ကံကြမ္မာ၏ မျက်နှာသာမပေးမှုများနှင့် ဘဝ၏ မရေရာသော အလှည့်အပြောင်းများကို မြင့်မြတ်သော ဒဿနအမြင်များဖြင့် တွဲဖက်ရေးသားထားသည်။ ဇာတ်ကောင်တို့၏ အတွင်းစိတ် ပဋိပက္ခများကို လှပလွန်းသော မြန်မာစကားပြေ အရေးအသားဖြင့် ဖွဲ့ဆိုထားသဖြင့် စာဖတ်သူတိုင်း၏ နှလုံးသားကို ညှို့ယူဖမ်းစားနိုင်ပါသည်။",
                EditionNotes = "ဆရာကြီး တက္ကသိုလ်ဘုန်းနိုင်၏ လက်ရာစစ်စစ်ကို မူရင်းစာမူအတိုင်း တိကျစွာ ပြန်လည်ရိုက်နှိပ်ထားသော စာအုပ်ဖြစ်ပြီး အမာဖုံးတု စက္ကူထူဖြင့် အဆင့်မြင့်စွာ စီမံထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "သူငယ်ချင်းလို့ပဲ ဆက်၍ခေါ်မည် ခိုင်",
                Author = "တက္ကသိုလ်ဘုန်းနိုင်",
                Genre = "ဂန္ထဝင်အချစ်ဝတ္ထုရှည်",
                Isbn = "978-9997107039",
                Price = 11000m,
                StockQuantity = 19,
                ReorderLevel = 5,
                EditionName = "အမှတ်တရ ပြန်လည်ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2018-09-01"),
                PageCount = 416,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "တက္ကသိုလ်ကျောင်းသားဘဝ၏ သန့်စင်သော ချစ်ခြင်းမေတ္တာနှင့် သူငယ်ချင်းသံယောဇဉ်ကြားမှ ရွေးချယ်မှုများကို ဖွဲ့ဆိုထားသည့် ဝတ္ထုကောင်းဖြစ်ပါသည်။ ချစ်လျက်နှင့် သူငယ်ချင်းအဖြစ်သာ ဆက်လက်နေထိုင်သွားရသည့် နှလုံးသား၏ စွန့်လွှတ်မှုနှင့် အနစ်နာခံမှုတို့ကို ရင်နင့်ဖွယ် ရေးဖွဲ့ထားရာ မြန်မာစာဖတ်ပရိသတ် မျိုးဆက်အဆက်ဆက် ရင်ထဲစွဲထင်ကျန်ရစ်ခဲ့ပါသည်။",
                EditionNotes = "ကျောင်းသားလူငယ်များ အထူးနှစ်သက်သော ဤဝတ္ထုကို မူရင်းအတိုင်း ပြန်လည်စစ်ဆေး ထုတ်ဝေထားပြီး ဖတ်ရှုရ လွယ်ကူစေမည့် ယူနီကုဒ်စာလုံးပုံစံသစ်ဖြင့် ဖန်တီးထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မျက်ခြယ်",
                Author = "ခင်ခင်ထူး",
                Genre = "သမိုင်းနောက်ခံ / ရိုးရာဓလေ့",
                Isbn = "978-9997102218",
                Price = 9500m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2019-11-20"),
                PageCount = 310,
                Publisher = "စိတ်ကူးချိုချို စာပေ",
                CoverImageUrl = "",
                PlotSummary = "ကုန်းဘောင်ခေတ်နှောင်း မြန်မာ့ရိုးရာ ယဉ်ကျေးမှုနှင့် နန်းတွင်းကျေးလက် ဆက်နွှယ်မှုများကို အခြေခံထားသည့် ဝတ္ထုရှည်ဖြစ်ပါသည်။ မြန်မာအမျိုးသမီးတို့၏ အလှအပဆိုင်ရာ ရိုးရာအသုံးအဆောင်များနှင့် စိတ်နေစိတ်ထား မြင့်မြတ်ပုံတို့ကို သမိုင်းနောက်ခံ အချက်အလက်များနှင့် ပေါင်းစပ်တင်ပြထားပါသည်။",
                EditionNotes = "စာအုပ်ပါ ရိုးရာဝေါဟာရများနှင့် ပတ်သက်သည့် အဖွင့်မှတ်စုများ ပါဝင်ပြီး အနုပညာလက်ရာမြောက် မျက်နှာဖုံးဖြင့် တန်ဆာဆင်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မိုးကုတ်စက်ဝိုင်းကို ကျော်လွန်၍",
                Author = "တက္ကသိုလ်ဘုန်းနိုင်",
                Genre = "ဂန္ထဝင်ဝတ္ထုရှည် / ဘဝဒဿန",
                Isbn = "978-9997107053",
                Price = 12000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "ရာပြည့် ဂန္ထဝင်အထူးထုတ်",
                PublishDate = DateTime.Parse("2016-12-10"),
                PageCount = 480,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "လူသားတို့၏ ရည်မှန်းချက်၊ မေတ္တာတရားနှင့် ဘဝရုန်းကန်မှုများကို မိုးကုတ်စက်ဝိုင်းဟူသော သင်္ကေတဖြင့် သရုပ်ဖော်ထားသည့် နာမည်ကျော် ဝတ္ထုရှည်ဖြစ်ပါသည်။ အတိုင်းအဆမဲ့သော လောကဓံလှိုင်းတံပိုးများကို ရင်ဆိုင်ဖြတ်သန်းရင်း အချစ်နှင့် သစ္စာတရား၏ တန်ဖိုးကို ရှာဖွေတွေ့ရှိသွားပုံကို ဖွဲ့ဆိုထားပါသည်။",
                EditionNotes = "ဆရာကြီး တက္ကသိုလ်ဘုန်းနိုင်၏ လက်ရာမွန်များကို စုစည်းထုတ်ဝေသည့် ရာပြည့်စာအုပ်တိုက်၏ ဂန္ထဝင်စီးရီးဝင် စာအုပ်ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "အတိတ်မှာကျန်ခဲ့သော နေ့ရက်များ",
                Author = "ဂျူး",
                Genre = "ရသဝတ္ထုတိုနှင့် ဝတ္ထုရှည်",
                Isbn = "978-9997103321",
                Price = 9000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2018-05-15"),
                PageCount = 250,
                Publisher = "ဆည်းဆာစာပေ",
                CoverImageUrl = "",
                PlotSummary = "လူငယ်ဘဝ၏ အမှတ်တရများ၊ လွမ်းမောဖွယ် အတိတ်ပုံရိပ်များနှင့် မေ့မရနိုင်သော သံယောဇဉ်ကြိုးများကို ပြန်လည်တူးဆွထားသည့် ဝတ္ထုဖြစ်ပါသည်။ ဆရာမဂျူး၏ ထုံးစံအတိုင်း သဘာဝပတ်ဝန်းကျင်ကို ချစ်မြတ်နိုးသည့် စိတ်ဓာတ်နှင့် လူသားတို့၏ အတွင်းစိတ်ခံစားချက်များကို သိမ်မွေ့စွာ ပေါင်းစပ်ထားပါသည်။",
                EditionNotes = "စာဖတ်သူများအတွက် မျက်စိအေးစေမည့် အရည်အသွေးမြင့် စက္ကူဖြင့် ပုံနှိပ်ထားပြီး လက်ကိုင်ဖတ်ရှုရန် သင့်လျော်သော အရွယ်အစားဖြင့် စီစဉ်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မီးလျှံဝေဝေနှင်းတွေချွေ",
                Author = "တက္ကသိုလ်ဘုန်းနိုင်",
                Genre = "ဂန္ထဝင်ဝတ္ထုရှည်",
                Isbn = "978-9997107084",
                Price = 10500m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2017-11-01"),
                PageCount = 380,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "မီးလျှံကဲ့သို့ ပူပြင်းလှသော လောဘ၊ မောဟများနှင့် နှင်းကဲ့သို့ အေးချမ်းလှသော မေတ္တာ၊ သစ္စာတရားတို့၏ ဆန့်ကျင်ဘက် အားပြိုင်မှုများကို စိတ်ဝင်စားဖွယ် ဇာတ်လမ်းဆင်ထားသော ဝတ္ထုဖြစ်ပါသည်။ ဇာတ်ကောင်များ၏ အမှားနှင့် အမှန်၊ အရှုံးနှင့် အနိုင်ကို ဒဿနအမြင်ဖြင့် မီးမောင်းထိုးပြထားပါသည်။",
                EditionNotes = "စာလုံးပေါင်းသတ်ပုံ စနစ်တကျ ပြင်ဆင်ပြီး စာဖတ်ပရိသတ် လက်ခံသိမ်းဆည်းရန် ခိုင်ခံ့သော စာအုပ်ချုပ်စနစ်ဖြင့် ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "တောအလှရိုးရာ ကျေးလက်ပုံပြင်များ",
                Author = "ခင်ခင်ထူး",
                Genre = "ပုံပြင်နှင့် ရိုးရာဓလေ့ မှတ်တမ်း",
                Isbn = "978-9997102300",
                Price = 8500m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "ပထမအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2020-03-01"),
                PageCount = 240,
                Publisher = "စိတ်ကူးချိုချို စာပေ",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာ့ကျေးလက်တောရွာများတွင် ကာလရှည်ကြာ လက်ဆင့်ကမ်းပြောဆိုလာခဲ့ကြသော ပါးစပ်ရာဇဝင်ပုံပြင်များ၊ တောတောင်ရေမြေ အလှအပများနှင့် ကျေးလက်နေပြည်သူတို့၏ ယုံကြည်ကိုးကွယ်မှု ဓလေ့များကို စုစည်းတင်ပြထားသော အဖိုးတန်စာအုပ်ဖြစ်ပါသည်။",
                EditionNotes = "ရိုးရာဓလေ့ ပုံရိပ်များနှင့် ကလေးလူကြီးမရွေး ဖတ်ရှုနိုင်သော အကြောင်းအရာများ ပါဝင်ပြီး ချောမွေ့သပ်ရပ်စွာ ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ပုဂံမှာကျန်ခဲ့တဲ့ ခြေရာများ",
                Author = "ဒဂုန်တာရာ",
                Genre = "ရသစာတမ်းနှင့် သမိုင်းခံစားမှု",
                Isbn = "978-9997108012",
                Price = 9000m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "အထူးထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2015-08-15"),
                PageCount = 220,
                Publisher = "ဒဂုန်စာပေ",
                CoverImageUrl = "",
                PlotSummary = "ဆရာကြီး ဒဂုန်တာရာ၏ သိမ်မွေ့နူးညံ့လှသော စာအရေးအသားဖြင့် ပုဂံခေတ် ယဉ်ကျေးမှု၊ ရှေးဟောင်းစေတီပုထိုးများ၏ အလှတရားနှင့် သမိုင်းထဲက လူသားတို့၏ ခြေရာများကို ကဗျာဆန်ဆန် ပြန်လည်ခံစားရေးဖွဲ့ထားသည့် ရသစာတမ်းပေါင်းချုပ်ဖြစ်ပါသည်။",
                EditionNotes = "စာပေဝေဖန်ရေးဆရာများ အထူးချီးကျူးကြသော ဆရာကြီးဒဂုန်တာရာ၏ ဂန္ထဝင်လက်ရာဖြစ်ပြီး မူရင်းဓာတ်ပုံအချို့ဖြင့် တန်ဆာဆင်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မြိုင်",
                Author = "ဂျူး",
                Genre = "သဘာဝပတ်ဝန်းကျင်နှင့် ရသဝတ္ထုရှည်",
                Isbn = "978-9997103406",
                Price = 11000m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2018-12-01"),
                PageCount = 340,
                Publisher = "ကံ့ကော်ဝတ်ရည်စာပေ",
                CoverImageUrl = "",
                PlotSummary = "သစ်တောသယံဇာတ ထိန်းသိမ်းရေးနှင့် သဘာဝပတ်ဝန်းကျင် ချစ်မြတ်နိုးမှုကို အခြေခံ၍ လူသားတို့၏ လောဘနှင့် မေတ္တာတရားတို့ အားပြိုင်ပုံကို ရေးဖွဲ့ထားသည့် ဆရာမဂျူး၏ ထူးခြားသော ဝတ္ထုရှည်ဖြစ်ပါသည်။ သစ်တောကြိုးဝိုင်းအတွင်း အမှုထမ်းနေသော သစ်တောအရာရှိများနှင့် ဒေသခံတို့၏ ဘဝကို ကွင်းဆက်မိမိ တင်ပြထားပါသည်။",
                EditionNotes = "သဘာဝအလှ မျက်နှာဖုံးပန်းချီနှင့်အတူ ဆရာမဂျူး၏ သဘာဝထိန်းသိမ်းရေးဆိုင်ရာ အမှာစကားများ ထည့်သွင်းထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ဝေဒနာနှင့် ဘဝခရီးသည်",
                Author = "မောင်သိန်းဆိုင်",
                Genre = "လူမှုဒရာမာ / ရသဝတ္ထု",
                Isbn = "978-9997104618",
                Price = 10000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2016-05-20"),
                PageCount = 320,
                Publisher = "ပန်းဆက်လမ်း စာပေ",
                CoverImageUrl = "",
                PlotSummary = "လူ့လောကတွင် တွေ့ကြုံရတတ်သော ဆင်းရဲဒုက္ခ၊ ဝေဒနာများနှင့် ဘဝကြမ္မာကို မဆုတ်မနစ် ကြံ့ကြံ့ခံ ရင်ဆိုင်ကျော်လွှားခဲ့ရသည့် လူသားတစ်ဦး၏ ဖြစ်စဉ်ကို သရုပ်ဖော်ထားသည့် ဝတ္ထုဖြစ်ပါသည်။ လူတို့၏ ကိုယ်ချင်းစာတရားနှင့် မေတ္တာတရား၏ စွမ်းအားကို ပေါ်လွင်စေပါသည်။",
                EditionNotes = "ဆရာမောင်သိန်းဆိုင်၏ စာဖတ်ပရိသတ်များ နှစ်ခြိုက်ကြသည့် ဝတ္ထုကောင်းဖြစ်ပြီး သပ်ရပ်သော စာမျက်နှာ အပြင်အဆင်ဖြင့် ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မိုးတိမ်တောင်ဆီသို့",
                Author = "ဂျူး",
                Genre = "ခရီးသွားရသနှင့် ဝတ္ထုရှည်",
                Isbn = "978-9997103482",
                Price = 10500m,
                StockQuantity = 19,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2019-07-10"),
                PageCount = 300,
                Publisher = "ဆည်းဆာစာပေ",
                CoverImageUrl = "",
                PlotSummary = "တောင်တန်းဒေသ ခရီးသွားအတွေ့အကြုံများနှင့် သဘာဝတောတောင်ရေမြေ အလှအပများကြားတွင် ဖြစ်ပေါ်လာသော အချစ်ဇာတ်လမ်းတစ်ပုဒ်ကို ရေးဖွဲ့ထားပါသည်။ စိတ်အေးချမ်းမှုနှင့် စွန့်စားမှုတို့ကို တစ်ပြိုင်နက်တည်း ခံစားရစေမည့် ဝတ္ထုဖြစ်ပါသည်။",
                EditionNotes = "စာဖတ်သူအကြိုက် လှပသော တောင်တန်းရှုခင်း မျက်နှာဖုံးနှင့်အတူ အရည်အသွေးကောင်းမွန်သော စက္ကူဖြင့် ပုံနှိပ်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မောင်",
                Author = "ခင်ခင်ထူး",
                Genre = "လူမှုဘဝ / အချစ်ဝတ္ထုရှည်",
                Isbn = "978-9997102454",
                Price = 9500m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2020-08-15"),
                PageCount = 290,
                Publisher = "စိတ်ကူးချိုချို စာပေ",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာအမျိုးသမီးတို့၏ စိတ်နေသဘောထားနှင့် မေတ္တာတရားကြီးမားပုံကို အညာဒေသ ယဉ်ကျေးမှုနောက်ခံဖြင့် သရုပ်ဖော်ထားသည့် ဝတ္ထုကောင်းဖြစ်ပါသည်။ အချစ်နှင့် တာဝန်၊ မာနနှင့် သစ္စာတို့ကြားက အားပြိုင်မှုများကို စိတ်ဝင်စားဖွယ် ဖတ်ရှုရမည်ဖြစ်ပါသည်။",
                EditionNotes = "ဆရာမကြီး ခင်ခင်ထူး၏ နာမည်ကျော် လက်ရာတစ်ခုဖြစ်ပြီး ပရိသတ်တောင်းဆိုမှုအရ ပြန်လည်ပုံနှိပ်ထားခြင်း ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "သခင်ကိုယ်တော်မှိုင်း အတ္ထုပ္ပတ္တိ",
                Author = "လင်းယုန်မောင်မောင်",
                Genre = "သမိုင်းဝင် အတ္ထုပ္ပတ္တိ",
                Isbn = "978-9997109019",
                Price = 12000m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "ရာပြည့် အထူးထုတ်",
                PublishDate = DateTime.Parse("2016-03-23"),
                PageCount = 420,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာ့လွတ်လပ်ရေးနှင့် ငြိမ်းချမ်းရေး ဗိသုကာကြီး ဆရာကြီး သခင်ကိုယ်တော်မှိုင်း၏ မွေးဖွားရာမှ စတင်၍ နိုင်ငံရေး၊ စာပေရေးရာ လှုပ်ရှားဆောင်ရွက်ခဲ့မှုများကို သမိုင်းအထောက်အထား ခိုင်လုံစွာဖြင့် ရေးသားပြုစုထားသော ဂန္ထဝင် အတ္ထုပ္ပတ္တိ စာအုပ်ကြီးဖြစ်ပါသည်။",
                EditionNotes = "ရှားပါးသမိုင်းဝင် မှတ်တမ်းဓာတ်ပုံများ၊ ဆရာကြီး၏ ကဗျာလေးချိုးတိုများနှင့် အတ္ထုပ္ပတ္တိဆိုင်ရာ အချက်အလက်များကို ပြည့်စုံစွာ ထည့်သွင်းထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "အချစ်၏နောက်ဆက်တွဲစာမျက်နှာ",
                Author = "မင်းလူ",
                Genre = "ဟာသနှင့် လူမှုသရုပ်ဖော် ဝတ္ထု",
                Isbn = "978-9997108517",
                Price = 8500m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2017-04-10"),
                PageCount = 230,
                Publisher = "တူဒေးစာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "ဆရာမင်းလူ၏ ထုံးစံအတိုင်း ဟာသရသနှင့် လူမှုဝေဖန်ရေးကို ပေါ့ပေါ့ပါးပါး စကားပြောဟန်ဖြင့် တွဲဖက်ရေးသားထားသည့် ဝတ္ထုဖြစ်ပါသည်။ အချစ်တစ်ခု၏ အစနှင့် အဆုံး၊ အိမ်ထောင်သည်ဘဝ၏ အခက်အခဲနှင့် ရယ်မောဖွယ်ရာများကို လှပစွာ သရုပ်ဖော်ထားပါသည်။",
                EditionNotes = "ဖတ်ရှုရ လွယ်ကူပေါ့ပါးသော အပြင်အဆင်နှင့် ဆရာမင်းလူ၏ အမှတ်တရ လက်ရာတစ်ခုဖြစ်သဖြင့် လူငယ်စာချစ်သူများ လက်မလွှတ်သင့်သော စာအုပ်ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ရာဇဝင်ထဲမှာ မောင့်ကိုရှာ",
                Author = "မင်းလူ",
                Genre = "ခေတ်ပေါ်ရသနှင့် ဟာသဒရာမာ",
                Isbn = "978-9997108524",
                Price = 9000m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "စတုတ္ထအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2018-02-14"),
                PageCount = 260,
                Publisher = "တူဒေးစာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "သမိုင်းနှင့် ရာဇဝင်ထဲက အလွမ်းပုံရိပ်များကို ခေတ်သစ်လူငယ်တို့၏ အချစ်နှင့် ပေါင်းစပ်၍ ဟာသသဘောရောနှောကာ ရေးဖွဲ့ထားသည့် ဆရာမင်းလူ၏ လူကြိုက်များသော ဝတ္ထုတစ်ပုဒ်ဖြစ်ပါသည်။ အချစ်၏ အဓိပ္ပာယ်ကို ထူးခြားသော ရှုထောင့်မှ ချဉ်းကပ်တင်ပြထားပါသည်။",
                EditionNotes = "လူကြိုက်များခဲ့သော ရုပ်ရှင်ဇာတ်ကားအဖြစ်လည်း ဖန်တီးခဲ့ဖူးသည့် နာမည်ကျော်စာအုပ်ဖြစ်ပြီး အသစ်တဖန် ပြန်လည်ပုံနှိပ်ထားခြင်း ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ကျောင်းသားဘဝ ချစ်သောရက်စွဲများ",
                Author = "မင်းလူ",
                Genre = "ကျောင်းသားဘဝ ရသဝတ္ထု",
                Isbn = "978-9997108555",
                Price = 8500m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "ပဉ္စမအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2019-06-20"),
                PageCount = 220,
                Publisher = "တူဒေးစာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "တက္ကသိုလ်ကျောင်းသားဘဝ၏ ပျော်ရွှင်ဖွယ်ရာများ၊ သူငယ်ချင်းများအကြား ဟာသတိုက်ပွဲများနှင့် မေ့မရနိုင်သော စာသင်ခန်းတွင်း အမှတ်တရများကို ရင်ထဲနွေးထွေးသွားစေမည့် ဟန်ပန်ဖြင့် ရေးသားထားသော ဝတ္ထုတိုနှင့် အက်ဆေးများဖြစ်ပါသည်။",
                EditionNotes = "ကျောင်းသားလူငယ်တိုင်း နှစ်သက်သဘောကျသည့် ဆရာမင်းလူ၏ အကောင်းဆုံးလက်ရာများထဲမှ တစ်ခုဖြစ်ပြီး သေသပ်စွာ ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "အလွမ်းဧကရီ",
                Author = "မင်းလူ",
                Genre = "ရသဝတ္ထုရှည်",
                Isbn = "978-9997108586",
                Price = 9000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2018-10-12"),
                PageCount = 250,
                Publisher = "တူဒေးစာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "မာနကြီးပြီး ထူးခြားလှပသော အမျိုးသမီးတစ်ဦးနှင့် သူမအပေါ် သစ္စာရှိစွာ ချစ်မြတ်နိုးခဲ့သော လူငယ်တစ်ဦးတို့၏ အချစ်နှင့် အလွမ်းဇာတ်လမ်းကို စိတ်ဝင်စားဖွယ် တင်ဆက်ထားသည့် ဝတ္ထုကောင်းဖြစ်ပါသည်။",
                EditionNotes = "စာဖတ်သူတို့ အကြိုက်တွေ့စေမည့် စာအုပ်ဒီဇိုင်းနှင့် အရည်အသွေးမြင့် စက္ကူအသုံးပြုကာ စနစ်တကျ ပြန်လည်ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ကျွန်တော်နှင့် စာရေးဆရာများ",
                Author = "မင်းလူ",
                Genre = "အမှတ်တရ ရသစာတမ်း",
                Isbn = "978-9997108609",
                Price = 9500m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2017-09-01"),
                PageCount = 270,
                Publisher = "လောကသစ်စာပေ",
                CoverImageUrl = "",
                PlotSummary = "ဆရာမင်းလူ ဖြတ်သန်းခဲ့ရသော စာပေလောကခရီးနှင့် ခေတ်ပြိုင်စာရေးဆရာကြီးများဖြစ်သည့် ဦးသော်တာဆွေ၊ မောင်သာရ၊ ဒဂုန်တာရာ စသော ပုဂ္ဂိုလ်ကြီးများနှင့် ပတ်သက်သည့် ရယ်မောဖွယ်၊ လွမ်းမောဖွယ် အမှတ်တရများကို ရေးဖွဲ့ထားသည့် စာအုပ်ဖြစ်ပါသည်။",
                EditionNotes = "မြန်မာစာပေသမိုင်းနှင့် စာရေးဆရာတို့၏ အတွင်းရေးဘဝများကို သိရှိလိုသူများအတွက် အထူးသင့်လျော်သည့် စာအုပ်ကောင်းဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ငွေတောင်တက်တဲ့ ခြေရာများ",
                Author = "ချစ်စံဝင်း",
                Genre = "သမိုင်းနှင့် ခရီးသွားမှတ်တမ်း",
                Isbn = "978-9997109507",
                Price = 10000m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2016-01-25"),
                PageCount = 310,
                Publisher = "ရွှေမင်းသမီးစာပေ",
                CoverImageUrl = "",
                PlotSummary = "ကယားပြည်နယ်နှင့် ရှမ်းပြည်နယ်တောင်ပိုင်းရှိ တိုင်းရင်းသားလူမျိုးတို့၏ ရိုးရာယဉ်ကျေးမှု၊ ငွေတောင်ဒေသ သမိုင်းကြောင်းနှင့် သဘာဝအလှအပများကို ကွင်းဆင်းလေ့လာ မှတ်တမ်းတင်ထားသော ခရီးသွားရသ စာအုပ်ကောင်းဖြစ်ပါသည်။",
                EditionNotes = "တိုင်းရင်းသားရိုးရာ ဓလေ့ထုံးတမ်း ဓာတ်ပုံမှတ်တမ်းများ ပါဝင်ပြီး သမိုင်းနှင့် ခရီးသွားဝါသနာရှင်များအတွက် တန်ဖိုးရှိသော စာအုပ်ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "လရောင်အောက်က သံလွင်မြစ်",
                Author = "ချစ်စံဝင်း",
                Genre = "သမိုင်းနောက်ခံ ဝတ္ထုရှည်",
                Isbn = "978-9997109521",
                Price = 11000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2018-07-15"),
                PageCount = 350,
                Publisher = "ရွှေမင်းသမီးစာပေ",
                CoverImageUrl = "",
                PlotSummary = "သံလွင်မြစ်ရိုးတစ်လျှောက် နေထိုင်ကြသော တိုင်းရင်းသား ညီအစ်ကိုများ၏ သမိုင်းကြောင်း၊ ချစ်ကြည်ရင်းနှီးမှုနှင့် သဘာဝမြစ်ကြီး၏ ဂယက်ရိုက်ခတ်မှုများကို အခြေခံ၍ ရေးသားထားသော သမိုင်းနောက်ခံ ဝတ္ထုကောင်းဖြစ်ပါသည်။",
                EditionNotes = "သံလွင်မြစ်အလှ မျက်နှာဖုံးနှင့်အတူ စာလုံးပေါင်းသတ်ပုံ တိကျမှန်ကန်စွာဖြင့် စီစဉ်ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ရွှေပြည်ကြီး မိုးချုပ်လေပြီ",
                Author = "တက်တိုး",
                Genre = "သမိုင်းဝင် ဘဝသရုပ်ဖော်",
                Isbn = "978-9997106506",
                Price = 10500m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "ရာပြည့် ဂန္ထဝင်ထုတ်",
                PublishDate = DateTime.Parse("2015-10-10"),
                PageCount = 300,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "ကိုလိုနီခေတ် မြန်မာ့လူနေမှုဘဝနှင့် နိုင်ငံရေးအပြောင်းအလဲများကို မျက်ဝါးထင်ထင် တွေ့ကြုံခဲ့ရသော ဆရာကြီးတက်တိုး၏ သမိုင်းဝင် ရသစာအုပ်ဖြစ်ပါသည်။ မြန်မာပြည်သူတို့၏ ဘဝရပ်တည်မှုနှင့် ခေတ်စနစ်၏ ရိုက်ခတ်မှုများကို ပီပြင်စွာ ဖွင့်ဆိုထားပါသည်။",
                EditionNotes = "ဆရာကြီးတက်တိုး၏ ဂန္ထဝင်လက်ရာတစ်ခုဖြစ်ပြီး သမိုင်းလေ့လာသူများနှင့် စာချစ်သူများအတွက် မရှိမဖြစ် စာအုပ်ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ဘဝစာမျက်နှာ",
                Author = "ရွှေဥဒေါင်း",
                Genre = "ကိုယ်တိုင်ရေး အတ္ထုပ္ပတ္တိ",
                Isbn = "978-9997106803",
                Price = 13000m,
                StockQuantity = 14,
                ReorderLevel = 5,
                EditionName = "အပြည့်အစုံထုတ် (ဂန္ထဝင်ထုတ်)",
                PublishDate = DateTime.Parse("2017-06-18"),
                PageCount = 520,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာစာပေလောက၏ မဟူရာသရဖူကြီး ဆရာကြီးရွှေဥဒေါင်း၏ ဘဝတစ်လျှောက်လုံး ဖြတ်သန်းခဲ့ရသော အတက်အကျ အနိမ့်အမြင့်များကို ပွင့်လင်းရိုးသားစွာ ရေးဖွဲ့ထားသည့် မြန်မာ့စာပေသမိုင်းတွင် အထင်ရှားဆုံး ကိုယ်တိုင်ရေး အတ္ထုပ္ပတ္တိကြီးဖြစ်ပါသည်။",
                EditionNotes = "ဆရာကြီး၏ ဘဝမှတ်တမ်း ရှားပါးဓာတ်ပုံများနှင့်အတူ စာမျက်နှာ ၅၀၀ ကျော်ကို ခိုင်ခံ့သော စာအုပ်ချုပ်စနစ်ဖြင့် ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "စုံထောက်ဦးစံရှား ပေါင်းချုပ်",
                Author = "ရွှေဥဒေါင်း",
                Genre = "စုံထောက်ဝတ္ထုတို ပေါင်းချုပ်",
                Isbn = "978-9997106827",
                Price = 14000m,
                StockQuantity = 20,
                ReorderLevel = 5,
                EditionName = "ဂန္ထဝင်ပေါင်းချုပ် အထူးထုတ်",
                PublishDate = DateTime.Parse("2018-04-05"),
                PageCount = 560,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "မြန်မာ့စုံထောက်ဝတ္ထုလောကတွင် အထင်ရှားဆုံးနှင့် အစောဆုံးဖြစ်သည့် စုံထောက်ကြီး ဦးစံရှား၏ အမှုလိုက် မှတ်တမ်းဝတ္ထုများကို စုစည်းထားသည့် စာအုပ်ကြီးဖြစ်ပါသည်။ ဉာဏ်ပညာထက်မြက်မှု၊ ဆင်ခြင်တုံတရားနှင့် စိတ်လှုပ်ရှားဖွယ်ရာ အမှုများကို စိတ်ဝင်စားဖွယ် ဖတ်ရှုရမည်ဖြစ်ပါသည်။",
                EditionNotes = "ဦးစံရှားဝတ္ထုတိုများ အားလုံးနီးပါးကို ပြည့်စုံစွာ စုစည်းထားပြီး အမာဖုံးဒီဇိုင်းသစ်ဖြင့် တန်ဖိုးထား သိမ်းဆည်းရန် ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "သွေးစွန်းသော ချိုင့်ဝှမ်း",
                Author = "ရွှေဥဒေါင်း",
                Genre = "စုံထောက်ဝတ္ထုရှည်",
                Isbn = "978-9997106841",
                Price = 9500m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2016-08-20"),
                PageCount = 260,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "ဆာအာသာကိုနန်ဒွိုင်း၏ ရှာလော့ဟုမ်းစ် ဝတ္ထုကို ဆရာကြီးရွှေဥဒေါင်းက မြန်မာမှုပြုထားသည့် အထူးအောင်မြင်သော စုံထောက်ဝတ္ထုရှည်ဖြစ်ပါသည်။ လျှို့ဝှက်ဆန်းကြယ်သော လူသတ်မှုနှင့် လျှို့ဝှက်အသင်းအဖွဲ့တို့၏ အန္တရာယ်ကို ဖော်ထုတ်ပုံကို စိတ်လှုပ်ရှားဖွယ် တွေ့ရမည်ဖြစ်ပါသည်။",
                EditionNotes = "မြန်မာမှုပြု စုံထောက်စာပေ၏ စံပြလက်ရာဖြစ်ပြီး စာဖတ်သူတို့ အကြိုက်တွေ့စေမည့် စာအုပ်ဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မိန်းမလှကျွန်း",
                Author = "ရွှေဥဒေါင်း",
                Genre = "စွန့်စားခန်းနှင့် စုံထောက်ဝတ္ထု",
                Isbn = "978-9997106865",
                Price = 9000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2017-12-15"),
                PageCount = 240,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "ပင်လယ်ပြင် စွန့်စားခန်းနှင့် လျှို့ဝှက်ဆန်းကြယ်သော ကျွန်းတစ်ကျွန်းပေါ်က အဖြစ်အပျက်များကို စိတ်လှုပ်ရှားဖွယ် ရေးဖွဲ့ထားသည့် ဆရာကြီးရွှေဥဒေါင်း၏ နာမည်ကျော် ဝတ္ထုကောင်းဖြစ်ပါသည်။",
                EditionNotes = "စာဖတ်သူများ လက်လှမ်းမီလွယ်စေရန် ချိုသာသော ဈေးနှုန်းဖြင့် သေသပ်စွာ ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "စံပယ်ဖြူတို့ လွင့်လွင့်ဝေ",
                Author = "နုနုရည်(အင်းဝ)",
                Genre = "လူမှုဘဝနှင့် မိန်းမသားသရုပ်ဖော်",
                Isbn = "978-9997107510",
                Price = 9500m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2019-03-10"),
                PageCount = 280,
                Publisher = "ပုဂံစာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "အင်းဝဒေသ ကျေးလက်လူနေမှုဘဝနှင့် မိန်းမသားတို့၏ ဘဝကြမ္မာကို စံပယ်ဖြူပန်းလေးများနှင့် နှိုင်းယှဉ်ကာ ရေးဖွဲ့ထားသော ဆရာမ နုနုရည်(အင်းဝ) ၏ နာမည်ကျော် ဝတ္ထုကောင်းဖြစ်ပါသည်။ ရိုးသားဖြူစင်မှုနှင့် လောကဓံ၏ ရက်စက်မှုကြားက ရုန်းကန်ရပုံကို ပီပြင်စွာ တွေ့ရမည်ဖြစ်ပါသည်။",
                EditionNotes = "ဆရာမ နုနုရည်(အင်းဝ)၏ အထင်ကရ လက်ရာတစ်ခုဖြစ်ပြီး ပရိသတ်တောင်းဆိုမှုအရ ပြန်လည်ရိုက်နှိပ်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "အပြာရောင်ညနေခင်း",
                Author = "နုနုရည်(အင်းဝ)",
                Genre = "ရသဝတ္ထုတို ပေါင်းချုပ်",
                Isbn = "978-9997107534",
                Price = 8500m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2018-08-01"),
                PageCount = 220,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "လူလတ်တန်းစားတို့၏ ရုန်းကန်မှု၊ အိမ်ထောင်ရေးနှင့် လူမှုပတ်ဝန်းကျင်၏ သိမ်မွေ့နက်နဲသော စိတ်လှုပ်ရှားမှုများကို ဝတ္ထုတိုများအဖြစ် စုစည်းထားသော စာအုပ်ကောင်းဖြစ်ပါသည်။ ညနေခင်းတစ်ခု၏ အေးချမ်းမှုနှင့် လွမ်းမောဖွယ် ခံစားချက်များကို ရရှိစေပါသည်။",
                EditionNotes = "ဆရာမကြီး၏ ဝတ္ထုတိုလက်ရာကောင်းများကို စုစည်းထားပြီး သေသပ်သော အပြင်အဆင်ဖြင့် ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ကမ္ဘာ့အေးအေးလေး",
                Author = "နုနုရည်(အင်းဝ)",
                Genre = "ရသဝတ္ထုရှည်",
                Isbn = "978-9997107558",
                Price = 10000m,
                StockQuantity = 15,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2017-05-15"),
                PageCount = 310,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "အေးချမ်းရိုးစင်းသော ဘဝတစ်ခုကို ပိုင်ဆိုင်လိုသည့် လူသားတို့၏ ဆန္ဒနှင့် ခေတ်ကာလ၏ အပြောင်းအလဲများကြား ရုန်းကန်ရပုံကို ဖွဲ့ဆိုထားသည့် ဝတ္ထုဖြစ်ပါသည်။ လူတို့၏ မေတ္တာနှင့် ကရုဏာတရားကို အဓိက ဖော်ညွှန်းထားပါသည်။",
                EditionNotes = "စာဖတ်သူတို့၏ စိတ်နှလုံးကို အေးချမ်းစေမည့် အရေးအသားဖြင့် စီစဉ်ထားပြီး သပ်ရပ်စွာ ပုံနှိပ်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "မြစိမ်းပြာရောင် ပင်လယ်နှင့် မုန်တိုင်း",
                Author = "ဂျူး",
                Genre = "ရသဝတ္ထုရှည်",
                Isbn = "978-9997103550",
                Price = 11500m,
                StockQuantity = 18,
                ReorderLevel = 5,
                EditionName = "စတုတ္ထအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2019-09-25"),
                PageCount = 360,
                Publisher = "ဆည်းဆာစာပေ",
                CoverImageUrl = "",
                PlotSummary = "ပင်လယ်ကမ်းခြေဒေသ၏ သဘာဝအလှနှင့် မုန်တိုင်းထန်သော ပင်လယ်ပြင်ကဲ့သို့ လှိုင်းထန်လှသည့် အချစ်၊ အမုန်း၊ မာနတို့၏ ပဋိပက္ခကို ဇာတ်လမ်းဆင်ထားသော ဆရာမဂျူး၏ နာမည်ကျော် ဝတ္ထုဖြစ်ပါသည်။",
                EditionNotes = "ဆရာမဂျူး၏ ပရိသတ်အကြိုက်ဆုံး ဝတ္ထုများတွင် တစ်ခုအပါအဝင်ဖြစ်ပြီး မျက်နှာဖုံးအလှဖြင့် ပြန်လည်ထုတ်ဝေထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "နွေဦးပုံပြင်",
                Author = "နုနုရည်(အင်းဝ)",
                Genre = "အချစ်နှင့် လူမှုဘဝ",
                Isbn = "978-9997107589",
                Price = 9000m,
                StockQuantity = 17,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2018-01-20"),
                PageCount = 240,
                Publisher = "ရာပြည့်စာအုပ်တိုက်",
                CoverImageUrl = "",
                PlotSummary = "နွေဦးရာသီ၏ လန်းဆန်းမှုနှင့်အတူ ရောက်ရှိလာတတ်သော အချစ်သစ်၊ မျှော်လင့်ချက်သစ်များနှင့် ကျေးလက်လူငယ်တို့၏ စိတ်ကူးအိပ်မက်များကို သရုပ်ဖော်ထားသည့် နူးညံ့သိမ်မွေ့သော ဝတ္ထုဖြစ်ပါသည်။",
                EditionNotes = "နွေဦးအလှ မျက်နှာဖုံးနှင့်အတူ စာလုံးအရွယ်အစား မျှတစွာဖြင့် ဖတ်ရှုရ လွယ်ကူစေရန် စီစဉ်ထားပါသည်။"
            },
            new BookSeedItem
            {
                Title = "ပန်းစကား",
                Author = "မင်းလူ",
                Genre = "ရသဝတ္ထုတို ပေါင်းချုပ်",
                Isbn = "978-9997108654",
                Price = 8500m,
                StockQuantity = 16,
                ReorderLevel = 5,
                EditionName = "ဒုတိယအကြိမ် ပုံနှိပ်ခြင်း",
                PublishDate = DateTime.Parse("2019-04-18"),
                PageCount = 210,
                Publisher = "လောကသစ်စာပေ",
                CoverImageUrl = "",
                PlotSummary = "ပန်းလေးများ၏ အလှတရားနှင့် သဘာဝကို လူသားတို့၏ အချစ်၊ သံယောဇဉ်၊ မေတ္တာတရားတို့နှင့် နှိုင်းယှဉ်၍ ဆရာမင်းလူက သိမ်မွေ့နက်နဲစွာ ရေးဖွဲ့ထားသော ရသဝတ္ထုတိုများဖြစ်ပါသည်။",
                EditionNotes = "ဆရာမင်းလူ၏ ပေါ့ပါးသွက်လက်သော အရေးအသားကို နှစ်သက်သူများအတွက် အထူးလက်ဆောင် စာအုပ်ကောင်းဖြစ်ပါသည်။"
            },
            new BookSeedItem
            {
                Title = "အလင်းတန်းဆီသို့",
                Author = "ဂျူး",
                Genre = "ရသဝတ္ထုရှည်",
                Isbn = "978-9997103604",
                Price = 10500m,
                StockQuantity = 19,
                ReorderLevel = 5,
                EditionName = "တတိယအကြိမ် ထုတ်ဝေခြင်း",
                PublishDate = DateTime.Parse("2020-02-10"),
                PageCount = 310,
                Publisher = "ကံ့ကော်ဝတ်ရည်စာပေ",
                CoverImageUrl = "",
                PlotSummary = "မှောင်မိုက်သော ဘဝအခက်အခဲများနှင့် စိတ်ဒဏ်ရာများကို ဖြတ်သန်းကာ အလင်းရောင်ရှိရာ ပန်းတိုင်ဆီသို့ သတ္တိရှိရှိ လျှောက်လှမ်းခဲ့ကြသည့် လူငယ်တို့၏ ဘဝခရီးကို စိတ်ဓာတ်ခွန်အားဖြစ်ဖွယ် တင်ဆက်ထားသည့် ဝတ္ထုကောင်းဖြစ်ပါသည်။",
                EditionNotes = "ဆရာမဂျူး၏ စာချစ်သူများ လက်မလွှတ်တမ်း ဝယ်ယူဖတ်ရှုကြသော စာအုပ်ဖြစ်ပြီး စက္ကူချောဖြင့် သေသပ်စွာ ထုတ်ဝေထားပါသည်။"
            },
    };

    public static async Task<bool> EnsureBookDataAsync(IServiceProvider serviceProvider, bool forceReset = false)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("BookDataSeeder");
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            int currentEditionCount = await db.TblBookEditions.CountAsync(e => !e.IsDeleted);
            // If already seeded with all editions and not forceReset, skip
            if (currentEditionCount >= TargetBooks.Count && !forceReset)
            {
                logger.LogInformation("BookDataSeeder => Database already contains {Count} editions, skipping seeding.", currentEditionCount);
                return true;
            }

            logger.LogInformation("BookDataSeeder => Seeding complete 112 inventory book editions...");

            // 1. Soft-delete all existing books and book editions
            var existingBooks = await db.TblBooks.Where(b => !b.IsDeleted).ToListAsync();
            foreach (var b in existingBooks)
            {
                b.IsDeleted = true;
                b.UpdatedAt = DateTime.Now;
            }

            var existingEditions = await db.TblBookEditions.Where(e => !e.IsDeleted).ToListAsync();
            foreach (var e in existingEditions)
            {
                e.IsDeleted = true;
                e.UpdatedAt = DateTime.Now;
            }

            await db.SaveChangesAsync();

            // 2. Cache authors, genres, and editions
            var authorCache = new Dictionary<string, TblAuthor>(StringComparer.OrdinalIgnoreCase);
            var genreCache = new Dictionary<string, TblGenre>(StringComparer.OrdinalIgnoreCase);
            var editionCache = new Dictionary<string, TblEdition>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in TargetBooks)
            {
                // Author
                if (!authorCache.ContainsKey(item.Author))
                {
                    var a = await db.TblAuthors.FirstOrDefaultAsync(x => x.AuthorName == item.Author && !x.IsDeleted);
                    if (a == null)
                    {
                        a = new TblAuthor
                        {
                            AuthorName = item.Author,
                            IsDeleted = false,
                            CreateAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        db.TblAuthors.Add(a);
                        await db.SaveChangesAsync();
                    }
                    authorCache[item.Author] = a;
                }

                // Genre
                if (!genreCache.ContainsKey(item.Genre))
                {
                    var g = await db.TblGenres.FirstOrDefaultAsync(x => x.GenreName == item.Genre && !x.IsDeleted);
                    if (g == null)
                    {
                        g = new TblGenre
                        {
                            GenreName = item.Genre,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        db.TblGenres.Add(g);
                        await db.SaveChangesAsync();
                    }
                    genreCache[item.Genre] = g;
                }

                // Edition Type
                string editionName = item.EditionName.Length > 50 ? item.EditionName[..50] : item.EditionName;
                if (!editionCache.ContainsKey(editionName))
                {
                    var ed = await db.TblEditions.FirstOrDefaultAsync(x => x.EditionName == editionName && !x.IsDeleted);
                    if (ed == null)
                    {
                        ed = new TblEdition
                        {
                            EditionName = editionName,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        db.TblEditions.Add(ed);
                        await db.SaveChangesAsync();
                    }
                    editionCache[editionName] = ed;
                }

                // 3. Create Book or reuse if already exists (for titles with multiple editions)
                var book = await db.TblBooks.FirstOrDefaultAsync(b => b.Title == item.Title && !b.IsDeleted);
                if (book == null)
                {
                    book = new TblBook
                    {
                        Title = item.Title,
                        Description = item.PlotSummary,
                        AuthorId = authorCache[item.Author].AuthorId,
                        GenreId = genreCache[item.Genre].GenreId,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    db.TblBooks.Add(book);
                    await db.SaveChangesAsync();
                }

                // 4. Create Book Edition
                var bookEdition = new TblBookEdition
                {
                    BookId = book.BookId,
                    EditionId = editionCache[editionName].EditionId,
                    Isbn = item.Isbn,
                    Price = item.Price,
                    StockQuantity = item.StockQuantity,
                    ReorderLevel = item.ReorderLevel,
                    PublishDate = item.PublishDate,
                    PageCount = item.PageCount,
                    Publisher = item.Publisher,
                    EditionNote = item.EditionNotes,
                    CoverImageUrl = item.CoverImageUrl,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.TblBookEditions.Add(bookEdition);
                await db.SaveChangesAsync();
            }

            int totalBooksSeeded = await db.TblBooks.CountAsync(b => !b.IsDeleted);
            int totalEditionsSeeded = await db.TblBookEditions.CountAsync(e => !e.IsDeleted);
            logger.LogInformation("BookDataSeeder => Successfully seeded {Books} books with {Editions} editions!", totalBooksSeeded, totalEditionsSeeded);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "BookDataSeeder => Error seeding curated books: {Message}", ex.Message);
            return false;
        }
    }
}
