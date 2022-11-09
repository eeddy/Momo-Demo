using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerEMG : MonoBehaviour
{
    private float speed = 8;
    private float upwardsForce = 2;
    public Rigidbody2D rb;
    private Vector2 velocity;
    private MyoEMGRawReader emgReader;

    private SoundManager soundManager;
    private bool alreadyJumped = false; 

    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
        emgReader = new MyoEMGRawReader();
        emgReader.StartReadingData();
    }

    void Update()
    {
        string control = emgReader.ReadControlFromArmband();
        float speed = emgReader.ReadSpeedFromArmband();
        if (control == "0") {
            rb.velocity = Vector3.zero;
            velocity = new Vector2(0, upwardsForce);
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime * speed);
            // rb.AddForce(new Vector2(0,1) * upwardsForce);
            // soundManager.PlayJumpSound();
            // alreadyJumped = true;
        } else if (control == "2") {
            alreadyJumped = false;
            rb.velocity = Vector3.zero;
            velocity = new Vector2(speed, 0);
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime * speed);
        } else if (control == "3") {
            alreadyJumped = false;
            rb.velocity = Vector3.zero;
            velocity = new Vector2(-speed, 0);
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime * speed);
        }
    }
}
