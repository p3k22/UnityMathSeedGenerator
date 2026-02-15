namespace P3k.SeedGenerator
{
   using System;
   using System.Linq;

   using Unity.Mathematics;

   using UnityEngine;

   /// <summary>
   ///    Provides deterministic seed generation based on a master seed and string identifiers.
   /// </summary>
   public sealed class SeedGenerator
   {
      /// <summary>
      ///    Gets the current master seed used to derive all other seeds.
      /// </summary>
      public uint MasterSeed { get; private set; }

      /// <summary>
      ///    Gets the previous master seed before the most recent update.
      /// </summary>
      public uint LastMasterSeed { get; private set; }

      /// <summary>
      ///    Initializes a new instance of the <see cref="SeedGenerator" /> class.
      /// </summary>
      /// <param name="masterSeed">Optional master seed value; a runtime value is generated when zero.</param>
      public SeedGenerator(uint masterSeed = 0)
      {
         MasterSeed = masterSeed != 0 ? masterSeed : MakeRuntimeSeed();
         LastMasterSeed = MasterSeed;
      }

      /// <summary>
      ///    Generates a new master seed and logs the value.
      /// </summary>
      /// <param name="masterSeed">Optional master seed value; a runtime value is generated when zero.</param>
      public void GenerateMasterSeed(uint masterSeed = 0)
      {
         LastMasterSeed = MasterSeed;
         MasterSeed = masterSeed != 0 ? masterSeed : MakeRuntimeSeed();
         Debug.Log($"Generated new master seed: {MasterSeed}");
      }

      /// <summary>
      ///    Derives a deterministic non-zero seed from the master seed and a name.
      /// </summary>
      /// <param name="name">Name used to derive the seed.</param>
      /// <returns>A non-zero seed derived from the master seed and the name.</returns>
      public uint GenerateSeed(string name)
      {
         var nameHash = HashName(name);
         return DeriveNonZeroSeed(MasterSeed, nameHash);
      }

      /// <summary>
      ///    Mixes a base seed and index to produce a non-zero deterministic seed.
      /// </summary>
      private static uint DeriveNonZeroSeed(uint baseSeed, uint index)
      {
         // Mix the input values with constants to spread bits before hashing.
         var mixed = math.hash(new uint2(baseSeed ^ 0x9E3779B9u, index + 0x85EBCA6Bu));
         return mixed == 0 ? 1u : mixed;
      }

      /// <summary>
      ///    Creates a stable hash for the provided name using Unity's Hash128.
      /// </summary>
      private static uint HashName(string name)
      {
         if (string.IsNullOrEmpty(name))
         {
            return 0u;
         }

         // Convert the Hash128 to four uint values for consistent cross-platform hashing.
         var hash = Hash128.Compute(name);
         var hex = hash.ToString();
         var a = Convert.ToUInt32(hex[..8], 16);
         var b = Convert.ToUInt32(hex.Substring(8, 8), 16);
         var c = Convert.ToUInt32(hex.Substring(16, 8), 16);
         var d = Convert.ToUInt32(hex.Substring(24, 8), 16);
         return math.hash(new uint4(a, b, c, d));
      }

      /// <summary>
      ///    Generates a non-zero seed based on runtime values.
      /// </summary>
      private static uint MakeRuntimeSeed()
      {
         // Prefer non-Unity random sources so all RNG in the system is derived from Unity.Mathematics.Random.
         var s = (uint) (Environment.TickCount ^ Time.frameCount ^ (uint) DateTime.UtcNow.Ticks);
         return s == 0 ? 1u : s;
      }
   }
}