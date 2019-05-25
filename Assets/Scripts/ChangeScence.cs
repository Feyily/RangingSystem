using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScence : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Scene s = SceneManager.GetActiveScene();
        GameObject _btn = new GameObject();
        switch (s.buildIndex)
        {
            case 0:
                _btn = GameObject.Find("Canvas/SelectPanel/Btn_Length");
                break;
            case 1:
                _btn = GameObject.Find("Canvas/SelectPanel/Btn_Angle");
                break;
            case 2:
                _btn = GameObject.Find("Canvas/SelectPanel/Btn_Distance");
                break;
            case 3:
                _btn = GameObject.Find("Canvas/SelectPanel/Btn_Area");
                break;
        }
        if (_btn != null)
            _btn.GetComponent<Button>().interactable = false;
    }

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
