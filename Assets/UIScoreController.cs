using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using UniRx;

public class UIScoreController : MonoBehaviour {

    [Inject]
    readonly GameMain gameMain;

    private TextMeshProUGUI textTime;
    private TextMeshProUGUI textTimeShortest;

    // Use this for initialization
    void Start () {
        this.textTime = this.gameObject.transform.Find("TextTime").GetComponent<TextMeshProUGUI>(); ;
        this.textTimeShortest = this.gameObject.transform.Find("TextSTime").GetComponent<TextMeshProUGUI>();

        this.gameMain.time.Subscribe(t => {
            this.textTime.text = string.Format("{0:F2}", t);
        });
        this.gameMain.shortestTime.Subscribe(t => {
            this.textTimeShortest.text = string.Format("{0:F2}", t);
        });

    }

    // Update is called once per frame
    void Update () {
		
	}
}
