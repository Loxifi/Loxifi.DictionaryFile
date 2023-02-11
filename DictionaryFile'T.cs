namespace Loxifi
{
    /// <summary>
    /// An instance of a <see cref="Dictionary{TKey, TValue}"/> where 
    /// the key and value are the same type
    /// </summary>
    /// <typeparam name="T">The key value type</typeparam>
    public class DictionaryFile<T> : DictionaryFile<T, T>
    {
        /// <summary>
        /// Constructs a new instance of this class with the provided serialization settings
        /// </summary>
        /// <param name="path">The file path to use for the underlying data</param>
        /// <param name="dictionarySerializationSettings"></param>
        /// <param name="autoflush">
        /// If true, saves file every time the collection is modified.
        /// Safer, but has a performance penalty
        /// </param>
        public DictionaryFile(string path, DictionarySerializationSettings<T, T> dictionarySerializationSettings, bool autoflush = true) : base(path, dictionarySerializationSettings, autoflush)
        {
        }

        /// <summary>
        /// Constructs a new instance of this class with the default serialization settings
        /// </summary>
        /// <param name="path">The file path to use for the underlying data</param>
        /// <param name="autoflush">
        /// If true, saves file every time the collection is modified.
        /// Safer, but has a performance penalty
        /// </param>
        public DictionaryFile(string path, bool autoflush = true) : base(path, autoflush)
        {
        }
    }
}