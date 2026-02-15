# Seed Generator

`SeedGenerator` provides deterministic `uint` seeds from a master seed and a string name.

## Usage

Create an instance with a fixed master seed for deterministic results:

```csharp
var generator = new SeedGenerator(12345u);
var terrainSeed = generator.GenerateSeed("Terrain");
var biomeSeed = generator.GenerateSeed("Biome");
```

Create an instance with `0` to generate a runtime master seed:

```csharp
var generator = new SeedGenerator();
generator.GenerateMasterSeed();
var seed = generator.GenerateSeed("Default");
```

## Notes

- `GenerateSeed` hashes the name and mixes it with `MasterSeed`.
- `GenerateMasterSeed` can accept a specific master seed or generate a runtime value when zero. Useful for replaying a previously known seed.
- `LastMasterSeed` stores the previous master seed after regeneration.
