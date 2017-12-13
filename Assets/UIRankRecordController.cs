using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;

public class UIRankRecordController : MonoBehaviour {
    private TextMeshProUGUI textOrder;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textTime;
    private TextMeshProUGUI textInputType;

    public class Factory : Factory<UIRankRecordController>
    {
    }

    public class Pool : MonoMemoryPool<int, string, float, GameMain.InputType, UIRankRecordController>
    {
        protected override void Reinitialize(int order, string name, float time, GameMain.InputType typeInput, UIRankRecordController record)
        {
            record.Reset(order, name, time, typeInput);
        }
    }

    [Inject]
    public void Construct()
    {
        this.textOrder = this.gameObject.transform.Find("TextOrder").GetComponent<TextMeshProUGUI>();
        this.textName = this.gameObject.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
        this.textTime = this.gameObject.transform.Find("TextTime").GetComponent<TextMeshProUGUI>();
        this.textInputType = this.gameObject.transform.Find("TextInputType").GetComponent<TextMeshProUGUI>();
    }

    public void Reset(int order, string name, float time, GameMain.InputType typeInput)
    {
        this.textOrder.text = string.Format("{0:D}", order);
        this.textName.text = name;
        this.textTime.text = string.Format("{0:F2}", time);
        this.textInputType.text = (typeInput == GameMain.InputType.RotateByLeftRight ? "左右" : "方向");
    }
}
