using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FanLayout : MonoBehaviour
{

    [Header("卡牌尺寸（固定）")]
    [SerializeField] private float cardWidth = 130f;       // 每张卡牌的宽度
    [SerializeField] private float cardGap = 20f;          // 卡牌之间的间距

    [Header("弧形参数（自动计算）")]
    [SerializeField] private float curveHeightRatio = 0.6f; // 弧形高度 = spread × 这个比例
    [SerializeField] private float maxRotation = 25f;       // 边缘卡牌最大倾斜

    [Header("整体偏移")]
    [SerializeField] private Vector2 offset = new Vector2(0, -150f);

    private List<RectTransform> _cards = new();

    public void Refresh()
    {
        _cards.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                var rect = child.GetComponent<RectTransform>();
                if (rect != null) _cards.Add(rect);
            }
        }
        int count = _cards.Count;
        if (count == 0) return;

        // ========== 动态计算 spread ==========
        // 总宽度 = 所有卡牌宽度 + 所有间距
        float totalWidth = count * cardWidth + (count - 1) * cardGap;

        // spread = 总宽度的一半（因为 t 范围是 -1 到 1）
        float spread = totalWidth / 2f;

        // 弧形高度 = spread × 比例（自动适配）
        float curveHeight = spread * curveHeightRatio;

        for (int i = 0; i < count; i++)
        {
            // t = -1 到 1，0 是中间
            float t = count == 1 ? 0f : (float)i / (count - 1) * 2f - 1f;

            // X：均匀分布
            float x = t * spread;

            // Y：中间最高（抛物线）
            float heightFactor = 1f - t * t;
            float y = heightFactor * curveHeight;

            // 旋转：中间正，边缘倾斜
            float rotationZ = -t * maxRotation;

            // 应用
            var rect = _cards[i];
            rect.anchoredPosition = new Vector2(x, y) + offset;
            rect.localEulerAngles = new Vector3(0, 0, rotationZ);
        }
    }

    // ========== 可选：预览 ==========
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        int count = transform.childCount;
        if (count == 0) return;

        float totalWidth = count * cardWidth + (count - 1) * cardGap;
        float spread = totalWidth / 2f;
        float curveHeight = spread * curveHeightRatio;

        Vector3 center = transform.position + (Vector3)offset;
        Gizmos.color = Color.green;

        for (int i = 0; i <= 20; i++)
        {
            float t = (float)i / 20f * 2f - 1f;
            float x = t * spread;
            float y = (1f - t * t) * curveHeight;
            Vector3 pos = center + new Vector3(x, y, 0);

            if (i == 0) continue;
            float prevT = (float)(i - 1) / 20f * 2f - 1f;
            float prevX = prevT * spread;
            float prevY = (1f - prevT * prevT) * curveHeight;
            Vector3 prevPos = center + new Vector3(prevX, prevY, 0);
            Gizmos.DrawLine(prevPos, pos);
        }
    }
#endif
}
