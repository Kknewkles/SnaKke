using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PopupManager : MonoBehaviour
{
    Canvas canvas;

    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject DeathScreen;

    void Start()
    {
        // pause everything
        DeathScreen.SetActive(false);

        Time.timeScale = 0;
    }

    // Start Screen ---
    // will be replaced by Level Select 
    public void StartGame()
    {
        
        Time.timeScale = 1;
        MainMenu.SetActive(false);        
    }

    // Settings
    
    // Exit Game

    // ---


    // Options Menu ---
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    // ---


    // Death Screen ---
    public void ShowDeathScreen()
    {
        Time.timeScale = 0;
        DeathScreen.SetActive(true);
    }

    // is this restarting the level or the whole thing?
    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    // ---
}
