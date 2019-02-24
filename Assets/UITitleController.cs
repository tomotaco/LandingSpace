using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
//using DG.Tweening;
using Zenject;


public class UITitleController : MonoBehaviour {

    [Inject]
    readonly GameMain gameMain;

    [Inject(Id = "buttonStart")]
    private Button buttonStart;
    [Inject(Id = "buttonRanking")]
    private Button buttonRanking;
    [Inject(Id = "buttonConfig")]
    private Button buttonConfig;
    [Inject(Id = "buttonExit")]
    private Button buttonExit;

    [Inject]
    void Construct ()
    {
        this.OnEnableAsObservable().Subscribe(_ => {
//            this.buttonStart.Select();
        });
            
        this.buttonStart.OnClickAsObservable().Subscribe(_ => {            
            this.gameMain.start();
        });

        this.buttonRanking.OnClickAsObservable().Subscribe(_ => {
            this.gameMain.ranking();
        });

        this.buttonConfig.OnClickAsObservable().Subscribe(_ => {
            this.gameMain.config();
        });
    }

}
