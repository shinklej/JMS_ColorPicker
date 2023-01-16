using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Swatch : MonoBehaviour
{
    [SerializeField]
    private Image SwatchPreview;
    [SerializeField]
    private Material GradientMaterial_Base;
    [SerializeField]
    private bool isAddSwatch = false;

    public Toggle ThisSwatchToggle;
    
    // used to hold the swatches color info on each tile object
    public float r, g, b, a;
    public int idx;
    public string label;

    private Material Swatch_Cloned_Material;
    private ColorPicker ColorPickerInstance;

    private void Start() 
    {
        UpdateSwatchPreview();
    }

    public void UpdateSwatchPreview() 
    {
        //update the appearance of this swatch tile
        if (isAddSwatch == true) return;

        if (GradientMaterial_Base != null)
        {
            Swatch_Cloned_Material = new Material(GradientMaterial_Base);
        }
        Color scolor1 = new Color(r,g,b,a);
        Color scolor2 = new Color(r,g,b,a);
        Color scolormix1 = Color.Lerp(scolor1, scolor2, 0.5f);
        Swatch_Cloned_Material.SetColor("_ColorTop", scolor1);
        Swatch_Cloned_Material.SetColor("_ColorMid", scolormix1);
        Swatch_Cloned_Material.SetColor("_ColorBot", scolor2);
        Swatch_Cloned_Material.SetFloat("_Middle", 0.25f);

        SwatchPreview.material = Swatch_Cloned_Material;
    }

    public void UseThisSwatch()
    {
        // used when clicked on a swatch, or the +(add) tile
        if (ColorPickerInstance == null) ColorPickerInstance = GetComponentInParent<ColorPicker>();
       
        if (isAddSwatch == true)
        {
            ColorPickerInstance.AddSwatchButton = GetComponent<Image>();
            ColorPickerInstance.SwatchManager.AddSwatch();
        }
        else
        {
            if (ColorPickerInstance.SwatchManager.isLoadingLibrary == false)
            {
                // don't set prev/selected color on the main controls script if we are loading swatches from a library
                // because this gets called when the swatch is instantiated (ex: while loading a swatch library)
                ColorPickerInstance.PreviousColor = ColorPickerInstance.SelectedColor;
                ColorPickerInstance.CurrentAlphaValue = a;

                ColorPickerInstance.SelectedColor = new Color(r,g,b,a);
                ColorPickerInstance.CurrentAlphaValue = a;

                ColorPickerInstance.ProcessAndUpdateUI(-1, true, true);
                ColorPickerInstance.ProcessAndUpdateUI(4, true, true);
            }
        }
    }

    private void DeleteThisSwatch()
    {
        //delete it from the temp list 
        ColorPickerInstance.SwatchManager.DeleteSwatchFromTempList(idx);

        // destroy the preview tile
        Destroy(this.gameObject);

        // save/update current json file
        ColorPickerInstance.SwatchManager.SaveSwatchLibrary(ColorPickerInstance.SwatchManager.currentSwatchLibName);
    }

    private void Update() 
    {
        //listen for delete key presses, and delete this swatch if it is selected first
        if ((UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == this.gameObject) && (Input.GetKeyDown("delete"))) 
        {
            DeleteThisSwatch();
        }
    }
}
