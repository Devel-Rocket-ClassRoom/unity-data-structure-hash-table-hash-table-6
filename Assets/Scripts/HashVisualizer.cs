using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HashVisualizer : MonoBehaviour
{
    SimpleHashTable<int, int> simple;
    public GameObject slotPrefab;
    public List<SlotController> slots;

    public TMP_InputField keyInput;
    public TMP_InputField valueInput;

    public void OnClickAdd()
    {
        string inputKey = keyInput.text;
        string inputValue = valueInput.text;

        Debug.Log($"입력된 값 - Key: {inputKey}, Value: {inputValue}");


        //inPutField 초기화
        keyInput.text = "";
        valueInput.text = "";
    }
    public void OnRemove()
    {

    }
    public void OnClear()
    {

    }
    
}
