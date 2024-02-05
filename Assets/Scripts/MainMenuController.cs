using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{
    public Animator animator;
    public void OnStartGameClick()
    {
        StartCoroutine(LoadNextLevel());
    }
    public void OnExitGameClick()
    {
        Application.Quit();
    }

    private IEnumerator LoadNextLevel()
    {
        animator.SetTrigger("StartLoading");
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("EndLoading");
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene("Game");

    }
}
