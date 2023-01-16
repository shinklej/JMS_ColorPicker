using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using StackOverflow.Snippets;

// main script for the ColorPicker control -- should be on the root of the GameObject/prefab
public class ColorPicker : MonoBehaviour, IPointerDownHandler
{
    // all of the various main UI items that need to be
    // slotted into these variables in the inspector
    [SerializeField]
    private SaturationValueSelector SatValSlider;
    [SerializeField]
    private RadialSlider HueSlider;
    public SwatchLibManager SwatchManager;

    //various color display boxes that need to be
    // slotted into these variables in the inspector
    [SerializeField]
    private Image DisplayColor;
    [SerializeField]
    private Image PreviousDisplayColor;
    [SerializeField]
    private Image SVDisplayColor;

    public Image AddSwatchButton;
    public Color SelectedColor;
    public Color PreviousColor;

    //this needs to be independent for the control away from the stored SelectedColor
    //itself, because when we pick a new color source from huewheel+satvalbox it will
    //come in with a 100% alpha value, so we keep track of it for the sake of UI controls
    public float CurrentAlphaValue { get; set; } = 1.0f;
 
    // all of the sliders components, and each InputField/bg image for RGBAHSV
    // these also all need to be slotted into these in the inspector
    [SerializeField]
    private Slider SliderComponent_R;
    [SerializeField]
    private Image SliderBg_R;
    [SerializeField]
    private InputField SliderValTextInput_R;    
    [SerializeField]
    private Slider SliderComponent_G;
    [SerializeField]
    private Image SliderBg_G;
    [SerializeField]
    private InputField SliderValTextInput_G;    
    [SerializeField]
    private Slider SliderComponent_B;
    [SerializeField]
    private Image SliderBg_B;
    [SerializeField]
    private InputField SliderValTextInput_B;
    [SerializeField]
    private Slider SliderComponent_A;
    [SerializeField]
    private Image SliderBg_A;
    [SerializeField]
    private InputField SliderValTextInput_A;
    [SerializeField]
    private Slider SliderComponent_H;
    [SerializeField]
    private Image SliderBg_H;
    [SerializeField]
    private InputField SliderValTextInput_H;    
    [SerializeField]
    private Slider SliderComponent_S;
    [SerializeField]
    private Image SliderBg_S;
    [SerializeField]
    private InputField SliderValTextInput_S;    
    [SerializeField]
    private Slider SliderComponent_V;
    [SerializeField]
    private Image SliderBg_V;
    [SerializeField]
    private InputField SliderValTextInput_V;
    [SerializeField]
    private InputField HexValTextInput;
    [SerializeField]
    private Material GradientMaterial_Base;

    // the two different holder rect transforms (gameObjects)
    // that hold the RGB and HSV sliders seperately -- they
    // need to be slotted in
    [SerializeField]
    private GameObject RGB_Sliders;
    [SerializeField]
    private GameObject HSV_Sliders;

    // the color number format mode dropdown must be slotted into this
    [SerializeField]
    private Dropdown ValueScale_Dropdown;

    private float valuesScale = 255f;
    private float prevValuesScale = 255f;
    private float hueMax = 360f;
    
    // background gradient materials for various sliders, internal use only
    private Material SaturationBG_Material;
    private Material ValueBG_Material;
    private Material Red_Channel_Material;
    private Material Green_Channel_Material;
    private Material Blue_Channel_Material;

    // used for error prevention while switching color number format modes
    private bool switchingModes = false;
    
    public UnityEvent onValueChanged;

    private void Start()
    {
        // used to prevent startup errors that might hit onValueChanged
        // before they are setup in OnEnable()
        if (onValueChanged == null) onValueChanged = new UnityEvent();
    } 

    public void OnPointerDown(PointerEventData eventData)
    {
		SwatchManager.PopupMenu.SetActive(false);
    }

    private void CreateMaterials()
    {
        // this makes clones of the slotted GradientMaterial_Base
        // for our different RGBSV slider background images
        SaturationBG_Material = new Material(GradientMaterial_Base);
        SliderBg_S.material = SaturationBG_Material;

        ValueBG_Material = new Material(GradientMaterial_Base);
        SliderBg_V.material = ValueBG_Material;

        Red_Channel_Material = new Material(GradientMaterial_Base);
        SliderBg_R.material = Red_Channel_Material;

        Green_Channel_Material = new Material(GradientMaterial_Base);
        SliderBg_G.material = Green_Channel_Material;

        Blue_Channel_Material = new Material(GradientMaterial_Base);
        SliderBg_B.material = Blue_Channel_Material;
    }

