using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using TMPro;

public class UIConfigController : MonoBehaviour {

    [Inject]
    readonly private GameMain gameMain;

    [Inject(Id = "audioController")]
    readonly AudioController audioController;

    [Inject(Id = "dropDownInput")]
    readonly private TMP_Dropdown dropDownInput;


    [Inject(Id = "buttonBackFromConfig")]
    readonly private Button buttonBack;

    [Inject]
    void Construct ()
    {
        this.OnEnableAsObservable().Subscribe(_ => {
            //            this.buttonBack.Select();
            var value = PlayerPrefs.GetInt("PlayerInput");
            this.dropDownInput.value = value;
        });

        this.dropDownInput.onValueChanged.AsObservable()
            .Subscribe(value => {
                Debug.Log("TMP_Dropdown.onValueChanged(): value=" + value.ToString());
                PlayerPrefs.SetInt("PlayerInput", value);
                this.audioController.playSelect();
            });

        this.buttonBack.OnClickAsObservable()
            .Subscribe(_ => {
                this.gameMain.uiFadeOut(this);
                this.gameMain.title();
            });


    }
}
