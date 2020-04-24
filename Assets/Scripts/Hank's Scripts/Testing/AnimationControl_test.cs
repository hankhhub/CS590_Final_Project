using System.Collections;
using UnityEngine;
using System;

public class AnimationControl_test : MonoBehaviour
{
    public GameObject marker;
    GameObject marker_start;
    GameObject marker_end;
    GameObject marker_contact;
    AnimeRule rule;

    MeshRaycast_test highlighter;
    [Range(0, 10f)]
    public float speed;

    private bool animeReady = false;
    private float distThreshold = 0.1f;
    private bool clockwise = false;
    private bool isAligned = false;

    void Start()
    {
        highlighter = GameObject.Find("MeshHighlighter").GetComponent<MeshRaycast_test>();   
    }
 
    void Update()
    {
        // For now, use keyboard to move screw to touch the nut to trigger animation
        if (Input.GetKey(KeyCode.W) && animeReady)
        {
            transform.position += rule.targetNormal * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S) && animeReady)
        {
            transform.position -= rule.targetNormal * Time.deltaTime * speed;
        }

        // Animation disabled until rules imported
        if (animeReady && isAligned)
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
           
            GearControl gear = children[i].GetComponent<GearControl>();     
            if(gear.reference != null)
            {               
                gear.clockwise = !gear.reference.clockwise;
                if (gear.clockwise)                
                    children[i].Rotate(rotateAxis, angleOffset);              

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
        rule = new AnimeRule();
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
            //Debug.Log("Dot: " + dot);
            transform.position -= rule.targetNormal * Time.deltaTime * speed;
            direction = (marker_end.transform.position - marker_contact.transform.position).normalized;
            dot = Vector3.Dot(direction, rule.targetNormal);
            transform.Rotate(rule.targetNormal, 30f, relativeTo:Space.World);
            yield return null;
        }
        animeReady = true;
    }

    private void AlignObjects(AnimeRule rule)
    {
        Transform sourceObj = GameObject.Find(rule.objNames[0]).transform;
        Transform targetObj = GameObject.Find(rule.objNames[1]).transform;
        transform.rotation = Quaternion.FromToRotation(-rule.sourceNormal, rule.targetNormal) * transform.rotation;
        transform.position = rule.targetCentroid + rule.targetNormal;       
        isAligned = true;      
    }


    // Imports animation rules from AnimationRule class (or from text file containing AnimationRule info)
    // Sets animeReady flag indicating animation ready to play in Update()
    [ExposeInEditor(RuntimeOnly = false)]
    public void ImportScrewRule(int i)
    {      
        rule = new AnimeRule(highlighter.animationRule.objNames);
        //Create markers                  
        //Transform sourceObj = GameObject.Find(rule.objNames[0]).transform;
        Transform targetObj = GameObject.Find(rule.objNames[1]).transform;
        rule.targetNormal = targetObj.TransformVector(highlighter.animationRule.targetNormal);
        rule.sourceNormal = transform.TransformVector(highlighter.animationRule.sourceNormal);
        //rule.targetCentroid = targetObj.TransformPoint(highlighter.animationRule.targetCentroid);
        rule.targetCentroid = targetObj.TransformPoint(highlighter.animationRule.contactlist[i]);
        rule.startpt = highlighter.animationRule.startpt;
        rule.endpt = highlighter.animationRule.endpt;
        rule.contactpt = highlighter.animationRule.contactlist[i];

        marker_start = Instantiate(marker, transform.TransformPoint(rule.startpt), Quaternion.identity);
        marker_start.name = "start";
        marker_end = Instantiate(marker, transform.TransformPoint(rule.endpt), Quaternion.identity);
        marker_end.name = "end";
        marker_start.transform.parent = transform;
        marker_end.transform.parent = transform;
       
        marker_contact = Instantiate(marker, targetObj.TransformPoint(rule.contactpt), Quaternion.identity);

        marker_contact.name = "contact";
        marker_contact.transform.parent = targetObj;

        AlignObjects(rule);
        animeReady = true;
    }
}
