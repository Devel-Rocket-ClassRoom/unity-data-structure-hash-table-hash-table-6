using UnityEngine;
using UnityEngine.InputSystem;

public class OAHT_Test : MonoBehaviour
{
    private OpenAddressingHashTable<string, int> hashTable;
    public ProvingMode mode;

    private void Start()
    {
        hashTable = new OpenAddressingHashTable<string, int>(mode);

        hashTable["1"] = 1;
        hashTable["2"] = 2;
        hashTable["3"] = 3;
        hashTable["4"] = 4;
        hashTable["5"] = 5;
        hashTable["6"] = 6;
        hashTable["7"] = 7;
        hashTable["8"] = 8;
        hashTable["9"] = 9;
        hashTable["10"] = 10;
        hashTable["11"] = 11;
        hashTable["12"] = 12;
        hashTable["13"] = 13;
        hashTable["14"] = 14;
        hashTable["15"] = 15;
        hashTable.Remove("1");

        foreach (var key in hashTable.Keys)
        {
            Debug.Log($"{key} : {hashTable[key]}");
        }

        hashTable.Remove("1");

        Debug.Log("========================");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var e = hashTable.GetEnumerable();

            foreach (var item in e)
            {
                if (item.Key != null)
                {
                    Debug.Log($"{item.Key} : {hashTable[item.Key]}");
                }
                else
                {
                    Debug.Log("null");
                }
            }
        }
    }
}
