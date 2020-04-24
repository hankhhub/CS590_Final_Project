﻿using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class AnimationControl_Novice : MonoBehaviour
{
    public GameObject marker;
    GameObject marker_start;
    GameObject marker_end;
    GameObject marker_contact;
    AnimationRule_Novice rule;
    AnimationRule_BeltandPulley rule_BeltandPulley;

    MeshRaycast_Novice highlighter;
    [Range(0, 10f)]
    float speed = 2.0f;
    float speedTransmission = 0.1f;

    private bool animeReady = false;
    private float distThreshold = 55.0f;
    private bool transmission = false;

    private string info;
    public TextAsset textAsset_BeltandPulley, textAsset_Gear, textAsset_Screw_Nut_Interaction, textAsset_Transmission;

    [Serializable]
    public class Function_Screw_Nut_Interaction
    {
        public string combination_name;
        public string Animation_name;
        public string sourceRotation;
        public string sourceNormal;
        public string targetCentroid;
        public string targetNormal;
        public string sourcePosition;
        public string startpt;
        public string endpt;
        public string contactpt;
        public string speed;
    };

    [Serializable]
    public class Functions_Screw_Nut_Interaction
    {
        public List<Function_Screw_Nut_Interaction> functions;
    }

    private Function_Screw_Nut_Interaction CustomFunction_Screw_Nut_Interaction = new Function_Screw_Nut_Interaction();

    [Serializable]
    public class Function_Gear
    {
        public string combination_name;
        public string Animation_name;
        public string gearAngle;
        public string surfaceNormal;
        public string speed;
    };

    [Serializable]
    public class Functions_Gear
    {
        public List<Function_Gear> functions;
    }

    private Function_Gear CustomFunction_Gear = new Function_Gear();

    [Serializable]
    public class Function_BeltandPulley
    {
        public string combination_name;
        public string Animation_name;
        public string big_pulley_radius;
        public string small_pulley_radius;
        public string surfaceNormal;
        public string big_pulley_speed;
    };

    [Serializable]
    public class Functions_BeltandPulley
    {
        public List<Function_BeltandPulley> functions;
    }

    private Function_BeltandPulley CustomFunction_BeltandPulley = new Function_BeltandPulley();

    [Serializable]
    public class Function_Transmission
    {
        public string combination_name;
        public string Animation_name;
        public string sourceRotation;
        public string sourceNormal;
        public string targetCentroid;
        public string targetNormal;
        public string sourcePosition;
        public string startpt;
        public string endpt;
        public string contactpts;
        public string speed1;
        public string gearAngle;
        public string surfaceNormal;
        public string big_pulley_radius;
        public string small_pulley_radius;
        public string speed2;
    };

    [Serializable]
    public class Functions_Transmission
    {
        public List<Function_Transmission> functions;
    }

    private static Function_Transmission CustomFunction_Transmission = new Function_Transmission();

    void Start()
    {
        highlighter = GameObject.Find("MeshHighlighter").GetComponent<MeshRaycast_Novice>();
    }

    void Update()
    {
        switch (SelectObject.combinationName)
        {
            case "Screw_Nut_Interaction":
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
                if (animeReady)
                {
                    float distance = Vector3.Distance(marker_start.transform.position, marker_contact.transform.position);

                    if (distance < distThreshold)
                    {
                        StartCoroutine(MovementAnimation());
                        animeReady = false;
                    }
                }
                break;
            case "Gear":
                break;
            case "Belt and Pulley":
                if (animeReady)
                {
                    StartCoroutine(MovementAnimation_BeltandPulley());
                    animeReady = false;
                }
                break;
            case "Transmission":
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
                if (animeReady && transmission)
                {
                    float distance = Vector3.Distance(marker_start.transform.position, marker_contact.transform.position);

                    if (distance < distThreshold)
                    {
                        StartCoroutine(MovementAnimationTransmission());
                        animeReady = false;
                    }
                }

                if(animeReady && (transmission == false))
                {
                    StartCoroutine(MovementAnimation_BeltandPulleyTransmission());
                    animeReady = false;
                }
                break;
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
        if (animeReady)
        {
            if (SelectObject.combinationName == "Transmission")
            {
                gearSource.speed = float.Parse(CustomFunction_Transmission.speed2);
            }
            else
            {
                gearSource.speed = float.Parse(CustomFunction_Gear.speed);
            }
        }
        else
        {
            gearSource.speed = 0;
        }
        

        for (int i = 2; i < children.Length; i++)
        {          
            GearControl gear = children[i].GetComponent<GearControl>();
            if (gear.reference != null)
            {
                gear.clockwise = !gear.reference.clockwise;
                if (gear.clockwise)
                    children[i].Rotate(rotateAxis, angleOffset);

                if (animeReady)
                {
                    if (SelectObject.combinationName == "Transmission")
                    {
                        float gear_ratio = gear.reference.teeth / gear.teeth;
                        gear.speed = gear_ratio * float.Parse(CustomFunction_Transmission.speed2);
                    }
                    else
                    {
                        gear.speed = float.Parse(CustomFunction_Gear.speed);
                    }
                }
                else
                {
                    gear.speed = 0;
                }
                gear.rotateAxis = !gear.reference.clockwise ? -rotateAxis : rotateAxis;
            }
            else
            {
                Debug.LogError("Gear " + i + " is missing reference gear!");
            }
            if(SelectObject.combinationName == "Transmission")
            {
                speedTransmission = gear.speed;
            }

        }

    }

    //[ExposeInEditor(RuntimeOnly = false)]
    public void ImportGearRule()
    {
        rule = new AnimationRule_Novice();
        rule.gearAngle = highlighter.animationRule.gearAngle;
        rule.targetNormal = highlighter.animationRule.targetNormal;
        Transform[] gears = gameObject.GetComponentsInChildren<Transform>();
        GearSetup(rule.targetNormal, rule.gearAngle, gears);
        animeReady = true;
    }

    // Screw case animation 
    IEnumerator MovementAnimation()
    {
        Vector3 direction = (marker_end.transform.position - marker_contact.transform.position).normalized;

        float dot = Vector3.Dot(direction, rule.targetNormal);

        while ((dot >= 0.3) && (animeReady))
        {
            Debug.Log("Dot: " + dot);
            speed = float.Parse(CustomFunction_Screw_Nut_Interaction.speed);
            GameObject.Find("Screw").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            direction = (marker_end.transform.position - marker_contact.transform.position).normalized;
            dot = Vector3.Dot(direction, rule.targetNormal);
            GameObject.Find("Screw").transform.Rotate(rule.targetNormal, 30f, relativeTo: Space.World);
            yield return null;
        }
        animeReady = true;
    }

    IEnumerator MovementAnimationTransmission()
    {
        Vector3 direction = (marker_end.transform.position - marker_contact.transform.position).normalized;

        float dot = Vector3.Dot(direction, rule.targetNormal);

        while ((dot >= 0.3) && (animeReady))
        {
            Debug.Log("Dot: " + dot);
            speed = float.Parse(CustomFunction_Transmission.speed1);
            GameObject.Find("Screw_02_0").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            GameObject.Find("Screw_02_1").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            GameObject.Find("Screw_02_2").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            GameObject.Find("Screw_02_3").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            GameObject.Find("Screw_02_4").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            GameObject.Find("Screw_02_5").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            direction = (marker_end.transform.position - marker_contact.transform.position).normalized;
            dot = Vector3.Dot(direction, rule.targetNormal);
            GameObject.Find("Screw_02_0").transform.Rotate(rule.targetNormal, 30f, relativeTo: Space.World);
            GameObject.Find("Screw_02_1").transform.Rotate(rule.targetNormal, 30f, relativeTo: Space.World);
            GameObject.Find("Screw_02_2").transform.Rotate(rule.targetNormal, 30f, relativeTo: Space.World);
            GameObject.Find("Screw_02_3").transform.Rotate(rule.targetNormal, 30f, relativeTo: Space.World);
            GameObject.Find("Screw_02_4").transform.Rotate(rule.targetNormal, 30f, relativeTo: Space.World);
            GameObject.Find("Screw_02_5").transform.Rotate(rule.targetNormal, 30f, relativeTo: Space.World);
            yield return null;
        }
        animeReady = true;
    }

    // Belt and Pulley case animation 
    IEnumerator MovementAnimation_BeltandPulley()
    {
        float big_pulley_speed, small_pulley_speed, big_pulley1_speed, small_pulley1_speed, big_pulley2_speed, small_pulley2_speed;
        float pulley_speed_ratio = (float.Parse(CustomFunction_BeltandPulley.big_pulley_radius)) / (float.Parse(CustomFunction_BeltandPulley.small_pulley_radius));
        Transform transform = GameObject.Find("Belt and Pulley").transform;
        foreach (Transform child in transform)
        {
            if (child.name == "BIG PULLEY")
            {
                big_pulley_speed = float.Parse(CustomFunction_BeltandPulley.big_pulley_speed);
                child.Rotate(rule_BeltandPulley.surfaceNormal, big_pulley_speed, relativeTo: Space.World);
            }
            else if (child.name == "SMALL PULLEY")
            {
                small_pulley_speed = (float.Parse(CustomFunction_BeltandPulley.big_pulley_speed) * pulley_speed_ratio);
                child.Rotate(rule_BeltandPulley.surfaceNormal, small_pulley_speed, relativeTo: Space.World);
            }
            else if (child.name == "BIG PULLEY_1")
            {
                big_pulley1_speed = (float.Parse(CustomFunction_BeltandPulley.big_pulley_speed) * pulley_speed_ratio);
                child.Rotate(rule_BeltandPulley.surfaceNormal, big_pulley1_speed, relativeTo: Space.World);
            }
            else if (child.name == "SMALL PULLEY_1")
            {
                small_pulley1_speed = (float.Parse(CustomFunction_BeltandPulley.big_pulley_speed) * pulley_speed_ratio * pulley_speed_ratio);
                child.Rotate(rule_BeltandPulley.surfaceNormal, small_pulley1_speed, relativeTo: Space.World);
            }
            else if (child.name == "BIG PULLEY_2")
            {
                big_pulley2_speed = (float.Parse(CustomFunction_BeltandPulley.big_pulley_speed));
                child.Rotate(rule_BeltandPulley.surfaceNormal, big_pulley2_speed, relativeTo: Space.World);
            }
            else if (child.name == "SMALL PULLEY_2")
            {
                small_pulley2_speed = (float.Parse(CustomFunction_BeltandPulley.big_pulley_speed) * pulley_speed_ratio);
                child.Rotate(rule_BeltandPulley.surfaceNormal, small_pulley2_speed, relativeTo: Space.World);
            }
        }
        yield return null;
        animeReady = true;
    }

    IEnumerator MovementAnimation_BeltandPulleyTransmission()
    {
        float big_pulley_speed, small_pulley_speed;
        float pulley_speed_ratio = (float.Parse(CustomFunction_Transmission.big_pulley_radius)) / (float.Parse(CustomFunction_Transmission.small_pulley_radius));
        Transform transform = GameObject.Find("Transmission").transform;
        foreach (Transform child in transform)
        {
            if (child.name == "Pulley2")
            {
                big_pulley_speed = speedTransmission;
                child.Rotate(rule.targetNormal * Time.deltaTime * big_pulley_speed, relativeTo: Space.Self);
            }
            else if (child.name == "Pulley1")
            {
                small_pulley_speed = (speedTransmission * pulley_speed_ratio);
                child.Rotate(rule.targetNormal * Time.deltaTime * small_pulley_speed, relativeTo: Space.Self);
            }
        }
        yield return null;
        animeReady = true;
    }


    void AlignObjectsNoviceClick()
    {
        GameObject.Find("Screw").transform.rotation = Quaternion.FromToRotation(-rule.sourceNormal, rule.targetNormal) * rule.sourceRotation;//GameObject.Find("Screw").transform.rotation; //rule.sourceRotation;
        GameObject.Find("Screw").transform.position = GameObject.Find("Nut").transform.TransformPoint(rule.targetCentroid) + rule.targetNormal; //rule.sourcePosition;
    }


    // Imports animation rules from AnimationRule class (or from text file containing AnimationRule info)
    // Sets animeReady flag indicating animation ready to play in Update()
    //[ExposeInEditor(RuntimeOnly = false)]
    public void ImportRule()
    {
        var fileName = "";
        switch (SelectObject.combinationName)
        {
            case "Screw_Nut_Interaction":
                fileName = textAsset_Screw_Nut_Interaction.name + ".txt";
                info = textAsset_Screw_Nut_Interaction.text;
                Functions_Screw_Nut_Interaction CustomFunctions_Screw_Nut_Interaction = new Functions_Screw_Nut_Interaction();
                CustomFunctions_Screw_Nut_Interaction = JsonUtility.FromJson<Functions_Screw_Nut_Interaction>(info);
                for (int i = 0; i < CustomFunctions_Screw_Nut_Interaction.functions.Count; i++)
                {
                    if ((CustomFunctions_Screw_Nut_Interaction.functions[i].combination_name == SelectObject.combinationName) && (CustomFunctions_Screw_Nut_Interaction.functions[i].Animation_name == SaveFunction_Novice.animationName))
                    {
                        CustomFunction_Screw_Nut_Interaction = CustomFunctions_Screw_Nut_Interaction.functions[i];
                    }
                }

                rule = new AnimationRule_Novice();
                //Create markers    
                rule.sourceRotation = StringToQuaternion(CustomFunction_Screw_Nut_Interaction.sourceRotation);
                rule.sourceNormal = StringToVector3(CustomFunction_Screw_Nut_Interaction.sourceNormal);
                rule.targetCentroid = StringToVector3(CustomFunction_Screw_Nut_Interaction.targetCentroid);
                rule.targetNormal = StringToVector3(CustomFunction_Screw_Nut_Interaction.targetNormal);
                rule.sourcePosition = StringToVector3(CustomFunction_Screw_Nut_Interaction.sourcePosition);
                rule.startpt = StringToVector3(CustomFunction_Screw_Nut_Interaction.startpt);
                rule.endpt = StringToVector3(CustomFunction_Screw_Nut_Interaction.endpt);
                rule.contactpt = StringToVector3(CustomFunction_Screw_Nut_Interaction.contactpt);

                GameObject sourceObj = GameObject.Find("Screw");
                GameObject targetObj = GameObject.Find("Nut");
                marker_start = Instantiate(marker, transform.TransformPoint(rule.startpt), Quaternion.identity);
                marker_start.name = "start";
                marker_end = Instantiate(marker, transform.TransformPoint(rule.endpt), Quaternion.identity);
                marker_end.name = "end";
                marker_start.transform.parent = sourceObj.transform;
                marker_end.transform.parent = sourceObj.transform;

                marker_contact = Instantiate(marker, targetObj.transform.TransformPoint(rule.contactpt), Quaternion.identity);

                marker_contact.name = "contact";
                marker_contact.transform.parent = targetObj.transform;
                AlignObjectsNoviceClick();

                if (SaveFunction_Novice.animationRun == true)
                {
                    animeReady = true;
                }
                else
                {
                    animeReady = false;
                }
                break;
            case "Gear":
                fileName = textAsset_Gear.name + ".txt";
                info = textAsset_Gear.text;
                Functions_Gear CustomFunctions_Gear = new Functions_Gear();
                CustomFunctions_Gear = JsonUtility.FromJson<Functions_Gear>(info);
                for (int i = 0; i < CustomFunctions_Gear.functions.Count; i++)
                {
                    if ((CustomFunctions_Gear.functions[i].combination_name == SelectObject.combinationName) && (CustomFunctions_Gear.functions[i].Animation_name == SaveFunction_Novice.animationName))
                    {
                        CustomFunction_Gear = CustomFunctions_Gear.functions[i];
                    }
                }

                rule = new AnimationRule_Novice();
                rule.gearAngle = float.Parse(CustomFunction_Gear.speed);
                rule.targetNormal = StringToVector3(CustomFunction_Gear.surfaceNormal);
                Transform[] gears = GameObject.Find("Gear").GetComponentsInChildren<Transform>();
                if (SaveFunction_Novice.animationRun == true)
                {
                    animeReady = true;
                    GearSetup(rule.targetNormal, rule.gearAngle, gears);
                }
                else
                {
                    animeReady = false;
                    GearSetup(rule.targetNormal, rule.gearAngle, gears);
                }
                break;
            case "Belt and Pulley":
                fileName = textAsset_BeltandPulley.name + ".txt";
                info = textAsset_BeltandPulley.text;
                Functions_BeltandPulley CustomFunctions_BeltandPulley = new Functions_BeltandPulley();
                CustomFunctions_BeltandPulley = JsonUtility.FromJson<Functions_BeltandPulley>(info);
                for (int i = 0; i < CustomFunctions_BeltandPulley.functions.Count; i++)
                {
                    if ((CustomFunctions_BeltandPulley.functions[i].combination_name == SelectObject.combinationName) && (CustomFunctions_BeltandPulley.functions[i].Animation_name == SaveFunction_Novice.animationName))
                    {
                        CustomFunction_BeltandPulley = CustomFunctions_BeltandPulley.functions[i];
                    }
                }
                rule_BeltandPulley = new AnimationRule_BeltandPulley();
                //Create markers    
                rule_BeltandPulley.surfaceNormal = StringToVector3(CustomFunction_BeltandPulley.surfaceNormal);
                if (SaveFunction_Novice.animationRun == true)
                {
                    animeReady = true;
                }
                else
                {
                    animeReady = false;
                }
                break;
            case "Transmission":
                fileName = textAsset_Transmission.name + ".txt";
                info = textAsset_Transmission.text;
                Functions_Transmission CustomFunctions_Transmission = new Functions_Transmission();
                CustomFunctions_Transmission = JsonUtility.FromJson<Functions_Transmission>(info);
                for (int i = 0; i < CustomFunctions_Transmission.functions.Count; i++)
                {
                    if ((CustomFunctions_Transmission.functions[i].combination_name == SelectObject.combinationName) && (CustomFunctions_Transmission.functions[i].Animation_name == SaveFunction_Novice.animationName))
                    {
                        CustomFunction_Transmission = CustomFunctions_Transmission.functions[i];
                    }
                }
                Debug.Log(CustomFunction_Transmission.speed1);
                if (CustomFunction_Transmission.speed1 != "")
                {
                    transmission = true;
                    List<string> contactpts = new List<string>();
                    string[] points = new String[] { };
                    points = CustomFunction_Transmission.contactpts.Split(';');
                    for (int i = 0; i < 6; i++)
                    {
                        contactpts.Add(points[i]);
                        rule = new AnimationRule_Novice(highlighter.animationRule.objNames);
                        //Create markers                  
                        //Transform sourceObj = GameObject.Find(rule.objNames[0]).transform;
                        Transform targetObj_Transmission = GameObject.Find("Transmission").transform.Find("Handle_Gear2/Plate");
                        /*rule.targetNormal = targetObj_Transmission.TransformVector(highlighter.animationRule.targetNormal);
                        rule.sourceNormal = transform.TransformVector(highlighter.animationRule.sourceNormal);
                        //rule.targetCentroid = targetObj.TransformPoint(highlighter.animationRule.targetCentroid);
                        rule.targetCentroid = targetObj_Transmission.TransformPoint(highlighter.animationRule.contactlist[i]);
                        rule.startpt = highlighter.animationRule.startpt;
                        rule.endpt = highlighter.animationRule.endpt;
                        rule.contactpt = highlighter.animationRule.contactlist[i];*/

                        //Create markers    
                        //rule.sourceRotation = StringToQuaternion(CustomFunction_Screw_Nut_Interaction.sourceRotation);
                        
                        rule.targetCentroid = targetObj_Transmission.TransformPoint(StringToVector3(contactpts[i]));
                        rule.targetNormal = targetObj_Transmission.TransformVector(StringToVector3(CustomFunction_Transmission.targetNormal));
                        rule.sourcePosition = StringToVector3(CustomFunction_Transmission.sourcePosition);
                        rule.startpt = StringToVector3(CustomFunction_Transmission.startpt);
                        rule.endpt = StringToVector3(CustomFunction_Transmission.endpt);
                        rule.contactpt = StringToVector3(contactpts[i]);

                        Transform sourceObj_Transmission = GameObject.Find("Transmission/Handle_Gear2/Plate/Screw_02_" + i).transform;
                        //GameObject targetObj = GameObject.Find("Nut");
                        rule.sourceNormal = sourceObj_Transmission.TransformVector(StringToVector3(CustomFunction_Transmission.sourceNormal));

                        marker_start = Instantiate(marker, sourceObj_Transmission.transform.TransformPoint(rule.startpt), Quaternion.identity);
                        marker_start.name = "start";
                        marker_end = Instantiate(marker, sourceObj_Transmission.transform.TransformPoint(rule.endpt), Quaternion.identity);
                        marker_end.name = "end";
                        marker_start.transform.parent = sourceObj_Transmission.transform;
                        marker_end.transform.parent = sourceObj_Transmission.transform;

                        marker_contact = Instantiate(marker, targetObj_Transmission.TransformPoint(rule.contactpt), Quaternion.identity);

                        marker_contact.name = "contact";
                        marker_contact.transform.parent = targetObj_Transmission;

                        sourceObj_Transmission.rotation = Quaternion.FromToRotation(-rule.sourceNormal, rule.targetNormal) * sourceObj_Transmission.rotation;
                        sourceObj_Transmission.position = rule.targetCentroid + rule.targetNormal;
                        //AlignObjects(rule);
                    }
                    if (SaveFunction_Novice.animationRun == true)
                    {
                        animeReady = true;
                    }
                    else
                    {
                        animeReady = false;
                    }
                }
                else
                {
                    transmission = false;
                    rule = new AnimationRule_Novice(highlighter.animationRule.objNames);
                    rule.gearAngle = float.Parse(CustomFunction_Transmission.speed2);
                    rule.targetNormal = StringToVector3(CustomFunction_Transmission.surfaceNormal);
                    GameObject gearTransmission = GameObject.Find("Transmission/Gear");
                    Debug.Log(gearTransmission.name);
                    Transform[] gearsTransmission = gearTransmission.GetComponentsInChildren<Transform>();
                    if (SaveFunction_Novice.animationRun == true)
                    {
                        animeReady = true;
                        GearSetup(rule.targetNormal, rule.gearAngle, gearsTransmission);
                    }
                    else
                    {
                        animeReady = false;
                        GearSetup(rule.targetNormal, rule.gearAngle, gearsTransmission);
                    }
                }
                break;
        }
    }

    private void AlignObjects(AnimationRule_Novice rule)
    {
        Transform sourceObj = GameObject.Find(rule.objNames[0]).transform;
        Transform targetObj = GameObject.Find(rule.objNames[1]).transform;
        transform.rotation = Quaternion.FromToRotation(-rule.sourceNormal, rule.targetNormal) * transform.rotation;
        transform.position = rule.targetCentroid + rule.targetNormal;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    public static Quaternion StringToQuaternion(string tQuaternion)
    {
        // Remove the parentheses
        if (tQuaternion.StartsWith("(") && tQuaternion.EndsWith(")"))
        {
            tQuaternion = tQuaternion.Substring(1, tQuaternion.Length - 2);
        }

        // split the items
        string[] tArray = tQuaternion.Split(',');

        // store as a Quaternion
        Quaternion result = new Quaternion(
            float.Parse(tArray[0]),
            float.Parse(tArray[1]),
            float.Parse(tArray[2]),
            float.Parse(tArray[3]));

        return result;
    }
}
