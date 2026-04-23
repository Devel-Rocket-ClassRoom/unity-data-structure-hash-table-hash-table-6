using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private int size;
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

    public ICollection<TKey> Keys => throw new System.NotImplementedException();

    public ICollection<TValue> Values => throw new System.NotImplementedException();

    public int Count => count;

    public bool IsReadOnly => false;

    public SimpleHashTable(
        IEqualityComparer<TKey> keyComparer = null,
        IEqualityComparer<TValue> valueComparer = null
    )
    {
        size = 10;
        buckets = new KeyValuePair<TKey, TValue>[size];
        isOccupied = new bool[size];
        count = 0;

        this.keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
    }

    public void Add(TKey key, TValue value)
    {
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
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(TKey key)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new System.NotImplementedException();
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
        return Mathf.Abs(keyComparer.GetHashCode(key)) % size;
    }
}