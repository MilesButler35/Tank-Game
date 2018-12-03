using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    GameObject[] gameOverObjects;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    //[SerializeField] private AudioSource loseSound; TFAY
    //[SerializeField] private AudioSource winSound; TFAY
    int count, temp;
    string sceneName;
    // Use this for initialization
    void Start()
    {
      //  winSound = GetComponent<AudioSource>(); TFAY
       // loseSound  = GetComponent<AudioSource>(); TFAY
        count = GameObject.FindGameObjectsWithTag("Enemy").Length;
        temp = count;

        Time.timeScale = 1;
        gameOverObjects = GameObject.FindGameObjectsWithTag("ShowOnWin");
        HidePaused();
        sceneName = SceneManager.GetActiveScene().name;

    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log(sceneName);
       // count = GameObject.FindGameObjectsWithTag("Enemy").Length;


        //if (temp != count)
        //{
            
           // Time.timeScale = 1;
           // pauseControl();
            //Debug.Log("There's a change in number of Enemy!");
         
        //}

        //uses the missing enemy to bring up canvas objects
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < 1 && sceneName != "Title-Screen") //if all tanks are gone bring up screen
        {
           
            if (Time.timeScale == 1)
            {               
                Time.timeScale = 0;
                Debug.Log("SHOWTIME");
                //showPaused();
                winPanel.SetActive(true);
                
               // winSound.Play(); TFAY
            }
          //  else if (Time.timeScale == 0)
           // {
                //Debug.Log("hide");
             //   Time.timeScale = 1;
               // HidePaused();
           // }
        }

        if(GameObject.FindGameObjectsWithTag("Player").Length < 1 && sceneName != "Title-Screen")
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                Debug.Log("SHOWTIME");
                //showPaused();
                losePanel.SetActive(true);
               // loseSound.Play(); TFAY
            }
        }


    }


    //Reloads the Level
    public void Reload()
    {
        Debug.Log("reload");
        Scene scene = SceneManager.GetActiveScene();
        
        SceneManager.LoadScene(scene.name);
     
    }

    //controls the pausing of the scene
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            HidePaused();
        }
    }

    //shows objects with ShowOnPause tag
    public void showPaused()
    {
        //Debug.Log("You made it!");
        foreach (GameObject g in gameOverObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void HidePaused()
    {
        foreach (GameObject g in gameOverObjects)
        {
            g.SetActive(false);
        }
    }

    //loads inputted level
    public void LoadLevel()
    {
        Debug.Log("Back to stage select");
        SceneManager.LoadScene("StageSelect");
    }

    public void toTitle()
    {
        Debug.Log("Back to Title Screen");
        SceneManager.LoadScene("Title-Screen");
    }
}
