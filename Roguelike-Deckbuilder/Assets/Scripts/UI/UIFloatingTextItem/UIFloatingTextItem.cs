using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIFloatingTextItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector2 _startPos;

    private void Awake() => _rectTransform = GetComponent<RectTransform>();

    // 由Pool取出后调用，返回UniTask供外部等待回收
    public async UniTask PlayAsync(string text, Vector2 screenPos, Color color, float fontSize, bool isCritical)
    {
        _text.text = text;
        _text.color = color;
        _text.fontSize = isCritical ? fontSize * 1.8f : fontSize;
        _canvasGroup.alpha = 1f;
        _rectTransform.anchoredPosition = screenPos;
        _startPos = screenPos;

        // 核心动画序列：上飘 + 渐隐 + 轻微缩放
        var sequence = DOTween.Sequence();
        sequence.Join(_rectTransform.DOAnchorPosY(_startPos.y + 80f, 0.8f).SetEase(Ease.OutCubic));
        sequence.Join(_canvasGroup.DOFade(0f, 0.8f).SetEase(Ease.InQuad));

        if (isCritical)
        {
            sequence.Insert(0, _rectTransform.DOScale(0.5f, 0.1f).From().SetEase(Ease.OutBack));
        }
        else
        {
            sequence.Insert(0, _rectTransform.DOScale(1.2f, 0.15f).From().SetEase(Ease.OutBack));
        }

        // 桥接 UniTask，等待动画彻底结束
        await sequence.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        // 动画结束，重置Scale（防止污染对象池）
        _rectTransform.localScale = Vector3.one;
    }
}
