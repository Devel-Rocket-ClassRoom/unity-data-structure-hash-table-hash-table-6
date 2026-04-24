using System.Linq;
using UnityEngine;

public class ChainingTest : MonoBehaviour
{
    void Start()
    {
        ChainingHashTable<string, int> ChainingTable = new ChainingHashTable<string, int>
        {
            // ─────────────────────────────────────────────
            // 1. 기본 Add / 조회
            // ─────────────────────────────────────────────
            { "1", 100 },
            { "2", 200 },
            { "3", 300 }
        };

        Debug.Assert(ChainingTable.Count == 3, "Count should be 3");
        Debug.Assert(ChainingTable["1"] == 100, "ChainingTable['1'] should be 100");
        Debug.Assert(ChainingTable["2"] == 200, "ChainingTable['2'] should be 200");
        Debug.Assert(ChainingTable["3"] == 300, "ChainingTable['3'] should be 300");


        // ─────────────────────────────────────────────
        // 2. ContainsKey / Contains
        // ─────────────────────────────────────────────
        Debug.Assert(ChainingTable.ContainsKey("1"), "ContainsKey('1') should return true.");
        Debug.Assert(ChainingTable.ContainsKey("4"), "ContainsKey('4') should return false.");

        Debug.Assert(ChainingTable.Contains(new System.Collections.Generic.KeyValuePair<string, int>("1", 100)));
        Debug.Assert(!ChainingTable.Contains(new System.Collections.Generic.KeyValuePair<string, int>("1", 999)));
        Debug.Log("[PASS] ContainsKey / Contains");


        // ─────────────────────────────────────────────
        // 3. TryGetValue
        // ─────────────────────────────────────────────
        bool found = ChainingTable.TryGetValue("2", out int val);
        Debug.Assert(found && val == 200, "TryGetValue('2') should return 200");

        bool notFound = ChainingTable.TryGetValue("99", out int missing);
        Debug.Assert(!notFound && missing == 0, "TryGetValue('99') should fail");
        Debug.Log("[PASS] TryGetValue");


        // ─────────────────────────────────────────────
        // 4. Remove
        // ─────────────────────────────────────────────
        bool removed = ChainingTable.Remove("2");
        Debug.Assert(removed, "Remove('2') should return true");
        Debug.Assert(ChainingTable.Count == 2, "Count should be 2 after remove");
        Debug.Assert(!ChainingTable.ContainsKey("2"), "Key '2' should not exist after remove");

        bool removeFail = ChainingTable.Remove("99");
        Debug.Assert(!removeFail, "Remove('99') should return false");
        Debug.Log("[PASS] Remove");


        // ─────────────────────────────────────────────
        // 5. Keys / Values 컬렉션
        // ─────────────────────────────────────────────
        Debug.Assert(ChainingTable.Keys.Count == ChainingTable.Count, "Keys count mismatch");
        Debug.Assert(ChainingTable.Values.Count == ChainingTable.Count, "Values count mismatch");
        Debug.Log("[PASS] Keys / Values 컬렉션");


        // ─────────────────────────────────────────────
        // 6. Clear
        // ─────────────────────────────────────────────
        ChainingTable.Clear();
        Debug.Assert(ChainingTable.Count == 0, "Count should be 0 after Clear");
        Debug.Assert(!ChainingTable.ContainsKey("1"), "Key '1' should not exist after Clear");
        Debug.Log("[PASS] Clear");


        // ─────────────────────────────────────────────
        // 7. Resize — 부하율 0.7 초과 시 자동 리사이즈
        //    초기 capacity 10이므로 7개 삽입 시 리사이즈 트리거
        // ─────────────────────────────────────────────
        var ChainingTable2 = new ChainingHashTable<string, int>();
        for (int i = 0; i < 8; i++)
            ChainingTable2.Add(i.ToString(), i * 10);

        Debug.Assert(ChainingTable2.Count == 8, "Count should be 8 after resize");
        for (int i = 0; i < 8; i++)
            Debug.Assert(ChainingTable2[i.ToString()] == i * 10, $"Value mismatch after resize: key={i}");
        Debug.Log("[PASS] Resize");


        // ─────────────────────────────────────────────
        // 8. 존재하지 않는 키 접근 → KeyNotFoundException
        // ─────────────────────────────────────────────
        try
        {
            _ = ChainingTable["999"];
            Debug.LogError("[FAIL] KeyNotFoundException이 발생해야 합니다");
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            Debug.Log("[PASS] KeyNotFoundException");
        }


        // ─────────────────────────────────────────────
        // 9. null 키 → ArgumentNullException
        // ─────────────────────────────────────────────
        var ChainingTableStr = new ChainingHashTable<string, int>();
        try
        {
            ChainingTableStr.Add(null, 1);
            Debug.LogError("[FAIL] ArgumentNullException이 발생해야 합니다");
        }
        catch (System.ArgumentNullException)
        {
            Debug.Log("[PASS] ArgumentNullException (null key)");
        }


        // ─────────────────────────────────────────────
        // 10. foreach 열거
        // ─────────────────────────────────────────────
        var ChainingTable3 = new ChainingHashTable<string, int>
        {
            { "1", 1000 },
            { "2", 2000 },
            { "3", 3000 },
            { "4", 4000 }
        };

        int iterCount = 0;
        foreach (var pair in ChainingTable3)
        {
            Debug.Assert(pair.Value == int.Parse(pair.Key) * 1000, $"Enumerator value mismatch: {pair.Key}");
            iterCount++;
        }
        Debug.Assert(iterCount == 4, "foreach should iterate 4 items");
        Debug.Log("[PASS] foreach 열거");


        // ─────────────────────────────────────────────
        // 11. CopyTo
        // ─────────────────────────────────────────────
        var arr = new System.Collections.Generic.KeyValuePair<string, int>[4];
        ChainingTable3.CopyTo(arr, 0);
        Debug.Assert(arr[0].Key != null || arr[1].Key != null || arr[2].Key != null || arr[3].Key != null, "CopyTo should fill array");
        Debug.Log("[PASS] CopyTo");

        Debug.Log("===== 모든 테스트 통과 =====");
    }
}