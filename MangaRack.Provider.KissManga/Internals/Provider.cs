// ======================================================================
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaRack.Provider.Interfaces;

namespace MangaRack.Provider.KissManga.Internals {
	/// <summary>
	/// Represents a KissManga provider.
	/// </summary>
	class Provider : IProvider {
		#region Properties
		/// <summary>
		/// Contains the domain.
		/// </summary>
		public static string Domain {
			get {
				return "http://kissmanga.com/";
			}
		}
		#endregion

		#region IProvider:Methods
		/// <summary>
		/// Open a series.
		/// </summary>
		/// <param name="location">The unique identifier.</param>
		public ISeries Open(string location) {
			if (Regex.Match(location, @"^http://kissmanga\.com/Manga/(.*)$", RegexOptions.IgnoreCase).Success) {
				return new Series(location);
			}
			return null;
		}

        /// <summary>
        /// Search series.
        /// </summary>
        /// <param name="input">The input.</param>
        public async Task<ISearch> SearchAsync(string input)
        {
            var search = new Search(input);
            await search.PopulateAsync();
            return search;
        }
		#endregion

		#region IProvider:Properties
		/// <summary>
		/// Contains the location.
		/// </summary>
		public string Location {
			get {
				return Domain;
			}
		}
		#endregion
	}
}