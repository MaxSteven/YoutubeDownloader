# YoutubeDownloader

Command line executable that can download Youtube videos or entire playlists.

## Download

- [See releases](https://github.com/Tyrrrz/YoutubeDownloader/releases)

## Usage examples

- `YoutubeDownloader.exe "https://www.youtube.com/watch?v=I7RHr4o7t7E"` (video by URL)
- `YoutubeDownloader.exe I7RHr4o7t7E` (video by ID)
- `YoutubeDownloader.exe "https://www.youtube.com/watch?v=0KSOMA3QBU0&list=PLMC9KNkIncKtPzgY-5rmhvj7fax8fdxoj"` (playlist by URL)
- `YoutubeDownloader.exe PLMC9KNkIncKtPzgY-5rmhvj7fax8fdxoj` (playlist by ID)

You can include multiple URLs or IDs simply by separating the arguments with space, like so:

- `YoutubeDownloader.exe I7RHr4o7t7E 05nwADM_X-U ucDCFsiN5Gc` (multiple videos by ID)

Or you can also combine the URLs with IDs:

- `YoutubeDownloader.exe "https://www.youtube.com/watch?v=I7RHr4o7t7E" 05nwADM_X-U ucDCFsiN5Gc` (multiple videos by ID)

Or even combine playlists with videos:

- `YoutubeDownloader.exe I7RHr4o7t7E PLMC9KNkIncKtPzgY-5rmhvj7fax8fdxoj` (video by ID and playlist by ID)

## Libraries used

- [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode)
- [CliWrap](https://github.com/Tyrrrz/CliWrap)
- [Tyrrrz.Extensions](https://github.com/Tyrrrz/Extensions)

## Runtimes used

- [FFMPEG](https://ffmpeg.org)
