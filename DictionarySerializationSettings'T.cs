namespace Loxifi
{
    /// <summary>
    /// Defines how objects are serialized and deserialized
    /// </summary>
    /// <typeparam name="TKey">The key type of the dictionary</typeparam>
    /// <typeparam name="TValue">The value type of the dictionary</typeparam>
    public class DictionarySerializationSettings<TKey, TValue>
    {
        /// <summary>
        /// A character to use when separating key value pairs in the underlying dictionary
        /// </summary>
        public char ItemSeparator { get; set; } = '\t';

        /// <summary>
        /// The serialization settings for the key. <see cref="SerializationSettings{T}"/>
        /// </summary>
        public SerializationSettings<TKey> KeySerialization { get; set; } = new();

        /// <summary>
        /// The serialization settings for the value. <see cref="SerializationSettings{T}"/>
        /// </summary>
        public SerializationSettings<TValue> ValueSerialization { get; set; } = new();
    }
}