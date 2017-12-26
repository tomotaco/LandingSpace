using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class UIRankingController : MonoBehaviour {
    [Inject]
    readonly private GameMain gameMain;

    [Inject]
    readonly private LeaderboardManager leaderBoardManager;

    [Inject(Id = "rankRecordPool")]
    readonly private UIRankRecordController.Pool rankRecordPool;

    [Inject(Id = "buttonBackFromRanking")]
    readonly private Button buttonBack;

    private List<UIRankRecordController> rankRecords = new List<UIRankRecordController>();

    // Use this for initialization
    [Inject]
    void Construct () {
        Debug.Log("UIRankingController.Construct(): enable=" + this.enabled.ToString());

        this.OnEnableAsObservable()
            .Subscribe(_ => {
                Debug.Log("UIRankingController.OnEnableAsObservable()");
//                this.buttonBack.Select();
                StartCoroutine(this.leaderBoardManager.GetScoreList(10, scoreData => {
                    int order = 1;
                    float y = 40.0f;
                    foreach (var record in scoreData.results) {
                        var inputType = (GameMain.InputType)System.Enum.ToObject(typeof(GameMain.InputType), record.inputType);
                        var uiRecord = this.rankRecordPool.Spawn(order, record.playerName, record.landingTime, inputType, record.bytesPngScreenShot);
                        uiRecord.transform.SetParent(this.transform);
                        var rectRransform = uiRecord.GetComponent<RectTransform>();
                        rectRransform.localPosition = new Vector3(0.0f, y, 0.0f);
                        this.rankRecords.Add(uiRecord);
/*                        Debug.Log("order=" + order.ToString() + ": playerName=" + record.playerName + ", " +
                            "landingTime=" + record.landingTime + ", inputType=" + inputType.ToString());
*/
                        y -= 20.0f;
                        order++;
                    }
                }));

            });
        this.OnDisableAsObservable()
            .Subscribe(_ => {
                Debug.Log("UIRankingController.OnDisableAsObservable()");
                while (0 < this.rankRecords.Count) {
                    var item = this.rankRecords[0];
                    this.rankRecordPool.Despawn(item);
                    this.rankRecords.RemoveAt(0);
                }
            });
        this.buttonBack.OnClickAsObservable()
            .Subscribe(_ => {
                this.gameMain.uiFadeOut(this);
                this.gameMain.title();
            });
    }
}
