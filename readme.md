# defMONRelocator

CLI tool to relocate PRG music files created using defMON music editor for Commodore 64.
Additionally you can change zero page address used by the player. Default address are: $FB, $FC, $96. 

It was prepared for defMON rev. 20181101 and should work with all newer revisions as player code didn't changed until now (2024-03-01).

## Description

This small tool was created during creation of Samar demo 'NGC 1277' in 2019. Music was composed by Samar musician F7sus4.
We needed to relocate the music to different location and all available tools didn't work with this kind of player.

NGC 1277 demo: https://csdb.dk/release/?id=179107

defMON: https://defmon.vandervecken.com/doku.php?id=start

## Getting Started

### Dependencies

.NET 6.0

- Windows: https://dotnet.microsoft.com/en-us/download
- Linux: apt-get install dotnet-runtime-6.0

### Executing program

Example for relocating music to $9000, together with changing zeropage address used by player.

```
defmonrelocator.exe music-original.prg music-relocated.prg 9000 02 03 04
```

Program doesn't have any advanced error handling, so you will get an exception in case of wrong parameters.

## Authors

DKT / Samar

## Version History

* 1.0.0 [2024-03-01]
  * Initial Release.
* 1.0.1 [2024-03-01]
  * Better CLI arguments passing.
* 1.1.0 [2024-03-01]
  * Fix of zero page addresses change for tunes located at location different than $1000.