using System;
using System.Collections.Generic;
using System.Text;

namespace Otis.Sample.Domain
{
	class MusicService : IMusicService 
	{
		public Artist[] GetAllArtists()
		{
			List<Artist> artists = new List<Artist>();
			artists.Add(new Artist(1, "Fugazi", "USA"));
			artists.Add(new Artist(2, "Pixies", "USA"));
			artists.Add(new Artist(3, "P.J. Harvey", "UK"));

			// fugazi - killtaker
			artists[0].Records.Add(new Record(10, "In on the killtaker", 1993));
			artists[0].Records[0].Songs.Add(new Song("Facet Squared", 2 * 60 + 41));
			artists[0].Records[0].Songs.Add(new Song("Public Witness Program", 2 * 60 + 04));
			artists[0].Records[0].Songs.Add(new Song("Returning the Screw", 3 * 60 + 14));
			artists[0].Records[0].Songs.Add(new Song("Smallpox Champion", 4 * 60 + 00));
			artists[0].Records[0].Songs.Add(new Song("Rend It", 3 * 60 + 47));
			artists[0].Records[0].Songs.Add(new Song("23 Beats Off", 6 * 60 + 42));
			artists[0].Records[0].Songs.Add(new Song("Sweet and Low", 3 * 60 + 34));
			artists[0].Records[0].Songs.Add(new Song("Cassavetes", 2 * 60 + 30));
			artists[0].Records[0].Songs.Add(new Song("Great Cop", 1 * 60 + 52));
			artists[0].Records[0].Songs.Add(new Song("Walken's Syndrome", 3 * 60 + 17));
			artists[0].Records[0].Songs.Add(new Song("Instrument", 3 * 60 + 44));
			artists[0].Records[0].Songs.Add(new Song("Last Chance for a Slow Dance", 4 * 60 + 37));

			artists[0].Records[0].Songs[3].Comments.Add(new Comment("me", "great song"));
			artists[0].Records[0].Songs[3].Ratings.Add(new Rating(5));
			artists[0].Records[0].Songs[3].Ratings.Add(new Rating(5));
			artists[0].Records[0].Songs[9].Ratings.Add(new Rating(5));
			artists[0].Records[0].Songs[11].Ratings.Add(new Rating(4));

			// fugazi - red medicine
			artists[0].Records.Add(new Record(11, "Red medicine", 1995));
			artists[0].Records[1].Songs.Add(new Song("Do You Like Me",  3 * 60 + 16));
			artists[0].Records[1].Songs.Add(new Song("Bed for the Scraping",  2 * 60 + 50));
			artists[0].Records[1].Songs.Add(new Song("Latest Disgrace",  3 * 60 + 34));
			artists[0].Records[1].Songs.Add(new Song("Birthday Pony",  3 * 60 + 08));
			artists[0].Records[1].Songs.Add(new Song("Forensic Scene",  3 * 60 + 05));
			artists[0].Records[1].Songs.Add(new Song("Combination Lock",  3 * 60 + 06));
			artists[0].Records[1].Songs.Add(new Song("Fell, Destroyed", 3 * 60 + 46));
			artists[0].Records[1].Songs.Add(new Song("By You", 5 * 60 + 11));
			artists[0].Records[1].Songs.Add(new Song("Version", 3 * 60 + 20));
			artists[0].Records[1].Songs.Add(new Song("Target", 3 * 60 + 32));
			artists[0].Records[1].Songs.Add(new Song("Back To Base", 1 * 60 + 45));
			artists[0].Records[1].Songs.Add(new Song("Downed City", 2 * 60 + 53));
			artists[0].Records[1].Songs.Add(new Song("Long Distance Runner", 4 * 60 + 17));

			artists[0].Records[1].Songs[0].Comments.Add(new Comment("me", "a great intro"));
			artists[0].Records[1].Songs[1].Comments.Add(new Comment("me", "this one rocks!!"));
			artists[0].Records[1].Songs[1].Comments.Add(new Comment("me", "instumental"));
			artists[0].Records[1].Songs[1].Ratings.Add(new Rating(5));
			artists[0].Records[1].Songs[0].Ratings.Add(new Rating(5));
			artists[0].Records[1].Songs[8].Ratings.Add(new Rating(4));
			artists[0].Records[1].Songs[12].Ratings.Add(new Rating(4));
			artists[0].Records[1].Style = "Punk/Rock";


			// pixies - surfer rosa
			artists[1].Records.Add(new Record(12, "Surfer rosa", 1988));
			artists[1].Records[0].Songs.Add(new Song("Bone Machine", 3 * 60 + 03));
			artists[1].Records[0].Songs.Add(new Song("Break My Body", 2 * 60 + 05));
			artists[1].Records[0].Songs.Add(new Song("Something Against You", 1 * 60 + 47));
			artists[1].Records[0].Songs.Add(new Song("Broken Face", 1 * 60 + 30));
			artists[1].Records[0].Songs.Add(new Song("Gigantic", 3 * 60 + 54));
			artists[1].Records[0].Songs.Add(new Song("River Euphrates", 2 * 60 + 31));
			artists[1].Records[0].Songs.Add(new Song("Where Is My Mind", 3 * 60 + 53));
			artists[1].Records[0].Songs.Add(new Song("Cactus", 2 * 60 + 16));
			artists[1].Records[0].Songs.Add(new Song("Tony`s Theme", 1 * 60 + 52));
			artists[1].Records[0].Songs.Add(new Song("Oh My Golly", 1 * 60 + 47));
			artists[1].Records[0].Songs.Add(new Song("Interlude", 0 * 60 + 45));
			artists[1].Records[0].Songs.Add(new Song("Vamos", 4 * 60 + 20));
			artists[1].Records[0].Songs.Add(new Song("I`m Amazed", 1 * 60 + 42));
			artists[1].Records[0].Songs.Add(new Song("Brick Is Red", 2 * 60 + 00));

			artists[1].Records[0].Songs[1].Ratings.Add(new Rating(5));
			artists[1].Records[0].Songs[0].Ratings.Add(new Rating(5));
			artists[1].Records[0].Songs[9].Ratings.Add(new Rating(5));
			artists[1].Records[0].Songs[10].Ratings.Add(new Rating(3));
			artists[1].Records[0].Songs[11].Ratings.Add(new Rating(4));


			// pixies - doolittle
			artists[1].Records.Add(new Record(13, "Doolittle", 1990));
			artists[1].Records[1].Songs.Add(new Song("Debaser", 2 * 60 + 52));
			artists[1].Records[1].Songs.Add(new Song("Tame", 1 * 60 + 55));
			artists[1].Records[1].Songs.Add(new Song("Wave Of Mutilation", 2 * 60 + 04));
			artists[1].Records[1].Songs.Add(new Song("I Bleed", 2 * 60 + 34));
			artists[1].Records[1].Songs.Add(new Song("Here Comes Your Man", 3 * 60 + 21));
			artists[1].Records[1].Songs.Add(new Song("Dead", 2 * 60 + 21));
			artists[1].Records[1].Songs.Add(new Song("Monkey Gone To Heaven", 2 * 60 + 56));
			artists[1].Records[1].Songs.Add(new Song("Mr. Grieves", 2 * 60 + 05));
			artists[1].Records[1].Songs.Add(new Song("Crackity Jones", 1 * 60 + 24));
			artists[1].Records[1].Songs.Add(new Song("La La Love You", 2 * 60 + 43));
			artists[1].Records[1].Songs.Add(new Song("No. 13 Baby", 3 * 60 + 51));
			artists[1].Records[1].Songs.Add(new Song("There Goes My Gun", 1 * 60 + 49));
			artists[1].Records[1].Songs.Add(new Song("Hey", 3 * 60 + 31));
			artists[1].Records[1].Songs.Add(new Song("Silver", 2 * 60 + 25));
			artists[1].Records[1].Songs.Add(new Song("Gouge Away", 2 * 60 + 44));

			artists[1].Records[0].Style = "Pop";

			artists[1].Records[1].Songs[4].Comments.Add(new Comment("", "too cheesy"));
			artists[1].Records[1].Songs[4].Ratings.Add(new Rating(3));
			artists[1].Records[1].Songs[0].Ratings.Add(new Rating(5));
			artists[1].Records[1].Songs[1].Ratings.Add(new Rating(5));

			// p.j. harvey   - dry
			artists[2].Records.Add(new Record(14, "Dry", 1992));
			artists[2].Records[0].Songs.Add(new Song("Oh My Lover", 4 * 60 + 02));
			artists[2].Records[0].Songs.Add(new Song("O Stella", 2 * 60 + 30));
			artists[2].Records[0].Songs.Add(new Song("Dress", 3 * 60 + 18));
			artists[2].Records[0].Songs.Add(new Song("Victory", 3 * 60 + 15));
			artists[2].Records[0].Songs.Add(new Song("Happy And Bleeding", 4 * 60 + 51));
			artists[2].Records[0].Songs.Add(new Song("Sheela-Na-Gig", 3 * 60 + 10));
			artists[2].Records[0].Songs.Add(new Song("Hair", 3 * 60 + 47));
			artists[2].Records[0].Songs.Add(new Song("Joe", 2 * 60 + 33));
			artists[2].Records[0].Songs.Add(new Song("Plants And Rags", 4 * 60 + 09));
			artists[2].Records[0].Songs.Add(new Song("Fountain", 3 * 60 + 53));
			artists[2].Records[0].Songs.Add(new Song("Water", 4 * 60 + 33));

			artists[2].Records[0].Songs[1].Ratings.Add(new Rating(5));
			artists[2].Records[0].Songs[6].Ratings.Add(new Rating(4));
			artists[2].Records[0].Songs[9].Ratings.Add(new Rating(5));

			// p.j. harvey   - rid of me
			artists[2].Records.Add(new Record(14, "Rid of me", 1993));
			artists[2].Records[1].Songs.Add(new Song("Rid Of Me", 4 * 60 + 28));
			artists[2].Records[1].Songs.Add(new Song("Missed", 4 * 60 + 25));
			artists[2].Records[1].Songs.Add(new Song("Legs", 3 * 60 + 40));
			artists[2].Records[1].Songs.Add(new Song("Rub 'Till It Bleeds", 5 * 60 + 03));
			artists[2].Records[1].Songs.Add(new Song("Hook", 3 * 60 + 57));
			artists[2].Records[1].Songs.Add(new Song("Man-Size", 2 * 60 + 18));
			artists[2].Records[1].Songs.Add(new Song("Highway '61 Revisited", 2 * 60 + 57));
			artists[2].Records[1].Songs.Add(new Song("50 Ft Queenie", 2 * 60 + 23));
			artists[2].Records[1].Songs.Add(new Song("Yuri-G", 3 * 60 + 28));
			artists[2].Records[1].Songs.Add(new Song("Man-Size", 3 * 60 + 16));
			artists[2].Records[1].Songs.Add(new Song("Dry", 3 * 60 + 23));
			artists[2].Records[1].Songs.Add(new Song("Me-Jane", 2 * 60 + 42));
			artists[2].Records[1].Songs.Add(new Song("Snake", 1 * 60 + 36));
			artists[2].Records[1].Songs.Add(new Song("Ecstasy", 4 * 60 + 26));

			artists[2].Records[1].Songs[4].Comments.Add(new Comment("", "a Bob Dylan cover"));
			artists[2].Records[1].Songs[0].Ratings.Add(new Rating(5));
			artists[2].Records[1].Songs[12].Ratings.Add(new Rating(4));
			artists[2].Records[1].Songs[12].Ratings.Add(new Rating(5));
			artists[2].Records[1].Songs[7].Ratings.Add(new Rating(5));
			artists[2].Records[1].Songs[8].Ratings.Add(new Rating(5));
			artists[2].Records[1].Songs[10].Ratings.Add(new Rating(5));

			return artists.ToArray();
		}
	}
}

