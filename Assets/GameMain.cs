using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Unity.Linq;
using NCMBRest;

public class GameMain : IInitializable, IFixedTickable {
    public enum GameState
    {
        Title,
        Ready,
        Game,
        Config,
        Ranking,
        Finish,
        Pause
    };

    public enum InputType : int
    {
        RotateByLeftRight = 0,
        OrientByStick = 1
    };

    private const float timeToFadeDefault = 0.5f;
    private const float alphaButtonImage = 0.4f;

    private PlayerController player;
    private List<ConnectableBlockController> connectableCubes = new List<ConnectableBlockController>();
    private List<ConnectableBlockController> connectableCylinders = new List<ConnectableBlockController>();
    private List<BreakableBlockController> breakableCubes = new List<BreakableBlockController>();
    private List<BreakableBlockController> breakableCylinders = new List<BreakableBlockController>();

    [Inject]
    LeaderboardManager leaderBoardManager;

    [Inject]
    readonly private PlayerController.Factory playerFactory;
 
    [Inject(Id = "connectableCubePool")]
    readonly private ConnectableBlockController.Pool connectableCubePool;
    [Inject(Id = "connectableCylinderPool")]
    readonly private ConnectableBlockController.Pool connectableCylinderPool;
    [Inject(Id = "breakableCubePool")]
    readonly private BreakableBlockController.Pool breakableCubePool;
    [Inject(Id = "breakableCylinderPool")]
    readonly private BreakableBlockController.Pool breakableCylinderPool;

    [Inject(Id = "uiTitle")]
    readonly private UITitleController uiTitleController;
    [Inject(Id = "uiReady")]
    readonly private UIEmptyController uiReadyController;
    [Inject(Id = "uiGo")]
    readonly private UIEmptyController uiGoController;
    [Inject(Id = "uiRanking")]
    readonly private UIRankingController uiRankingController;
    [Inject(Id = "uiConfig")]
    readonly private UIConfigController uiConfigController;
    [Inject(Id = "uiFinish")]
    readonly private UIFinishController uiFinishController;
    [Inject(Id = "uiPause")]
    readonly private UIPauseController uiPauseController;
    [Inject(Id = "uiScore")]
    readonly private UIScoreController uiScoreController;

    public ReactiveProperty<GameState> state = new ReactiveProperty<GameState>(GameState.Title);
    public ReactiveProperty<float> time = new ReactiveProperty<float>(0.0f);
    public ReactiveProperty<float> shortestTime = new ReactiveProperty<float>(0.0f);
    public InputType typeInput = InputType.RotateByLeftRight;
    private float timeOnGo = 0.0f;

    private Dictionary<GameState, MonoBehaviour> mapStateToUI = new Dictionary<GameState, MonoBehaviour>();
    [Inject]
    public void Construct()
    {
        this.mapStateToUI.Add(GameState.Title, this.uiTitleController);
        this.mapStateToUI.Add(GameState.Ready, this.uiReadyController);
        this.mapStateToUI.Add(GameState.Game, null);
        this.mapStateToUI.Add(GameState.Pause, this.uiPauseController);
        this.mapStateToUI.Add(GameState.Finish, this.uiFinishController);
        this.mapStateToUI.Add(GameState.Ranking, this.uiRankingController);
        this.mapStateToUI.Add(GameState.Config, this.uiConfigController);

        this.state.Where(s => s == GameState.Title).Subscribe(_ => {
            this.uiFadeIn(this.uiTitleController);
        });

        this.state.Where(s => s == GameState.Ranking).Subscribe(_ => {
            this.uiFadeOut(this.uiTitleController);
            this.uiFadeIn(this.uiRankingController);
        });

        this.state.Where(s => s == GameState.Config).Subscribe(_ => {
            this.uiFadeOut(this.uiTitleController);
            this.uiFadeIn(this.uiConfigController);
        });

        this.state.Where(s => s == GameState.Finish).Subscribe(_ => {
//            this.uiFinishController.takeScreenshot();
            this.uiFadeIn(this.uiFinishController);
            if (this.shortestTime.Value < float.Epsilon ||
                this.time.Value < this.shortestTime.Value)
                this.shortestTime.Value = this.time.Value;
        });

        this.state.Where(s => s == GameState.Ready).Subscribe(_ => {
            this.uiFadeOut(this.uiTitleController);
            this.uiFadeIn(this.uiReadyController);
        });
        this.state.Where(s => s == GameState.Ready).Delay(System.TimeSpan.FromSeconds(2.0f)).Subscribe(_ => {
            this.typeInput = (InputType)System.Enum.ToObject(typeof(InputType), PlayerPrefs.GetInt("PlayerInput", (int)(InputType.RotateByLeftRight)));
            this.state.Value = GameState.Game;
        });
        this.state.Where(s => s == GameState.Game).Subscribe(_ => {
            this.uiFadeOut(this.uiReadyController, 0.2f);
            this.uiFadeIn(this.uiGoController, 0.2f);
            this.time.Value = 0.0f;
            this.timeOnGo = Time.time;
        });
        this.state.Where(s => s == GameState.Game).Delay(System.TimeSpan.FromSeconds(1.0f)).Subscribe(_ => {
            this.uiFadeOut(this.uiGoController, 0.2f);
        });
    }

