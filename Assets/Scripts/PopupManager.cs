using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    
    public GameObject Curtain;          // Main menu; transitions - level change, retry
    public GameObject Resume_Button;    // Settings menu; active in-game.
    bool inGame = false;
    int level = 1;

    public GameObject TitleScreen;
    public GameObject LevelSelectScreen;
    public GameObject Settings;         // settings, called form start screen settings menu and options in game
    public GameObject DeathScreen;      // 

    
    void Awake()
    {
        instance = this;
    }

    // set up variables
    // call the game init/reset
    void Start()
    {
        Game_Reset();
    }

    // pause tracking
    void Update()
    {
        // track escape, pause/unpause the game
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(inGame)
            {
                Pause();
                Settings_ShowFromGame();
            }
        }
        
    }


    // ========= COMMON / CONVENIENCE FUNCTIONS =========

    public void Pause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    

    public void Game_Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    public void Hide_Everything()   // except for curtain
    {
        TitleScreen.SetActive(false);
        LevelSelectScreen.SetActive(false);
        DeathScreen.SetActive(false);
        Settings.SetActive(false);
        Resume_Button.SetActive(false);
    }

    public void Game_Reset()
    {
        // fruits aren't being reset

        // player's posiiton too
        //SnakeController.instance.Snake_Reset();

        inGame = false;
        Time.timeScale = 0;
                        
        // replace these by show main menu?
        TitleScreen_Show();
    }


    // ---

    public void TitleScreen_Show()
    {
        Hide_Everything();
        
        inGame = false;

        Curtain.SetActive(true);
        TitleScreen.SetActive(true);
    }
    
    public void LevelSelect_Show()
    {
        Hide_Everything();

        LevelSelectScreen.SetActive(true);
    }

    public void Game_Start()
    {
        Hide_Everything();
        Curtain.SetActive(false);

        FruitManager.instance.Show();

        inGame = true;
        Time.timeScale = 1;
    }
    
    public void Settings_ShowFromTitle()
    {
        Hide_Everything();

        inGame = false;
        Settings.SetActive(true);
    }

    // --- EXIT THE GAME
    // -> Game_Exit()

    // \\ ========= =========
    

    // // ========= SETTINGS =========
    // --- music checkbox
    // --- sound effects checkbox

    // --- FROM MENU - RESUME BUTTON : INACTIVE
    // -> Settings_ShowFromTitle()

    // --- FROM GAME - RESUME BUTTON : ACTIVE
    public void Settings_ShowFromGame()
    {
        if(Time.timeScale == 0)
            Settings.SetActive(true);
        else if(Time.timeScale == 1)
            Settings.SetActive(false);
        Resume_Button.SetActive(true);
    }
    
    // resume
    // ->
    public void Game_Resume()
    {
        Hide_Everything();
        
        Pause();    //?
    }

    // --- EXIT TO MAIN
    // -> Game_Reset()

    // \\ ========= =========

    
    // // --------- EXIT GAME --------- \\
    // -> Game_Exit()

    // // ========= LEVEL SELECT =========
    // --- level 1
    public void LevelSelect_First()
    {
        level = 1;
        Level_Select(level);
    }

    // --- level 2
    public void LevelSelect_Second()
    {
        level = 2;
        Level_Select(level);
    }

    // use this to reset the current level on death->retry
    public void Level_Select(int number)
    {
        foreach(Transform transform in XMLToLevel.instance.wallsEmptyObject.transform)
            GameObject.Destroy(transform.gameObject);
        foreach(Transform transform in XMLToLevel.instance.obstaclesEmptyObject.transform)
            GameObject.Destroy(transform.gameObject);

        for(int i = 1; i < SnakeController.instance.Snake.Count; i++)
            Destroy(SnakeController.instance.Snake[i].gameObject);

        SnakeController.instance.Snake.RemoveRange(1, SnakeController.instance.Snake.Count - 1);
        
        SnakeController.instance.Snake_Reset();

        XMLToLevel.instance.LoadLevel(number);
        Game_Start();
    }

    
    // // ========= DEATH SCREEN =========
    public void DeathScreen_Show()
    {
        inGame = false;
        DeathScreen.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void Death_Retry()
    {
        Hide_Everything();

        Level_Select(level);
        // null tails
        // null fruitsEaten
    }

    // --- RETRY
    // -> load level(current)
    // -> GameStart()? GameReset for now.

    // --- EXIT TO MENU 
    // -> Game_Reset()


}
