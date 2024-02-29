# defMONRelocator

CLI tool to relocate PRG music files created using defMON music editor for Commodore 64.

## Description

This small tool was created during creation of Samar demo 'NGC 1277'. Music was composed by Samar musician F7sus4.
We needed to relocate the music to different location and all available tools didn't work with this kind of player.

NGC 1277 demo: https://csdb.dk/release/?id=179107

defMON: https://defmon.vandervecken.com/doku.php?id=start

## Getting Started

### Dependencies

.NET 8.0

### Executing program

Example for relocating music to $9000, together with changing zeropage address used by player.

```
defmonrelocator.exe music-original.prg music-relocated.prg 9000 02 03 04
```

## Authors

DKT / Samar

## Version History

* 1.0 [2024-02-29]
    * Initial Release
