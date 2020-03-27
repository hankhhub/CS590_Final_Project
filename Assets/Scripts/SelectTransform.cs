using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTransform : MonoBehaviour
{
    // Start is called before the first frame update
    private bool source, target;
    private bool translate, rotate, scale;
    private bool x_control, y_control, z_control;
    private bool positive, negative;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) //Source
        {
            source = true;
            target = false;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) //Target
        {
            source = false;
            target = true;
        }
        if (Input.GetKey(KeyCode.UpArrow)) //Positive
        {
            positive = true;
            negative = false;
            translate = false;
            rotate = false;
            scale = false;
            x_control = false;
            y_control = false;
            z_control = false;
            if (Input.GetKey(KeyCode.Alpha1)) //Translate in X
            {
                translate = true;
                rotate = false;
                scale = false;
                x_control = true;
                y_control = false;
                z_control = false;                
            }
            if (Input.GetKey(KeyCode.Alpha2)) //Translate in y
            {
                translate = true;
                rotate = false;
                scale = false;
                x_control = false;
                y_control = true;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha3)) //Translate in z
            {
                translate = true;
                rotate = false;
                scale = false;
                x_control = false;
                y_control = false;
                z_control = true;
            }
            if (Input.GetKey(KeyCode.Alpha4)) //Scale
            {
                translate = false;
                rotate = false;
                scale = true;
                x_control = false;
                y_control = false;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha5)) //Rotate in x
            {
                translate = false;
                rotate = true;
                scale = false;
                x_control = true;
                y_control = false;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha6)) //Rotate in y
            {
                translate = false;
                rotate = true;
                scale = false;
                x_control = false;
                y_control = true;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha7)) //Rotate in z
            {
                translate = false;
                rotate = true;
                scale = false;
                x_control = false;
                y_control = false;
                z_control = true;
            }
            SelectObject.onBtnClick(translate, rotate, scale);
            SelectObject.showAxes(x_control, y_control, z_control);
            SelectObject.transformObject(source, target, translate, rotate, scale, x_control, y_control, z_control, positive, negative);
        }
        if (Input.GetKey(KeyCode.DownArrow)) //Negative
        {
            positive = false;
            negative = true;
             translate = false;
            rotate = false;
            scale = false;
            x_control = false;
            y_control = false;
            z_control = false;
            if (Input.GetKey(KeyCode.Alpha1)) //Translate in X
            {
                translate = true;
                rotate = false;
                scale = false;
                x_control = true;
                y_control = false;
                z_control = false;                
            }
            if (Input.GetKey(KeyCode.Alpha2)) //Translate in y
            {
                translate = true;
                rotate = false;
                scale = false;
                x_control = false;
                y_control = true;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha3)) //Translate in z
            {
                translate = true;
                rotate = false;
                scale = false;
                x_control = false;
                y_control = false;
                z_control = true;
            }
            if (Input.GetKey(KeyCode.Alpha4)) //Scale
            {
                translate = false;
                rotate = false;
                scale = true;
                x_control = false;
                y_control = false;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha5)) //Rotate in x
            {
                translate = false;
                rotate = true;
                scale = false;
                x_control = true;
                y_control = false;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha6)) //Rotate in y
            {
                translate = false;
                rotate = true;
                scale = false;
                x_control = false;
                y_control = true;
                z_control = false;
            }
            if (Input.GetKey(KeyCode.Alpha7)) //Rotate in z
            {
                translate = false;
                rotate = true;
                scale = false;
                x_control = false;
                y_control = false;
                z_control = true;
            }
            SelectObject.onBtnClick(translate, rotate, scale);
            SelectObject.showAxes(x_control, y_control, z_control);
            SelectObject.transformObject(source, target, translate, rotate, scale, x_control, y_control, z_control, positive, negative);
        }
    }
}
