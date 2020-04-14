using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public Vector3 rotateAxis;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right*Time.deltaTime* 100f);       
    }
}
