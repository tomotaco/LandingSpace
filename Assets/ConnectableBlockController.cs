using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ConnectableBlockController : MonoBehaviour {
    [Inject]
    readonly GameMain gameMain;

    [Inject(Id = "audioController")]
    readonly AudioController audioController;

    private Rigidbody2D rb;
    private HingeJoint2D joint;
    private PlayerController controllerPlayerConnected;

    public class Factory : PlaceholderFactory<ConnectableBlockController>
    {
    }
    public class Pool : MonoMemoryPool<Vector2, Vector2, float, ConnectableBlockController>
    {
        protected override void Reinitialize(Vector2 position, Vector2 velocity, float angle, ConnectableBlockController cube)
        {
            cube.Reset(position, velocity, angle);
        }
    }

    [Inject]
    public void Construct()
    {
        this.rb = this.GetComponent<Rigidbody2D>();
        this.joint = this.GetComponent<HingeJoint2D>();
        this.Reset();
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (this.controllerPlayerConnected) {
            if (!controllerPlayerConnected.isGrabbingConnector) {
                this.joint.connectedBody = null;
                this.joint.enabled = false;
                this.controllerPlayerConnected = null;
            }
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //        Debug.Log("ConnectableBlockController.OnCollisionEnter2D():tag=" + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Connector")) {
            if (this.controllerPlayerConnected == null) {
                //            Debug.Log("Hit connector");
                this.joint.enabled = true;
                this.joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
                var transformChain = collision.gameObject.transform.parent;
                this.controllerPlayerConnected = transformChain.parent.gameObject.GetComponent<PlayerController>();
                this.audioController.playConnect();
            }
        }
    }
}
