using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeTransform : MonoBehaviour
{
    public Button transformBtn;
    public Dropdown dropdownSource;
    public GameObject sourceScrew;
    public GameObject sourceGear;
    public GameObject sourceHand;
    public Material material1, material2, material3, material4;
    public static float sliderX = 0, sliderY = 0, sliderZ = 0;
    public static int btnCounter = 0;
    public static string source = "Screw";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onBtnClick()
    {
        btnCounter++;
        switch (btnCounter % 4)
        {
            case 0:
                transformBtn.GetComponentInChildren<Text>().text = "Select Transform";
                transformBtn.image.color = material1.color;
                break;
            case 1:
                transformBtn.GetComponentInChildren<Text>().text = "Translate";
                transformBtn.image.color = material2.color;
                break;
            case 2:
                transformBtn.GetComponentInChildren<Text>().text = "Rotate";
                transformBtn.image.color = material3.color;
                break;
            case 3:
                transformBtn.GetComponentInChildren<Text>().text = "Scale";
                transformBtn.image.color = material4.color;
                break;
        }
    }

    public void onDropDownChanged()
    {
        source = "Screw";
    }

    public void onDropdownSelected()
    {
        Debug.Log(dropdownSource.options[dropdownSource.value].text);
        string sourceName = dropdownSource.options[dropdownSource.value].text;
        sourceScrew.gameObject.SetActive(false);
        sourceGear.gameObject.SetActive(false);
        //sourceHand.gameObject.SetActive(true);
        switch (sourceName)
        {
            case "Screw":
                sourceScrew.gameObject.SetActive(true);
                break;
            case "Gear":
                sourceGear.gameObject.SetActive(true);
                break;
            case "Hand":
                //sourceHand.gameObject.SetActive(true);
                break;
        }
    }
}
