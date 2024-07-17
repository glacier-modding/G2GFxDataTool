# G2GFxDataTool
G2GFxDataTool is a tool for generating GFXF and UICT/UICB files for the Glacier 2 engine utilising https://github.com/OrfeasZ/ZHMTools and SwfOp. 

## Usage
Requires a copy of GFxExport which can be found [here](https://files.hitmods.com) (v4.01 for SDK v4.6.34 recommended).

```
G2GFxDataTool 1.1.0
Copyright (C) Glacier 2 Modding Organisation

  -i, --input                 Required. Path to the SWF file or directory containing SWF files.

  -o, --output                Path to output the files (defaults to the current working directory).

  -b, --base-assembly-path    Base assembly path (defaults to /ui/controls/).

  -g, --gfxexport             Path to gfxexport.exe (defaults to "gfxexport.exe" in the folder where G2GFxDataTool.exe
                              is located).

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
```
.\G2GFxDataTool.exe -i "bin\HealthBar.swf" -o "..\content\chunk0\scaleform\" -v -s
```

## Example Output
```
GFxExport v4.01 for SDK v4.6.34, (c) 2006-2011 Scaleform Corporation
This program uses:
        NVIDIA Texture Tools 2, (c) 2007 NVIDIA Corporation
        PVRTexLib (c) 2010, Imagination Technologies Ltd.

Loading SWF file: bin\HealthBar.swf - Processing 0 images -
100%
Total images written: 0
Saving list of generated files as 'C:\Users\Josh\AppData\Local\Temp\HealthBar.lst'
Saving stripped SWF file as 'C:\Users\Josh\AppData\Local\Temp\healthbar.gfx'

Saving GFXF file as '..\content\chunk0\scaleform\001D0947392E2DD6.GFXF'

Cleaning up temporary files:
C:\Users\Josh\AppData\Local\Temp\HealthBar.gfx
C:\Users\Josh\AppData\Local\Temp\HealthBar.lst

Found class: common.BaseControl:
Saving UICT file as '..\content\chunk0\scaleform\00DE44EF8E598769.UICT
Saving UICB file as '..\content\chunk0\scaleform\00FB1120C3B2DF8A.UICB

Found class: healthbar.HealthBar:
        Found pin: RequestData type: E_ATTRIBUTE_TYPE_VOID kind: E_ATTRIBUTE_KIND_OUTPUT_PIN
        Found pin: MainColours type: E_ATTRIBUTE_TYPE_OBJECT kind: E_ATTRIBUTE_KIND_INPUT_PIN
        Found pin: SecondaryColours type: E_ATTRIBUTE_TYPE_OBJECT kind: E_ATTRIBUTE_KIND_INPUT_PIN
        Found pin: SetBarHealth type: E_ATTRIBUTE_TYPE_FLOAT kind: E_ATTRIBUTE_KIND_INPUT_PIN
        Found pin: SetTextHealth type: E_ATTRIBUTE_TYPE_FLOAT kind: E_ATTRIBUTE_KIND_INPUT_PIN
        Found pin: SetInfected type: E_ATTRIBUTE_TYPE_BOOL kind: E_ATTRIBUTE_KIND_INPUT_PIN
        Found property: DebugMode type: Boolean
        Found property: Height type: Number
        Found property: Width type: Number
Saving UICT file as '..\content\chunk0\scaleform\005E8CE7C12D96C2.UICT
Saving UICB file as '..\content\chunk0\scaleform\003F45DD25EEE393.UICB
```