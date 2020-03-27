using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float inputX, inputZ;
    float speed = 10.0f;

    private float speedH = 1.0f;
    private float speedV = 1.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        if (inputX != 0)
        {
            rotate();
        }
        if (inputZ != 0)
        {
            move();
        }

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    private void move()
    {
        transform.position += transform.forward * inputZ * Time.deltaTime*speed;
    }
    private void rotate()
    {
        transform.Rotate(new Vector3(0f, inputX * Time.deltaTime*speed, 0f));
        transform.position += transform.right * inputX * Time.deltaTime * speed;
    }
}
