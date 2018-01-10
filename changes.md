#### General
- Brand new build automation workflow (bee 🐝)
- Completely automated build, sync and release flow for faster future updates (bzzz 🐝)
- Only Entitas.zip is attached to GitHub releases
- Jenny CLI is only bundled in Asset Store version
- Added Assets folder to Entitas.zip #535
- More flexible plugin-based CLI architecture

#### Jenny
- Unity support for multiple properties files by adding switch button to Entitas Preferences in case multiple properties files exist #536
- Better CLI support for multiple properties files by showing a warning in case multiple properties files exist #536
- Fixes for server / client errors (ObjectDisposedException) #529
- Renamed key `CodeGenerator.CLI.Ignore.UnusedKeys` to `Ignore.Keys`
- `auto-import` reflects assemblies and sets plugins based on content instead of name
- `auto-import` automatically detects custom plugins in Unity project without manually setting up searchPaths
- Added visual lines to `dump`
- Renamed `ICodeGeneratorBase` to `ICodeGenerationPlugin`
- Fixed `IConfigurable` commands not getting configured
- Added minified properties support

#### Asset Store
- Fix for NullReferenceException (Entitas.Roslyn.SymbolExtension.ToCompilableString) #534
- Support for WrapperComponent #532