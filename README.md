# G2GFxDataTool
G2GFxDataTool is a tool for generating GFXF files for the Glacier 2 engine (UICT/UICB support is planned).

## Usage
Requires a copy of GFxExport which can be found [here](https://files.hitmods.com) (v4.01 for SDK v4.6.34 recommended).

```
G2GFxDataTool 1.0.0
Copyright (C) Glacier 2 Modding Organisation

  -i, --input                 Required. Path to the SWF file or directory containing SWF files.

  -o, --output                Path to output the files (defaults to the current working directory).

  -b, --base-assembly-path    Base assembly path (defaults to /ui/controls/).

  -g, --gfxexport             Path to gfxexport.exe (defaults to "gfxexport.exe").

  -s, --save-paths            Saves Scaleform GFx, UIControl and aspect paths to scaleformgfx.txt, uicontrol.txt and
                              aspect.txt text files in the output directory.

  --game                      Game version. Possible options are Hitman2016, Hitman2 and Hitman3 (defaults to Hitman3).
                              Note: these are case sensitive.

  -v, --verbose               Sets output to verbose messages mode.

  -l, --licenses              Prints license information for G2GFxDataTool and third party libraries that are used.

  --help                      Display this help screen.

  --version                   Display version information.
```

## Example Usage
.\G2GFxDataTool.exe -i "InputPath\HealthBar.swf" -o "OutputPath" -v -s