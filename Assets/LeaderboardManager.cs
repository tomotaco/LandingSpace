using NCMBRest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

//[RequireComponent(typeof(NCMBRestController))]
public class LeaderboardManager : MonoBehaviour
{
    [Inject]
    readonly private NCMBRestController ncmbRestController;

    private static readonly string PLAYERNAME = "PlayerName";
    private static readonly string OBJECT_ID = "ObjectId";
    private static readonly string SHORTEST_TIME = "ShortestTime";
//    private static readonly string PLAYER_INPUT = "PlayerInput";
    private static readonly string DATASTORE_CLASSNAME = "Leaderboard"; //スコアを保存するデータストア名//

    public IEnumerator SendScore(string playerName, float time, int inputType /* , byte[] bytesJpeg, */ , bool isAllowDuplication = false)
    {
        //ユーザーごとのスコアの重複を許すか//
        if (isAllowDuplication == false)
        {
            //過去のスコアがあるか//
            if (PlayerPrefs.HasKey(OBJECT_ID))
            {
                //そのスコアはハイスコアか//
                if (time < PlayerPrefs.GetFloat(SHORTEST_TIME))
                {
                    //レコードの更新//
                    yield return PutScore(playerName, time, inputType, /* bytesJpeg, */ PlayerPrefs.GetString(OBJECT_ID));
                    //ローカルのハイスコアを更新//
                    PlayerPrefs.SetFloat(SHORTEST_TIME, time);
                    PlayerPrefs.SetString(PLAYERNAME, playerName);
                    yield break;
                }
                else
                {
                    Debug.Log("ハイスコアが更新されていないため、スコアを送信しませんでした。");
                    yield break;
                }
            }
        }

        yield return SendScoreUncheck(playerName, time, inputType);
    }

    private IEnumerator SendScoreUncheck(string playerName, float time, int inputType)
    {
        //レコードの新規作成//
        IEnumerator postScoreCoroutine = PostScore(playerName, time, inputType);

        yield return postScoreCoroutine;

        string objectId = (string)postScoreCoroutine.Current;

        PlayerPrefs.SetString(OBJECT_ID, objectId);//ObjectIdを保存//
        PlayerPrefs.SetFloat(SHORTEST_TIME, time);//ローカルのハイスコアを保存//
        PlayerPrefs.SetString(PLAYERNAME, playerName);//プレイヤーネームを保存 名前を変えたときのチェック用//
    }

    private IEnumerator PostScore(string playerName, float time, int inputType)
    {
        Debug.Log(playerName + "のスコア" + time + "(操作: " + inputType.ToString() + ")を新規投稿します。");

        ScoreData scoreData = new ScoreData(playerName, time, inputType);
        NCMBDataStoreParamSet paramSet = new NCMBDataStoreParamSet(scoreData);

        IEnumerator coroutine = ncmbRestController.Call(NCMBRestController.RequestType.POST, "classes/" + DATASTORE_CLASSNAME, paramSet);

        yield return StartCoroutine(coroutine);

        JsonUtility.FromJsonOverwrite((string)coroutine.Current, paramSet);

        yield return paramSet.objectId;
    }

    private IEnumerator PutScore(string playerName, float time, int inputType, /* byte[] bytesJpeg, */ string objectId)
    {
        string formerPlayerName = PlayerPrefs.GetString(PLAYERNAME);

        if(formerPlayerName != playerName)
        {
            Debug.Log("プレイヤー名が " + formerPlayerName + " から " + playerName + " に変更されました");
            PlayerPrefs.SetString(PLAYERNAME, playerName);
        }

        Debug.Log(playerName+"のスコア"+time + "を更新します。レコードのID：" + objectId);

        ScoreData scoreData = new ScoreData(playerName, time, inputType); // , bytesJpeg);
        NCMBDataStoreParamSet paramSet = new NCMBDataStoreParamSet(scoreData);

        IEnumerator coroutine = ncmbRestController.Call(
            NCMBRestController.RequestType.PUT, "classes/" + DATASTORE_CLASSNAME + "/" + objectId, paramSet, 
            (erroCode) => 
            {
                if(erroCode == 404)
                {
                    Debug.Log("レコードID：" + objectId +"が見つからなかったため、新規レコードを作成します");
                    StartCoroutine(SendScoreUncheck(playerName, time, inputType));
                }
            }

            );

        yield return StartCoroutine(coroutine);

        JsonUtility.FromJsonOverwrite((string)coroutine.Current, paramSet);

        yield return paramSet.objectId;
    }

    public IEnumerator GetScoreList(int num, UnityAction<ScoreDatas> callback)
    {
        Debug.Log("Get Data");
        NCMBDataStoreParamSet paramSet = new NCMBDataStoreParamSet();
        paramSet.Limit = num;
        paramSet.SortColumn = "landingTime";

        IEnumerator coroutine = ncmbRestController.Call(NCMBRestController.RequestType.GET, "classes/" + DATASTORE_CLASSNAME, paramSet);

        yield return StartCoroutine(coroutine);

        string jsonStr = (string)coroutine.Current;

        //取得したjsonをScoreDatasとして展開//
        ScoreDatas scores = JsonUtility.FromJson<ScoreDatas>(jsonStr);

        if (scores.results.Count == 0)
        {
            Debug.Log("no data");
        }

        callback(scores);
    }

    public IEnumerator GetScoreListByStr(int num, UnityAction<string> callback)
    {
        yield return GetScoreList(num, (scores) =>
        {
            string str = string.Empty;

            int i = 1;

            foreach (ScoreData s in scores.results)
            {
                str += i + ": " + s.playerName + ": " + s.landingTime.ToString() + ": " + s.inputType.ToString() + "\n";
                i++;
            }

            callback(str);
        });
    }

    public void ClearLocalData()
    {
        PlayerPrefs.DeleteKey(PLAYERNAME);
        PlayerPrefs.DeleteKey(OBJECT_ID);
        PlayerPrefs.DeleteKey(SHORTEST_TIME);
        Debug.Log("ローカルのハイスコアとObjectIdが削除されました");
    }

    [Serializable]
    public class ScoreDatas
    {
        public List<ScoreData> results;
    }

    [Serializable]
    public class ScoreData
    {
        public ScoreData(string playerName, float time, int inputType)
        {
            this.playerName = playerName;
            this.landingTime = time;
            this.inputType = inputType;
        }

        public string playerName;
        public float landingTime;
        public int inputType;
    }
}