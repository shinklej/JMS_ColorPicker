using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

// Saturation & Value selector -- variant of 
// SpecialUIGizmo for class inheritence ex
public class SaturationValueSelector: SpecialUIGizmo
{
    [SerializeField]
    private RectTransform Area;

    private float Width;
    private float Height;
   
    // unique to this variant of SpecialUIGizmo
    protected override void Start()
    {
        base.Start();
 
        // grab area width/height for init
        Width = Area.rect.width + 2;
        Height = Area.rect.height + 2;

        // match the starting sat/val to wherever it is
        // placed in the prefab/scene for starting out
        onValueChanged.Invoke();
    }
 
    // this provides the sat&val values x&y
    // each variant of SpecialUIGizmo will most likely have unique types of
    // value properties
    public Vector2 value()
    {
        float x = 0;
        float y = 0;
        Vector2 returnValue; 
        x = Handle.transform.localPosition.x / Width;
        y = Handle.transform.localPosition.y / Height; 
        returnValue = new Vector2(x, y); 
        return returnValue;
    }

    // used to set the saturation selection handle when slider values, or text
    // inputs are submitted/changed, ex: ProcessUI() in ColorPicker.cs & 
    // TrackPointer in SpecialUIGizmo base, etc
    public override void CalcSetHandlePosition(Vector2 sat_val)
    {
        // actually move the handle/selection indicator    
        Handle.transform.localPosition = sat_val; 
        // some error corrections for the handle position to remain within the selection area
        if (Handle.transform.localPosition.x < 0) Handle.transform.localPosition = new Vector3(0, Handle.transform.localPosition.y, Handle.transform.localPosition.z);
        else if (Handle.transform.localPosition.x > Width) Handle.transform.localPosition = new Vector3(Width, Handle.transform.localPosition.y, Handle.transform.localPosition.z);
        if (Handle.transform.localPosition.y < 0) Handle.transform.localPosition = new Vector3(Handle.transform.localPosition.x, 0, Handle.transform.localPosition.z);
        else if (Handle.transform.localPosition.y > Height) Handle.transform.localPosition = new Vector3(Handle.transform.localPosition.x, Height, Handle.transform.localPosition.z);
    }

    // unique override of TrackPointerLogic, simply passes
    // vector2 -> CalcSetHandlePosition() for this variant of SpecialUIGizmo
    protected override void TrackPointerLogic(Vector2 localPos)
    {
        CalcSetHandlePosition(localPos); 
    }
}
