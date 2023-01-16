using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

// base (parent) class for inheritence ex -- special UI gizmos
public class SpecialUIGizmo: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField]
	public GameObject Handle; //the handle of the gizmo
	protected bool isPointerDown = false; 	//mouse pointer state holder var
	public UnityEvent onValueChanged;

	protected virtual void Start() 
	{
		// if onValueChanged is empty, populate it with a blank
		// UnityEvent to avoid startup errors
        if (onValueChanged == null) onValueChanged = new UnityEvent();
	}

	// called when the pointer enters the GUI component
	public void OnPointerEnter( PointerEventData eventData )
	{
		StartCoroutine(TrackPointer());
	}
	
	// called when the pointer exits the GUI component
	public void OnPointerExit( PointerEventData eventData )
	{
		StartCoroutine(TrackPointer());
	}

    // called when the pointer is down the GUI component
	public void OnPointerDown(PointerEventData eventData)
	{
		isPointerDown = true;
	}

    // called when the pointer is up the GUI component
	public void OnPointerUp(PointerEventData eventData)
	{
		isPointerDown = false;
	}

	// called when a pointer event occurs
	// the same for each variant of this base
	private IEnumerator TrackPointer()
	{
		var ray = GetComponentInParent<GraphicRaycaster>();
		var input = FindObjectOfType<StandaloneInputModule>();

		if( ray != null && input != null )
		{
			while( Application.isPlaying )
			{
				if (isPointerDown)
				{
					Vector2 localPos; //we get mouse position into this variable using out below
					RectTransformUtility.ScreenPointToLocalPointInRectangle( transform as RectTransform, Input.mousePosition, ray.eventCamera, out localPos );
						
					TrackPointerLogic(localPos);

					//fire off unity-event that the hue wheel value has changed
					onValueChanged.Invoke();					
				}
				yield return 0;
			}        
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not find GraphicRaycaster and/or StandaloneInputModule on same component as RadialSlider");
		}
	}

	// these will be custom for each variant of this base
	protected virtual void TrackPointerLogic(Vector2 localPos)
	{
	}
	
	public virtual void CalcSetHandlePosition(float angle)
	{
	}

	public virtual void CalcSetHandlePosition(Vector2 localPos)
	{
	}
}