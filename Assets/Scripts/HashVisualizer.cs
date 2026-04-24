using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class HashVisualizer : MonoBehaviour
{
    IHashTable<string, string> hashTable;

    public GameObject slotPrefab;
    public List<SlotController<string, string>> slots;

    public TMP_InputField keyInput;
    public TMP_InputField valueInput;
    public TextMeshProUGUI addCount;
    public TextMeshProUGUI errorCount;
    public TextMeshProUGUI Percent;
    public int count = 0;


    private void Start()
    {
        hashTable = new SimpleHashTable<string, string>();
        slots = new List<SlotController<string, string>>();
        Resize();
    }
    private void Update()
    {
        addCount.text = $"{count}";
    }

    public void OnTableChange(int idx)
    {

        for (int i = 0; i < slots.Count; i++)
        {
            Destroy(slots[i].gameObject);
        }
        slots.Clear();

        switch (idx)
        {
            case 0:
                hashTable = new SimpleHashTable<string, string>(); break;
            case 1: 
                hashTable = new ChainingHashTable<string, string>(); break;
            case 2:
                hashTable = new OpenAddressingHashTable<string, string>(); break;
        }

        for (int i = 0; i < hashTable.Capacity; i++)
        {
            var gameObj = Instantiate(slotPrefab, transform);
            slots.Add(gameObj.GetComponent<SlotController<string, string>>());
        }


    }

    public void OnClickAdd()
    {
        string inputKey = keyInput.text;
        string inputValue = valueInput.text;
        count++;
        

        Debug.Log(slots.Count);

        hashTable.Add(inputKey, inputValue);
        slots[hashTable.GetIndex(inputKey)].AddData(inputKey, inputValue);


        Debug.Log($"입력된 값 - Key: {inputKey}, Value: {inputValue}");


        //inPutField 초기화
        keyInput.text = "";
        valueInput.text = "";
        Resize();
    }
    public void OnRemove()
    {

    }
    public void OnClear()
    {
        count = 0;
    }

    void Resize()
    {
        if (slots.Count >= hashTable.Capacity) return;
        Debug.Log("ASD");

        var list = new List<KeyValuePair<string, string>>();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].DataList.Count > 0)
            {
                foreach (var item in slots[i].DataList)
                {
                    list.Add(item);
                }
                slots[i].Clear();
            }
        }

        for (int i = slots.Count; i < hashTable.Capacity; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, transform);
            slots.Add(slotObject.GetComponent<SlotController<string, string>>());
        }

        Debug.Log(list.Count);

        foreach (var item in list)
        {
            slots[hashTable.GetIndex(item.Key)].AddData(item.Key, item.Value);
            Debug.Log($"{item.Key}, {item.Value}");

        }
    }
}
