using UnityEngine;

public class MenuController : MonoBehaviour
{
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
