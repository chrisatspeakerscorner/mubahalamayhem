using UnityEngine;

public class Init : MonoBehaviour
{
    [SerializeField] private Canvas _introCanvas;
    [SerializeField] private Canvas _menuCanvas;
    [SerializeField] private Canvas _characterSelectionCanvas;
    [SerializeField] private Canvas _sceneSelectionCanvas;
    [SerializeField] private Canvas _gameCanvas;
    [SerializeField] private Canvas _settingsCanvas;

    private void Awake()
    {
        _introCanvas.gameObject.SetActive(true);
        _settingsCanvas.gameObject.SetActive(false);
        _menuCanvas.gameObject.SetActive(false);
        _characterSelectionCanvas.gameObject.SetActive(false);
        _sceneSelectionCanvas.gameObject.SetActive(false);
        _gameCanvas.gameObject.SetActive(false);
    }
}
