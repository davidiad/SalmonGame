using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LandscapeSceneManager : MonoBehaviour {

    private int currentLandscape;
    private int previousLandscape; // keep track of previous, so can check if it's change since last frame
    public int[] activeLandscapes;
    private int totalOffset;
    private GameObject fish;
    private bool currentLandscapeHasChanged;

    // Init an array that will hold all the landscape scenes, whether loaded or unloaded
    Scene[] landscapeScenes; // may need to be an array of integers, or of strings (names), rather thean Scene's. Can you store a scene into an array
    // or should you store the names, to access them.

	void Start () {
        previousLandscape = 0;
        currentLandscape = 0;
        totalOffset = 0;
        activeLandscapes = new int[] { -1, 0, 1 };
        currentLandscapeHasChanged = false;
        fish = GameObject.FindGameObjectWithTag("Fishy");
        // Populate the landscapesScenes array
        // Set up a folder containing all these scenes, and populate by name and or number (order)
        // Assuming for now that all scenes are sequentially numbered, and in a straight line

	    	
	}
	
	void Update () {
        // Check the position of the player fish
        // (Should be a number in meters. Divide by 100,   should give the array index of the scenes needed. 
        CheckCurrentLandscape();
        if (currentLandscapeHasChanged)
        {
            UpdateActiveLandscapes();
            // reset for the next time
            currentLandscapeHasChanged = false;
            previousLandscape = currentLandscape;
        }
        // but modified by an offset. Add the offset amount to the number above.
        // should be also the one before, and the one after.
        // Need a method to read in the fish position, and return whether a new scene needs to be loaded, and which ones they are
        // Load or enable or destroy new scenes as needed
        // Move all scenes and relevant game objects to keep everything near the center of Unity's world (not more than 10,000 m)
	}

    private void CheckCurrentLandscape() 
    {
        currentLandscape = Mathf.FloorToInt(fish.transform.position.z * 0.01f);
        if (currentLandscape != previousLandscape)
        {
            currentLandscapeHasChanged = true;
        }

    }

    private void UpdateActiveLandscapes() 
    {
        activeLandscapes[0] = currentLandscape - 1;
        activeLandscapes[1] = currentLandscape;
        activeLandscapes[2] = currentLandscape + 1;
        for (int i = 0; i < activeLandscapes.Length; i++)
        {
            // TODO: check whether scene is already loaded. If not, then load it
            LoadLandscape(activeLandscapes[i]);
            EnableLandscape(); // ditto, also check first if enabled, and if not, then enable it
        }
        //TODO: Update lighting to match for the current scene
        //Set the currentScene to be active scene (if it isn't already), so that its lightmaps are used
    }

    private void OffsetLandscapes(int offsetAmount) 
    {
        // keep track of the total offset
        totalOffset += offsetAmount;
        // Loop through current scenes, and offset them by the correct amount
    }

    private void LoadLandscape(int landscapeID)
    {
        
    }

    private void EnableLandscape()
    {
        // Check for correct offset before enabling
    }
}
/***************Strategy******************
 *Async load scenes within 2 of the center, current scene, but with game objects not active. So 5 scenes at a time
 *Make one scene before and after active, so 3 active scenes at a time
 *check whether this scheme 1. displays the scenes by the time they are needed (no gaps in the view)
 *and 2. circumvents noticable freezing/pausing while loading/enabling
 *Initially, add scenes to build manually.
 *Eventually, automatically add scenes from a folder of scenes (because there will be a lot of them)
// To add scenes to build automatically by script:
// from: https://answers.unity.com/questions/1212383/add-scenes-to-build-through-script.html
/*
---------- edit ----------
EditorBuildSettings is deprecrated since 5.3, use EditorSceneManager instead.Thanks to chomps32 for correction.
Add comment ·  Hide 1 · Share
avatar imagegilley033 · Oct 31, 2017 at 04:16 PM 0
I cannot find any methods in the new EditorSceneManager class that allow you to modify the build setting scenes.If you could point one out, that would be great, but otherwise, I think your initial answer of using EditorBuildSettings is still the only way to do this.
avatar image
0
Answer by chomps32 · Jul 08, 2016 at 10:30 AM
@Jessespike that class is depreciated since 5.3,
Use EditorSceneManager:
http://docs.unity3d.com/ScriptReference/SceneManagement.EditorSceneManager.html
Add comment ·  Hide 1 · Share
avatar imageJessespike · Jul 08, 2016 at 04:13 PM 0
Nice correction, thank you
avatar image
0
Answer by gilley033 · Oct 31, 2017 at 04:22 PM
I could not find any method to do this using the EditorSceneManager, and I don't see anything about EditorBuildSettings being deprecated.
Here is how I do it:
 void AddSceneToBuildSettings(string pathOfSceneToAdd)
{
    //Loop through and see if the scene already exist in the build settings
    int indexOfSceneIfExist = -1;

    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
    {
        if (EditorBuildSettings.scenes[i].path == pathOfSceneToAdd)
        {
            indexOfSceneIfExist = i;
            break;
        }
    }

    EditorBuildSettingsScene[] newScenes;

    if (indexOfSceneIfExist == -1)
    {
        newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];

        //Seems inefficent to add scene to build settings after creating each scene (rather than doing it all at once
        //after they are all created, however, it's necessary to avoid memory issues.
        int i = 0;
        for (; i < EditorBuildSettings.scenes.Length; i++)
            newScenes[i] = EditorBuildSettings.scenes[i];

        newScenes[i] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
    }
    else
    {
        newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length];

        int i = 0, j = 0;
        for (; i < EditorBuildSettings.scenes.Length; i++)
        {
            //skip over the scene that is a duplicate
            //this will effectively delete it from the build settings
            if (i != indexOfSceneIfExist)
                newScenes[j++] = EditorBuildSettings.scenes[i];
        }
        newScenes[j] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
    }

    EditorBuildSettings.scenes = newScenes;
        }---------- edit ----------
EditorBuildSettings is deprecrated since 5.3, use EditorSceneManager instead.Thanks to chomps32 for correction.
Add comment ·  Hide 1 · Share
avatar imagegilley033 · Oct 31, 2017 at 04:16 PM 0
I cannot find any methods in the new EditorSceneManager class that allow you to modify the build setting scenes.If you could point one out, that would be great, but otherwise, I think your initial answer of using EditorBuildSettings is still the only way to do this.
avatar image
0
Answer by chomps32 · Jul 08, 2016 at 10:30 AM
@Jessespike that class is depreciated since 5.3,
Use EditorSceneManager:
http://docs.unity3d.com/ScriptReference/SceneManagement.EditorSceneManager.html
Add comment ·  Hide 1 · Share
avatar imageJessespike · Jul 08, 2016 at 04:13 PM 0
Nice correction, thank you
avatar image
0
Answer by gilley033 · Oct 31, 2017 at 04:22 PM
I could not find any method to do this using the EditorSceneManager, and I don't see anything about EditorBuildSettings being deprecated.
Here is how I do it:
 void AddSceneToBuildSettings(string pathOfSceneToAdd)
{
    //Loop through and see if the scene already exist in the build settings
    int indexOfSceneIfExist = -1;

    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
    {
        if (EditorBuildSettings.scenes[i].path == pathOfSceneToAdd)
        {
            indexOfSceneIfExist = i;
            break;
        }
    }

    EditorBuildSettingsScene[] newScenes;

    if (indexOfSceneIfExist == -1)
    {
        newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];

        //Seems inefficent to add scene to build settings after creating each scene (rather than doing it all at once
        //after they are all created, however, it's necessary to avoid memory issues.
        int i = 0;
        for (; i < EditorBuildSettings.scenes.Length; i++)
            newScenes[i] = EditorBuildSettings.scenes[i];

        newScenes[i] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
    }
    else
    {
        newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length];

        int i = 0, j = 0;
        for (; i < EditorBuildSettings.scenes.Length; i++)
        {
            //skip over the scene that is a duplicate
            //this will effectively delete it from the build settings
            if (i != indexOfSceneIfExist)
                newScenes[j++] = EditorBuildSettings.scenes[i];
        }
        newScenes[j] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
    }

    EditorBuildSettings.scenes = newScenes;
}
*/