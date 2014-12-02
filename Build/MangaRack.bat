:: ==================================================
:: .NET
:: ==================================================

:: --------------------------------------------------
:: Merge
:: --------------------------------------------------
cd .NET/Release
call "%ProgramFiles(x86)%\Microsoft\ILMerge\ILMerge.exe" /allowDup /targetplatform:v4,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /out:../../MangaRack.exe MangaRack.exe AForge.dll AForge.Imaging.dll AForge.Math.dll CommandLine.dll HtmlAgilityPack-PCL.dll ICSharpCode.SharpZipLib.dll MangaRack.Core.dll MangaRack.Provider.dll MangaRack.Provider.Batoto.dll MangaRack.Provider.KissManga.dll MangaRack.Provider.MangaFox.dll TinyHttp-PCL.dll
call "%ProgramFiles(x86)%\Microsoft\ILMerge\ILMerge.exe" /allowDup /targetplatform:v4,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /out:../../MangaRack.Analyze.exe MangaRack.Analyze.exe ICSharpCode.SharpZipLib.dll MangaRack.Core.dll
cd ../..

:: --------------------------------------------------
:: Archive
:: --------------------------------------------------
echo. 2>MangaRack.txt
call "%ProgramFiles%\WinRAR\WinRAR.exe" a -afzip -df "MangaRack for .NET.zip" MangaRack.Analyze.exe MangaRack.exe MangaRack.txt
del MangaRack.txt

:: --------------------------------------------------
:: Trash
:: --------------------------------------------------
del MangaRack.Analyze.exe
del MangaRack.Analyze.pdb
del MangaRack.exe
del MangaRack.pdb

:: ==================================================
:: Mono
:: ==================================================

:: --------------------------------------------------
:: Merge
:: --------------------------------------------------
cd Mono/Release
call "%ProgramFiles(x86)%\Microsoft\ILMerge\ILMerge.exe" /allowDup /out:../../MangaRack.exe MangaRack.exe AForge.dll AForge.Imaging.dll AForge.Math.dll CommandLine.dll HtmlAgilityPack.dll ICSharpCode.SharpZipLib.dll MangaRack.Core.dll MangaRack.Provider.dll MangaRack.Provider.Batoto.dll MangaRack.Provider.KissManga.dll MangaRack.Provider.MangaFox.dll TinyHttp.dll
call "%ProgramFiles(x86)%\Microsoft\ILMerge\ILMerge.exe" /allowDup /out:../../MangaRack.Analyze.exe MangaRack.Analyze.exe ICSharpCode.SharpZipLib.dll MangaRack.Core.dll
cd ../..

:: --------------------------------------------------
:: Archive
:: --------------------------------------------------
echo. 2>MangaRack.txt
call "%ProgramFiles%\WinRAR\WinRAR.exe" a -afzip -df "MangaRack for Mono.zip" MangaRack.Analyze.exe MangaRack.exe MangaRack.txt
del MangaRack.txt

:: --------------------------------------------------
:: Trash
:: --------------------------------------------------
del MangaRack.Analyze.exe
del MangaRack.Analyze.pdb
del MangaRack.exe
del MangaRack.pdb