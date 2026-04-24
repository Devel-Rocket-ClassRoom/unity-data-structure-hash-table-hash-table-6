using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private int capacity;
    private KeyValuePair<TKey, TValue>[] buckets;
    private bool[] isOccupied;
    private int count;

    private readonly IEqualityComparer<TKey> keyComparer;
    private readonly IEqualityComparer<TValue> valueComparer;

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"Key not found: {key}");
            }
        }
        set => Add(key, value);
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            foreach (var pair in this)
            {
                keys.Add(pair.Key);
            }
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();
            foreach (var pair in this)
            {
                values.Add(pair.Value);
            }
            return values;
        }
    }

    public int Count => count;

    public bool IsReadOnly => false;

    public SimpleHashTable(
        IEqualityComparer<TKey> keyComparer = null,
        IEqualityComparer<TValue> valueComparer = null
    )
    {
        capacity = 10;
        buckets = new KeyValuePair<TKey, TValue>[capacity];
        isOccupied = new bool[capacity];
        count = 0;

        this.keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
    }

    public void Add(TKey key, TValue value)
    {
        if ((float)count / capacity > 0.7f)
        {
            Resize();
        }

        int index = GetHash(key);

        if (isOccupied[index])
            throw new InvalidOperationException($"Collision at index {index}");

        buckets[index] = new KeyValuePair<TKey, TValue>(key, value);
        isOccupied[index] = true;
        count++;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        buckets = new KeyValuePair<TKey, TValue>[capacity];
        isOccupied = new bool[capacity];
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return TryGetValue(item.Key, out TValue value)
            && valueComparer.Equals(value, item.Value);
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var pair in this)
        {
            array[arrayIndex++] = pair;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (isOccupied[i])
            {
                yield return buckets[i];
            }
        }
    }

    public bool Remove(TKey key)
    {
        int index = GetHash(key);

        if (isOccupied[index] && keyComparer.Equals(buckets[index].Key, key))
        {
            buckets[index] = default;
            isOccupied[index] = false;
            count--;
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Contains(item) && Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = GetHash(key);

        if (isOccupied[index] && keyComparer.Equals(buckets[index].Key, key))
        {
            value = buckets[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // 키의 해시 값 얻기
    public int GetHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return Mathf.Abs(keyComparer.GetHashCode(key)) % capacity;
    }

    public int GetIndex(TKey key)
    {
        return GetHash(key);
    }

    public void Resize()
    {
        int newSize = capacity * 2;
        var oldBuckets = buckets;
        var oldOccupied = isOccupied;

        capacity = newSize;
        buckets = new KeyValuePair<TKey, TValue>[capacity];
        isOccupied = new bool[capacity];
        count = 0;

        for (int i = 0; i < oldBuckets.Length; i++)
        {
            if (oldOccupied[i])
                Add(oldBuckets[i].Key, oldBuckets[i].Value);
        }
    }
}