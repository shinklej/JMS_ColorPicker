using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

// RadialSlider (for hue wheel) -- variant of
// SpecialUIGizmo for class inheritence ex
public class RadialSlider: SpecialUIGizmo
{
	// good values for a RadialSlider of this style based
	// on the textures/sizes used and from testing the control
	private float Radius = 64f; 
	private float angleMultiplier = 6.25f;
	private float angleDifference = 89.5f;

	// each variant of SpecialUIGizmo will most likely have unique types of value properties
	public float value { get; set; } = 0f;
	
	// unique, override of CalcSetHandlePosition for a RadialSlider 
	public override void CalcSetHandlePosition(float angle)
	{
		GetComponent<Image>().fillAmount = angle;		
		value = angle;
					
		// calculate location and actually move the indicator handle/knob 
		Vector2 _centre = new Vector2(0f,0f);
		float _angle = (angle * angleMultiplier) - angleDifference;
		var offset = new Vector2(Mathf.Sin(_angle), Mathf.Cos(_angle)) * Radius;
		Handle.transform.localPosition = _centre + offset;
	}

	// unique override of TrackPointerLogic, computes fill angle for dial and sets it
	protected override void TrackPointerLogic(Vector2 localPos)
	{
		float angle = (Mathf.Atan2(-localPos.y, localPos.x)*180f/Mathf.PI+180f)/360f;
		CalcSetHandlePosition(angle); 
	}
}