    // Use this for initialization
    public void Initialize () {
        this.initCharacters();
    }

    // Update is called once per frame
    public void FixedTick () {
        if (this.state.Value == GameState.Game) {
            this.time.Value = Time.time - this.timeOnGo;
        }
	}

    public void start()
    {
        clearCharacters();
        initCharacters();
        this.state.Value = GameState.Ready;
    }

    public void finish()
    {
        this.state.Value = GameState.Finish;
    }

    public void title()
    {
        this.state.Value = GameState.Title;
    }

    public void config()
    {
        this.state.Value = GameState.Config;
    }

    public void ranking()
    {
        this.state.Value = GameState.Ranking;
    }

    public void despawnCharacter(BreakableBlockController obj)
    {
        if (obj.tag == "BreakableCube") {
            var index = this.breakableCubes.IndexOf(obj);
            if (0 <= index) {
                this.breakableCubePool.Despawn(obj);
                this.breakableCubes.RemoveAt(index);
            }
        } else if (obj.tag == "BreakableCylinder") {
            var index = this.breakableCylinders.IndexOf(obj);
            if (0 <= index) {
                this.breakableCylinderPool.Despawn(obj);
                this.breakableCylinders.RemoveAt(index);
            }
        }
    }

    private void clearCharacters()
    {
        Object.Destroy(this.player.gameObject);
        this.clearCharacters<ConnectableBlockController>(this.connectableCubes, this.connectableCubePool);
        this.clearCharacters<ConnectableBlockController>(this.connectableCylinders, this.connectableCylinderPool);
        this.clearCharacters<BreakableBlockController>(this.breakableCubes, this.breakableCubePool);
        this.clearCharacters<BreakableBlockController>(this.breakableCylinders, this.breakableCylinderPool);
    }

    private void initCharacters()
    {
        Random.InitState(0);
        this.player = this.playerFactory.Create();
        for (var index = 0; index < 5; index++) {
            this.connectableCubes.Add(this.connectableCubePool.Spawn(new Vector2(Random.Range(-9.0f, -7.0f), Random.Range(-9.0f, 0.0f)), Vector2.zero, 0.0f));
            this.connectableCylinders.Add(this.connectableCylinderPool.Spawn(new Vector2(Random.Range(-9.0f, -7.0f), Random.Range(-9.0f, 0.0f)), Vector2.zero, 0.0f));
        }
        for (var index = 0; index < 40; index++) {
            var cube = this.breakableCubePool.Spawn(new Vector2(Random.Range(-9.0f, 9.0f), Random.Range(-4.0f, 9.0f)), Vector2.zero, 0.0f);
            this.breakableCubes.Add(cube);
            var cylinder = this.breakableCylinderPool.Spawn(new Vector2(Random.Range(-9.0f, 9.0f), Random.Range(-4.0f, 9.0f)), Vector2.zero, 0.0f);
            this.breakableCylinders.Add(cylinder);
        }
    }

    private void clearCharacters<TValue>(List<TValue> list, MonoMemoryPool<Vector2, Vector2, float, TValue> pool) where TValue : MonoBehaviour
    {
        while (0 < list.Count) {
            var item = list[0];
            pool.Despawn(item);
            list.RemoveAt(0);
        }
    }

    public void uiFadeOut(MonoBehaviour controller, float timeToFade = timeToFadeDefault)
    {
        var texts = controller.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts) {
            text.DOFade(1.0f, 0.0f);
            text.DOFade(0.0f, timeToFade).OnComplete(() => {
                controller.gameObject.SetActive(false);
            });
        }
        var images = controller.GetComponentsInChildren<Image>(true);
        foreach (var image in images) {
            image.DOFade(alphaButtonImage, 0.0f);
            image.DOFade(0.0f, timeToFade);
        }
    }

    public void uiFadeIn(MonoBehaviour controller, float timeToFade = timeToFadeDefault)
    {
        controller.gameObject.SetActive(true);
        var texts = controller.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts) {
            text.DOFade(0.0f, 0.0f);
            text.DOFade(1.0f, timeToFade);
        }
        var imagesButton = controller.gameObject.Descendants().Where(x => x.name.StartsWith("Button")).Select(obj => obj.GetComponent<Image>()).ToArray();
        foreach (var image in imagesButton) {
            image.DOFade(0.0f, 0.0f);
            image.DOFade(alphaButtonImage, timeToFade);
        }
        var imagesOther = controller.gameObject.Descendants().Where(x => !x.name.StartsWith("Button") && x.GetComponent<Image>() != null).Select(obj => obj.GetComponent<Image>()).ToArray();
        foreach (var image in imagesOther) {
            image.DOFade(0.0f, 0.0f);
            image.DOFade(1.0f, timeToFade);
        }
    }

}
