namespace Loxifi
{
    /// <summary>
    /// An instance of a <see cref="Dictionary{TKey, TValue}"/> where
    /// both the key and value are strings
    /// </summary>
    public class DictionaryFile : DictionaryFile<string>
    {
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