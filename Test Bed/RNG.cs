namespace EasyDeliveryAPI.TestBed
{
    internal static class RNG
    {
        private static System.Random _random = new System.Random();

        internal static byte GetByte() => (byte)_random.Next(byte.MinValue, byte.MaxValue + 1);
        internal static sbyte GetSByte() => (sbyte)_random.Next(sbyte.MinValue, sbyte.MaxValue + 1);
        internal static short GetShort() => (short)_random.Next(short.MinValue, short.MaxValue + 1);
        internal static ushort GetUShort() => (ushort)_random.Next(ushort.MinValue, ushort.MaxValue + 1);
        internal static int GetInt() => _random.Next(int.MinValue, int.MaxValue); // This can't generate int.MaxValue, but oh well. ¯\_(ツ)_/¯
        internal static uint GetUInt() => (uint)_random.Next(int.MinValue, int.MaxValue) | (uint)_random.Next(int.MinValue, (int)((long)uint.MaxValue >> 32)) << 32;
        internal static ulong GetULong() => ((ulong)GetUInt()) | (((ulong)GetUInt()) << 32);
        internal static long GetLong() => (long)GetULong();
        internal static float GetFloat() => (float)(_random.NextDouble() * float.MaxValue); // This is subject to serious float imprecision issues, but it's good enough for our use case.
        internal static double GetDouble() => _random.NextDouble() * double.MaxValue; // This is subject to serious float imprecision issues, but it's good enough for our use case.
    }
}