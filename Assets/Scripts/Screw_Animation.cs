using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw_Animation : MonoBehaviour
{
    private static GameObject selectedObject;

    private float speed = 1000;
    private float speed1 = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        selectedObject = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (DetectCollision.trigger)
        {
            selectedObject.transform.position -= selectedObject.transform.up * speed1;
            selectedObject.transform.RotateAround(selectedObject.transform.position, selectedObject.transform.up, speed * Time.deltaTime);
            Debug.Log("Screw move");
        }
    }
}
