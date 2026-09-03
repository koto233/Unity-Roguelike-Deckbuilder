using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using LitFramework.UI.Core.Window;
using TMPro;
using UnityEngine;

public class FloatingTextItem : UIBase
{
    [Header("字体参数")]
    private float _criticalFontSizeMultiplier = 1.5f;           // 暴击放大 1.5 倍
    private float _fontSize = 70f;
    private float _distance = 100f;
    private TextMeshProUGUI _damageText;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector2 _startPos;

    protected override void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _damageText = GetComponent<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();
        // _fontSize = Mathf.Clamp(_fontSizeRatio * Screen.height, _minFontSize, _maxFontSize);
        // float screenHeight = Screen.height;
        // _distance = 10f;
        // _distance = Mathf.Clamp(_distance, _minFloatDistance, _maxFloatDistance);
    }

    /// <summary>
    /// 播放飘字动画
    /// </summary>
    /// <param name="text"></param>
    /// <param name="screenPos"></param>
    /// <param name="color"></param>
    /// <param name="fontSize"></param>
    /// <param name="isCritical"></param>
    /// <returns></returns> <summary>
    public async UniTask PlayAsync(string text, Vector2 screenPos, bool isCritical)
    {
        _damageText.SetText(text);
        // _damageText.fontSize = isCritical ? _fontSize * 1.8f : _fontSize;
        _damageText.fontSize = _fontSize;
        _canvasGroup.alpha = 1f;
        float randomOffset = Random.Range(-30f, 30f);
        screenPos.x += randomOffset;
        screenPos.y += randomOffset;
        _rectTransform.transform.position = screenPos;

        _startPos = screenPos;

        // 核心动画序列：上飘 + 渐隐 + 轻微缩放
        var sequence = DOTween.Sequence();
        sequence.Join(_rectTransform.DOMoveY(_startPos.y + _distance, 0.8f).SetEase(Ease.OutCubic));
        sequence.Join(_canvasGroup.DOFade(0f, 0.8f).SetEase(Ease.InQuad));
        // if (isCritical)
        // {
        //     sequence.Insert(0, _rectTransform.DOScale(0.5f, 0.1f).From().SetEase(Ease.OutBack));
        // }
        // else
        // {
        //     sequence.Insert(0, _rectTransform.DOScale(1.2f, 0.15f).From().SetEase(Ease.OutBack));
        // }
        // 桥接 UniTask，等待动画彻底结束
        await sequence.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

        // 动画结束，重置Scale（防止污染对象池）
        _rectTransform.localScale = Vector3.one;
    }
}
