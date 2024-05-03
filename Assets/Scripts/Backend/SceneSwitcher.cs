//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    //New and improved scene switcher: now with fancy transitions! 
    //This scene switcher now uses coroutines to load scenes, and smoothly crossfades in and out a loading screen.
    public static SceneSwitcher instance; //public static self reference for other scripts to access and call coroutines
    [SerializeField] CanvasGroup loadingScreenCanvasGroup; //canvas group of the loading screen to modify the alpha and block raycasts (stops player from clicking buttons) during loading.
    void Awake()
    {
        if(instance != this && instance != null) //Ensures it is the only instance at runtime because it doesn't destroy on load
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        loadingScreenCanvasGroup = GetComponentInChildren<CanvasGroup>(); //Gets reference to canvas group
    }
    void Start()
    {
        StartCoroutine(RevealScene()); //Runs reveal scene at start to imitate the game loading in.
        Time.timeScale = 0; //Sets time scale to 0 so the game speeds up as the loading screen fades away.
    }

    public IEnumerator SwitchScenes(string sceneName, float addedDelay = 0)
    {
        StopCoroutine("RevealScene");

        loadingScreenCanvasGroup.blocksRaycasts = true; //blocks raycasts so player cannot click buttons during loading and potentially interupt the loading process.
        while(loadingScreenCanvasGroup.alpha < 1) //uses the alpha as a measurement, as we don't want to change scenes until the loading screen completely hides the scene.
        {
            loadingScreenCanvasGroup.alpha = Mathf.MoveTowards(loadingScreenCanvasGroup.alpha, 1, 0.01f);
            Time.timeScale = 1 - loadingScreenCanvasGroup.alpha; //uses inverse of the alpha as the timescale, so the game slows down/speeds up as the scene is hidden.
            yield return new WaitForSecondsRealtime(0.01f);
        }
        yield return new WaitForSecondsRealtime(addedDelay); //added delay is optional, but is used to simulate longer load times.
        SceneManager.LoadScene(sceneName); //loads scene
        StartCoroutine(RevealScene()); //begins hiding loading screen
    }
    IEnumerator RevealScene()
    {
        while(loadingScreenCanvasGroup.alpha > 0) //again, uses alpha as measurement
        {
            loadingScreenCanvasGroup.alpha = Mathf.MoveTowards(loadingScreenCanvasGroup.alpha, 0, 0.05f); //hides loading screen slightly faster than loading screen is revealed
            Time.timeScale = 1 - loadingScreenCanvasGroup.alpha; //same as above coroutine
            yield return new WaitForSecondsRealtime(0.05f);
        }
        loadingScreenCanvasGroup.blocksRaycasts = false; //turns off raycast blocking, because even when alpha is 0, it still blocks raycasts.
        Time.timeScale = 1; //returns time scale  to 1 to ensure the game always returns to full speed.
    }
}
