using System.Runtime.InteropServices;
using UnityEngine;

public class WebLinkOpener : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenNewTab(string url);

    public void OpenLink()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        OpenNewTab("https://buymeacoffee.com/chrisatspeakerscorner");
#else
        Application.OpenURL("https://buymeacoffee.com/chrisatspeakerscorner");
#endif
    }
}
