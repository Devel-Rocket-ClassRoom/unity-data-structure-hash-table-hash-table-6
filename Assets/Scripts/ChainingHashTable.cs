using System;
using System.Collections.Generic;
using UnityEngine;


public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private LinkedList<KeyValuePair<TKey, TValue>>[] ChainingIndex;    // 내부 링크드 리스트 배열
    private const double LoadFactorThreshold = 0.75;   // 최대 허용 로드팩터 (예: 0.75)
    private int count;
    public int capacity => ChainingIndex.Length;   // 현재 내부 배열 크기 (size)
    public int Count => count;  // 현재 저장된 데이터 개수
    public bool IsReadOnly => false;

    // 초기 생성자
    public ChainingHashTable(int Capacity = 16)
    {
        ChainingIndex = new LinkedList<KeyValuePair<TKey, TValue>>[Capacity];
    }

    private int GetIndex(TKey key)
    {
        int hash = key.GetHashCode();
        return (hash & 0x7fffffff) % capacity;
    }

    // 삽입 - 새로운 키-값 쌍 추가 (충돌 시 링크드 리스트로 연결)
    public void Add(TKey key, TValue value)
    {
        int index = GetIndex(key);

        if (ChainingIndex[index] == null)
            ChainingIndex[index] = new LinkedList<KeyValuePair<TKey, TValue>>();

        bool addedNew = Crash(key, value, index);   // 충돌 메서드 호출하여 링크드 리스트에 노드 추가, 새로운 데이터가 추가된 경우에만 true 반환

        if (addedNew)
        {
            count++;

            if ((double)count / Capacity > LoadFactorThreshold) // 로드팩터가 임계값을 초과하는 경우 내부 배열 크기 2배 증가
                Resize();
        }
    }
    // index변수에 입력된 키 인덱스를 계산하여 할당
    // 해당 슬롯이 비어있다면 새로운 링크드 리스트를 만듬


    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }
    // Add 메서드 오버라이드


    // 충돌 - 같은 슬롯의 노드에 링크드 리스트로 연결
    private void Crash(TKey key, TValue value, int index)
    {
        var list = ChainingIndex[index];
        var current = list.First;
        var newPair = new KeyValuePair<TKey, TValue>(key, value);

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Value.Key, key))
            {
                current.Value = newPair;   // 기존 키면 값 업데이트
                return false;
            }

            current = current.Next;
        }

        list.AddLast(newPair);   // 같은 키 없으면 새로 추가
        return true;
    }
    // list 변수에 해당 슬롯의 링크드 리스트 가져오기
    // 현재 node에 링크드 리스트의 첫 번째 노드를 할당
    // 새로운 key-value쌍을 newPair 변수에 할당
    // 현재 노드가 null이 아니면 현재 노드의 키와 입력된 키가 같은지 비교하고
    // 같으면 현재 노드의 값을 newPair로 업데이트 하고 false반환
    // 같지 않다면 현재 노드를 다음 노드로 이동
    // 현재 노드가 null이 되면 새로운 key-value를 링크드 리스트의 마지막으로 추가하고 true반환



    // Resize - 내부 배열 크기 증가 (size * 2) -> 기존 노드 재배치 (Rehashing)
    public void Resize()
    {
        int capacity2 = capacity * 2;   // size 2배 증가
        var newChainingIndex = new LinkedList<KeyValuePair<TKey, TValue>>[capacity2];   // 새로운 내부 링크드 리스트 배열 생성

        // 기존 노드 재배치 (Rehashing)
        foreach (var list in ChainingIndex)
        {
            if (list == null)
            {
                continue;
            }
            foreach (var kvp in list)
            {
                int newIndex = (kvp.Key.GetHashCode() & 0x7fffffff) % capacity2;   // 새로운 인덱스 계산
                if (newChainingIndex[newIndex] == null)   // 새로운 슬롯이 비어있는 경우
                {
                    newChainingIndex[newIndex] = new LinkedList<KeyValuePair<TKey, TValue>>();
                }
                newChainingIndex[newIndex].AddLast(kvp);   // 기존 노드 재배치
            }
        }
    }



    // 검색 - 키에 해당하는 값 반환 (충돌 시 링크드 리스트 탐색)
    public bool Find(TKey key, out TValue value)
    {
        value = default;
        int index = GetIndex(key);
        var list = ChainingIndex[index];

        if (list == null)   // 해당 슬롯에 노드가 없는 경우
        {
            return false;
        }
        foreach (var kvp in list)
        {
            if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
            {
                value = kvp.Value;
                return true;
            }
        }
        return false;
    }

    // 삭제 - 기존 노드 제거
    public bool Remove(TKey key)
    {
        if (Find(key, out TValue value))    // 키가 존재하는 경우에만 삭제
        {
            int index = GetIndex(key);
            var list = ChainingIndex[index];
            list.Remove(new KeyValuePair<TKey, TValue>(key, value));
            count--;
            return true;
        }
        return false;   // 키가 존재하지 않는 경우
    }


    //--- IEnumerable로 구현하여 모든 키-값 쌍을 반환 ---//
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < ChainingIndex.Length; i++)  // 내부 배열의 각 슬롯을 순회
        {
            var list = ChainingIndex[i];    // list변수에 해당 슬롯의 링크드 리스트 가져오기

            if (list == null)   // 슬롯이 비어있는 경우 다음 슬롯으로 넘어가기
                continue;

            foreach (var item in list)
            {
                yield return item;  // 해당 슬롯의 링크드 리스트에 있는 모든 키-값 쌍 반환
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
