using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class AnimationControl : MonoBehaviour
{
    public GameObject marker;
    GameObject marker_start;
    GameObject marker_end;
    GameObject marker_contact;
    AnimationRule rule;
    AnimationRule_BeltandPulley rule_BeltandPulley;

    MeshRaycast highlighter;
    [Range(0, 10f)]
    float speed = 2.0f;

    private bool animeReady = false;
    private float distThreshold = 55.0f;

    private string info;
    public TextAsset textAsset_BeltandPulley, textAsset_Gear, textAsset_Screw_Nut_Interaction;

    [Serializable]
    public class Function_Screw_Nut_Interaction
    {
        public string combination_name;
        public string Animation_name;
        public string sourceRotation;
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

    void Start()
    {
        highlighter = GameObject.Find("MeshHighlighter").GetComponent<MeshRaycast>();
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
            gearSource.speed = float.Parse(CustomFunction_Gear.speed);
        }
        else
        {
            gearSource.speed = 0;
        }

        for (int i = 2; i < children.Length; i++)
        {
            //totOffset += angleOffset;
            //children[i].Rotate(Vector3.right, totOffset);           
            //gearSource = children[i-1].GetComponent<GearControl>();
            GearControl gear = children[i].GetComponent<GearControl>();
            if (gear.reference != null)
            {
                gear.clockwise = !gear.reference.clockwise;
                if (gear.clockwise)
                    children[i].Rotate(Vector3.right, angleOffset);

                if (animeReady)
                {
                    gear.speed = float.Parse(CustomFunction_Gear.speed);
                }
                else
                {
                    gear.speed = 0;
                }
                gear.rotateAxis = !gear.reference.clockwise ? -rotateAxis : rotateAxis;
            }
            else
            {
                Debug.LogError("Gear " + i + "is missing reference gear!");
            }

        }

    }

    //[ExposeInEditor(RuntimeOnly = false)]
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
        Vector3 direction = (marker_end.transform.position - marker_contact.transform.position).normalized;

        float dot = Vector3.Dot(direction, rule.targetNormal);

        while ((dot >= 0.3) && (animeReady))
        {
            Debug.Log("Dot: " + dot);
            speed = float.Parse(CustomFunction_Screw_Nut_Interaction.speed);
            GameObject.Find("Screw").transform.position -= rule.targetNormal * Time.deltaTime * speed;
            direction = (marker_end.transform.position - marker_contact.transform.position).normalized;
            dot = Vector3.Dot(direction, rule.targetNormal);
            GameObject.Find("Screw").transform.Rotate(rule.targetNormal, 10f, relativeTo: Space.World);
            yield return null;
        }
        animeReady = true;
    }

    // Belt and Pulley case animation 
    IEnumerator MovementAnimation_BeltandPulley()
    {
        float big_pulley_speed, small_pulley_speed;
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
                small_pulley_speed = (float.Parse(CustomFunction_BeltandPulley.big_pulley_speed) * float.Parse(CustomFunction_BeltandPulley.big_pulley_radius)) / (float.Parse(CustomFunction_BeltandPulley.small_pulley_radius));
                child.Rotate(rule_BeltandPulley.surfaceNormal, small_pulley_speed, relativeTo: Space.World);
            }
        }
        yield return null;
        animeReady = true;
    }


    void AlignObjectsNoviceClick()
    {
        GameObject.Find("Screw").transform.rotation = rule.sourceRotation;//Quaternion.FromToRotation(-rule.sourceNormal, rule.targetNormal) * GameObject.Find("Screw").transform.rotation;
        GameObject.Find("Screw").transform.position = rule.sourcePosition;//GameObject.Find("Nut").transform.TransformPoint(rule.targetCentroid) + rule.targetNormal;
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
                    if ((CustomFunctions_Screw_Nut_Interaction.functions[i].combination_name == SelectObject.combinationName) && (CustomFunctions_Screw_Nut_Interaction.functions[i].Animation_name == SaveFunction_BeltandPulley.animationName))
                    {
                        CustomFunction_Screw_Nut_Interaction = CustomFunctions_Screw_Nut_Interaction.functions[i];
                    }
                }

                rule = new AnimationRule(highlighter.animationRule.objNames);
                //Create markers    
                rule.sourceRotation = StringToQuaternion(CustomFunction_Screw_Nut_Interaction.sourceRotation);
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

                if (SaveFunction_BeltandPulley.animationRun == true)
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
                    if ((CustomFunctions_Gear.functions[i].combination_name == SelectObject.combinationName) && (CustomFunctions_Gear.functions[i].Animation_name == SaveFunction_BeltandPulley.animationName))
                    {
                        CustomFunction_Gear = CustomFunctions_Gear.functions[i];
                    }
                }

                rule = new AnimationRule();
                rule.gearAngle = float.Parse(CustomFunction_Gear.speed);
                rule.targetNormal = StringToVector3(CustomFunction_Gear.surfaceNormal);
                Transform[] gears = GameObject.Find("Gear").GetComponentsInChildren<Transform>();
                if (SaveFunction_BeltandPulley.animationRun == true)
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
                    if ((CustomFunctions_BeltandPulley.functions[i].combination_name == SelectObject.combinationName) && (CustomFunctions_BeltandPulley.functions[i].Animation_name == SaveFunction_BeltandPulley.animationName))
                    {
                        CustomFunction_BeltandPulley = CustomFunctions_BeltandPulley.functions[i];
                    }
                }
                rule_BeltandPulley = new AnimationRule_BeltandPulley(highlighter.animationRule_BeltandPulley.objNames);
                //Create markers    
                rule_BeltandPulley.surfaceNormal = StringToVector3(CustomFunction_BeltandPulley.surfaceNormal);
                if (SaveFunction_BeltandPulley.animationRun == true)
                {
                    animeReady = true;
                }
                else
                {
                    animeReady = false;
                }
                break;
        }
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
