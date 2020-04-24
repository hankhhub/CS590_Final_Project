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
    public Dropdown functionDropdown, animationDropdown, objectDropdown;
    private string info, functionName;
    public static string animationName;
    public static int functionNumber;
    public static bool funcEnable = false;
    public static bool animationRun = false;
    public static bool animationEnable = false;

    public TextAsset textAsset_BeltandPulley, textAsset_Gear, textAsset_Screw_Nut_Interaction;

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

    public static Function_BeltandPulley CustomFunction_BeltandPulley = new Function_BeltandPulley();

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

    public static Function_Gear CustomFunction_Gear = new Function_Gear();

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

    public static Function_Screw_Nut_Interaction CustomFunction_Screw_Nut_Interaction = new Function_Screw_Nut_Interaction();

    List<string> options = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        Authoring.gameObject.SetActive(true);
        Authoring.image.color = Color.cyan;
        Novice.gameObject.SetActive(true);
        Novice.image.color = Color.cyan;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onObjectDropdownClick()
    {
        Authoring.image.color = Color.cyan;
        Novice.image.color = Color.cyan;
        load.gameObject.SetActive(false);
        save.gameObject.SetActive(false);
        saveall.gameObject.SetActive(false);
        animation.gameObject.SetActive(false);
        functionDropdown.gameObject.SetActive(false);
        value.gameObject.SetActive(false);
        animationDropdown.gameObject.SetActive(false);
        run.gameObject.SetActive(false);
    }

    public void onAuthoringClick()
    {
        Authoring.image.color = Color.green;
        Novice.image.color = Color.cyan;
        load.gameObject.SetActive(true);
        load.enabled = true;       
        load.image.color = Color.green;
        save.gameObject.SetActive(true);
        save.enabled = false;        
        save.image.color = Color.cyan;
        saveall.gameObject.SetActive(true);
        saveall.enabled = false;
        saveall.image.color = Color.cyan;
        animationDropdown.gameObject.SetActive(false);
        run.gameObject.SetActive(false);
    }

    public void onNoviceClick()
    {
        Authoring.image.color = Color.cyan;
        Novice.image.color = Color.green;
        load.gameObject.SetActive(false);
        save.gameObject.SetActive(false);
        saveall.gameObject.SetActive(false);
        animationDropdown.gameObject.SetActive(true);
        PopulateDropdown(animationDropdown);
        run.gameObject.SetActive(true);
        run.image.color = Color.cyan;
        animation.gameObject.SetActive(false);
        functionDropdown.gameObject.SetActive(false);
        value.gameObject.SetActive(false);
    }

    public void onBtnClick()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "Load")
        {
            if (EventSystem.current.currentSelectedGameObject.activeSelf == true)
            {
                animation.gameObject.SetActive(true);
                animation.enabled = true;
                animation.text = default;
                functionDropdown.gameObject.SetActive(true);
                PopulateDropdown(functionDropdown);
                load.enabled = false;
                load.image.color = Color.cyan;
                value.gameObject.SetActive(false);
                value.enabled = false;
            }
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Save")
        {
            load.enabled = false;
            load.image.color = Color.cyan;
            save.enabled = false;
            save.image.color = Color.cyan;
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
            save.image.color = Color.cyan;
            saveall.enabled = false;
            saveall.image.color = Color.cyan;
            value.enabled = false;
            saveFunctions();
        }
        else if(EventSystem.current.currentSelectedGameObject.name == "Run")
        {
            if(run.GetComponentInChildren<Text>().text=="Run")
            {
                animationRun = true;
                run.GetComponentInChildren<Text>().text = "Stop";
                run.image.color = Color.green;
            }
            else
            {
                animationRun = false;
                run.GetComponentInChildren<Text>().text = "Run";
                run.image.color = Color.cyan;
            }
            //runAnimation(animationName);
        }
    }

    void runAnimation(String AnimationName)
    {
        switch (SelectObject.combinationName)
        {
            case "Screw_Nut_Interaction":
                break;
            case "Gear":
                break;
            case "Belt and Pulley":
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
                break;
        }
    }

    public void onAnimationDropdownCilck()
    {
        run.enabled = true;
        run.image.color = Color.cyan;
        run.GetComponentInChildren<Text>().text = "Run";
        animationName = animationDropdown.options[animationDropdown.value].text;
        animationEnable = true;
        switch (functionName)
        {
            case "Choose Animation":
                run.image.color = Color.cyan;
                break;
        }
    }

    public void onDropdownCilck ()
    {
        save.enabled = true;
        save.image.color = Color.green;
        functionName = functionDropdown.options[functionDropdown.value].text;
        funcEnable = false;
        switch (SelectObject.combinationName)
        {
            case "Screw_Nut_Interaction":
                switch (functionName)
                {
                    case "Choose Function":
                        value.gameObject.SetActive(false);
                        break;
                    case "targetNormal":
                        functionNumber = 3;
                        funcEnable = true;
                        CustomFunction_Screw_Nut_Interaction.targetNormal = "";
                        value.gameObject.SetActive(false);
                        break;
                    case "startpt":
                        functionNumber = 0;
                        funcEnable = true;
                        CustomFunction_Screw_Nut_Interaction.startpt = "";
                        value.gameObject.SetActive(false);
                        break;
                    case "endpt":
                        functionNumber = 1;
                        funcEnable = true;
                        CustomFunction_Screw_Nut_Interaction.endpt = "";
                        value.gameObject.SetActive(false);
                        break;
                    case "contactpt":
                        functionNumber = 2;
                        funcEnable = true;
                        CustomFunction_Screw_Nut_Interaction.contactpt = "";
                        value.gameObject.SetActive(false);
                        break;
                    case "speed":
                        value.gameObject.SetActive(true);
                        value.enabled = true;
                        value.text = default;
                        break;
                }
                break;
            case "Gear":
                switch (functionName)
                {
                    case "Choose Function":
                        value.gameObject.SetActive(false);
                        break;
                    case "gearAngle":
                        functionNumber = 0;
                        funcEnable = true;
                        CustomFunction_Gear.gearAngle = "";
                        value.gameObject.SetActive(false);
                        break;
                    case "surfaceNormal":
                        functionNumber = 1;
                        funcEnable = true;
                        CustomFunction_Gear.surfaceNormal = "";
                        value.gameObject.SetActive(false);
                        break;
                    case "speed":
                        value.gameObject.SetActive(true);
                        value.enabled = true;
                        value.text = default;
                        break;
                }
                break;
            case "Belt and Pulley":
                switch (functionName)
                {
                    case "Choose Function":
                        value.gameObject.SetActive(false);
                        break;
                    case "big_pulley_radius":
                        functionNumber = 0;
                        funcEnable = true;
                        value.gameObject.SetActive(false);
                        break;
                    case "small_pulley_radius":
                        functionNumber = 1;
                        funcEnable = true;
                        value.gameObject.SetActive(false);
                        break;
                    case "surfaceNormal":
                        functionNumber = 2;
                        funcEnable = true;
                        value.gameObject.SetActive(false);
                        break;
                    case "big_pulley_speed":
                        value.gameObject.SetActive(true);
                        value.enabled = true;
                        value.text = default;
                        break;
                }
                break;
        }
    }

    public void onChangeValue()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "Animation")
        {
            animation.enabled = false;
            switch (SelectObject.combinationName)
            {
                case "Screw_Nut_Interaction":
                    CustomFunction_Screw_Nut_Interaction.combination_name = SelectObject.combinationName;
                    CustomFunction_Screw_Nut_Interaction.Animation_name = animation.text;
                    break;
                case "Gear":
                    CustomFunction_Gear.combination_name = SelectObject.combinationName;
                    CustomFunction_Gear.Animation_name = animation.text;
                    break;
                case "Belt and Pulley":
                    CustomFunction_BeltandPulley.combination_name = SelectObject.combinationName;
                    CustomFunction_BeltandPulley.Animation_name = animation.text;
                    break;
            }
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Value")
        {
            switch (SelectObject.combinationName)
            {
                case "Screw_Nut_Interaction":
                    switch (functionName)
                    {
                        case "speed":
                            CustomFunction_Screw_Nut_Interaction.speed = value.text;
                            break;
                    }
                    break;
                case "Gear":
                    switch (functionName)
                    {
                        case "speed":
                            CustomFunction_Gear.speed = value.text;
                            break;
                    }
                    break;
                case "Belt and Pulley":
                    switch (functionName)
                    {
                        case "big_pulley_speed":
                            CustomFunction_BeltandPulley.big_pulley_speed = value.text;
                            break;
                    }
                    break;
            }           
        }
    }

    public void saveFunctions()
    {
        var path = "C:\\Users\\anany\\OneDrive\\Documents\\CGS_Final_Project\\CGS_Final_Project\\Assets\\Resources\\";
        var fileName = "";
        switch(SelectObject.combinationName)
        {
            case "Screw_Nut_Interaction":
                fileName = textAsset_Screw_Nut_Interaction.name + ".txt";
                info = textAsset_Screw_Nut_Interaction.text;
                Functions_Screw_Nut_Interaction CustomFunctions_Screw_Nut_Interaction = new Functions_Screw_Nut_Interaction();
                CustomFunctions_Screw_Nut_Interaction = JsonUtility.FromJson<Functions_Screw_Nut_Interaction>(info);
                Debug.Log(CustomFunctions_Screw_Nut_Interaction.functions.Count);
                CustomFunctions_Screw_Nut_Interaction.functions.Add(CustomFunction_Screw_Nut_Interaction);
                info = JsonUtility.ToJson(CustomFunctions_Screw_Nut_Interaction);
                break;
            case "Gear":
                fileName = textAsset_Gear.name + ".txt";
                info = textAsset_Gear.text;
                Functions_Gear CustomFunctions_Gear = new Functions_Gear();
                CustomFunctions_Gear = JsonUtility.FromJson<Functions_Gear>(info);
                Debug.Log(CustomFunctions_Gear.functions.Count);
                CustomFunctions_Gear.functions.Add(CustomFunction_Gear);
                info = JsonUtility.ToJson(CustomFunctions_Gear);
                break;
            case "Belt and Pulley":
                fileName = textAsset_BeltandPulley.name + ".txt";
                info = textAsset_BeltandPulley.text;
                Functions_BeltandPulley CustomFunctions_BeltandPulley = new Functions_BeltandPulley();
                CustomFunctions_BeltandPulley = JsonUtility.FromJson<Functions_BeltandPulley>(info);
                Debug.Log(CustomFunctions_BeltandPulley.functions.Count);
                CustomFunctions_BeltandPulley.functions.Add(CustomFunction_BeltandPulley);
                info = JsonUtility.ToJson(CustomFunctions_BeltandPulley);
                break;
        }      
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
            String[] optionsArray = new String[] { };
            switch (SelectObject.combinationName)
            {
                case "Screw_Nut_Interaction":
                    optionsArray = new String[] { "targetNormal", "startpt", "endpt", "contactpt", "speed"};
                    break;
                case "Gear":
                    optionsArray = new String[] { "gearAngle", "surfaceNormal", "speed" };
                    break;
                case "Belt and Pulley":
                    optionsArray = new String[] { "big_pulley_radius", "small_pulley_radius", "surfaceNormal", "big_pulley_speed" };
                    break;
            }
            options.Add("Choose Function");
            foreach (var option in optionsArray)
            {
                options.Add(option); // Or whatever you want for a label
            }
        }
        else if (dropdown.name == "AnimationDropdown")
        {
            options.Add("Choose Animation");
            var path = "C:\\Users\\anany\\OneDrive\\Documents\\CGS_Final_Project\\CGS_Final_Project\\Assets\\Resources\\";
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
                        if (CustomFunctions_Screw_Nut_Interaction.functions[i].combination_name == SelectObject.combinationName)
                        {
                            options.Add(CustomFunctions_Screw_Nut_Interaction.functions[i].Animation_name);
                        }
                    }
                    break;
                case "Gear":
                    fileName = textAsset_Gear.name + ".txt";
                    info = textAsset_Gear.text;
                    Functions_Gear CustomFunctions_Gear = new Functions_Gear();
                    CustomFunctions_Gear = JsonUtility.FromJson<Functions_Gear>(info);
                    for (int i = 0; i < CustomFunctions_Gear.functions.Count; i++)
                    {
                        if (CustomFunctions_Gear.functions[i].combination_name == SelectObject.combinationName)
                        {
                            options.Add(CustomFunctions_Gear.functions[i].Animation_name);
                        }
                    }
                    break;
                case "Belt and Pulley":
                    fileName = textAsset_BeltandPulley.name + ".txt";
                    info = textAsset_BeltandPulley.text;
                    Functions_BeltandPulley CustomFunctions_BeltandPulley = new Functions_BeltandPulley();
                    CustomFunctions_BeltandPulley = JsonUtility.FromJson<Functions_BeltandPulley>(info);
                    for (int i = 0; i < CustomFunctions_BeltandPulley.functions.Count; i++)
                    {
                        if (CustomFunctions_BeltandPulley.functions[i].combination_name == SelectObject.combinationName)
                        {
                            options.Add(CustomFunctions_BeltandPulley.functions[i].Animation_name);
                        }
                    }
                    break;
            }
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }
}
