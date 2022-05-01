using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tobii.XR;

public class DualCameraController : MonoBehaviour
{
public Camera activeCamera;
public Camera hiddenCamera;
public udpreceiver up;

public AudioSource chime;
public AudioSource nature1;
public AudioSource officeSound;
public AudioSource nature2;
public AudioSource boss;

public GameObject meme;



// Start is called before the first frame update
void Awake()
{
        //SceneManager.LoadScene(1, LoadSceneMode.Additive);

        var rt = new RenderTexture(Screen.width, Screen.height, 24);
        Shader.SetGlobalTexture("_DayDreamTexture", rt);
        hiddenCamera.targetTexture = rt;


}

void Start(){
        SwapCameras();
}
// Update is called once per frame

bool office = true;
bool forest = false;
[Range(-1.0f, 1.0f)]
public float threshold = 0.5f;

public GameObject gPlane;
void Update()
{

        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);
        var isLeftEyeBlinking = eyeTrackingData.IsLeftEyeBlinking;
        var isRightEyeBlinking = eyeTrackingData.IsRightEyeBlinking;
        var eyesClosed = isLeftEyeBlinking && isRightEyeBlinking;

        if (Input.GetKeyDown("space"))
        {
                SwapCameras();
        }

    /*    if (up.fromoto1 > 0.8f && !float.IsInfinity(up.fromoto1) && office && up.syncedyet == 1  ) {
                SwapCameras();
                Debug.Log("SWITCHING CAMS");
                chime.Play();
                office = false;
                StartCoroutine(StartFade(nature1, 2, 1 ));
        }*/
          if (Input.GetKeyDown("f")) {
                SwapCameras();
                Debug.Log("SWITCHING CAMS");
              //  chime.Play();
                office = false;
                nature1.Play();
                StartCoroutine(StartFade(nature1, 2, 1 ));
        }
        if (up.calibrationFinished && up.fromoto1 < 0.5f && !float.IsInfinity(up.fromoto1) && office && !forest && up.syncedyet == 1  ) {
                StartCoroutine(StartFade(officeSound, 2, 0 ));
                Debug.Log("OFFICE DOWN, STREAM UP ");
                //chime.Play();
                office = false;
                nature1.Play();
                StartCoroutine(StartFade(nature1, 4, 1 ));

        }

        if (up.fromoto1 < 0.25f && !float.IsInfinity(up.fromoto1) && !office && !forest && up.syncedyet == 1  ) {
                //StartCoroutine(StartFade(nature1, 2, 0 ));
                Debug.Log("SWITCHING CAMS");
                //chime.Play();
                office = false;
                nature2.Play();
                StartCoroutine(StartFade(nature2, 1, 1 ));
                SwapCameras();
                forest = true;
                float returnDelay = 30;
                StartCoroutine(StartGreyFade(gPlane, 2, 1, returnDelay ));
                StartCoroutine(StartFade(nature1, 2, 0, returnDelay));
                StartCoroutine(StartFade(nature2, 2, 0, returnDelay));
                StartCoroutine(StartFade(officeSound, 2, 1, returnDelay));
        }


        if (Input.GetKeyDown("g")) {
            //  SwapCameras();
            //  Debug.Log("SWITCHING CAMS");
            //  chime.Play();
            //  office = false;
            //  nature1.Play();
              StartCoroutine(StartGreyFade(gPlane, 2, 1 ));
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

public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume, float delay=0)
    {
        yield return new WaitForSeconds(delay);
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }


    public IEnumerator StartGreyFade(GameObject plane, float duration, float targetVolume, float delay=0)
        {
          meme.SetActive(true);
          yield return new WaitForSeconds(delay);

          var renderer = plane.GetComponent<Renderer>();
            float currentTime = 0;
            float start = 0.0f;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float val = Mathf.Lerp(start, targetVolume, currentTime / duration);
                renderer.material.SetColor("_Color", new Color(1,1,1,val));
                yield return null;
            }


            bool isLeftEyeBlinking = false;
            bool isRightEyeBlinking = false;

            while (!isLeftEyeBlinking && !isRightEyeBlinking)
            {
              var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.Local);
              isLeftEyeBlinking = eyeTrackingData.IsLeftEyeBlinking;
              isRightEyeBlinking = eyeTrackingData.IsRightEyeBlinking;
              yield return null;
            }
            boss.Play();
            SwapCameras();
            yield break;
        }
}
