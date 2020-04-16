using System.Collections;
using UnityEngine;
using System;

public class AnimationControl : MonoBehaviour
{
    public GameObject marker;
    GameObject marker_start;
    GameObject marker_end;
    GameObject marker_contact;
    AnimationRule rule;

    MeshRaycast highlighter;
    [Range(0, 10f)]
    public float speed = 2.0f;

    private bool animeReady = false;
    private float distThreshold = 0.1f;
    private bool clockwise = false;
   
    void Start()
    {
        highlighter = GameObject.Find("MeshHighlighter").GetComponent<MeshRaycast>();   
    }
 
    void Update()
    {
        // For now, use keyboard to move screw to touch the nut to trigger animation
        if (Input.GetKey(KeyCode.W) && animeReady)
        {
            transform.position += highlighter.animationRule.targetNormal * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S) && animeReady)
        {
            transform.position -= highlighter.animationRule.targetNormal * Time.deltaTime * speed;
        }

        // Animation disabled until rules imported
        if (animeReady && MeshRaycast.isAligned)
        {
            float distance = Vector3.Distance(marker_start.transform.position, marker_contact.transform.position);
            
            if (distance < distThreshold)
            {             
                StartCoroutine(MovementAnimation());
                animeReady = false;
            }
        }      
    }

    /// <summary>
    /// Before animation starts use text file animation rule to setup gear orientation
    /// </summary>
    /// <param name="rotateAxis"></param>
    /// <param name="angleOffset"></param>
    /// <param name="children"></param>
    public void GearSetup(Vector3 rotateAxis, float angleOffset, Transform[] children)
    {
        //children = gameObject.GetComponentsInChildren<Transform>();
    
        GearControl gearSource = children[1].GetComponent<GearControl>();
        gearSource.rotateAxis = rotateAxis;
        gearSource.clockwise = false;
        gearSource.speed = 100f;
      
        for (int i = 2; i < children.Length; i++)
        {
            //totOffset += angleOffset;
            //children[i].Rotate(Vector3.right, totOffset);           
            //gearSource = children[i-1].GetComponent<GearControl>();
            GearControl gear = children[i].GetComponent<GearControl>();     
            if(gear.reference != null)
            {               
                gear.clockwise = !gear.reference.clockwise;
                if (gear.clockwise)                
                    children[i].Rotate(Vector3.right, angleOffset);              

                gear.speed = 100f;
                gear.rotateAxis = !gear.reference.clockwise ? -rotateAxis : rotateAxis;
            }
            else
            {
                Debug.LogError("Gear " + i + "is missing reference gear!");
            }           
            
        }       
        
    }

    [ExposeInEditor(RuntimeOnly = false)]
    public void ImportGearRule()
    {
        rule = new AnimationRule();
        rule.gearAngle = highlighter.animationRule.gearAngle;
        rule.targetNormal = highlighter.animationRule.targetNormal;
        Transform[] gears = gameObject.GetComponentsInChildren<Transform>();
        GearSetup(rule.targetNormal, rule.gearAngle, gears);
        animeReady = true;
    }

    // Screw case animation 
    IEnumerator MovementAnimation()
    {
        Vector3 direction = (marker_end.transform.position- marker_contact.transform.position).normalized;

        float dot = Vector3.Dot(direction, rule.targetNormal);
        
        while (dot >= 0)
        {
            Debug.Log("Dot: " + dot);
            transform.position -= rule.targetNormal * Time.deltaTime * speed;
            direction = (marker_end.transform.position - marker_contact.transform.position).normalized;
            dot = Vector3.Dot(direction, rule.targetNormal);
            transform.Rotate(rule.targetNormal, 30f, relativeTo:Space.World);
            yield return null;
        }
        animeReady = true;
    }


    // Imports animation rules from AnimationRule class (or from text file containing AnimationRule info)
    // Sets animeReady flag indicating animation ready to play in Update()
    [ExposeInEditor(RuntimeOnly = false)]
    public void ImportRule()
    {      
        rule = new AnimationRule(highlighter.animationRule.objNames);
        //Create markers    
        rule.targetNormal = highlighter.animationRule.targetNormal;
        rule.startpt = highlighter.animationRule.startpt;
        rule.endpt = highlighter.animationRule.endpt;
        rule.contactpt = highlighter.animationRule.contactpt;

        GameObject sourceObj = GameObject.Find(rule.objNames[0]);
        GameObject targetObj = GameObject.Find(rule.objNames[1]);
        marker_start = Instantiate(marker, transform.TransformPoint(rule.startpt), Quaternion.identity);
        marker_start.name = "start";
        marker_end = Instantiate(marker, transform.TransformPoint(rule.endpt), Quaternion.identity);
        marker_end.name = "end";
        marker_start.transform.parent = sourceObj.transform;
        marker_end.transform.parent = sourceObj.transform;
       
        marker_contact = Instantiate(marker, targetObj.transform.TransformPoint(rule.contactpt), Quaternion.identity);

        marker_contact.name = "contact";
        marker_contact.transform.parent = targetObj.transform;

        animeReady = true;
    }
}
