using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public class SwatchLibManager : MonoBehaviour
{
    // our class/format for serializing entries to/from json with
    class ColorSwatchEntry
    {
        public float r;
        public float g;
        public float b;
        public float a;
        public int idx;
        public string label;

        public ColorSwatchEntry(float r, float g, float b, float a, int idx, string label) {
			this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            this.idx = idx;
		}
    }

    // all of the various UI items that need to be slotted in the inspector
    [SerializeField]
    private Toggle Toggle_ShowSwatches;
    [SerializeField]
    private Canvas Canvas_Swatches;
    [SerializeField]
    private GameObject SwatchTilePrefab;
    [SerializeField]
    private Transform SwatchTileSpawnpoint;
    [SerializeField]
    private GameObject AddSwatchPrefab;
    [SerializeField]
    GameObject CreateNewMenu;
    [SerializeField]
    private InputField NewLibraryName;
    [SerializeField]
    Transform SwatchLibrariesSpawnPoint;
    [SerializeField]
    GameObject SwatchLibEntryPrefab;
    public GameObject PopupMenu;

    // used to prevent certain things while loading a swatch library, such as setting the previous color for each swatchtile loaded
    public bool isLoadingLibrary { get; set; }

    // our temporary holder for our current swatches
    List<ColorSwatchEntry> CurrentSwatchLibrary = new List<ColorSwatchEntry>();

    // the interface for our DataService
    private IDataService DataService = new JsonDataService();

    // for internal use to track the current + swatch
    private GameObject AddSwatchObject;

    // cached ref to our main control's' script
    private ColorPicker ColorPickerControl;

    public string currentSwatchLibName { get; set; } = "Default";

    void Start()
    {
        ColorPickerControl = GetComponentInParent<ColorPicker>();

        // spawn the add color tile
        AddSwatchObject = Instantiate(AddSwatchPrefab, new Vector3(0f,0f,0f), new Quaternion(0f,0f,0f,0f), SwatchTileSpawnpoint);
         // prevent z value of spawned swatchtile from being incorrect
        AddSwatchObject.transform.localPosition = new Vector3(0f,0f,0f);
        ColorPickerControl.AddSwatchButton = AddSwatchObject.GetComponent<Image>();

        // populate list of available libraries in UI for Start()
        PopulateSwatchLibraryList();

        LoadSwatchLibrary(currentSwatchLibName);
    }

    public void AddSwatch()
    {
        isLoadingLibrary = true;
        CreateNewSwatch(ColorPickerControl.SelectedColor.r, ColorPickerControl.SelectedColor.g, ColorPickerControl.SelectedColor.b, ColorPickerControl.SelectedColor.a, "");

        // auto save swatch libray anytime one is added, default will be used if none is specified
        SaveSwatchLibrary(currentSwatchLibName);

        isLoadingLibrary = false;
    }

    public void AddSwatchManually(float r, float g, float b, float a, string label)
    {
        CreateNewSwatch(r, g, b, a, label);
    }

    Swatch CreateNewSwatch(float r, float g, float b, float a, string label)
    {
        // spawn the new tile         
        GameObject spawnedSwatchTile = Instantiate(SwatchTilePrefab, new Vector3(0f,0f,0f), new Quaternion(0f,0f,0f,0f), SwatchTileSpawnpoint);
        spawnedSwatchTile.transform.localPosition = new Vector3(0f,0f,0f); //prevent z value of spawned swatchtile from being incorrect
        
        // and ask its Swatch script to use the current color from ColorPicker
        Swatch swatchScript = spawnedSwatchTile.GetComponent<Swatch>();
        swatchScript.ThisSwatchToggle.group = swatchScript.GetComponentInParent<ToggleGroup>();

        swatchScript.r = r;
        swatchScript.g = g;
        swatchScript.b = b;
        swatchScript.a = a;
        swatchScript.label = label;
        swatchScript.UpdateSwatchPreview();

        // append the proper index on the new swatch
        swatchScript.idx = CurrentSwatchLibrary.Count + 1;

         // since we just added this swatch, it should be selected;
        swatchScript.ThisSwatchToggle.isOn = true;
        
        // add new swatch to current library
        CurrentSwatchLibrary.Add(new ColorSwatchEntry(r, g, b, a, CurrentSwatchLibrary.Count + 1, ""));

        RecreateAddSwatchTile();
        return swatchScript;
    }

    void RecreateAddSwatchTile() {
        // now destroy the old CurColorAddTile object and respawn it so it will be at the end of the swatch list not the begining
        Destroy(AddSwatchObject);
        AddSwatchObject = Instantiate(AddSwatchPrefab, new Vector3(0f,0f,0f), new Quaternion(0f,0f,0f,0f), SwatchTileSpawnpoint);        
        // prevent z value of spawned swatchtile from being incorrect
        AddSwatchObject.transform.localPosition = new Vector3(0f,0f,0f);
        ColorPickerControl.AddSwatchButton = AddSwatchObject.GetComponent<Image>();
    }

    public void SwatchListToggle() 
    {
        // show or hide the swatch tiles based on previous value
        if (Toggle_ShowSwatches.isOn) 
        {
            Canvas_Swatches.enabled = true;
        } 
        else
        {
            Canvas_Swatches.enabled = false;
        }
    }

    public void SaveSwatchLibrary(string libraryName) 
    {
        if (DataService.SaveData("/"+libraryName+".json", CurrentSwatchLibrary))
        {
            Debug.Log("Saved "+libraryName+".json to disk... # of swatches: " + CurrentSwatchLibrary.Count);
        }
    }

    public void LoadSwatchLibrary(string libraryName) 
    {
        try
        {
            List<ColorSwatchEntry> data = DataService.LoadData<List<ColorSwatchEntry>>("/"+libraryName+".json");

            CurrentSwatchLibrary.Clear();
            isLoadingLibrary = true;
            currentSwatchLibName = libraryName;

            // remove all previous swatch tiles from the display area
            for (int ds=0; ds<SwatchTileSpawnpoint.transform.childCount; ds++)
            {
                Destroy(SwatchTileSpawnpoint.transform.GetChild(ds).gameObject);
            }

            // add all swatch the entries from the json file
            int c = 0;
            for (int i=0; i<data.Count; i++)
            {
                AddSwatchManually(data[i].r, data[i].g, data[i].b, data[i].a, data[i].label);
                c++; 
            }

            RecreateAddSwatchTile();

            Debug.Log("Loaded "+libraryName+".json from disk... # of swatches: " + c);
            isLoadingLibrary = false;
        
        }
        catch
        {
            RecreateAddSwatchTile();
            
            Debug.Log("'"+libraryName+"' swatch library json file does not exist, it will be created ...");
            SaveSwatchLibrary(libraryName);
            isLoadingLibrary = false;
        }
    }

    public void DeleteSwatchFromTempList(int idx) 
    {
        for(int i=0; i<CurrentSwatchLibrary.Count; ++i)
        {
            if (CurrentSwatchLibrary[i].idx == idx) CurrentSwatchLibrary.Remove(CurrentSwatchLibrary[i]);
        }
    }

    public void RevealLibraryLocation() 
    {
        // open folder with swatch libraries in them via OS
        string appDataPath = Application.persistentDataPath;
        appDataPath = appDataPath.Replace("\\","/");
        Application.OpenURL("file://"+appDataPath );
        PopupMenu.SetActive(false);
    }

    public void ShowCreateNewLibraryDialog()
    {
        // clear filename entry InputField
        NewLibraryName.text = "";

        // hide dialog
        CreateNewMenu.SetActive(true);
        PopupMenu.SetActive(false);
    }

    public void CreateNewLibrary()
    {
        string newLibNameSanitized = NewLibraryName.text.ToString().Trim();
        newLibNameSanitized = Regex.Replace(newLibNameSanitized, "[^\\w\\._]", "");
        if (newLibNameSanitized != "") {
            currentSwatchLibName = newLibNameSanitized;

            //remove all previous swatch library entries from the UI menu area
            for (int i=0; i<SwatchLibrariesSpawnPoint.transform.childCount; i++)
            {
                Destroy(SwatchLibrariesSpawnPoint.transform.GetChild(i).gameObject);
            }

            //remove all previous swatch tiles from the display area
            for (int ds=0; ds<SwatchTileSpawnpoint.transform.childCount; ds++)
            {
                Destroy(SwatchTileSpawnpoint.transform.GetChild(ds).gameObject);
            }

            //remove all entries from data holder var
            CurrentSwatchLibrary.Clear();

            // create empty file for new swatch library
            SaveSwatchLibrary(currentSwatchLibName);

            // spawn swatching adder tile to display area
            RecreateAddSwatchTile();
           
            // clear filename entry InputField
            NewLibraryName.text = "";

            // hide dialog
            CreateNewMenu.SetActive(false);

            // refresh list of available libraries in UI
            PopulateSwatchLibraryList();
        }
    }

    public void CancelCreateNewLibrary()
    {
        // clear filename entry InputField
        NewLibraryName.text = "";

        // hide dialog
        CreateNewMenu.SetActive(false);
    }

    public void PopulateSwatchLibraryList()
    {
        bool booOneEntryIsSelected = false;

        // clear all spawned libraries in the UI list
        for (var i=SwatchLibrariesSpawnPoint.transform.childCount-1; i>=0; i--)
        {
            Destroy(SwatchLibrariesSpawnPoint.transform.GetChild(i).gameObject);
        }

        // clear swatch entries in holder var
        CurrentSwatchLibrary = new List<ColorSwatchEntry>();

        if (Directory.Exists(Application.persistentDataPath))
        {
            string libFolder = Application.persistentDataPath;
 
            DirectoryInfo d = new DirectoryInfo(libFolder);
            foreach (var file in d.GetFiles("*.json"))
            {
                // spawn new / refresh library entries list in UI            
                GameObject SpawnedLibraryEntry = Instantiate(SwatchLibEntryPrefab, new Vector3(0f,0f,0f), new Quaternion(0f,0f,0f,0f), SwatchLibrariesSpawnPoint);
                
                // prevent z value of spawned swatchtile from being incorrect
                SpawnedLibraryEntry.transform.localPosition = new Vector3(0f,0f,0f);

                // set display text / name and other important properties spawned menu item
                string fname = file.Name;
                fname = fname.Replace(".json","");
                SpawnedLibraryEntry.GetComponent<LoadSwatchLibrary>().LibName.text = fname;
                SpawnedLibraryEntry.GetComponent<Toggle>().group = SwatchLibrariesSpawnPoint.GetComponent<ToggleGroup>();
                if (currentSwatchLibName == fname)
                {
                    SpawnedLibraryEntry.GetComponent<Toggle>().isOn = true;
                    booOneEntryIsSelected = true;
                }
            }

            if (booOneEntryIsSelected == false)
            {
                //this is incase there is no Default.json yet, we will add it to the list to prevent an IndexOutOfRangeException
                //one the first swatch is selected it will create the actual file, but "Default" library should always exist
                GameObject SpawnedLibraryEntry = Instantiate(SwatchLibEntryPrefab, new Vector3(0f,0f,0f), new Quaternion(0f,0f,0f,0f), SwatchLibrariesSpawnPoint);
                SpawnedLibraryEntry.transform.localPosition = new Vector3(0f,0f,0f);
                SpawnedLibraryEntry.GetComponent<Toggle>().isOn = true;
                SpawnedLibraryEntry.GetComponent<Toggle>().group = SwatchLibrariesSpawnPoint.GetComponent<ToggleGroup>();
                SpawnedLibraryEntry.GetComponent<LoadSwatchLibrary>().LibName.text = "Default";

            }
        }
        else
        {
            File.Create(Application.persistentDataPath);
            return;
        }        
    }

    public void HidePopupMenu()
    {
        // force hide the popup menu
        PopupMenu.SetActive(false);
    }

    public void TogglePopupMenu()
    {
        // show/hide the popup ui/menu
        if (PopupMenu.activeSelf == true)
        {
            PopupMenu.SetActive(false);
        }
        else
        {
            PopulateSwatchLibraryList();
            PopupMenu.SetActive(true);
        }
    }
}