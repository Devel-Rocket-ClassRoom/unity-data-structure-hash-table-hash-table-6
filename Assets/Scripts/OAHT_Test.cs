using UnityEngine;

public class OAHT_Test : MonoBehaviour
{
    private OpenAddressingHashTable<string, int> hashTable;
    public ProvingMode mode;

    private void Start()
    {
        hashTable = new OpenAddressingHashTable<string, int>(mode);
    }

    private void Update()
    {

    }
}
