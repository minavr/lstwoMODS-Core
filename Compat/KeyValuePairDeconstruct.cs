using System.Collections.Generic;

namespace lstwoMODS_Core.Compat
{
    /// <summary>
    /// .NET Framework never shipped <c>KeyValuePair&lt;TKey,TValue&gt;.Deconstruct</c>
    /// (it arrived with .NET Core 2.0 / netstandard 2.1), so <c>foreach (var (k, v) in dict)</c>
    /// does not compile against the net472 reference assemblies. C# accepts an extension
    /// method for deconstruction, so this shim restores the pattern.
    /// </summary>
    public static class KeyValuePairDeconstruct
    {
        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key   = pair.Key;
            value = pair.Value;
        }
    }
}
