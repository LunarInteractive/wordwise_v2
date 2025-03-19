using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    public void Pause(){
        PauseGameWithDelay(.5f);
    }
    public void Resume(){
        Time.timeScale = 1f;
    }
    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PauseGameWithDelay(float delay){
        StartCoroutine(PauseAfterDelay(delay));
    }
    IEnumerator PauseAfterDelay (float delay){
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0;
    }
}
