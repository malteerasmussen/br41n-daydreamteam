using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DualCameraController : MonoBehaviour
{
    public Camera activeCamera;
    public Camera hiddenCamera;

    // Start is called before the first frame update
    void Awake()
    {
        //SceneManager.LoadScene(1, LoadSceneMode.Additive);

        var rt = new RenderTexture(Screen.width, Screen.height, 24);
        Shader.SetGlobalTexture("_DayDreamTexture", rt);
        hiddenCamera.targetTexture = rt;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SwapCameras();
        }
    }

    private void SwapCameras()
    {
        activeCamera.targetTexture = hiddenCamera.targetTexture;
        hiddenCamera.targetTexture = null;

        var swapCamera = activeCamera;
        activeCamera = hiddenCamera;
        hiddenCamera = swapCamera;
    }
}
