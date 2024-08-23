using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public float transitionTime = 1;
    public Animator transition;

    public void StartScene()
    {
        StartCoroutine(StartSceneWithTransition(sceneName));
    }

    // Just a wrapper that plays a fade transition whenever the scene changes
    public IEnumerator StartSceneWithTransition(string gameScene)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(gameScene);
    }


    public void Quit()
    {
        Application.Quit();
    }
}
