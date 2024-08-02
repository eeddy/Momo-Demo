using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerEMG : MonoBehaviour
{
    private float speed = 25;
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

    void FixedUpdate()
    {
        string control = emgReader.ReadControlFromArmband();
        float movSpeed = speed * emgReader.ReadSpeedFromArmband();
        Vector3 pos = rb.transform.position;
        if (control == "0") {
            // Debounce the jump
            if (Time.time - jumpTime > 0.5f) {
                rb.AddForce(new Vector2(0,1) * 300);
                soundManager.PlayJumpSound();
                jumpTime = Time.time;
            }
        } else if (control == "2") {
            //Extension:
            rb.AddForce(new Vector2(1,0) * movSpeed);
        } else if (control == "3") {
            //Flexion:
            rb.AddForce(new Vector2(-1,0) * movSpeed);
        }
        rb.transform.position = pos;
    }
}
