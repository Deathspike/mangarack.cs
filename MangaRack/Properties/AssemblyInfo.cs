[assembly: System.Reflection.AssemblyVersion("1.0.*")]

// Embedding
[assembly: System.Reflection.Obfuscation(Feature = "embed AForge.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed AForge.Imaging.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed AForge.Math.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed HtmlAgilityPack.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed ICSharpCode.SharpZipLib.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed MangaRack.Provider.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed Newtonsoft.Json.dll", Exclude = false)]

// Merging
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Provider.Batoto.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Provider.KissManga.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Provider.MangaFox.dll", Exclude = false)]

// Options
[assembly: System.Reflection.Obfuscation(Feature = "encrypt symbol names with password h3nk1s50fuN", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "rename symbol names with printable characters", Exclude = false)]