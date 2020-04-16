using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearControl : MonoBehaviour
{
    public GearControl reference;
    public Vector3 rotateAxis;
    public float speed;
    public bool clockwise;

    void Update()
    {
        //rotateAxis = oddGear ? -rotateAxis : rotateAxis;
        if(rotateAxis != null)
            transform.Rotate(rotateAxis * Time.deltaTime * speed, relativeTo:Space.Self);
        
    }

    
}
