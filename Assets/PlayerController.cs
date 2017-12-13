using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;

public class PlayerController : MonoBehaviour {

    public class Factory : Factory<PlayerController>
    {
    }

    [Inject]
    readonly GameMain gameMain;

    [SerializeField]
    private float ratioRotate = 5.0f;

    [SerializeField]
    private float ratioThurst = 15.0f;

    [SerializeField]
    private float intervalFire = 0.1f;

    private Rigidbody2D rb;
    private ParticleSystem thurst;
    private ParticleSystem bullet;
    private float angle = 0.0f;

    public bool isGrabbingConnector = true;

    [Inject]
    public void Construct () {
        this.rb = this.GetComponent<Rigidbody2D>();
        this.bullet = this.transform.Find("Bullet").GetComponent<ParticleSystem>();
        this.thurst = this.transform.Find("Thurst").GetComponent<ParticleSystem>();

        var fixedUpdateOnGameAsObservable = this.FixedUpdateAsObservable()
            .Where(_ => this.gameMain.state.Value == GameMain.GameState.Game)
            .Publish().RefCount();
        fixedUpdateOnGameAsObservable
            .Where(_ => this.gameMain.typeInput == GameMain.InputType.RotateByLeftRight)
            .Subscribe(_ => {
            var xAxis = Input.GetAxis("Horizontal");
            this.angle -= xAxis * ratioRotate;
            this.rb.MoveRotation(this.angle);
        });
        fixedUpdateOnGameAsObservable
            .Where(_ => this.gameMain.typeInput == GameMain.InputType.OrientByStick)
            .Subscribe(_ => {
                var xAxis = - Input.GetAxis("Horizontal");
                var yAxis = Input.GetAxis("Vertical");
                var orient = new Vector2(xAxis, yAxis);
                if (orient.magnitude < 0.3f) return;
                var angleTarget = Mathf.Atan2(xAxis, yAxis) * Mathf.Rad2Deg;
                this.angle = Mathf.LerpAngle(this.angle, angleTarget, 0.1f);
				this.rb.MoveRotation(this.angle);
            });

        fixedUpdateOnGameAsObservable
            .Where(_ => Input.GetButton("Fire1") || Input.GetKey(KeyCode.Z))
            .Subscribe(_ => {
            this.rb.AddForce(this.gameObject.transform.rotation * Vector2.up * this.ratioThurst);
            this.thurst.Emit(1);
        });
        fixedUpdateOnGameAsObservable
            .Where(_ => (Input.GetButton("Fire2") || Input.GetKey(KeyCode.X)))
            .ThrottleFirst(TimeSpan.FromSeconds(this.intervalFire))
            .Subscribe(_ => {
                this.bullet.Emit(1);
            });
        fixedUpdateOnGameAsObservable.Subscribe(_ => {
            this.isGrabbingConnector = Input.GetButton("Fire3") || Input.GetKey(KeyCode.C);
        });

        this.OnTriggerStay2DAsObservable()
            .Where(_ => this.gameMain.state.Value == GameMain.GameState.Game)
            .Where(collider => collider.CompareTag("Goal"))
            .Where(_ => this.rb.velocity.magnitude < 0.001f)
            .Subscribe(collider => {
            Debug.Log("Hit goal!. tag=" + collider.tag + ", " +
                "velocity.magnitude=" + this.rb.velocity.magnitude.ToString() + ", rotation=" + this.rb.rotation.ToString());
                this.gameMain.finish();
        });
    }

}
