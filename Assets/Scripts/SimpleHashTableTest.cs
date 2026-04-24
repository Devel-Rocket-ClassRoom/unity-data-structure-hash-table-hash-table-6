using UnityEngine;

public class SimpleHashTableTest : MonoBehaviour
{
    void Start()
    {
        SimpleHashTable<int, int> sht = new SimpleHashTable<int, int>();

        // ─────────────────────────────────────────────
        // 1. 기본 Add / 조회
        // ─────────────────────────────────────────────
        sht.Add(1, 100);
        sht.Add(2, 200);
        sht.Add(3, 300);

        Debug.Assert(sht.Count == 3, "Count should be 3");
        Debug.Assert(sht[1] == 100, "sht[1] should be 100");
        Debug.Assert(sht[2] == 200, "sht[2] should be 200");
        Debug.Assert(sht[3] == 300, "sht[3] should be 300");
        Debug.Log("[PASS] 기본 Add / 조회");

        // ─────────────────────────────────────────────
        // 2. ContainsKey / Contains
        // ─────────────────────────────────────────────
        Debug.Assert(sht.ContainsKey(1), "ContainsKey(1) should be true");
        Debug.Assert(!sht.ContainsKey(99), "ContainsKey(99) should be false");
        Debug.Assert(sht.Contains(new System.Collections.Generic.KeyValuePair<int, int>(1, 100)));
        Debug.Assert(!sht.Contains(new System.Collections.Generic.KeyValuePair<int, int>(1, 999)));
        Debug.Log("[PASS] ContainsKey / Contains");

        // ─────────────────────────────────────────────
        // 3. TryGetValue
        // ─────────────────────────────────────────────
        bool found = sht.TryGetValue(2, out int val);
        Debug.Assert(found && val == 200, "TryGetValue(2) should return 200");

        bool notFound = sht.TryGetValue(99, out int missing);
        Debug.Assert(!notFound && missing == 0, "TryGetValue(99) should fail");
        Debug.Log("[PASS] TryGetValue");

        // ─────────────────────────────────────────────
        // 4. Remove
        // ─────────────────────────────────────────────
        bool removed = sht.Remove(2);
        Debug.Assert(removed, "Remove(2) should return true");
        Debug.Assert(sht.Count == 2, "Count should be 2 after remove");
        Debug.Assert(!sht.ContainsKey(2), "Key 2 should not exist after remove");

        bool removeFail = sht.Remove(99);
        Debug.Assert(!removeFail, "Remove(99) should return false");
        Debug.Log("[PASS] Remove");

        // ─────────────────────────────────────────────
        // 5. Keys / Values 컬렉션
        // ─────────────────────────────────────────────
        Debug.Assert(sht.Keys.Count == sht.Count, "Keys count mismatch");
        Debug.Assert(sht.Values.Count == sht.Count, "Values count mismatch");
        Debug.Log("[PASS] Keys / Values 컬렉션");

        // ─────────────────────────────────────────────
        // 6. Clear
        // ─────────────────────────────────────────────
        sht.Clear();
        Debug.Assert(sht.Count == 0, "Count should be 0 after Clear");
        Debug.Assert(!sht.ContainsKey(1), "Key 1 should not exist after Clear");
        Debug.Log("[PASS] Clear");

        // ─────────────────────────────────────────────
        // 7. Resize — 부하율 0.7 초과 시 자동 리사이즈
        //    초기 capacity 10이므로 7개 삽입 시 리사이즈 트리거
        // ─────────────────────────────────────────────
        var sht2 = new SimpleHashTable<int, int>();
        for (int i = 0; i < 8; i++)
            sht2.Add(i, i * 10);

        Debug.Assert(sht2.Count == 8, "Count should be 8 after resize");
        for (int i = 0; i < 8; i++)
            Debug.Assert(sht2[i] == i * 10, $"Value mismatch after resize: key={i}");
        Debug.Log("[PASS] Resize");




        // ─────────────────────────────────────────────
        // 8. 존재하지 않는 키 접근 → KeyNotFoundException
        // ─────────────────────────────────────────────
        try
        {
            _ = sht[999];
            Debug.LogError("[FAIL] KeyNotFoundException이 발생해야 합니다");
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            Debug.Log("[PASS] KeyNotFoundException");
        }

        // ─────────────────────────────────────────────
        // 9. null 키 → ArgumentNullException
        // ─────────────────────────────────────────────
        var shtStr = new SimpleHashTable<string, int>();
        try
        {
            shtStr.Add(null, 1);
            Debug.LogError("[FAIL] ArgumentNullException이 발생해야 합니다");
        }
        catch (System.ArgumentNullException)
        {
            Debug.Log("[PASS] ArgumentNullException (null key)");
        }

        // ─────────────────────────────────────────────
        // 10. foreach 열거
        // ─────────────────────────────────────────────
        var sht3 = new SimpleHashTable<int, int>();
        sht3.Add(1, 1000);
        sht3.Add(2, 2000);
        sht3.Add(3, 3000);
        sht3.Add(4, 4000);

        int iterCount = 0;
        foreach (var pair in sht3)
        {
            Debug.Assert(pair.Value == pair.Key * 1000, $"Enumerator value mismatch: {pair.Key}");
            iterCount++;
        }
        Debug.Assert(iterCount == 4, "foreach should iterate 4 items");
        Debug.Log("[PASS] foreach 열거");

        // ─────────────────────────────────────────────
        // 11. CopyTo
        // ─────────────────────────────────────────────
        var arr = new System.Collections.Generic.KeyValuePair<int, int>[4];
        sht3.CopyTo(arr, 0);
        Debug.Assert(arr[0].Key != 0 || arr[1].Key != 0 || arr[2].Key != 0 || arr[3].Key != 0, "CopyTo should fill array");
        Debug.Log("[PASS] CopyTo");

        Debug.Log("===== 모든 테스트 통과 =====");
    }
}