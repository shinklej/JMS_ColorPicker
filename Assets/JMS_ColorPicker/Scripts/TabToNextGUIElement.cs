/* https://forum.unity.com/threads/tab-between-input-fields.263779/page-2 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
 
public class TabToNextGUIElement : MonoBehaviour
{
    [SerializeField]
    private List<Selectable> elements; // add UI elements in inspector in desired tabbing order
    int index;
 
    private void Start()
    {
        index = -1; // always leave at -1 initially
        //elements[0].Select(); // uncomment to have focus on first element in the list
    }
 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < elements.Count; i ++)
            {
                if (elements[i].gameObject.Equals(EventSystem.current.currentSelectedGameObject))
                {
                    index = i;
                    break;
                }
            }
 
            if (Input.GetKey(KeyCode.LeftShift))
            {
                index = index > 0 ? --index : index = elements.Count - 1;
            }
            else
            {
                index = index < elements.Count - 1 ? ++index : 0;
            }
            elements[index].Select();
        }
    }
}