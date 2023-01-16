using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// a simple script that has a hooked event into the main ColorPicker script in the inspector,
// to show how to utilize the currently selected color from it on a child or external scene object
public class ColorizeMeshFromPicker : MonoBehaviour
{
    [SerializeField]
    private ColorPicker colorPicker;

    public void SetMeshColor()
    {
        if (GetComponent<Renderer>() != null) GetComponent<Renderer>().material.SetColor("_Color", colorPicker.SelectedColor);
    }
}
