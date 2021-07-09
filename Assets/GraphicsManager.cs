using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [Range(30, 144)] public int TargetFrames = 144;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFrames;
    }

    // Update is called once per frame
    void Update()
    {
        if(Application.targetFrameRate != TargetFrames)
            Application.targetFrameRate = TargetFrames;
    }
}