    private void SetNumberFormat(Slider target)
    {
        // this turns wholeNumbers on/off on a target Slider 
        // based on the valuesScale/number format mode we are currently in
        if (valuesScale == 255f)
        {
            target.wholeNumbers = true;
            RGB_Sliders.SetActive(true);
            HSV_Sliders.SetActive(false);
        }
        else if (valuesScale == 100f)
        {
            target.wholeNumbers = true;    
            RGB_Sliders.SetActive(false);
            HSV_Sliders.SetActive(true);
        }
        else if (valuesScale == 1f)
        {
            target.wholeNumbers = false;
            RGB_Sliders.SetActive(true);
            HSV_Sliders.SetActive(false);
        }
    }

    private void UpdateSlidersForModeChange(float targetValueScale, int charLimit, int mode) 
    {
        // this void converts the values on all the sliders and their InputField's
        // to the new number format we are switching too
        prevValuesScale = valuesScale;
        valuesScale = targetValueScale;
        float newmax = valuesScale;
        float oldmax = prevValuesScale;

        // we are changing number format modes, so we need to set the maxValue, value & 
        // number formatting on each horizontal slider, as well as the characterLimit on
        // their InputField

        SliderComponent_R.maxValue = newmax;
        SliderComponent_G.maxValue = newmax;
        SliderComponent_B.maxValue = newmax;
        SliderComponent_A.maxValue = newmax;
        SliderComponent_S.maxValue = newmax;
        SliderComponent_V.maxValue = newmax;
        
        SliderValTextInput_R.characterLimit = charLimit;
        float oldvalue = float.Parse(SliderValTextInput_R.text);
        float newVal = (newmax * oldvalue) / oldmax;
        SliderComponent_R.value = newVal;
        SetNumberFormat(SliderComponent_R);        

        SliderValTextInput_G.characterLimit = charLimit;
        oldvalue = float.Parse(SliderValTextInput_G.text);
        newVal = (newmax * oldvalue) / oldmax;
        SliderComponent_G.value = newVal;
        SetNumberFormat(SliderComponent_G);        

        SliderValTextInput_B.characterLimit = charLimit;
        oldvalue = float.Parse(SliderValTextInput_B.text);
        newVal = (newmax * oldvalue) / oldmax;
        SliderComponent_B.SetValueWithoutNotify(newVal);
        SetNumberFormat(SliderComponent_B);       

        SliderValTextInput_A.characterLimit = charLimit;
        oldvalue = float.Parse(SliderValTextInput_A.text);
        newVal = (newmax * oldvalue) / oldmax;
        SliderComponent_A.SetValueWithoutNotify(newVal);
        SetNumberFormat(SliderComponent_A);

        SliderValTextInput_S.characterLimit = charLimit;
        oldvalue = float.Parse(SliderValTextInput_S.text);
        newVal = (newmax * oldvalue) / oldmax;
        SliderComponent_S.value = newVal;
        SetNumberFormat(SliderComponent_S);

        SliderValTextInput_V.characterLimit = charLimit;
        oldvalue = float.Parse(SliderValTextInput_V.text);
        newVal = (newmax * oldvalue) / oldmax;
        SliderComponent_V.value = newVal;
        SetNumberFormat(SliderComponent_V);
    }

    private void ApplyValueScaleDropdownValue()
    {
        // important, otherwise we get weird results when switching color mode display
        // values, blocking further ProcessUI and ProcessAndUpdateUI calls for just a
        // frame or two
        switchingModes = true;

        if (ValueScale_Dropdown.value == 0)
        {
            // RGB 0.0-1.0f mode
            // 9 digits allowed in inputfields for floating point
            UpdateSlidersForModeChange(1.0f, 9, 0);
        } 
        else if (ValueScale_Dropdown.value == 1)
        {
            // RGB 0-255 mode
            // 4 digits allowed in inputfields
            UpdateSlidersForModeChange(255f, 4, 1);
        }
        else if (ValueScale_Dropdown.value == 2)
        {
            // 0-100 mode for HSV
            // 3 digits allowed in inputfields
            UpdateSlidersForModeChange(100f, 3, 2);
        }

        switchingModes = false;
        ProcessAndUpdateUI(-1, true, false);
    }

