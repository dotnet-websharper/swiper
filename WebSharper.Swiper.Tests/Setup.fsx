(*

# Setup.fsx

Adds registry keys for proper F# Web project support.

**Problem**: pure-F# Web projects do not work well in Visual Studio by default,
in particular adding a new item to the project is not possible.

**Solution**: This will hopefully be addressed in future VS versions.

**Workaround**: The issue with new project items can be worked around by adding
appropriate registry keys. Executing this script does just that. Once executed, you can remove this file from your project.

Credits: Daniel Mohl [1], Mark Seemann.

[1]: http://bloggemdano.blogspot.com/2013/11/adding-new-items-to-pure-f-aspnet.html

*)

open Microsoft.Win32
open System
open System.IO
open System.Runtime.InteropServices

type RegistryKeyName =
    | HKCU of string
    | HKLM of string

type RegistryKey with

    member this.Find(name: string) =
        (this, name.Split([| '\\' |]))
        ||> Seq.fold (fun s t ->
            s.OpenSubKey(t, true))

    static member Find(name) =
        let (r, s) =
            match name with
            | HKCU s -> (Registry.CurrentUser, s)
            | HKLM s -> (Registry.LocalMachine, s)
        r.Find(s)

let Guid = "{F2A71F9B-5D33-465A-A702-920D77279786}"

let AddGlobalRegistryKeys (product, key) =
    try
        RegistryKey.Find(key)
            .OpenSubKey("LanguageTemplates", true)
            .SetValue(Guid, Guid, RegistryValueKind.String)
        printfn "Added to %s" product
    with _ ->
        printfn "Failed to detect: %s" product

let InstallFSharpWebCapability () =
    [
        "VWD Express 11", HKCU @"Software\Microsoft\VWDExpress\11.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VWD Express 12", HKCU @"Software\Microsoft\VWDExpress\12.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VWD Express 14", HKCU @"Software\Microsoft\VWDExpress\14.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS 11", HKCU @"Software\Microsoft\VisualStudio\11.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS WinDesktop Express 12", HKCU @"Software\Microsoft\VSWinDesktopExpress\12.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS 12", HKCU @"Software\Microsoft\VisualStudio\12.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS WinDesktop Express 14", HKCU @"Software\Microsoft\VSWinDesktopExpress\14.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS 14", HKCU @"Software\Microsoft\VisualStudio\14.0_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VWD Express 11 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VWDExpress\11.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VWD Express 12 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VWDExpress\12.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VWD Express 14 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VWDExpress\14.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS 11 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\11.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS WinDesktop Express 12 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VSWinDesktopExpress\12.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS 12 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS WinDesktop Express 14 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VSWinDesktopExpress\14.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
        "VS 14 (x64)", HKLM @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\14.0\Projects\{349C5851-65DF-11DA-9384-00065B846F21}"
    ]
    |> List.iter AddGlobalRegistryKeys

type RegSAM = AllAccess = 0x000f003f
[<DllImport("advapi32.dll", SetLastError = true)>]
extern int RegLoadAppKey(string hiveFile, nativeint& hKey, RegSAM samDesired, int options, int reserved)
[<DllImport("advapi32.dll", SetLastError = true)>]
extern int RegCloseKey(nativeint hKey)

let CreateOrOpenSubKey name (parent: RegistryKey) =
    match parent.OpenSubKey(name, true) with
    | null -> parent.CreateSubKey(name, true)
    | k -> k

/// Starting with VS2017, the global registry is not used, but a local one instead.
let InstallAppSpecificRegistryKeys () =
    let basedir = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "Microsoft", "VisualStudio")
    for dir in Directory.EnumerateDirectories(basedir) do
        let regfile = Path.Combine(dir, "privateregistry.bin")
        if File.Exists(regfile) then
            let mutable key = 0n
            let x = RegLoadAppKey(regfile, &key, RegSAM.AllAccess, 0, 0)
            try
                use hdl = new SafeHandles.SafeRegistryHandle(key, true)
                let hKey = RegistryKey.FromHandle(hdl)
                let version = Path.GetFileName dir
                hKey.Find(sprintf @"Software\Microsoft\VisualStudio\%s_Config\Projects\{349C5851-65DF-11DA-9384-00065B846F21}\LanguageTemplates" version)
                    .SetValue(Guid, "{76B279E8-36ED-494E-B145-5344F8DEFCB6}")
                printfn "Added to VS %s" version
            finally
                RegCloseKey(key) |> ignore

InstallFSharpWebCapability ()
InstallAppSpecificRegistryKeys ()