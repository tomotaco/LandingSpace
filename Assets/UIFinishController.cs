﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using UniRx.Triggers;
using naichilab;
using TMPro;
using DG.Tweening;

public class UIFinishController : MonoBehaviour {

    [Inject]
    readonly private GameMain gameMain;

    [Inject]
    readonly private LeaderboardManager leaderBoardManager;

    [Inject(Id = "buttonTwitter")]
    readonly private Button buttonTwitter;

    [Inject(Id = "buttonRegister")]
    readonly private Button buttonRegister;

    [Inject(Id = "buttonBackFromFinish")]
    readonly private Button buttonBack;

    [Inject(Id = "InputName")]
    readonly private TMP_InputField inputName;

    private TextMeshProUGUI textResultTime;
    private TextMeshProUGUI textNewRecord;
    public Texture2D textureScreenShot;

    private Sequence sequenceBlink;

    [Inject]
    void Construct ()
    {
        this.textResultTime = this.gameObject.transform.Find("TextResultTime").GetComponent<TextMeshProUGUI>();
        this.textNewRecord = this.gameObject.transform.Find("TextNewRecord").GetComponent<TextMeshProUGUI>();
        this.textNewRecord.enabled = false;

        this.OnEnableAsObservable()
            .Select(_ => (this.gameMain.time.Value < this.gameMain.shortestTime.Value ||
                            this.gameMain.shortestTime.Value < Mathf.Epsilon))
        .Subscribe(isShortest => {
                Debug.Log("UIFinishController.OnEnableAsObservable(): time=" + this.gameMain.time.Value +
                    ", shortestTime=" + this.gameMain.shortestTime.Value + ", isShortest=" + isShortest.ToString());
                this.textNewRecord.enabled = isShortest;
            if (isShortest) {
                this.gameMain.uiFadeIn(this.inputName);
                this.gameMain.uiFadeIn(this.buttonRegister);
                this.sequenceBlink.Play();
            } else {
                this.gameMain.uiFadeOut(this.inputName, 0.0f);
                this.gameMain.uiFadeOut(this.buttonRegister, 0.0f);
            }
        });

        this.buttonTwitter.OnClickAsObservable()
            .Subscribe(_ => {
                var t = this.gameMain.time.Value;
                var hashTags = new string[] { "unity1week", };
                UnityRoomTweet.Tweet("landingspace", string.Format("こちら着陸スペース、{0:F2}秒で着陸しましたオーバー。", t), hashTags);
            });
        this.buttonRegister.OnClickAsObservable()
            .Subscribe(_ => {
                var typeInput = PlayerPrefs.GetInt("PlayerInput", (int)(GameMain.InputType.RotateByLeftRight));
                var bytesScreenShot = this.textureScreenShot.EncodeToJPG();
            StartCoroutine(this.leaderBoardManager.SendScore(this.inputName.text, this.gameMain.time.Value, typeInput, false)); //, bytesScreenShot));
                this.gameMain.uiFadeOut(this);
                this.gameMain.title();
            });

        this.buttonBack.OnClickAsObservable()
            .Subscribe(_ => {
                sequenceBlink.Pause();
                sequenceBlink.Rewind();
                this.textNewRecord.enabled = false;
                this.gameMain.uiFadeOut(this);
                this.gameMain.title();
            });

        this.gameMain.time.Subscribe(_ => {
            this.textResultTime.text = string.Format("{0:F2}", this.gameMain.time.Value);
        });

        this.sequenceBlink = DOTween.Sequence();
        sequenceBlink.Append(this.textNewRecord.DOFade(0.0f, 0.1f));
        sequenceBlink.Append(this.textNewRecord.DOFade(1.0f, 0.1f));
        sequenceBlink.SetLoops(-1);

        this.textureScreenShot = new Texture2D(Screen.width / 2, Screen.height / 2, TextureFormat.RGB24, false, false);
    }

    public void takeScreenShot()
    {
        StartCoroutine(this.takeScreenShotEnumerator());
    }
    private IEnumerator takeScreenShotEnumerator()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Screen(" + Screen.width.ToString() + ", " + Screen.height.ToString() + "), " +
                "tex(" + this.textureScreenShot.width.ToString() + ", " + this.textureScreenShot.height.ToString() + ")");
        //            this.textureScreenShot.ReadPixels(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), 0, 0);
        this.textureScreenShot.ReadPixels(new Rect(0, 0, Screen.width / 2, Screen.height / 2), 0, 0);
    }

}
