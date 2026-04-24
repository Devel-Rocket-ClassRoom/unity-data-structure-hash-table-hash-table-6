using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class OpenAddressingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly double LoadFactor = 0.6;

    private KeyValuePair<TKey, TValue>[] buckets;
    private bool[] occupied;
    private bool[] deleted;

    private ProvingMode mode;


    public OpenAddressingHashTable()
    {
        buckets = new KeyValuePair<TKey, TValue>[16];
        occupied = new bool[16];
        deleted = new bool[16];
        Count = 0;
        mode = ProvingMode.DoubleHash;
    }

    public OpenAddressingHashTable(ProvingMode mode)
    {
        buckets = new KeyValuePair<TKey, TValue>[16];
        occupied = new bool[16];
        deleted = new bool[16];
        Count = 0;
        this.mode = mode;
    }


    public TValue this[TKey key]
    {
        get
        {
            if (Search(key, out int idx))
                return buckets[idx].Value;
            else
                throw new KeyNotFoundException("Å° ¾øÀ½");
        }
        set
        {
            Add(key, value);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            List<TKey> list = new List<TKey>();

            for (int i = 0; i<buckets.Length; i++)
            {
                if (occupied[i])
                {
                    list.Add(buckets[i].Key);
                }
            }

            return list;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            List<TValue> list = new List<TValue>();

            for (int i = 0; i < buckets.Length; i++)
            {
                if (occupied[i])
                {
                    list.Add(buckets[i].Value);
                }
            }

            return list;
        }
    }

    public int Count { get; private set; }
    public int Capacity { get { return buckets.Length; } }
    public bool IsReadOnly => false;


    public void Add(TKey key, TValue value)
    {
        Add(KeyValuePair.Create(key, value));
    }
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        int idx = GetHash(item.Key) % Capacity;
        int tryCount = 0;
        int? firstDeletedIndex = null;

        while (tryCount < Capacity)
        {
            if (idx >= Capacity) break;

            if (occupied[idx])
            {
                if (buckets[idx].Key.Equals(item.Key))
                {
                    buckets[idx] = item;
                    return;
                }
                else
                {
                    idx = Proving(item.Key, ++tryCount);
                    continue;
                }
            }
            else if (deleted[idx])
            {
                if (firstDeletedIndex == null) firstDeletedIndex = idx;
                idx = Proving(item.Key, ++tryCount);
                continue;
            }
            else break;
        }

        if (firstDeletedIndex != null)
        {
            idx = firstDeletedIndex.Value;
        }

        InsertInto(idx, item);
    }

    void InsertInto(int idx, KeyValuePair<TKey, TValue> item)
    {
        buckets[idx] = item;
        occupied[idx] = true;
        deleted[idx] = false;
        Count++;

        if ((double)Count / Capacity > LoadFactor)
        {
            Resize();
        }
    }


    public void Clear()
    {
        buckets = new KeyValuePair<TKey, TValue>[16];
        occupied = new bool[16];
        deleted = new bool[16];
        Count = 0;
    }


    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }
    public bool ContainsKey(TKey key)
    {
        return Search(key, out _);
    }


    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }


    public bool Remove(TKey key)
    {
        bool result = Search(key, out int idx);

        if (result)
        {
            occupied[idx] = false;
            deleted[idx] = true;
            Count--;
        }

        return result;
    }
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }


    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        bool isFind = Search(key, out int idx);

        value = buckets[idx].Value;

        return isFind;
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    void Resize()
    {
        int newCapacity = Capacity * 2;
        KeyValuePair<TKey, TValue>[] oldBuckets = buckets;
        bool[] oldOccupied = occupied;
        bool[] oldDeleted = deleted;

        buckets = new KeyValuePair<TKey, TValue>[newCapacity];
        occupied = new bool[newCapacity];
        deleted = new bool[newCapacity];
        Count = 0;

        for (int i = 0; i < oldBuckets.Length; i++)
        {
            if (oldOccupied[i] && !oldDeleted[i])
            {
                Add(oldBuckets[i]);
            }
        }
    }

    private int Proving(TKey key, int retryCount)
    {
        switch (mode)
        {
            case ProvingMode.Linear:
                return (GetHash(key) + retryCount) % Capacity;
            case ProvingMode.Quadratic:
                return (GetHash(key) + retryCount * retryCount) % Capacity;
            case ProvingMode.DoubleHash:
                return (GetHash(key) + retryCount * GetSecondaryHash(key)) % Capacity;
        }

        throw new InvalidOperationException();
    }

    public int GetHash(TKey key)
    {
        int hash = key.GetHashCode();
        return hash & 0x7FFFFFFF;
    }
    public int GetSecondaryHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode();
        return 1 + ((hash & 0x7fffffff) % (Capacity - 1));
    }

    private bool Search(TKey key, out int index)
    {
        int idx = GetHash(key) % Capacity;
        int tryCount = 0;

        while (tryCount < Capacity)
        {
            if (idx >= Capacity) break;

            if (occupied[idx])
            {
                if (buckets[idx].Key.Equals(key))
                {
                    index = idx;
                    return true;
                }
                else
                {
                    idx = Proving(key, ++tryCount);
                    continue;
                }
            }
            else if (deleted[idx])
            {
                idx = Proving(key, ++tryCount);
                continue;
            }

            break;
        }

        index = -1;
        return false;
    }

    public int GetIndex(TKey key)
    {
        if (Search(key, out int index))
            return index;
        else
            throw new KeyNotFoundException();
    }
}

public enum ProvingMode
{
    Linear, Quadratic, DoubleHash
}