using System.Collections.Generic;

public abstract class BufferContainer<TKey, TValue> {
    protected readonly Dictionary<TKey, TValue> buffer = new();

    public virtual bool Add(TKey key, TValue value) {
        return buffer.TryAdd(key, value);
    }

    public virtual bool Remove(TKey key) {
        return buffer.Remove(key);
    }

    public virtual void Clear() {
        buffer.Clear();
    }

    public virtual bool TryGet(TKey key, out TValue value) {
        return buffer.TryGetValue(key, out value);
    }

    public bool ContainsKey(TKey key) => buffer.ContainsKey(key);
    
    public virtual IReadOnlyDictionary<TKey, TValue> GetAll() {
        return buffer;
    }

    public int Count => buffer.Count;
}
