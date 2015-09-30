using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PopupManager : MonoBehaviour
{
    public GameObject Curtain;          // Main menu; transitions - level change, retry
    public GameObject Resume_Button;    // Settings menu; active in-game.
    bool inGame = false;
    //int level = 1;

    public GameObject TitleScreen;
    public GameObject LevelSelect;
    public GameObject Settings;         // settings, called form start screen settings menu and options in game
    public GameObject VictoryScreen;    // 
    public GameObject DeathScreen;      // 

    
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
    

    // ---
    // default menu state.
    // anything else?
    public void Game_Reset()   // This is the only way you will see the Main Menu;
                        //  the game's just been launched or we exited to it.
    {
        // fruits aren't being reset

        // player's posiiton too


        inGame = false;
        Time.timeScale = 0;
        // level = 1;
                
        // replace these by show main menu?
        TitleScreen_Show();
    }


    // ---

    // // ========= MAIN MENU =========
    // <SHOW MAIN MENU>
    public void TitleScreen_Show()
    {
        Curtain.SetActive(true);
        TitleScreen.SetActive(true);

        LevelSelect.SetActive(false);
        Settings.SetActive(false);
        VictoryScreen.SetActive(false);
        DeathScreen.SetActive(false);
    }
    // FUNCTIONS OF THE TITLE SCREEN

    // --- PLAY / LEVEL SELECT
    // --- play -> load the only level (which is not done through XMLToLevel Start(), which is meh)
    // level select -> level select menu
    
    // stub for now
    public void LevelSelect_Show()
    {
        // stub
        TitleScreen.SetActive(false);
        LevelSelect.SetActive(true);
    }

    public void Game_Start()
    {
        // hide excess stuff
        TitleScreen.SetActive(false);
        LevelSelect.SetActive(false);
        Curtain.SetActive(false);

        inGame = true;

        // unpause
        Time.timeScale = 1;
    }
    
    // --- SETTINGS
    // -> settings menu
    public void Settings_ShowFromTitle()
    {
        Resume_Button.SetActive(false);
        TitleScreen.SetActive(false);
        Settings.SetActive(true);
    }

    // --- EXIT THE GAME
    // -> Game_Exit()

    // \\ ========= =========
    

    // // ========= SETTINGS =========
    // --- aspect ratio drop down
    // --- resolution drop down
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
        Settings.SetActive(false);
        Pause();
    }

    // --- EXIT TO MAIN
    // -> Game_Reset()

    // \\ ========= =========

    
    // // --------- EXIT GAME --------- \\
    // -> Game_Exit()

    // // ========= LEVEL SELECT =========
    // --- level 1
    // --- level 2
    // --- level 3
    // -> loadLevel(i) -> XMLToLevel(path_i)

    // \\ ========= =========


    // // ========= VICTORY SCREEN =========
    public void VictoryScreen_Show()
    {
        VictoryScreen.SetActive(true);
        Time.timeScale = 0;
    }
    
    // NEXT LEVEL
    // -> loadLevel(curr+1)

    // EXIT TO MENU
    // -> Game_Reset() 

    // \\ ========= =========

    
    // // ========= DEATH SCREEN =========
    public void DeathScreen_Show()
    {
        DeathScreen.SetActive(true);
        Time.timeScale = 0;
    }
    
    // --- RETRY
    // -> load level(current)
    // -> GameStart()? GameReset for now.

    // --- EXIT TO MENU 
    // -> Game_Reset()


}
