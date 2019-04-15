using System.Collections.Generic;
using BurningKnight.level.biome;
using Lens.util.math;

namespace BurningKnight.level.paintings {
	public static class PaintingRegistry {
		private static List<Info> paintings = new List<Info>();

		static PaintingRegistry() {
			Add("rexcellent", "egordorichev");
			Add("grannylisa", "DSF100");
			Add("maanex", "DSF100");
			Add("bk", "mate");
			Add("failpositive", "FIXME: ADD AUTHOR"); // fixme
			Add("old_man", "???");
			Add("arthouse", "???");
			Add("black", "???");
			Add("milt", "egordorichev");
			Add("skyscraper", "egordorichev");
			Add("egor", "egordorichev", 0.25f);
			Add("null", "SEGFAULT", 0.5f);
		}
		
		public static void Add(string id, string author, float chance = 1f, string[] biomes = null) {
			paintings.Add(new Info {
				Id = id,
				Author = author,
				Chance = chance,
				Biomes = biomes
			});
		}

		public static Painting Generate(Biome biome) {
			var length = paintings.Count;
			float sum = 0;

			foreach (var info in paintings) {
				if (biome.IsPresent(info.Biomes)) {
					sum += info.Chance;
				}
			}

			float value = Random.Float(sum);
			sum = 0;

			for (int i = 0; i < length; i++) {
				var info = paintings[i];

				if (!biome.IsPresent(info.Biomes)) {
					continue;
				}

				sum += info.Chance;

				if (value < sum) {
					return new Painting {
						Id = info.Id,
						Author = info.Author
					};
				}
			}

			return null;
		}

		private class Info {
			public string Id;
			public string Author;
			public float Chance;
			public string[] Biomes;
		}
	}
}