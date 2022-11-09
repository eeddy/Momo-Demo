using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private float speed = 5;
    private float upwardsForce = 300;
    public Rigidbody2D rb;
    private Vector2 velocity;

    private SoundManager soundManager; 

    void Start() {
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Update()
    {
        Vector3 pos = rb.transform.position;
        if (Input.GetKey(KeyCode.LeftArrow)) {
            pos.x -= speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            pos.x += speed * Time.deltaTime;
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            rb.AddForce(new Vector2(0,1) * upwardsForce);
            soundManager.PlayJumpSound();
        }
        rb.transform.position = pos;
    }
}
