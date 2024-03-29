﻿# AsciiPhoto
Console application that generates ASCII art from line drawings or screen region by matching letters over a larger image.
Alternatively it can match brightness instead of contours. This works better for other types of pictures than line drawing.

## Usage
`AsciiPhoto [options]` or `AsciiPhoto "@parameters.txt"`

| Option | Explanation |
| :-- |:-- |
| `--brightness-offset <brightness-offset>`         | Increase or decrease the brightness of the read pixel before processing it.                                                                            |
| `--brightness-threshold <brightness-threshold>`   | The brightness, expressed in percent, above which pixels are considered empty/white.                                                                   |
| `--chars-in-row <chars-in-row>`                   | Sets the width of the result to this many characters.                                                                                                  |
| `--clear-screen`                                  | If true, the console will be cleared before every file.                                                                                                |
| `--delay <delay>`                                 | The wait time after displaying each frame in milliseconds.                                                                                             |
| `--horizontal-offset <horizontal-offset>`         |                                                                                                                                                        |
| `--inverse`                                       | If true, the colors will be displayed in the negative.                                                                                                 |
| `--loop-nr <loop-nr>`                             | How many times should the input be processed.                                                                                                          |
| `--match-brightness`                              | If true, the brightness of the image will be processed instead of the oiutlines.                                                                       |
| `--output-file <output-file>`                     | The full path to the file where the results will be written. If empty, no output file will be written. If the file exists, it will be overwritten.     |
| `--path <path>`                                   | The folder that is searched in all depth for picture files to process (jpg, png, gif).                                                                 |
| `--pixels-between-rows <pixels-between-rows>`     | Number of pixels assumed between console rows. Text editors usually leave 2 px distance, console leaves 1.                                             |
| `--print-font-matrices`                           | If true, bitmatrices of the fonts are printed before starting the processing of the images.                                                            |
| `--print-results-asap`                            | If true, characters are printed as soon as they are generated.                                                                                         |
| `--ratio <ratio>`                                 | Resizes a picture before creating ASCII art, by multiplying width and height with this percent.                                                        |
| `--return-to-start`                               | If true, the new result will start on position 0,0 of the console.                                                                                     |
| `--refresh-sources`                               | If true, each frame will be reread from the source, instead of being used from memory. This is useful for cases when the source is being modified and needs to be displayed. Static sources should use false for this setting. |
| `--screen-nr <screen-nr>`                         | Not used at the moment.                                                                                                                                |
| `--source <File/Folder/NotSet/Screen>`            | The input method: Folder = 1, File = 2, Screen = 4                                                                                                     |
| `--verbose`                                       | If true, all information will be written in the console, otherwise only the results will be written. File content is not affected, that is always full.|
| `--vertical-offset <vertical-offset>`             |                                                                                                                                                        |
| `--weight-offset-percent <weight-offset-percent>` |                                                                                                                                                        |
| `--weight-total-pixels <weight-total-pixels>`     |                                                                                                                                                        |
| `--version`                                       | Show version information                                                                                                                               |
| `-?, -h, --help`                                  | Show help and usage information                                                                                                                        |

```
┌────────────∙
│                                                                             ▄█▌  │
│                                                                            ▐███  │
│                            ____  ____   ___                              ▄█▀"▄█▌ │
│                            ▀█▄▀  *▐▄▀' ╙▐█▀╛                        __▄▀▀" ▄▀▀▐▌ │
│                            ╓█▐╖   ▐▄■   ▐▄■           _____▄▄∙▄■■▀▀▀▀"__∙∙▀Γ  ▐█ │
│  ▄___ _     __   _________▐██▀█▀¬███▀█~███▀█═════****""""`'        `""        ▐█ │
│ ▐█▐▌▐███████▐████""**_____┴▀╨_═══▀▀══▀═▀┴⌐─▀══■■■■▄__   ,              ,,     ▐█ │
│ `█▀▀▀°"`""█▀▀▐████▀▀▀▀▀▀▀▀██▀▀▀▀▀██▀▀█▀██▀▀█▀▀▀▀▄▄▄_▀▀█████████▄╖▄∙∩,_ __*■▀██▄█ │
│         ■▀° ▄▀*°          ▐█■X ▌ █▌  ▐ ██V'▐      `▀▀■`∙  `"""*═▀▀▀▐▄█┤▐▀▀═.`▀█▌ │
│        ▐▀  █Γ             ▐█.  ▌ ██  ▐ ██  ▐         ▀▄ \            `*∙▀▀▄_▀██▌ │
│        ▌  :▌             _■▀*▐,*█""****""**╨*****"──═─█⌐ ▌═■▄            `∙█▄■█▌ │
│        ▌  ▐▌           ■■ ,▄▀▀▀█▀██▀▀█▀█████▀▀▀▀▀█▀██▀█▌ ▌▄■"▀,            └▄▀█  │
│        ▌  :▌,⌐═▄═▄═══▄▄Γ ▐██▌  ▌▌█▌__▌_████████▄▄▄▄██▄█▌ ▌■Γ╙■▌══════▄═▄__  ▀▀*  │
│        ▐▌  ■▀_▄▀▄█▀▀▀▀█  ▐██⌐  ▌▐████▄▄▄▄▄▄▄▄▄▄▄▄▄▄__▄▀ ▐__■═█▄▄▄▄▄▄▄█_█_"▀■     │
│         █, ▐▄*▀▄═▄═══▄▄█,"▀█■■■╖■▀▀▀▐█▀▀████___▄_¬¬_∙" ■█▌¬▄█▄_______,__▐⌐ ▐Γ    │
│         `▀▄_"▀▄█▄█▄▄█████▄__"▐▀U▌───▀▀▀▀▀═█¬∙█════"__▄███__________▄_█▌█▄▄▄▀     │
│           *▀▄▄▄_═══■■▀███████▀▀████▀▀▄▄▄▄▀▀▄██▀▀▀██▌""""""""""""""""""▀*"°       │
│              "▀▀▀▀▄▄▄█▀▀▀" █▌¬■▌`██_▄████▀▀▀██U▄▄▀▀▀                             │
│                            ▀▀█▀*`▀▀█▀▀ ▀▀█▀*                                     │
│                                                                                  │
                                                                      ∙────────────┘
```
# ToDos
* Try using [Floyd–Steinberg dithering](https://en.wikipedia.org/wiki/Floyd%E2%80%93Steinberg_dithering) on the photo before determining pixel brightness
* Use the real color of the images
* Check out http://caca.zoy.org/wiki/libcaca