using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class BreakableBlockController : MonoBehaviour {
    [Inject]
    readonly GameMain gameMain;

    [Inject(Id = "audioController")]
    readonly AudioController audioController;

    private const int lifeInitial = 20;

    [SerializeField]
    private IntReactiveProperty life = new IntReactiveProperty(lifeInitial);

    private Rigidbody2D rb;
    private Color color;
//    private AudioSource audioSource;

    [Inject]
    public void Construct()
    {
        this.rb = this.GetComponent<Rigidbody2D>();
//        this.audioSource = this.GetComponent<AudioSource>();
        this.Reset();

        var renderer = this.gameObject.GetComponent<MeshRenderer>();
        var material = renderer.material;
        this.color = material.color;

        this.OnParticleCollisionAsObservable().Where(_ => 0 < this.life.Value).Subscribe(_ => {
            this.life.Value -= 5;

            material.color = Color.white;
            material.DOColor(this.color, 0.1f);
            this.audioController.playDamage();
        });

        this.life.Where(_ => this.life.Value <= 0).Subscribe(_ => {
            this.audioController.playDestruction();
            this.gameMain.despawnCharacter(this);
        });

    }

    public class Factory : PlaceholderFactory<BreakableBlockController>
    {
    }

    public class Pool : MonoMemoryPool<Vector2, Vector2, float, BreakableBlockController>
    {
        protected override void Reinitialize(Vector2 position, Vector2 velocity, float angle, BreakableBlockController cube)
        {
            cube.Reset(position, velocity, angle);
        }
    }

    public void Reset()
    {
        this.Reset(Vector2.zero, Vector2.zero, 0.0f);
    }

    public void Reset(Vector2 position, Vector2 velocity, float angle)
    {
        this.rb.position = position;
        this.rb.velocity = velocity;
        this.rb.rotation = angle;
        this.life.Value = lifeInitial;
    }
}
