using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderUtility : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private Slider _slider; 
    [SerializeField] private AudioSource _settingsAS;
    [SerializeField] private AudioClip _settingsSFX;

    public void OnPointerUp(PointerEventData eventData)
    {
        _settingsAS.PlayOneShot(_settingsSFX);
    }
}
