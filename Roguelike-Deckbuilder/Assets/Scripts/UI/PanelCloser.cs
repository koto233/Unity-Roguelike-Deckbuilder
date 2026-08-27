// PanelCloser.cs
using UnityEngine;
using UnityEngine.UI;

public class PanelCloser : MonoBehaviour
{
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        if (_closeButton == null)
            _closeButton = GetComponentInChildren<Button>(); // 自动找子物体里的按钮

        if (_closeButton != null)
            _closeButton.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}