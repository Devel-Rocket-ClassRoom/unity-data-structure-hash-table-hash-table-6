using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    int Capacity { get; }
    int GetIndex(TKey key);
}
