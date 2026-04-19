using System.Collections.Generic;
using BepInEx;

namespace EasyDeliveryAPI.TestBed
{

    [BepInDependency("EasyDeliveryAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("EasyDeliveryAPI.TestBed", "EZDAPI Tester", "1")]
    public class TestBedPlugin : BaseUnityPlugin
    {
        static internal readonly Dictionary<string, string> Phase = new Dictionary<string, string>
        {
            ["0"]   = "Phase0_Initialization",
            ["0.1"] = "Phase0.1_TestingTitleScreenUI", // Will only run if a test save file is not detected. If one is detected then it skips to Phase 4.
            ["1"]   = "Phase1_WritingSaveData", // Creates a test save file and forces a load into it.
            ["1.1"] = "Phase1.1_TestingMainUI",
            ["2"]   = "Phase2_TestingWorldSpawns", // Forces the game to load into Mountain Town if it is not already there.
            ["3"]   = "Phase3_ReadingSavedData", // Forces a load into the title screen and then back into the save game. Checks if read data is the same as the written data.
            ["3.1"] = "Phase3.1_DeletingSaveData",
            ["4"]   = "Phase4_VerifyingDeletedSaveData", // Verifies that equipped modded items are unequipped if a mod is removed. Requires a game restart.
            ["5"]   = "Phase5_TestingWeirdUI", // Forces a load into the endless maze and tests the weird UI.
            ["6"]   = "Phase6_CleaningUp", // Forces a load into the title screen and deletes the test save file.
            ["7"]   = "Phase7_Summary" // Loads up a UI that prints a summary of the test results.
        };
        internal string CurrentPhase = Phase["0"];
        internal struct TestStruct
        {
            internal string CurrentPhase; // Used by the test bed to determine which stage of testing we're in.
            internal bool BoolTrue;
            internal bool BoolFalse;
            internal byte ByteMax;
            internal byte ByteMin;
            internal byte ByteRand;
            internal sbyte SByteMax;
            internal sbyte SByteMin;
            internal sbyte SByteZero;
            internal sbyte SByteRand;
            internal short ShortMax;
            internal short ShortMin;
            internal short ShortZero;
            internal short ShortRand;
            internal ushort UShortMax;
            internal ushort UShortMin;
            internal ushort UShortRand;
            internal int IntMax;
            internal int IntMin;
            internal int IntZero;
            internal int IntRand;
            internal uint UIntMax;
            internal uint UIntMin;
            internal uint UIntRand;
            internal long LongMax;
            internal long LongMin;
            internal long LongZero;
            internal long LongRand;
            internal ulong ULongMax;
            internal ulong ULongMin;
            internal ulong ULongRand;
            internal float FloatMax;
            internal float FloatMin;
            internal float FloatZero;
            internal float FloatNaN;
            internal float FloatInfinity;
            internal float FloatNegativeInfinity;
            internal float FloatRand;
            internal double DoubleMax;
            internal double DoubleMin;
            internal double DoubleZero;
            internal double DoubleNaN;
            internal double DoubleInfinity;
            internal double DoubleNegativeInfinity;
            internal double DoubleRand;
            internal string StringEmpty;
            internal string StringNull;
            internal string StringDeceptivelyNotNull;
            internal string StringSimple;
            internal string StringMean; // Contains complex characters that are often mishandled by serializers, such as newlines, tabs, and quotes.
            internal string StringFullTest; // Contains every character in the ASCII table.
            internal int[] IntArray;
            internal float[] FloatArray;
        }
        class TestSaveSystem : EasyDeliveryAPI.ModdedSaveSystem<TestStruct>
        {
            public TestSaveSystem() : base("EZDAPI_TestBed") {}
            public new void OnDataLoaded(Dictionary<string, string> string_data)
            {
                base.OnDataLoaded(string_data);
            }
        }
        private void Awake()
        {
            Logger.LogInfo("TestBed Loaded. Starting " + Phase["0"] + ".");
            TestStruct testStruct = new TestStruct
            {
                CurrentPhase = Phase["3"],
                BoolTrue = true,
                BoolFalse = false,
                ByteMax = byte.MaxValue,
                ByteMin = byte.MinValue,
                ByteRand = RNG.GetByte(),
                SByteMax = sbyte.MaxValue,
                SByteMin = sbyte.MinValue,
                SByteZero = 0,
                SByteRand = RNG.GetSByte(),
                ShortMax = short.MaxValue,
                ShortMin = short.MinValue,
                ShortZero = 0,
                ShortRand = RNG.GetShort(),
                UShortMax = ushort.MaxValue,
                UShortMin = ushort.MinValue,
                UShortRand = RNG.GetUShort(),
                IntMax = int.MaxValue,
                IntMin = int.MinValue,
                IntZero = 0,
                IntRand = RNG.GetInt(),
                UIntMax = uint.MaxValue,
                UIntMin = uint.MinValue,
                UIntRand = RNG.GetUInt(),
                LongMax = long.MaxValue,
                LongMin = long.MinValue,
                LongZero = 0L,
                LongRand = RNG.GetLong(),
                ULongMax = ulong.MaxValue,
                ULongMin = ulong.MinValue,
                ULongRand = RNG.GetULong(),
                FloatMax = float.MaxValue,
                FloatMin = float.MinValue,
                FloatZero = 0f,
                FloatNaN = float.NaN,
                FloatInfinity = float.PositiveInfinity,
                FloatNegativeInfinity = float.NegativeInfinity,
                FloatRand = RNG.GetFloat(),
                DoubleMax = double.MaxValue,
                DoubleMin = double.MinValue,
                DoubleZero = 0d,
                DoubleNaN = double.NaN,
                DoubleInfinity = double.PositiveInfinity,
                DoubleNegativeInfinity = double.NegativeInfinity,
                DoubleRand = RNG.GetDouble(), // Note that this will lose precision due to the nature of floating-point numbers.
                StringEmpty = string.Empty,
                StringNull = null,
                StringDeceptivelyNotNull = "null",
                StringSimple = "EZDAPI is great!",
                StringMean = "This\n\tis the \"worst\" string.\n_&@#$%^&*()_+|~`\':;?/>.<,\r\\\b"
            };

            // Add the entire ascii table into the string
            // Yes, I know it's 16-bit Unicode, but I don't feel like saving 131KB of characters for an overkill string test.
            testStruct.StringFullTest = string.Empty;
            for (byte i = 0; i < 255; i++)
            {
                testStruct.StringFullTest += (char)i;
            }

            // Create an array of integers and an array of floats with random data
            testStruct.IntArray = new int[16];
            testStruct.FloatArray = new float[16];
            for (int i = 0; i < 16; i++)
            {
                testStruct.IntArray[i] = RNG.GetInt();
                testStruct.FloatArray[i] = RNG.GetFloat();
            }

            // Create a title screen UI and check that it works.
            Logger.LogInfo("Finished. Starting " + Phase["0.1"] + ".");
        }
    }
}
