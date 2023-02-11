using System.Collections;

namespace Loxifi
{
    /// <summary>
    /// An IDictionary implementation that saves data to a file on disk 
    /// for persistence
    /// </summary>
    /// <typeparam name="TKey">The type to use in memory for the key</typeparam>
    /// <typeparam name="TValue">The type to use in memory for the value</typeparam>
    public class DictionaryFile<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
    {
        /// <summary>
        /// gets/sets the value matching the specified key
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <returns>The value found</returns>
        public TValue this[TKey key]
        {
            get => _backingDictionary[key];
            set
            {
                _ = Remove(key);

                Add(key, value);
            }
        }

        private readonly IDictionary<TKey, TValue> _backingDictionary;

        private readonly ListFile _backingFile;

        private readonly DictionarySerializationSettings<TKey, TValue> _dictionarySerializationSettings;

        /// <summary>
        /// Count of the items in the underlying collection
        /// </summary>
        public int Count => _backingDictionary.Count;

        /// <summary>
        /// True if read only
        /// Should never be true
        /// </summary>
        public bool IsReadOnly => _backingDictionary.IsReadOnly;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _backingDictionary.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => _backingDictionary.Values;

        private SerializationSettings<TKey> KeySerialization => _dictionarySerializationSettings.KeySerialization;

        private SerializationSettings<TValue> ValueSerialization => _dictionarySerializationSettings.ValueSerialization;

        /// <summary>
        /// Constructs a new instance of this class with the default serialization settings
        /// </summary>
        /// <param name="path">The file path to use for the underlying data</param>
        /// <param name="autoflush">
        /// If true, saves file every time the collection is modified.
        /// Safer, but has a performance penalty
        /// </param>
        public DictionaryFile(string path, bool autoflush = true) : this(path, new DictionarySerializationSettings<TKey, TValue>(), autoflush)
        {
        }

        /// <summary>
        /// Constructs a new instance of this class with the provided serialization settings
        /// </summary>
        /// <param name="path">The file path to use for the underlying data</param>
        /// <param name="dictionarySerializationSettings"></param>
        /// <param name="autoflush">
        /// If true, saves file every time the collection is modified.
        /// Safer, but has a performance penalty
        /// </param>
        public DictionaryFile(string path, DictionarySerializationSettings<TKey, TValue> dictionarySerializationSettings, bool autoflush = true)
        {
            _dictionarySerializationSettings = dictionarySerializationSettings;

            _backingFile = new ListFile(path, autoflush);
            _backingDictionary = new Dictionary<TKey, TValue>();

            foreach (string line in _backingFile)
            {
                _backingDictionary.Add(
                    //TODO: Only cut at the first instance of the item separator to allow for the 
                    //value to contain it.
                    KeySerialization.Deserialize(line.Split(_dictionarySerializationSettings.ItemSeparator)[0]), 
                    ValueSerialization.Deserialize(line.Split(_dictionarySerializationSettings.ItemSeparator)[1])
                );
            }
        }

        /// <summary>
        /// Flushes the underlying collection to disk
        /// </summary>
        public void Flush() => _backingFile.Flush();

        /// <summary>
        /// Adds the specified key value pair to the underlying collection
        /// </summary>
        /// <param name="key">The item key</param>
        /// <param name="value">The item value</param>
        public void Add(TKey key, TValue value)
        {
            _backingDictionary.Add(key, value);
            _backingFile.Add(GetRow(key, value));
        }

        /// <summary>
        /// Adds the specified key value pair to the underlying collection
        /// </summary>
        /// <param name="item">The key value pair to add</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _backingDictionary.Add(item);
            _backingFile.Add(GetRow(item));
        }

        /// <summary>
        /// Clears the underlying collection
        /// </summary>
        public void Clear()
        {
            _backingDictionary.Clear();
            _backingFile.Clear();
        }

        /// <summary>
        /// Returns true if the backing collection contains the 
        /// specified key value pair
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <returns>
        /// True if the backing collection contains the 
        /// specified key value pair
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) => _backingDictionary.Contains(item);

        /// <summary>
        /// Returns true if the backing collection contains the specified key
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>True if the backing collection contains the key</returns>
        public bool ContainsKey(TKey key) => _backingDictionary.ContainsKey(key);

        /// <summary>
        /// Copies the backing dictionary to the specified key value pair array
        /// </summary>
        /// <param name="array">The array to copy to</param>
        /// <param name="arrayIndex">The source index to start at</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _backingDictionary.CopyTo(array, arrayIndex);

        /// <summary>
        /// Gets the enumerator for the underlying dictionary
        /// </summary>
        /// <returns>The enumerator for the underlying dictionary</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _backingDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_backingDictionary).GetEnumerator();

        /// <summary>
        /// Removes the item with the specified key from the underlying dictionary
        /// </summary>
        /// <param name="key">
        /// The key to check for
        /// </param>
        /// <returns>True if the key was found and the item was removed</returns>
        public bool Remove(TKey key)
        {
            bool v = _backingDictionary.Remove(key);

            string sKey = KeySerialization.Serialize(key);

            foreach (string line in _backingFile)
            {
                if (line.StartsWith($"{sKey}{_dictionarySerializationSettings.ItemSeparator}", StringComparison.OrdinalIgnoreCase))
                {
                    _ = _backingFile.Remove(line);
                    break;
                }
            }

            return v;
        }

        /// <summary>
        /// Attempts to remove a key value pair from the backing dictionary 
        /// </summary>
        /// <param name="item">They key value pair to remove</param>
        /// <returns>True if the key value pair was found and removed</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool v = _backingDictionary.Remove(item);
            _ = _backingFile.Remove($"{item.Key}{_dictionarySerializationSettings.ItemSeparator}{item.Value}");
            return v;
        }

        /// <summary>
        /// Tries to get an item with the specified key from the underlying dictionary
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <param name="value">The value returned if its found</param>
        /// <returns>True if the key was found, otherwise false</returns>
        public bool TryGetValue(TKey key, out TValue value) => _backingDictionary.TryGetValue(key, out value);

        private string GetRow(TKey key, TValue value) => $"{KeySerialization.Serialize(key)}{_dictionarySerializationSettings.ItemSeparator}{ValueSerialization.Serialize(value)}";

        private string GetRow(KeyValuePair<TKey, TValue> item) => GetRow(item.Key, item.Value);

        /// <summary>
        /// Flushes the data to disk and disposes of the object
        /// </summary>
        public void Dispose() => ((IDisposable)_backingFile).Dispose();
    }
}