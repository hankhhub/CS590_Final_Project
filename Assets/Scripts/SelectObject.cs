using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Dropdown dropdownSource;
    private bool dropdownOpen = false;
    public GameObject sourceScrew;
    public GameObject targetNut;
    public GameObject sourceGear;
    public GameObject targetGear;
    public GameObject BeltandPulley;
    private static GameObject sourceObject, targetObject, selectedObject;
    private static Button transformBtn;
    public static GameObject x_control, y_control, z_control;
    public Button Button;
    public GameObject x_axis, y_axis, z_axis;
    public static int btnCounter = 0;
    private static bool animationBtnPressed = false;
    private static int bigpulley_speed;

    void Start()
    {
        transformBtn = Button;
        x_control = x_axis;
        y_control = y_axis;
        z_control = z_axis;
    }

    // Update is called once per frame
    void Update()
    {
        if (dropdownOpen && Input.GetKeyDown(KeyCode.A))
        {
            dropdownSource.value = 1;
        }
        if (dropdownOpen && Input.GetKeyDown(KeyCode.B))
        {
            dropdownSource.value = 2;
        }
        if (dropdownOpen && Input.GetKeyDown(KeyCode.C))
        {
            dropdownSource.value = 3;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            dropdownOpen = !dropdownOpen;
            if(dropdownOpen == true)
            {
                dropdownSource.Show();
            }
           else
            {
                dropdownSource.Hide();
            }
        }
        if(animationBtnPressed)
        {
            sourceObject.transform.RotateAround(sourceObject.transform.position, sourceObject.transform.forward, bigpulley_speed * Time.deltaTime);
        }
    }

    public void onDropdownSelected()
    {
        Debug.Log(dropdownSource.options[dropdownSource.value].text);
        string sourceName = dropdownSource.options[dropdownSource.value].text;
        //sourceScrew.gameObject.SetActive(false);
        //sourceGear.gameObject.SetActive(false);
        BeltandPulley.gameObject.SetActive(true);
        switch (sourceName)
        {
            case "Screw":
                sourceScrew.gameObject.SetActive(true);
                targetNut.gameObject.SetActive(true);
                sourceObject = sourceScrew.gameObject;
                targetObject = targetNut.gameObject;
                break;
            case "Gear":
                sourceGear.gameObject.SetActive(true);
                targetGear.gameObject.SetActive(true);
                sourceObject = sourceScrew.gameObject;
                targetObject = targetNut.gameObject;
                break;
            case "Belt and Pulley":
                BeltandPulley.gameObject.SetActive(true);
                sourceObject = BeltandPulley.gameObject.transform.Find("BIG PULLEY").gameObject;
                break;
        }
    }

    public static void transformObject(bool source, bool target, bool t, bool r, bool s, bool x, bool y, bool z, bool p, bool n)
    {
        var speed = 100;
        var speed1 = 1;
        if (source)
        {
            selectedObject = sourceObject;
        }
        else if(target)
        {
            selectedObject = targetObject;
        }
        if(t)
        {
            if (x)
            {
                if (p)
                    selectedObject.transform.position += selectedObject.transform.right * speed1;
                else if (n)
                    selectedObject.transform.position -= selectedObject.transform.right * speed1;
            }
            if (y)
            {
                if (p)
                    selectedObject.transform.position += selectedObject.transform.up * speed1;
                else if (n)
                    selectedObject.transform.position -= selectedObject.transform.up * speed1;
            }
            if (z)
            {
                if (p)
                    selectedObject.transform.position += selectedObject.transform.forward * speed1;
                else if (n)
                    selectedObject.transform.position -= selectedObject.transform.forward * speed1;
            }
        }
        if(r)
        {
            if(x)
            {
                if (p)
                    //Debug.Log("Here");
                    selectedObject.transform.RotateAround(selectedObject.transform.position, selectedObject.transform.right, speed * Time.deltaTime);
                else if (n)
                    selectedObject.transform.RotateAround(selectedObject.transform.position, selectedObject.transform.right, 0 - speed * Time.deltaTime);
            }
            if(y)
            {
                if(p)
                    selectedObject.transform.RotateAround(selectedObject.transform.position, selectedObject.transform.up, speed * Time.deltaTime);
                else if(n)
                    selectedObject.transform.RotateAround(selectedObject.transform.position, selectedObject.transform.up, 0 - speed * Time.deltaTime);
            }
            if(z)
            {
                if(p)
                    selectedObject.transform.RotateAround(selectedObject.transform.position, selectedObject.transform.forward, speed * Time.deltaTime);
                else if(n)
                    selectedObject.transform.RotateAround(selectedObject.transform.position, selectedObject.transform.forward, 0 - speed * Time.deltaTime);
            }
        }
        if(s)
        {
            if (p)
            {
                sourceObject.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
                targetObject.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
            }
            else if (n)
            {
                sourceObject.transform.localScale -= new Vector3(-0.01f, -0.01f, -0.01f);
                targetObject.transform.localScale -= new Vector3(-0.01f, -0.01f, -0.01f);
            }
        }
    }

    public static void onBtnClick(bool t, bool r, bool s)
    {
            if(t)
                transformBtn.GetComponentInChildren<Text>().text = "Translate";
            else if(r)
                transformBtn.GetComponentInChildren<Text>().text = "Rotate";
            else if(s)
                transformBtn.GetComponentInChildren<Text>().text = "Scale";
    }

    public static void showAxes(bool x, bool y, bool z)
    {
        x_control.GetComponent<MeshRenderer>().material.color = Color.red;
        y_control.GetComponent<MeshRenderer>().material.color = Color.green;
        z_control.GetComponent<MeshRenderer>().material.color = Color.blue;
        if (x)
            x_control.GetComponent<MeshRenderer>().material.color = Color.black;
        else if (y)
            y_control.GetComponent<MeshRenderer>().material.color = Color.black;
        else if (z)
            z_control.GetComponent<MeshRenderer>().material.color = Color.black;
    }

    public static string getSourceName()
    {
        return sourceObject.name;
    }

    public static string getTargetName()
    {
        return targetObject.name;
    }

    public static string getObjectName()
    {
        return selectedObject.name;
    }

    public static void runAnimation_BeltandPulley(int speed)
    {
        animationBtnPressed = !animationBtnPressed;
        bigpulley_speed = speed;
    }

}
