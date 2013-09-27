[assembly: System.Reflection.AssemblyCopyright("(C) Roel van Uden. All rights reserved.")]
[assembly: System.Reflection.AssemblyVersion("2.0.*")]

// Embedding
[assembly: System.Reflection.Obfuscation(Feature = "embed AForge.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed AForge.Imaging.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed AForge.Math.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed CommandLine.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "embed ICSharpCode.SharpZipLib.dll", Exclude = false)]

// Merging
[assembly: System.Reflection.Obfuscation(Feature = "merge with HtmlAgilityPack-PCL.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Core.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Provider.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Provider.Batoto.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Provider.KissManga.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with MangaRack.Provider.MangaFox.dll", Exclude = false)]
[assembly: System.Reflection.Obfuscation(Feature = "merge with TinyHttp-PCL.dll", Exclude = false)]

// Options
[assembly: System.Reflection.Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]