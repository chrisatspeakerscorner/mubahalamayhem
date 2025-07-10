using System.Runtime.InteropServices;
using UnityEngine;

public class FullscreenManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RequestFullScreen();
#endif

    public void GoFullscreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestFullScreen();
#else
        Debug.Log("Fullscreen is only available in WebGL builds.");
#endif
    }
}
