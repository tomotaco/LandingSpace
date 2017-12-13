using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private float distanceCamera = 20.0f;

    private Vector3 positionCurrent;

	void Start () {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;
        this.positionCurrent = player.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;
        this.positionCurrent = Vector3.Slerp(this.positionCurrent, player.transform.position, 0.1f);
        this.transform.position = this.positionCurrent - new Vector3(0.0f, 0.0f, this.distanceCamera);
	}
}
