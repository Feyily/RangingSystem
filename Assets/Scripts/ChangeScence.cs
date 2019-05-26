using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScence : MonoBehaviour {

    public void ChangeToLengthMeasure() {
        SceneManager.LoadScene(0);
    }
    public void ChangeToAngleMeasure()
    {
        SceneManager.LoadScene(1);
    }
    public void ChangeToDistanceMeasure()
    {
        SceneManager.LoadScene(2);
    }
    public void ChangeToAreaMeasure()
    {
        SceneManager.LoadScene(3);
    }
}
