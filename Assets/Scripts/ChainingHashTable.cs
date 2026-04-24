
using System;
using System.Collections;
using System.Collections.Generic;


public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private LinkedList<KeyValuePair<TKey, TValue>>[] ChainingIndex;    // 내부 링크드 리스트 배열
    private const double LoadFactorThreshold = 0.75;   // 최대 허용 로드팩터 (예: 0.75)
    public int Capacity => ChainingIndex.Length;   // 현재 내부 배열 크기 (size)
    public int Count { get; private set; }   // 현재 데이터 개수
    public bool IsReadOnly => false;

    public ICollection<TKey> Keys
    {
        get
        {
            List<TKey> keys = new List<TKey>();

            foreach (var item in this)
            {
                keys.Add(item.Key);
            }

            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            List<TValue> values = new List<TValue>();

            foreach (var item in this)
            {
                values.Add(item.Value);
            }

            return values;
        }
    }

    public TValue this[TKey key]
    {

        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            throw new KeyNotFoundException($"키 '{key}'가 존재하지 않습니다.");
        }
        set
        {
            if (EqualityComparer<TKey>.Default.Equals(key, default))
            {
                this[key] = value;   // 키가 존재할 경우 업데이트
            }
            else
            {
                Add(key, value);    // 키가 존재하지 않을 경우 추가
            }
        }
    }

    // 초기 생성자 - 초기 크기 16으로 설정
    public ChainingHashTable(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "용량은 양수여야 합니다.");
        }
        ChainingIndex = new LinkedList<KeyValuePair<TKey, TValue>>[capacity];
    }
    public ChainingHashTable() : this(16)
    {
    }

    private int GetIndex(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "키는 null일 수 없습니다.");
        }
        int hash = key.GetHashCode();
        return (hash & 0x7fffffff) % Capacity;
    }

    // 삽입 - 새로운 키-값 쌍 추가 (충돌 시 링크드 리스트로 연결)
    public void Add(TKey key, TValue value)
    {
        int index = GetIndex(key);

        // 해당 슬롯이 비어있다면 새로운 링크드 리스트를 만들고 키-값 쌍 추가
        if (ChainingIndex[index] == null)
        {

            ChainingIndex[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            ChainingIndex[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            Count++;
        }
        else    // 해당 슬롯이 비어있지 않다면 충돌 처리
        {
            bool addedNew = HandleCollision(key, value, index);   // 충돌 메서드 호출하여 링크드 리스트에 노드 추가
            if (addedNew)
            {
                Count++;
            }
        }

        if ((double)Count / Capacity > LoadFactorThreshold) // 로드팩터가 임계값을 초과하는 경우 내부 배열 크기 2배 증가
        {
            Resize();
        }
    }
    // GetIndex로 키에 대한 인덱스 계산
    // 해당 슬롯이 비어있다면 새로운 링크드 리스트를 만들고 키-값 쌍 추가
    // 해당 슬롯이 비어있지 않다면 충돌 처리 메서드 호출하여 링크드 리스트에 노드 추가 후 데이터 개수 증가
    // 로드팩터 체크하여 필요 시 Resize 메서드 호출

    public void Add(KeyValuePair<TKey, TValue> item)    // Add 메서드 오버라이드
    {
        Add(item.Key, item.Value);
    }



    // 충돌 - 같은 슬롯의 노드에 링크드 리스트로 연결
    private bool HandleCollision(TKey key, TValue value, int index)
    {
        var list = ChainingIndex[index];
        var current = list.First;
        var newPair = new KeyValuePair<TKey, TValue>(key, value);

        while (current != null)
        {
            // 중복 키 처리 - 기존 키가 있으면 오류
            if (EqualityComparer<TKey>.Default.Equals(current.Value.Key, key))
            {
                // 오류
                throw new ArgumentNullException($"키 '{key}'는 이미 존재합니다.");
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
    private void Resize()
    {
        int Capacity2 = Capacity * 2;   // size 2배 증가
        var newChainingIndex = new LinkedList<KeyValuePair<TKey, TValue>>[Capacity2];   // 새로운 내부 링크드 리스트 배열 생성

        // 기존 노드 재배치 (Rehashing)
        foreach (var list in ChainingIndex)
        {
            if (list == null)
            {
                continue;
            }
            foreach (var kvp in list)
            {
                int newIndex = (kvp.Key.GetHashCode() & 0x7fffffff) % Capacity2;   // 새로운 인덱스 계산
                if (newChainingIndex[newIndex] == null)   // 새로운 슬롯이 비어있는 경우
                {
                    newChainingIndex[newIndex] = new LinkedList<KeyValuePair<TKey, TValue>>();
                }
                newChainingIndex[newIndex].AddLast(kvp);   // 기존 노드가 있는 슬롯이 없는 경우
            }
        }
        ChainingIndex = newChainingIndex;   // 내부 배열 교체
    }
    // 로드팩터가 임계값을 초과하는 경우 Capacity2 변수에 size 2배 할당
    // size 2배인 상태의 새로운 링크드 리스트 배열 생성
    // Rehashing - 기존 노드 재배치 
    // 기존 노드가 있는 슬롯을 순회하여 각 노드의 새로운 인덱스 계산
    // 새로운 슬롯이 비어있는 경우 새로운 링크드 리스트 생성 후 기존 노드 추가
    // 기존 노드가 있는 슬롯이 없는 경우 다음 슬롯으로 넘어가기
    // 모든 노드 재배치 후 내부 배열 교체


    public void Clear()
    {
        ChainingIndex = new LinkedList<KeyValuePair<TKey, TValue>>[Capacity];   // 내부 배열 초기화
        Count = 0;  // 데이터 개수 초기화
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)   // 키-값 쌍 존재 여부 확인
    {
        if (TryGetValue(item.Key, out TValue value))
        {
            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }
        return false;
    }
    // TryGetValue로 키 존재 여부 확인 후 값 비교하여 키-값 쌍 존재 여부 반환

    public bool ContainsKey(TKey key)   // 키 존재 여부 확인
    {
        return TryGetValue(key, out _);   // TryGetValue로 키 존재 여부 확인
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var item in this)
        {
            array[arrayIndex++] = item;   // 배열에 키-값 쌍 복사
        }
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

    // 삭제 - 기존 노드 제거
    public bool Remove(TKey key)
    {
        int index = GetIndex(key);
        var list = ChainingIndex[index];

        if (list == null)   // 해당 슬롯에 노드가 없는 경우
        {
            return false;
        }
        var current = list.First;   // 현재 노드에 링크드 리스트의 첫 번째 노드 할당

        while (current != null) // 해당 슬롯의 링크드 리스트를 순회하면서 키가 있는 지 확인
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Value.Key, key))  // 현재 노드의 키와 입력된 키가 같은 경우
            {
                list.Remove(current);   // 해당 노드 제거
                Count--;    // 데이터 개수 감소
                return true;
            }
            current = current.Next; // 삭제 안됐을 때 다음 노드로 이동하면서 계속 찾기
        }
        return false;   // 키를 찾지 못한 경우
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item)) // 키-값 쌍이 존재하는 경우 Remove 메서드 호출하여 키 제거
        {
            return Remove(item.Key);
        }

        return false;
    }


    public bool TryGetValue(TKey key, out TValue value) // 키가 있는 지 확인하면서 있으면 값도 반환
    {
        value = default;
        int index = GetIndex(key);
        var list = ChainingIndex[index];

        if (list == null)   // 해당 슬롯에 노드가 없는 경우
        {
            return false;
        }
        foreach (var kvp in list)   // 해당 슬롯의 링크드 리스트를 순회하면서 키가 있는지 확인
        {
            if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))    // 키가 있는 경우 값 반환
            {
                value = kvp.Value;
                return true;
            }
        }
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
