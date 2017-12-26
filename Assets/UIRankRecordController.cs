using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;

public class UIRankRecordController : MonoBehaviour {
    private TextMeshProUGUI textOrder;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textTime;
    private TextMeshProUGUI textInputType;
    private RawImage rawImageScreenShot;

    private Texture2D textureScreenShot;

    public class Factory : Factory<UIRankRecordController>
    {
    }

    public class Pool : MonoMemoryPool<int, string, float, GameMain.InputType, byte[], UIRankRecordController>
    {
        protected override void Reinitialize(int order, string name, float time, GameMain.InputType typeInput, byte[] bytesPngScreenShot, UIRankRecordController record)
        {
            record.Reset(order, name, time, typeInput, bytesPngScreenShot);
        }
    }

    [Inject]
    public void Construct()
    {
        this.textOrder = this.gameObject.transform.Find("TextOrder").GetComponent<TextMeshProUGUI>();
        this.textName = this.gameObject.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
        this.textTime = this.gameObject.transform.Find("TextTime").GetComponent<TextMeshProUGUI>();
        this.textInputType = this.gameObject.transform.Find("TextInputType").GetComponent<TextMeshProUGUI>();
        this.rawImageScreenShot = this.gameObject.transform.Find("RawImageScreenShot").GetComponent<RawImage>();

		this.rawImageScreenShot.OnPointerEnterAsObservable()
			.Subscribe(_ => {
				Debug.Log("rawImageScreenShot.OnPointerEnterAsObservable()");
                this.transform.SetSiblingIndex(100);

                this.rawImageScreenShot.transform.DOScale(0.8f, 0.5f);
				this.rawImageScreenShot.transform.DOLocalMoveX(240.0f, 0.5f);
			});
		this.rawImageScreenShot.OnPointerExitAsObservable()
			.Subscribe(_ => {
				Debug.Log("rawImageScreenShot.OnPointerExitAsObservable()");
                this.rawImageScreenShot.transform.DOScale(0.13f, 0.5f);
				this.rawImageScreenShot.transform.DOLocalMoveX(220.0f, 0.5f).OnComplete(() => {
                    this.transform.SetSiblingIndex(1);
                });
			});
	}

    private void Start()
    {
//        if (this.textureScreenShot == null) {
//            this.textureScreenShot = new Texture2D(ScreenShotController.widthScreenShot, ScreenShotController.heightScreenShot);
//        }
    }

    public void Reset(int order, string name, float time, GameMain.InputType typeInput, byte[] bytesPngScreenShot)
    {
        this.textOrder.text = string.Format("{0:D}", order);
        this.textName.text = name;
        this.textTime.text = string.Format("{0:F2}", time);
        this.textInputType.text = (typeInput == GameMain.InputType.RotateByLeftRight ? "左右" : "方向");
        Debug.Log("UIRankRecordController.Reset(): name=" + name + ", ytesPngScreenShot=" + (bytesPngScreenShot != null ? bytesPngScreenShot.Length.ToString() : "null"));
        if (bytesPngScreenShot != null && 0 < bytesPngScreenShot.Length) {
            if (this.textureScreenShot == null) {
                this.textureScreenShot = new Texture2D(ScreenShotController.widthScreenShot, ScreenShotController.heightScreenShot,TextureFormat.RGB24, false, true);
            }
            this.textureScreenShot.LoadImage(bytesPngScreenShot /* .Clone() as byte[] */);
            this.rawImageScreenShot.texture = textureScreenShot;
            this.rawImageScreenShot.DOFade(0.0f, 0.0f);
            this.rawImageScreenShot.DOFade(1.0f, 0.5f);
        } else {
            this.rawImageScreenShot.texture = null;
            this.rawImageScreenShot.DOFade(0.0f, 0.0f);
        }
    }
}