    public void ProcessUI(int mode) {
        // this void is hooked into onValueChanged events on the sliders directly,
        // with modes 0 or 1 defined in the event
        if (switchingModes == true) return;
        ProcessAndUpdateUI(mode, true, true);
    }

    public void ProcessAndUpdateUI(int mode, bool updateUIcomps, bool setPreviousColor)
    {
        // if we are in the middle of switching number
        // format modes, don't process UI this time around
        if (switchingModes == true) return;  

        // this void is used internally + for ProcessUI (slider change events) and is
        // hooked into the onValueChanged() UnityEvent directly on all of the sliders
        // and their input boxes; hue wheel, hexcode field etc using a different mode
        // value for each 0 thru 5, which is specified in each hooked event on each

        float hue = 0f;
        float sat = 0f;
        float val = 0f;
        
        // get the new color based on which thing it came from
        if (mode == -1)
        {
            // mode -1 -- special mode used when changing numbers display modes that doesn't affect the selected color
        }
        else if (mode == 0)
        {
            // mode 0 - Input from sliders - RGB method
            SelectedColor = new Color(SliderComponent_R.value / valuesScale, SliderComponent_G.value / valuesScale, SliderComponent_B.value / valuesScale, SliderComponent_A.value / valuesScale);
            CurrentAlphaValue = SliderComponent_A.value / valuesScale;
        } 
        else if (mode == 1) 
        {
            // mode 1 - Input from sliders - HSV method
            SelectedColor = Color.HSVToRGB(SliderComponent_H.value / hueMax, SliderComponent_S.value / valuesScale, SliderComponent_V.value / valuesScale);
            CurrentAlphaValue = SliderComponent_A.value / valuesScale;
        } 
        else if (mode == 2)
        {
            // mode 2 - Input from Hue wheel or SatVal slider
            Vector2 satValValue = SatValSlider.value();
            hue = HueSlider.value;
            sat = (satValValue.x);
            val = (satValValue.y);
            SelectedColor = Color.HSVToRGB(hue, sat, val);  
            //no need to modify alpha when changing hue wheel/satval box value
        }
        else if (mode == 3) 
        {
            // mode 3 - Input from Hexcode InputField
            string hexcode = HexValTextInput.text;
            SelectedColor = ColorCodeTools.hexToColor(hexcode);
            
            //only use alpha from hexcode if it's length is 8, otherwise use UI value
            if (HexValTextInput.text.ToString().Length == 8)
            {
                CurrentAlphaValue = SelectedColor.a;
            }
        }
        else if (mode == 4) 
        {
            // mode 4 - Input from slider text InputField's - RGB method
            SelectedColor = new Color(float.Parse(SliderValTextInput_R.text) / valuesScale, float.Parse(SliderValTextInput_G.text) / valuesScale, float.Parse(SliderValTextInput_B.text) / valuesScale, float.Parse(SliderValTextInput_A.text) / valuesScale);
            CurrentAlphaValue = float.Parse(SliderValTextInput_A.text) / valuesScale;
        }
        else if (mode == 5) 
        {
            // mode 5 - Input from slider text InputField's - HSV method
            SelectedColor = Color.HSVToRGB(float.Parse(SliderValTextInput_H.text) / valuesScale, float.Parse(SliderValTextInput_S.text) / valuesScale, float.Parse(SliderValTextInput_V.text) / valuesScale);
            CurrentAlphaValue = float.Parse(SliderValTextInput_A.text) / valuesScale;
        }

        // repopulate hue, sat, val variables at this point based of the value of SelectedColor
        Color.RGBToHSV(SelectedColor, out hue, out sat, out val);

        if ((!SaturationBG_Material) || (!ValueBG_Material) || (!Red_Channel_Material) || (!Green_Channel_Material) || (!Blue_Channel_Material))
        {
            // adjust gradient color values on the cloned materials for slider
            // background's, since they dont exist yet -- this should be a one
            // time thing per instance of the ColorPicker control/object
            CreateMaterials();
        }

        // set the base color of the alpha slider graphic
        SelectedColor.a = CurrentAlphaValue;
        SliderBg_A.color = SelectedColor;

        //this could also be done with a gradient, but not enough time for now
        AddSwatchButton.color = new Color(SelectedColor.r,SelectedColor.g,SelectedColor.b,CurrentAlphaValue);

        // did not compact the sections below into a function so
        // that it can be all looked at a better understanding of
        // how we generate the proper gradient for each background
        // of the various sliders

        // setup the changes for the saturation slider background material        
        Color scolor1 = new Color(val,val,val);
        Color scolor2 = Color.HSVToRGB(hue, 1f, val);
        Color scolormix1 = Color.Lerp(scolor1, scolor2, 0.5f);
        SaturationBG_Material.SetColor("_ColorTop", scolor1);
        SaturationBG_Material.SetColor("_ColorMid", scolormix1);
        SaturationBG_Material.SetColor("_ColorBot", scolor2);
        SaturationBG_Material.SetFloat("_Middle", 0.25f);

        // setup the changes for the value slider background material
        Color scolor3 = new Color(hue,0f,0f);
        Color scolor4 = Color.HSVToRGB(hue, sat, 1f);
        Color scolormix2 = Color.Lerp(scolor3, scolor4, 0.5f);
        ValueBG_Material.SetColor("_ColorTop", scolor3);
        ValueBG_Material.SetColor("_ColorMid", scolormix2);
        ValueBG_Material.SetColor("_ColorBot", scolor4);
        ValueBG_Material.SetFloat("_Middle", 0.25f);

        // setup the changes for the different rgb sliders background materials
        // red
        Color scolor5 = new Color(1f, 0f, 0f, 0f) + SelectedColor;
        Color scolor6 = new Color(0f, SelectedColor.g, SelectedColor.b, 1f);
        Color scolormix3 = Color.Lerp(scolor5, scolor6, 0.5f);
        Red_Channel_Material.SetColor("_ColorBot", scolor5);         
        Red_Channel_Material.SetColor("_ColorMid", scolormix3);
        Red_Channel_Material.SetColor("_ColorTop", scolor6);
        Red_Channel_Material.SetFloat("_Middle", 0.25f);
        
        // green
        Color scolor7 = new Color(0f, 1f, 0f, 0f) + SelectedColor;
        Color scolor8 = new Color(SelectedColor.r, 0f, SelectedColor.b, 1f);
        Color scolormix4 = Color.Lerp(scolor7, scolor8, 0.5f);
        Green_Channel_Material.SetColor("_ColorBot", scolor7);
        Green_Channel_Material.SetColor("_ColorMid", scolormix4);
        Green_Channel_Material.SetColor("_ColorTop", scolor8);
        Green_Channel_Material.SetFloat("_Middle", 0.25f);
        
        // blue
        Color scolor9 = new Color(0f, 0f, 1f, 0f) + SelectedColor;
        Color scolor10 = new Color(SelectedColor.r, SelectedColor.g, 0f, 1f);
        Color scolormix5 = Color.Lerp(scolor9, scolor10, 0.5f);
        Blue_Channel_Material.SetColor("_ColorBot", scolor9);
        Blue_Channel_Material.SetColor("_ColorMid", scolormix5);
        Blue_Channel_Material.SetColor("_ColorTop", scolor10);
        Blue_Channel_Material.SetFloat("_Middle", 0.25f); 

        // process this code from all input sources except hue wheel/satval
        // box -- we dont want to update it when we are changing number 
        // format modes or using the hue wheel/satval inputs themselves!
        if ( (mode != 2) && ( mode != -1 ) )
        {
            // force HueWheel and SaturationArea to update their handle
            // indicators to the new values from the various color 
            // selecting methods in the if-block near the top of this void
            SatValSlider.CalcSetHandlePosition(new Vector2(sat * 64f, val * 64f)); 
            HueSlider.CalcSetHandlePosition(hue);
            
            // update colors of SaturationSelectionArea control
            Vector2 satValValue = SatValSlider.value();
            HueSlider.value = hue;
            satValValue.x = sat;
            satValValue.y = val;
        }

        // execute the rest of the process -- updating all the UI components
        // to match the new values computed above, but only if updateUIcomps is true
        if (updateUIcomps == true) UpdateUIComponents(SelectedColor.r,SelectedColor.g,SelectedColor.b,SelectedColor.a,hue,sat,val, true, setPreviousColor);
    }

