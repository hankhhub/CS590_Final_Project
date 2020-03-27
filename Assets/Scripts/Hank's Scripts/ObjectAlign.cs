using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAlign : MonoBehaviour
{
    public GameObject object2;
    // Start is called before the first frame update
    void Start()
    {
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);
        gameObject.transform.position = object2.transform.position + object2.transform.TransformDirection(new Vector3(0, 0, -1)); //nut z == world y axis
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
