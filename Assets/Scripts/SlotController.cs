using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class SlotController : MonoBehaviour
{
    public Transform DataContainer;
    public GameObject dataBoxPrefab;
    public TextMeshProUGUI IndexText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            init(1);
            AddData(Random.Range(0, 9), Random.Range(0, 1000));
        }
        
    }
    public void init(int index)
    {
        IndexText.text = $"버킷 {index}";
    }
    public void Remove()
    {

    }
    public void Clear()
    {

    }
    public void AddData(int key, int value)
    {
        if (false) //체이닝일때 옆으로 생성
        {
            
        }

        var dataBox = Instantiate(dataBoxPrefab, DataContainer);
        dataBox.GetComponentInChildren<TextMeshProUGUI>().text = $"({key}, {value})";
    }

}
