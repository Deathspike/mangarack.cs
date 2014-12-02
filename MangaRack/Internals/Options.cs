// ======================================================================
using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace MangaRack.Internals {
	/// <summary>
	/// Represents a collection of options.
	/// </summary>
	class Options {
		#region Constructor
		/// <summary>
		/// Initialize a new instance of the Options class.
		/// </summary>
		public Options() {
			FileExtension = "cbz";
			MaximumParallelWorkerThreads = Environment.ProcessorCount;
			SourceFile = string.Format("{0}.txt", Assembly.GetExecutingAssembly().GetName().Name);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		[HelpOption]
		public override string ToString() {
			var text = HelpText.AutoBuild(this);
			text.AddPreOptionsLine("\r\n  Usage: mangarack [options] [location, ...]");
			return text.ToString();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Indicates whether animation framing is disabled.
		/// </summary>
		[Option('a', "animation", HelpText = "Disable animation framing.")]
		public bool DisableAnimationFraming { get; set; }

		/// <summary>
		/// Indicates whether duplication prevention is disabled.
		/// </summary>
		[Option('d', "duplication", HelpText = "Disable duplication prevention.")]
		public bool DisableDuplicationPrevention { get; set; }

		/// <summary>
		/// Indicates whether footer incision is disabled.
		/// </summary>
		[Option('f', "footer", HelpText = "Disable footer incision.")]
		public bool DisableFooterIncision { get; set; }

		/// <summary>
		/// Indicates whether grayscale size comparison and save is disabled.
		/// </summary>
		[Option('g', "grayscale", HelpText = "Disable grayscale size comparison and save.")]
		public bool DisableGrayscaleSizeComparisonAndSave { get; set; }

		/// <summary>
		/// Indicates whether image processing is disabled.
		/// </summary>
		[Option('i', "image", HelpText = "Disable image processing.")]
		public bool DisableImageProcessing { get; set; }

		/// <summary>
		/// Indicates whether keep-alive behavior is disabled.
		/// </summary>
		[Option('k', "keep-alive", HelpText = "Disable keep-alive behavior.")]
		public bool DisableKeepAliveBehavior { get; set; }

		/// <summary>
		/// Indicates whether embedded meta-information is disabled.
		/// </summary>
		[Option('m', "meta", HelpText = "Disable embedded meta information.")]
		public bool DisableMetaInformation { get; set; }

		/// <summary>
		/// Indicates whether repair and error tracking is disabled.
		/// </summary>
		[Option('r', "repair", HelpText = "Disable repair and error tracking.")]
		public bool DisableRepairAndErrorTracking { get; set; }

		/// <summary>
		/// Indicates whether total elapsed time notification is disabled.
		/// </summary>
		[Option('t', "total", HelpText = "Disable total elapsed time notification.")]
		public bool DisableTotalElapsedTime { get; set; }

		/// <summary>
		/// Indicates whether embedded meta-information is overwritten.
		/// </summary>
		[Option('o', "overwrite", HelpText = "Enable embedded meta information overwriting.")]
		public bool EnableOverwriteMetaInformation { get; set; }

		/// <summary>
		/// Indicates whether persistent synchronization is enabled.
		/// </summary>
		[Option('p', "persistent", HelpText = "Enable persistent synchronization.")]
		public bool EnablePersistentSynchronization { get; set; }

		/// <summary>
		/// Contains the file extension for each output file.
		/// </summary>
		[Option('e', "extension", HelpText = "The file extension for each output file. (Default: cbz)")]
		public string FileExtension { get; set; }

		/// <summary>
		/// Contains the chapter filter.
		/// </summary>
		[Option('c', "chapter", HelpText = "The chapter filter.")]
		public double FilterOnChapter { get; set; }

		/// <summary>
		/// Contains the volume filter.
		/// </summary>
		[Option('v', "volume", HelpText = "The volume filter.")]
		public double FilterOnVolume { get; set; }

		/// <summary>
		/// Contains each location.
		/// </summary>
		[ValueList(typeof(List<string>))]
		public IList<string> Locations { get; set; }

		/// <summary>
		/// Contains the maximum parallel worker threads.
		/// </summary>
		[Option('w', "worker", HelpText = "The maximum parallel worker threads. (Default: # cores)")]
		public int MaximumParallelWorkerThreads { get; set; }

		/// <summary>
		/// Contains the batch-mode source file.
		/// </summary>
		[Option('s', "source", HelpText = "The batch-mode source file. (Default: MangaRack.txt)")]
		public string SourceFile { get; set; }
		#endregion
	}
}