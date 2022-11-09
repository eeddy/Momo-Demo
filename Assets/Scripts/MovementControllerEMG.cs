using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerEMG : MonoBehaviour
{
    private float speed = 5;
    private float upwardsForce = 300;
    public Rigidbody2D rb;
    private Vector2 velocity;
    private MyoEMGRawReader emgReader;
    private float jumpTime;

    private SoundManager soundManager;

    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        emgReader = new MyoEMGRawReader();
        emgReader.StartReadingData();
    }

    void Update()
    {
        string control = emgReader.ReadControlFromArmband();
        float movSpeed = emgReader.ReadSpeedFromArmband();
        if(movSpeed > 5) {
            movSpeed = 5;
        }
        Vector3 pos = rb.transform.position;
        if (control == "0") {
            if (Time.time - jumpTime > 1.0f) {
                rb.AddForce(new Vector2(0,1) * 300);
                soundManager.PlayJumpSound();
                jumpTime = Time.time;
            }
        } else if (control == "2") {
            //Extension:
            pos.x += (movSpeed) * Time.deltaTime;
        } else if (control == "3") {
            //Flexion:
            pos.x -= (movSpeed) * Time.deltaTime;
        }
        rb.transform.position = pos;
    }
}
