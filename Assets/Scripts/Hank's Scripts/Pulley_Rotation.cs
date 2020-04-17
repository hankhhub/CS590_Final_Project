using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulley_Rotation : MonoBehaviour
{
    float startAngle = Mathf.PI / 2;
    float endAngle = Mathf.PI;
    float totAngle = 0;
    float angle = 0;
    float speed = (2 * Mathf.PI) / 5; //2*PI in degress is 360, so you get 5 seconds to complete a circle
    float radius = 5;
    float x = 0;
    float y = 0;
    public GameObject target;

    public MeshFilter meshFilter;  

    private void Start()
    {
        radius = meshFilter.mesh.bounds.extents.x;
        totAngle = startAngle + endAngle;
        angle = startAngle;
    }
    void Update()
    {
        /*if(angle < totAngle)
        {
            angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
            x = Mathf.Cos(angle) * radius;
            y = Mathf.Sin(angle) * radius;
            transform.position = -target.transform.TransformPoint(new Vector3(x, transform.position.z, y));
        }*/

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
            //Debug.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal*100f, Color.blue);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            transform.position += transform.TransformDirection(-Vector3.right) * Time.deltaTime * 2f;
        }
        
    }
}
