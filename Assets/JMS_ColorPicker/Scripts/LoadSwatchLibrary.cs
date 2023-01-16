using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// a simple script that has LoadThisLibrary() hooked into toggle events for the swatch library entries in the menu UI
public class LoadSwatchLibrary : MonoBehaviour
{
    public Text LibName;
    private SwatchLibManager SwatchLibMgr;
    private bool AllowToFire = false;

    private void Start()
    {
        SwatchLibMgr = GetComponentInParent<SwatchLibManager>();
    }

    private void OnEnable()
    {
        StartCoroutine(DelayedSetAllowed());
    }

    IEnumerator DelayedSetAllowed() 
    {
        AllowToFire = false;
        yield return new WaitForSeconds(0.5f);
        AllowToFire = true;
    }

    public void LoadThisLibrary()
    {
        if (AllowToFire == false) return; //safety to prevent event from firing when menu is re-enabled causing a on library entry to auto load

        if (SwatchLibMgr == null) SwatchLibMgr = GetComponentInParent<SwatchLibManager>();
        GetComponent<Toggle>().isOn = true;
        SwatchLibMgr.LoadSwatchLibrary(LibName.text);
        SwatchLibMgr.HidePopupMenu();
    }
}
