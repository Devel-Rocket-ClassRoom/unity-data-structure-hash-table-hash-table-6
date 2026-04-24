using UnityEngine;

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
        hashTable["6"] = 8;

        foreach (var key in hashTable.Keys)
        {
            Debug.Log($"{key} : {hashTable[key]}");
        }

        Debug.Log(hashTable["7"]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
        }
    }
}
