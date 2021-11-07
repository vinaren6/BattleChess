using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class sceneManager : MonoBehaviour
{
    private static sceneManager instance;
    public Image black;
    public Animator anim;

    private bool running = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
           
            if (!running)
            {

                running = true;
                if (SceneManager.GetActiveScene().name == "Chess")
                {
                    

                    StartCoroutine(LoadBattle());
   


                    

                }
                else
                {
                    StartCoroutine(LoadChess());
                }
            }

        }

    }
    IEnumerator LoadYourAsyncScene(string scene)
    {
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

            yield return null;
        }


    }
    IEnumerator LoadBattle()
    {
        print("hello");
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene("Battle");
        Chessboard.instance.gameObject.SetActive(false);
        anim.SetBool("Fade", false);
        running = false;
    }
    IEnumerator LoadChess()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene("Chess");
        Chessboard.instance.gameObject.SetActive(true);
        anim.SetBool("Fade", false);
        running = false;
    }
}
