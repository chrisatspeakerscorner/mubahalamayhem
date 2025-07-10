using System.Collections;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    public float amplitude = 5f; // height of the wave
    public float frequency = 5f; // speed of the wave
    public float waveSpeed = 2f;

    public float delay = 0.1f; // Delay between letters

    [SerializeField] private bool _shouldTextWave = true;
    [SerializeField] private bool _startOnEnable = true;

    private TMP_TextInfo _textInfo;

    public TextMeshProUGUI _text;

    [TextArea] public string fullText;

    private Coroutine _typingCoroutine;

    private float _time;

    private void OnEnable()
    {
        _text.text = string.Empty;

        if(_startOnEnable)
        {
            StartTyping();
        }
    }

    private void OnDisable()
    {
        _text.text = string.Empty;
    }

    public void StartTyping()
    {
        if (null != _typingCoroutine)
        {
            StopCoroutine(_typingCoroutine);
        }

        _typingCoroutine = StartCoroutine(TypeText());
    }

    public void ClearTypedText()
    {
        _text.text = string.Empty;
    }

    private void Update()
    {
        if(!_shouldTextWave)
        {
            return;
        }

        _text.ForceMeshUpdate();
        _textInfo = _text.textInfo;

        _time += Time.deltaTime * waveSpeed;

        for (int i = 0; i < _textInfo.characterCount; i++)
        {
            if (!_textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = _textInfo.characterInfo[i].vertexIndex;
            int materialIndex = _textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = _textInfo.meshInfo[materialIndex].vertices;

            // Calculate the offset with a sine wave
            float offsetY = Mathf.Sin(_time + i * 0.2f) * amplitude;
            Vector3 offset = new Vector3(0, offsetY, 0);

            // Apply the offset to each vertex of the character
            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }
        }

        // Push the updated vertex positions back to the mesh
        for (int i = 0; i < _textInfo.meshInfo.Length; i++)
        {
            _textInfo.meshInfo[i].mesh.vertices = _textInfo.meshInfo[i].vertices;

            _text.UpdateGeometry(_textInfo.meshInfo[i].mesh, i);
        }
    }

    private IEnumerator TypeText()
    {
        _text.text = "";
        for (int i = 0; i <= fullText.Length; i++)
        {
            _text.text = fullText.Substring(0, i);

            yield return new WaitForSeconds(delay);
        }
    }
}
