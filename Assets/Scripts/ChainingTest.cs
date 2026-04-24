using System.Linq;
using UnityEngine;

public class ChainingTest : MonoBehaviour
{
    private bool testPassed = false;
    void Update()
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Assert(ChainingTable.Count == 3, "Count should be 3");
            Debug.Log("ChainingTable.Count: " + ChainingTable.Count);
            Debug.Assert(ChainingTable["1"] == 100, "ChainingTable['1'] should be 100");
            Debug.Log("ChainingTable['1']: " + ChainingTable["1"]);
            Debug.Assert(ChainingTable["2"] == 200, "ChainingTable['2'] should be 200");
            Debug.Log("ChainingTable['2']: " + ChainingTable["2"]);
            Debug.Assert(ChainingTable["3"] == 300, "ChainingTable['3'] should be 300");
            Debug.Log("ChainingTable['3']: " + ChainingTable["3"]);
            Debug.Log("[PASS] 기본 Add / 조회");
        }



        // ─────────────────────────────────────────────
        // 2. ContainsKey / Contains
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Assert(ChainingTable.ContainsKey("1"), "ContainsKey('1') should return true.");
            Debug.Log("ContainsKey('1'): " + ChainingTable.ContainsKey("1"));
            Debug.Assert(!ChainingTable.ContainsKey("4"), "ContainsKey('4') should return false.");
            Debug.Log("ContainsKey('4'): " + ChainingTable.ContainsKey("4"));

            Debug.Assert(ChainingTable.Contains(new System.Collections.Generic.KeyValuePair<string, int>("1", 100)));
            Debug.Log("Contains(KeyValuePair('1', 100)): " + ChainingTable.Contains(new System.Collections.Generic.KeyValuePair<string, int>("1", 100)));
            Debug.Assert(!ChainingTable.Contains(new System.Collections.Generic.KeyValuePair<string, int>("1", 999)));
            Debug.Log("Contains(KeyValuePair('1', 999)): " + ChainingTable.Contains(new System.Collections.Generic.KeyValuePair<string, int>("1", 999)));
            Debug.Log("[PASS] ContainsKey / Contains");
        }


        // ─────────────────────────────────────────────
        // 3. TryGetValue
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bool found = ChainingTable.TryGetValue("2", out int val);
            Debug.Assert(found && val == 200, "TryGetValue('2') should return 200");
            Debug.Log("TryGetValue('2'): " + found + ", Value: " + val);

            bool notFound = ChainingTable.TryGetValue("99", out int missing);
            Debug.Assert(!notFound && missing == 0, "TryGetValue('99') should fail");
            Debug.Log("TryGetValue('99'): " + notFound + ", Value: " + missing);

            Debug.Log("[PASS] TryGetValue");
        }


        // ─────────────────────────────────────────────
        // 4. Remove
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bool removed = ChainingTable.Remove("2");
            Debug.Assert(removed, "Remove('2') should return true");
            Debug.Assert(ChainingTable.Count == 2, "Count should be 2 after remove");
            Debug.Assert(!ChainingTable.ContainsKey("2"), "Key '2' should not exist after remove");
            Debug.Log("Remove('2'): " + removed);

            bool removeFail = ChainingTable.Remove("99");
            Debug.Assert(!removeFail, "Remove('99') should return false");
            Debug.Log("Remove('99'): " + removeFail);
            Debug.Log("[PASS] Remove");
        }


        // ─────────────────────────────────────────────
        // 5. Keys / Values 컬렉션
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Assert(ChainingTable.Keys.Count == ChainingTable.Count, "Keys count mismatch");
            Debug.Assert(ChainingTable.Values.Count == ChainingTable.Count, "Values count mismatch");
            Debug.Log("Keys: " + string.Join(", ", ChainingTable.Keys));
            Debug.Log("Values: " + string.Join(", ", ChainingTable.Values));
            Debug.Log("[PASS] Keys / Values 컬렉션");
        }


        // ─────────────────────────────────────────────
        // 6. Clear
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChainingTable.Clear();
            Debug.Assert(ChainingTable.Count == 0, "Count should be 0 after Clear");
            Debug.Assert(!ChainingTable.ContainsKey("1"), "Key '1' should not exist after Clear");
            Debug.Log("Clear: Count after clear: " + ChainingTable.Count);
            Debug.Log("[PASS] Clear");
        }


        // ─────────────────────────────────────────────
        // 7. Resize — 부하율 0.7 초과 시 자동 리사이즈
        //    초기 capacity 10이므로 7개 삽입 시 리사이즈 트리거
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            var ChainingTable2 = new ChainingHashTable<string, int>();
            for (int i = 0; i < 8; i++)
                ChainingTable2.Add(i.ToString(), i * 10);

            Debug.Assert(ChainingTable2.Count == 8, "Count should be 8 after resize");
            Debug.Log("Resize: resize 전 count: " + ChainingTable2.Count);
            for (int i = 0; i < 8; i++)
                Debug.Assert(ChainingTable2[i.ToString()] == i * 10, $"Value mismatch after resize: key={i}");
            Debug.Log("Resize: resize 후 values: " + string.Join(", ", ChainingTable2.Values));
            Debug.Log("[PASS] Resize");
        }


        // ─────────────────────────────────────────────
        // 8. 존재하지 않는 키 접근 → KeyNotFoundException
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            try
            {
                _ = ChainingTable["999"];
                Debug.LogError("[FAIL] KeyNotFoundException이 발생해야 합니다");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                Debug.Log("[PASS] KeyNotFoundException");
            }
        }


        // ─────────────────────────────────────────────
        // 9. null 키 → ArgumentNullException
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
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
        }


        // ─────────────────────────────────────────────
        // 10. foreach 열거
        // ─────────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
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

            testPassed = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0) && testPassed)
        {
            Debug.Log("===== 모든 테스트 통과 =====");
        }
    }
}