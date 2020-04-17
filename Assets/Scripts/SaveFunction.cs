using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SaveFunction : MonoBehaviour
{
    public Button load, save, saveall, source, target, translate, rotate, scale, x, y, z, name;
    public InputField animation, value;
    private string a, b, info;

    public TextAsset textAsset;

    [Serializable]
    public class Function
    {
        public string name;
        public string source;
        public string target;
        public string Object;
        public string Tx;
        public string Ty;
        public string Tz;
        public string Rx;
        public string Ry;
        public string Rz;
        public string S;
    };

    [Serializable]
    public class Functions
    {
        //public Function[] functions;
        public List<Function> functions;
    }

    Function CustomFunction = new Function();

    // Start is called before the first frame update
    void Start()
    {
        load.enabled = true;
        load.image.color = Color.green;
        save.enabled = false;
        save.image.color = Color.cyan;
        saveall.enabled = false;
        saveall.image.color = Color.cyan;
        a = "Name";
        info = textAsset.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onBtnClick()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "Load")
        {
            if (EventSystem.current.currentSelectedGameObject.activeSelf == true)
            {
                animation.gameObject.SetActive(true);
                load.enabled = false;
                load.image.color = Color.cyan;
                source.gameObject.SetActive(true);
                source.GetComponentInChildren<Text>().text = SelectObject.getSourceName();
                target.gameObject.SetActive(true);
                target.GetComponentInChildren<Text>().text = SelectObject.getTargetName();
                translate.gameObject.SetActive(false);
                rotate.gameObject.SetActive(false);
                scale.gameObject.SetActive(false);
                x.gameObject.SetActive(false);
                y.gameObject.SetActive(false);
                z.gameObject.SetActive(false);
                name.gameObject.SetActive(false);
                value.gameObject.SetActive(false);
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
            a = "Name";
            name.GetComponentInChildren<Text>().text = a;
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "SaveAll")
        {
            load.enabled = true;
            load.image.color = Color.green;
            save.enabled = false;
            save.image.color = Color.cyan;
            saveall.enabled = false;
            saveall.image.color = Color.cyan;
            a = "Name";
            name.GetComponentInChildren<Text>().text = a;
            saveFunctions();
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Source")
        {
            save.enabled = true;
            save.image.color = Color.green;
            source.enabled = false;
            source.image.color = Color.cyan;
            target.enabled = true;
            target.image.color = Color.green;
            translate.gameObject.SetActive(true);
            rotate.gameObject.SetActive(true);
            scale.gameObject.SetActive(true);
            translate.enabled = true;
            translate.image.color = Color.green;
            rotate.enabled = true;
            rotate.image.color = Color.green;
            scale.enabled = true;
            scale.image.color = Color.green;
            x.gameObject.SetActive(false);
            y.gameObject.SetActive(false);
            z.gameObject.SetActive(false);
            name.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
        }
        else if(EventSystem.current.currentSelectedGameObject.name == "Target")
        {
            save.enabled = true;
            save.image.color = Color.green;
            source.enabled = true;
            source.image.color = Color.green;
            target.enabled = false;
            target.image.color = Color.cyan;
            translate.gameObject.SetActive(true);
            rotate.gameObject.SetActive(true);
            scale.gameObject.SetActive(true);
            translate.enabled = true;
            translate.image.color = Color.green;
            rotate.enabled = true;
            rotate.image.color = Color.green;
            scale.enabled = true;
            scale.image.color = Color.green;
            x.gameObject.SetActive(false);
            y.gameObject.SetActive(false);
            z.gameObject.SetActive(false);
            name.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Translate")
        {
            a = "T";
            translate.enabled = false;
            translate.image.color = Color.cyan;
            rotate.enabled = true;
            rotate.image.color = Color.green;
            scale.enabled = true;
            scale.image.color = Color.green;
            x.gameObject.SetActive(true);
            y.gameObject.SetActive(true);
            z.gameObject.SetActive(true);
            x.enabled = true;
            x.image.color = Color.green;
            y.enabled = true;
            y.image.color = Color.green;
            z.enabled = true;
            z.image.color = Color.green;
            name.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Rotate")
        {
            a = "R";
            translate.enabled = true;
            translate.image.color = Color.green;
            rotate.enabled = false;
            rotate.image.color = Color.cyan;
            scale.enabled = true;
            scale.image.color = Color.green;
            x.gameObject.SetActive(true);
            y.gameObject.SetActive(true);
            z.gameObject.SetActive(true);
            x.enabled = true;
            x.image.color = Color.green;
            y.enabled = true;
            y.image.color = Color.green;
            z.enabled = true;
            z.image.color = Color.green;
            name.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Scale")
        {
            a = "S";
            translate.enabled = true;
            translate.image.color = Color.green;
            rotate.enabled = true;
            rotate.image.color = Color.green;
            scale.enabled = false;
            scale.image.color = Color.cyan;
            x.gameObject.SetActive(false);
            y.gameObject.SetActive(false);
            z.gameObject.SetActive(false);
            name.gameObject.SetActive(true);
            name.GetComponentInChildren<Text>().text = a;
            value.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "X_Button")
        {
            a = a + "x";
            x.enabled = false;
            x.image.color = Color.cyan;
            y.enabled = true;
            y.image.color = Color.green;
            z.enabled = true;
            z.image.color = Color.green;
            name.gameObject.SetActive(true);
            name.GetComponentInChildren<Text>().text = a;
            value.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Y_Button")
        {
            a = a + "y";
            x.enabled = true;
            x.image.color = Color.green;
            y.enabled = false;
            y.image.color = Color.cyan;
            z.enabled = true;
            z.image.color = Color.green;
            name.gameObject.SetActive(true);
            name.GetComponentInChildren<Text>().text = a;
            value.gameObject.SetActive(true);
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Z_Button")
        {
            a = a + "z";
            x.enabled = true;
            x.image.color = Color.green;
            y.enabled = true;
            y.image.color = Color.green;
            z.enabled = false;
            z.image.color = Color.cyan;
            name.gameObject.SetActive(true);
            name.GetComponentInChildren<Text>().text = a;
            value.gameObject.SetActive(true);
        }
    }

    public void onChangeValue()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "Animation")
        {
            CustomFunction.name = animation.text;
            CustomFunction.source = SelectObject.getSourceName();
            CustomFunction.target = SelectObject.getTargetName();
            CustomFunction.Object = SelectObject.getObjectName();
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "Value")
        {
            save.enabled = true;
            save.image.color = Color.green;
            switch (a)
            {
                case "Tx":
                    CustomFunction.Tx = value.text;
                    break;
                case "Ty":
                    CustomFunction.Ty = value.text;
                    break;
                case "Tz":
                    CustomFunction.Tz = value.text;
                    break;
                case "Rx":
                    CustomFunction.Rx = value.text;
                    break;
                case "Ry":
                    CustomFunction.Ry = value.text;
                    break;
                case "Rz":
                    CustomFunction.Rz = value.text;
                    break;
                case "S":
                    CustomFunction.S = value.text;
                    break;
            }
        }
    }

    public void saveFunctions()
    {
        var path = "C:\\Users\\cdi\\Documents\\CGS_Final_Project\\Assets\\Resources\\";
        var fileName = textAsset.name + ".txt";
        info = textAsset.text;
        Functions CustomFunctions = new Functions();
        CustomFunctions = JsonUtility.FromJson<Functions>(info);
        Debug.Log(CustomFunctions.functions.Count);
        //CustomFunctions.functions[CustomFunctions.functions.Length] = new Function(); //CustomFunction;
        CustomFunctions.functions.Add(CustomFunction);
        //CustomFunctions.functions[CustomFunctions.functions.Length] = CustomFunction;
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
}