    private void UpdateUIComponents(float r, float g, float b, float a, float hue, float sat, float val, bool updateHexText, bool setPreviousColor)
    {
        float proc_r, proc_g, proc_b, proc_a, proc_h, proc_s, proc_v = 0f;

        if (valuesScale != 1.0f)
        {
            // we round off the decimal (up) on any values going back into
            // UI components when not in RGB 0-1.0 floating point mode
            proc_r = Mathf.Floor(r * valuesScale);
            proc_g = Mathf.Floor(g * valuesScale);
            proc_b = Mathf.Floor(b * valuesScale);
            proc_a = Mathf.Floor(CurrentAlphaValue * valuesScale); //we use the UI's value for alpha instead of the color data that came from high up the chain of events (SelecectedColor var)
            proc_h = Mathf.Floor(hue * hueMax);
            proc_s = Mathf.Floor(sat * valuesScale);
            proc_v = Mathf.Floor(val * valuesScale);
        }
        else
        {
            proc_r = r * valuesScale;
            proc_g = g * valuesScale;
            proc_b = b * valuesScale;
            proc_a = CurrentAlphaValue * valuesScale; //and again
            proc_h = hue * hueMax;
            proc_s = sat * valuesScale;
            proc_v = val * valuesScale;
        }

        // update the actual position/value of all the RGBAHSV sliders
        SliderComponent_R.SetValueWithoutNotify(proc_r);
        SliderComponent_G.SetValueWithoutNotify(proc_g);
        SliderComponent_B.SetValueWithoutNotify(proc_b);
        SliderComponent_A.SetValueWithoutNotify(proc_a);
        SliderComponent_H.SetValueWithoutNotify(proc_h);
        SliderComponent_S.SetValueWithoutNotify(proc_s);
        SliderComponent_V.SetValueWithoutNotify(proc_v);

        // update each slider components InputField values
        SliderValTextInput_R.SetTextWithoutNotify(proc_r.ToString());
        SliderValTextInput_G.SetTextWithoutNotify(proc_g.ToString());
        SliderValTextInput_B.SetTextWithoutNotify(proc_b.ToString());
        SliderValTextInput_A.SetTextWithoutNotify(proc_a.ToString());
        SliderValTextInput_H.SetTextWithoutNotify(proc_h.ToString());
        SliderValTextInput_S.SetTextWithoutNotify(proc_s.ToString());
        SliderValTextInput_V.SetTextWithoutNotify(proc_v.ToString());

        SelectedColor = new Color(r,g,b,CurrentAlphaValue); 

        // convert to hue for SVDisplayColor
        Color.RGBToHSV(SelectedColor, out hue, out sat, out val);

        // only update the hexcode InputField when we didn't just input into that text inputfield itself
        if (updateHexText == true) HexValTextInput.SetTextWithoutNotify(ColorUtility.ToHtmlStringRGB(SelectedColor));
        
        // update the SatVal selection area base color && currently selecected color box
        DisplayColor.color = SelectedColor;
        if (setPreviousColor == true) 
        {
            PreviousDisplayColor.color = PreviousColor;
        }
        SVDisplayColor.color = Color.HSVToRGB(hue, 1, 1); 

        // invoke any UnityEvents added to the onValueChanged() list
        onValueChanged.Invoke();
    }
 
    private void OnEnable()
    {
        // this function is called when the object becomes enabled/active:
        // setup the onValueChanged listener's for SatValSlider and HueSlider
        SatValSlider.onValueChanged.AddListener(delegate { ProcessAndUpdateUI(2, true, false); });
        HueSlider.onValueChanged.AddListener(delegate { ProcessAndUpdateUI(2, true, false); });
    } 
 
    private void OnDisable()
    {
        // this function is called when the object becomes disabled/inactive:
        // remove the onValueChanged listener's for SatValSlider and HueSlider
        SatValSlider.onValueChanged.RemoveAllListeners();
        HueSlider.onValueChanged.RemoveAllListeners();
    }

    public void SetPreviousColorToCurrent()
    {
        // used to set the previous color when this instance of the control is closed/reopened/etc
        PreviousColor = SelectedColor;
        ProcessAndUpdateUI(2, true, true);
    }
}
