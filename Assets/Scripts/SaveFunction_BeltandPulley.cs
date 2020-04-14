using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SaveFunction_BeltandPulley : MonoBehaviour
{
    public Button load, save, saveall, Authoring, Novice, run;
    public InputField animation, value;
    public Dropdown functionDropdown, animationDropdown;
    private string info, functionName, animationName;

    public TextAsset textAsset;

    [Serializable]
    public class Function_BeltandPulley
    {
        public string combination_name;
        public string Animation_name;
        public string big_pulley_centroid;
        public string small_pulley_centroid;
        public string big_pulley_speed;
    };

    [Serializable]
    public class Functions_BeltandPulley
    {
        //public Function[] functions;
        public List<Function_BeltandPulley> functions;
    }

    Function_BeltandPulley CustomFunction = new Function_BeltandPulley();

    List<string> options = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        Authoring.gameObject.SetActive(true);
        Authoring.image.color = Color.blue;
        Novice.gameObject.SetActive(true);
        Novice.image.color = Color.blue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onAuthoringClick()
    {
        Authoring.image.color = Color.green;
        Novice.image.color = Color.blue;
        load.gameObject.SetActive(true);
        load.enabled = true;       
        load.image.color = Color.green;
        save.gameObject.SetActive(true);
        save.enabled = false;        
        save.image.color = Color.blue;
        saveall.gameObject.SetActive(true);
        saveall.enabled = false;
        saveall.image.color = Color.blue;
        info = textAsset.text;
        animationDropdown.gameObject.SetActive(false);
        run.gameObject.SetActive(false);
    }

    public void onNoviceClick()
    {
        Authoring.image.color = Color.blue;
        Novice.image.color = Color.green;
        load.gameObject.SetActive(false);
        save.gameObject.SetActive(false);
        saveall.gameObject.SetActive(false);
        animationDropdown.gameObject.SetActive(true);
        PopulateDropdown(animationDropdown);
        run.gameObject.SetActive(true);
        run.image.color = Color.blue;
    }

    public void onBtnClick()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "Load")
        {
            if (EventSystem.current.currentSelectedGameObject.activeSelf == true)
            {
                animation.gameObject.SetActive(true);
                functionDropdown.gameObject.SetActive(true);
                PopulateDropdown(functionDropdown);
                load.enabled = false;
                load.image.color = Color.blue;
                value.gameObject.SetActive(false);
                value.enabled = false;
            }
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Save")
        {
            load.enabled = false;
            load.image.color = Color.blue;
            save.enabled = false;
            save.image.color = Color.blue;
            saveall.enabled = true;
            saveall.image.color = Color.green;
            value.enabled = false;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "SaveAll")
        {
            animation.gameObject.SetActive(false);
            functionDropdown.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
            load.enabled = true;
            load.image.color = Color.green;
            save.enabled = false;
            save.image.color = Color.blue;
            saveall.enabled = false;
            saveall.image.color = Color.blue;
            value.enabled = false;
            saveFunctions();
        }
        else if(EventSystem.current.currentSelectedGameObject.name == "Run")
        {
            if(run.GetComponentInChildren<Text>().text=="Run")
            {
                run.GetComponentInChildren<Text>().text = "Stop";
            }
            else
            {
                run.GetComponentInChildren<Text>().text = "Run";
            }
            runAnimation(animationName);
        }
    }

    void runAnimation(String AnimationName)
    {
        Functions_BeltandPulley CustomFunctions = new Functions_BeltandPulley();
        CustomFunctions = JsonUtility.FromJson<Functions_BeltandPulley>(info);
        Function_BeltandPulley CustomFunction = new Function_BeltandPulley();
        for (int i = 0; i < CustomFunctions.functions.Count; i++)
        {
            if ((CustomFunctions.functions[i].combination_name == "Belt and Pulley") && (CustomFunctions.functions[i].Animation_name == AnimationName))
            {
                CustomFunction = CustomFunctions.functions[i];
                SelectObject.runAnimation_BeltandPulley(int.Parse(CustomFunction.big_pulley_speed));
            }
        }
    }

    public void onAnimationDropdownCilck()
    {
        run.enabled = true;
        run.image.color = Color.green;
        animationName = animationDropdown.options[animationDropdown.value].text;
        switch (functionName)
        {
            case "Choose Animation":
                run.image.color = Color.blue;
                break;
        }
    }

    public void onDropdownCilck ()
    {
        save.enabled = true;
        save.image.color = Color.green;
        functionName = functionDropdown.options[functionDropdown.value].text;
        value.gameObject.SetActive(true);
        switch(functionName)
        {
            case "Choose Function":
                value.enabled = false;
                break;
            case "big_pulley_centroid":
                value.enabled = false;
                break;
            case "small_pulley_centroid":
                value.enabled = false;
                break;
            case "big_pulley_speed":
                value.enabled = true;
                break;
        }
    }

    public void onChangeValue()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "Animation")
        {
            CustomFunction.combination_name = "Belt and Pulley";
            CustomFunction.Animation_name = animation.text;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Value")
        {
            switch (functionName)
            {
                case "big_pulley_speed":
                    CustomFunction.big_pulley_speed = value.text;
                    break;
            }
        }
    }

    public void saveFunctions()
    {
        var path = "C:\\Users\\cdi\\Documents\\CGS_Final_Project\\Assets\\Resources\\";
        var fileName = textAsset.name + ".txt";
        info = textAsset.text;
        Functions_BeltandPulley CustomFunctions = new Functions_BeltandPulley();
        CustomFunctions = JsonUtility.FromJson<Functions_BeltandPulley>(info);
        Debug.Log(CustomFunctions.functions.Count);
        CustomFunctions.functions.Add(CustomFunction);
        info = JsonUtility.ToJson(CustomFunctions);
        try
        {
            Debug.Log(path + fileName);
            System.IO.File.WriteAllText(path + fileName, info);
        }
        catch (System.Exception exception)
        {
            string ErrorMessages = "File Write Error\n" + exception.Message;
            Debug.LogError(ErrorMessages);
        }
    }

    void PopulateDropdown(Dropdown dropdown)
    {
        options.Clear();
        if (dropdown.name == "FunctionDropdown")
        {
            String[] optionsArray = new String[] { "big_pulley_centroid", "small_pulley_centroid", "big_pulley_speed" };
            options.Add("Choose Function");
            foreach (var option in optionsArray)
            {
                options.Add(option); // Or whatever you want for a label
            }
        }
        else if (dropdown.name == "AnimationDropdown")
        {
            options.Add("Choose Animation");
            var path = "C:\\Users\\cdi\\Documents\\CGS_Final_Project\\Assets\\Resources\\";
            var fileName = textAsset.name + ".txt";
            info = textAsset.text;
            Functions_BeltandPulley CustomFunctions = new Functions_BeltandPulley();
            CustomFunctions = JsonUtility.FromJson<Functions_BeltandPulley>(info);
            for(int i =0; i<CustomFunctions.functions.Count; i++)
            {
                if(CustomFunctions.functions[i].combination_name == "Belt and Pulley")
                {
                    options.Add(CustomFunctions.functions[i].Animation_name);
                }
            }
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }
}
