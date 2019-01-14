using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationController : MonoBehaviour {

    public GameObject infoPanel;
    public HotSpot[] hotSpots;
    public TextAsset[] textFiles;

    private Text title;
    private Text text;
    private int currentHotspot = 0;

    private void Start()
    {
        // Interface hotspot texts are updated dynamically when they are clicked on
        title = infoPanel.transform.Find("Title").GetComponent<Text>();
        text = infoPanel.transform.Find("Main Text").GetComponent<Text>();

        // Physical hotspot texts are all loaded in at the start
        PopulatePhysicalPanels();
    }

    public void OpenInfoPanel(string hotSpotNumber)
    {
        currentHotspot = int.Parse(hotSpotNumber);
        PopulateInterfacePanel();
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    public void OpenHelpPanel()
    {
        // Help is the 6th text file
        currentHotspot = 6;
        PopulateInterfacePanel();
    }

    public void CloseHelpPanel()
    {
        if (currentHotspot == 6)
        {
            CloseInfoPanel();
        }
    }

    private void PopulatePhysicalPanels()
    {
        for(int i=0; i< hotSpots.Length; i++)
        {
            HotSpot hotSpot = hotSpots[i];
            string file = textFiles[i].text;

            // Split the file into 2 parts, title and text
            string[] split = file.Split(new string[] { "." }, 2, System.StringSplitOptions.None);
            hotSpot.physicalTitle.text = split[0];
            hotSpot.physicalText.text = split[1];
        }
    }

    private void PopulateInterfacePanel()
    {
        GetInformation();
        infoPanel.SetActive(true);
    }

    private void GetInformation()
    {
        string file = textFiles[currentHotspot-1].text;

        // Split the file into 2 parts, title and text
        string[] split = file.Split(new string[] { "." }, 2, System.StringSplitOptions.None);
        title.text = split[0];
        text.text = split[1];
    }
}
