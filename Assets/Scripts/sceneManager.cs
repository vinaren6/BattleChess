using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class sceneManager : MonoBehaviour
{
    public static sceneManager instance;
    public Image black;
    public Animator anim;



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


    public IEnumerator LoadBattle()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene("Battle");
        Chessboard.instance.gameObject.SetActive(false);
        anim.SetBool("Fade", false);

    }
    public IEnumerator LoadChess()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene("Chess");
        Chessboard.instance.gameObject.SetActive(true);
      
        
        anim.SetBool("Fade", false);

        Chessboard.instance.afterCombat();

    }
    public void LoadChessFromMenu()
    {

        StartCoroutine(LoadChessFromMenuAnim());


    }
    private IEnumerator LoadChessFromMenuAnim()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => black.color.a == 1);
        SceneManager.LoadScene("Chess");
        anim.SetBool("Fade", false);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
