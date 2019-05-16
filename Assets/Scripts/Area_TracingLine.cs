using UnityEngine;

public class Area_TracingLine : MonoBehaviour
{
    LineRenderer p_lr;
    TextMesh text;
    Area_ScenceManager sm;

    // Use this for initialization
    void Awake()
    {
        p_lr = GetComponentInParent<LineRenderer>();
        text = GetComponentInChildren<TextMesh>();
        sm = FindObjectOfType<Area_ScenceManager>();
        text.text = sm.dispalyArea;

        int _p_count = p_lr.positionCount;
        float lx = 0, ly = 0, lz = 0;
        for (int i = 0; i < _p_count; i++)
        {
            lx += p_lr.GetPosition(i).x;
            ly += p_lr.GetPosition(i).y;
            lz += p_lr.GetPosition(i).z;
        }
        transform.position = new Vector3(lx / _p_count, ly / _p_count, lz / _p_count);
    }
}