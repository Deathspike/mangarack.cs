// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using ICSharpCode.SharpZipLib.Zip;
using MangaRack.Concrete;
using MangaRack.Core;
using MangaRack.Core.Concrete.Publisher;
using MangaRack.Core.Concrete.Xml;
using MangaRack.Extension;
using MangaRack.Provider;
using MangaRack.Provider.Abstract;
using MangaRack.Provider.Batoto;
using MangaRack.Provider.Extension;
using MangaRack.Provider.KissManga;
using MangaRack.Provider.MangaFox;

namespace MangaRack {
    /// <summary>
    /// Represents the application.
    /// </summary>
    public static class Application {
        /// <summary>
        /// Contains each provider.
        /// </summary>
        private static readonly IEnumerable<IProvider> Providers;

        #region Constructor
        /// <summary>
        /// Initialize the Program class.
        /// </summary>
        static Application() {
            Providers = new[] {
                Factory.Create<Batoto>(),
                Factory.Create<KissManga>(),
                Factory.Create<MangaFox>()
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Run in batch processing mode.
        /// </summary>
        /// <param name="options">The options.</param>
        public static bool Batch(Options options) {
            if (!File.Exists(options.SourceFile)) return false;
            var workerItems = new List<KeyValuePair<Options, string>>();
            foreach (var line in File.ReadAllLines(options.SourceFile)) {
                var lineOptions = new Options();
                if (!Parser.Default.ParseArguments(line.Split(' '), lineOptions) || lineOptions.Locations.Count == 0) continue;
                if (options.EnableOverwriteMetaInformation) {
                    lineOptions.EnableOverwriteMetaInformation = true;
                }
                foreach (var location in lineOptions.Locations) {
                    if (lineOptions.MaximumParallelWorkerThreads > 1 && options.MaximumParallelWorkerThreads > 1) {
                        workerItems.Add(new KeyValuePair<Options, string>(lineOptions, location));
                    } else {
                        Single(location, lineOptions);
                    }
                }
            }
            if (workerItems.Count == 0) return false;
            UnsafeParallel.For(0, workerItems.Count, options.MaximumParallelWorkerThreads, i => { Single(workerItems[i].Value, workerItems[i].Key); });
            return true;
        }

        /// <summary>
        /// Entry point of the application.
        /// </summary>
        /// <param name="arguments">Each command line argument.</param>
        public static void Main(string[] arguments) {
            var options = new Options();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
                Console.WriteLine(e.ExceptionObject);
                if (!options.DisableKeepAliveBehavior) {
                    Console.WriteLine("Press [ENTER] to exit.");
                    Console.ReadLine();
                }
                Environment.Exit(0);
            };
            if (!Parser.Default.ParseArguments(arguments, options)) return;
            var time = DateTime.Now.Ticks;
            if (options.MaximumParallelWorkerThreads < 1) {
                options.MaximumParallelWorkerThreads = 1;
            }
            if (options.Locations.Count == 0) {
                if (!Batch(options)) {
                    Console.WriteLine("Breaking due to invalid source file.");
                }
            } else {
                if (options.MaximumParallelWorkerThreads > 1) {
                    UnsafeParallel.For(0, options.Locations.Count, options.MaximumParallelWorkerThreads, i => { Single(options.Locations[i], options); });
                } else {
                    foreach (var location in options.Locations) {
                        Single(location, options);
                    }
                }
            }
            if (!options.DisableTotalElapsedTime) {
                var elapsed = new TimeSpan(DateTime.Now.Ticks - time);
                Console.WriteLine("Complete ({0}:{1}:{2})!", elapsed.Hours.ToString("00"), elapsed.Minutes.ToString("00"), elapsed.Seconds.ToString("00"));
            }
            if (!options.DisableKeepAliveBehavior) {
                Console.WriteLine("Press [ENTER] to exit.");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Persist the state.
        /// </summary>
        /// <param name="persistencePath">The persistence path.</param>
        /// <param name="persistence">The persistence.</param>
        public static void Persist(string persistencePath, List<List<string>> persistence) {
            File.WriteAllLines(persistencePath, persistence.Select(x => string.Join("\0", x.ToArray())).ToArray());
        }

        /// <summary>
        /// Run in single processing mode for the location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="options">The collection of options.</param>
        public static void Single(string location, Options options) {
            var provider = Providers.FirstOrDefault(x => x.Open(location) != null);
            if (provider == null) return;
            using (var series = provider.Open(location)) {
                using (series.Populate()) {
                    var seriesTitle = series.Title.InvalidatePath();
                    var persistencePath = Path.Combine(seriesTitle, ".mangarack-persist");
                    var persistence = new List<List<string>>();
                    if (File.Exists(persistencePath)) {
                        const int persistenceVersion = 2;
                        foreach (var pieces in File.ReadAllLines(persistencePath).Select(line => new List<string>(line.Split('\0')))) {
                            while (pieces.Count < persistenceVersion) pieces.Add(string.Empty);
                            persistence.Add(pieces);
                        }
                    }
                    foreach (var chapter in series.Children) {
                        var line = persistence.FirstOrDefault(x => string.Equals(x[1], chapter.UniqueIdentifier));
                        if (line == null) continue;
                        var currentFilePath = Path.Combine(seriesTitle, line[0]);
                        var nextFileName = chapter.ToFileName(seriesTitle, options);
                        if (!string.Equals(line[0], nextFileName) && File.Exists(currentFilePath)) {
                            File.Move(currentFilePath, Path.Combine(seriesTitle, nextFileName));
                            line[0] = nextFileName;
                            Persist(persistencePath, persistence);
                            Console.WriteLine("Switched {0}", nextFileName);
                        }
                    }
                    foreach (var chapter in series.Children.Filter(options)) {
                        var hasFailed = false;
                        var fileName = chapter.ToFileName(seriesTitle, options);
                        var filePath = Path.Combine(seriesTitle, fileName);
                        var persistenceFile = persistence.FirstOrDefault(x => string.Equals(x[0], fileName));
                        if (options.EnablePersistentSynchronization && persistenceFile != null) {
                            continue;
                        }
                        if (persistenceFile != null) {
                            persistenceFile[1] = chapter.UniqueIdentifier ?? string.Empty;
                        } else {
                            persistence.Add(new List<string> {fileName, chapter.UniqueIdentifier ?? string.Empty});
                        }
                        do {
                            if (options.DisableDuplicationPrevention || !File.Exists(filePath)) {
                                using (chapter.Populate()) {
                                    using (var publisher = new Publisher(filePath, options, provider)) {
                                        using (var synchronizer = new Synchronize(publisher, series, chapter)) {
                                            synchronizer.Populate();
                                            hasFailed = false;
                                        }
                                    }
                                }
                            } else {
                                if (options.EnableOverwriteMetaInformation) {
                                    var comicInfo = new ComicInfo();
                                    using (var zipFile = new ZipFile(filePath)) {
                                        var zipEntry = zipFile.GetEntry("ComicInfo.xml");
                                        if (zipEntry != null) {
                                            var previousComicInfo = ComicInfo.Load(zipFile.GetInputStream(zipEntry));
                                            comicInfo.Transcribe(series, chapter, previousComicInfo.Pages);
                                            if (comicInfo.Genre.Any(x => !previousComicInfo.Genre.Contains(x)) ||
                                                previousComicInfo.Genre.Any(x => !comicInfo.Genre.Contains(x)) ||
                                                comicInfo.Manga != previousComicInfo.Manga ||
                                                comicInfo.Number != previousComicInfo.Number ||
                                                comicInfo.PageCount != previousComicInfo.PageCount ||
                                                comicInfo.Penciller.Any(x => !previousComicInfo.Penciller.Contains(x)) ||
                                                previousComicInfo.Penciller.Any(x => !comicInfo.Penciller.Contains(x)) ||
                                                comicInfo.Series != previousComicInfo.Series ||
                                                comicInfo.Summary != previousComicInfo.Summary ||
                                                comicInfo.Title != previousComicInfo.Title ||
                                                comicInfo.Volume != previousComicInfo.Volume ||
                                                comicInfo.Writer.Any(x => !previousComicInfo.Writer.Contains(x)) ||
                                                previousComicInfo.Writer.Any(x => !comicInfo.Writer.Contains(x))) {
                                                using (var memoryStream = new MemoryStream()) {
                                                    comicInfo.Save(memoryStream);
                                                    memoryStream.Position = 0;
                                                    zipFile.BeginUpdate();
                                                    zipFile.Add(new DataSource(memoryStream), "ComicInfo.xml");
                                                    zipFile.CommitUpdate();
                                                    Console.WriteLine("Modified {0}", fileName);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!options.DisableRepairAndErrorTracking && File.Exists(string.Format("{0}.txt", filePath))) {
                                    using (chapter.Populate()) {
                                        ComicInfo comicInfo;
                                        var hasBrokenPages = false;
                                        using (var zipFile = new ZipFile(filePath)) {
                                            var zipEntry = zipFile.GetEntry("ComicInfo.xml");
                                            if (zipEntry == null) {
                                                return;
                                            }
                                            comicInfo = ComicInfo.Load(zipFile.GetInputStream(zipEntry));
                                        }
                                        using (var publisher = new Publisher(filePath, options, provider, true)) {
                                            using (var repair = new Repair(publisher, chapter, comicInfo, File.ReadAllLines(string.Format("{0}.txt", filePath)))) {
                                                repair.Populate();
                                                hasBrokenPages = publisher.HasBrokenPages;
                                                hasFailed = publisher.HasFailed = repair.HasFailed;
                                            }
                                        }
                                        if (!hasBrokenPages && File.Exists(string.Format("{0}.txt", filePath))) {
                                            File.Delete(string.Format("{0}.txt", filePath));
                                        }
                                    }
                                }
                            }
                        } while (hasFailed);
                        Persist(persistencePath, persistence);
                    }
                }
            }
        }
        #endregion
    }
}