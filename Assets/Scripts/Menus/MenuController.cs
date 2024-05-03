//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class MenuController : MonoBehaviour
{
    //holds functions that the buttons on the main menu and rules/help page use.
    public void OnClickMenu()
    {
        StartCoroutine(SceneSwitcher.instance.SwitchScenes("MainMenu"));
    }
    public void OnClickPlay()
    {
        StartCoroutine(SceneSwitcher.instance.SwitchScenes("Level"));

    }
    
    public void OnClickHelp()
    {
        StartCoroutine(SceneSwitcher.instance.SwitchScenes("Help"));
    }
    
    public void OnClickQuit()
    {
        Application.Quit();        
    }
}
