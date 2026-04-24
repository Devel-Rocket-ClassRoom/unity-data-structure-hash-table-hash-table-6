using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;



public class SlotController<TKey, TValue> : MonoBehaviour
{
    public Transform DataContainer;
    public GameObject dataBoxPrefab;
    public TextMeshProUGUI IndexText;
    public TextMeshProUGUI keyValueText;

    public LinkedList<KeyValuePair<TKey, TValue>> DataList = new LinkedList<KeyValuePair<TKey, TValue>>();    // 체이닝 방식일 때 데이터를 저장할 링크드 리스트

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        init(1);
    //        AddData(Random.Range(0, 9), Random.Range(0, 1000));
    //    }

    //}
    public void init(int index)
    {
        IndexText.text = $"버킷 {index}";
    }
    public void Remove()
    {

    }
    public void Clear()
    {
        DataList.Clear();
        UpdateText();
    }
    public void AddData(TKey key, TValue value)
    {
        if (DataList == null) Debug.Log(0);
        DataList.AddLast(new KeyValuePair<TKey, TValue>(key, value));

        UpdateText();
    }

    private void UpdateText()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in DataList)
        {
            sb.Append($"{item.Key}, {item.Value} ");
        }

        keyValueText.text = sb.ToString();
    }
}
