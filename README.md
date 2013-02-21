# MangaRack

MangaRack is a small application capable of synchronizing manga series from popular online manga scans and scanlations websites. Each downloaded chapter is stored on your computer as a comic book archive with additional embedded information, such as the writer and summary, ready to be imported to ComicRack.

## Motivation

It is easy to find downloadable sources for anime, but manga is different. It is often tedious to find the series you are looking for, and the series that can be found often come in unfamiliar archives and without embedded information. Reading the series online is possible but is also uncomfortable, especially when you are used to a light reading device and ComicRack for Android. The issue has been programmatically solved by creating ComicRack-ready archives directly from online resources.

## Binaries and MangaFox

http://comicrack.cyolito.com/forum/21-manga/29179-mangarack-manga-for-comicrack

## Usage Instructions

	1. Decide on the folder you wish to use for sychronized manga.
	2. Ensure the folder contains a MangaRack.txt file.
	3. Add supported unique identifiers of manga series you wish to sychronize to the text file.
	4. Run MangaRack and wait for synchronization to be completed.
	5. Import the folder with ComicRack (and sychronize to your reading device).
	6. Read. Have fun!

## Debugging Instructions

Set the command line argument **0 uniqueIdentifier**. If omitted, the master process is debugged.