using System;

public class ShrinkerBuff : BaseBuff
{
    private const float DamageReduction = 0.3f;

    // private EnemyView _enemyView;
    // private Vector3 _originalScale;

    public ShrinkerBuff(BuffConfig config, int stacks) : base(config, stacks)
    {
        // _enemyView = enemyView;
        // if (_enemyView != null)
        // {
        //     _originalScale = _enemyView.transform.localScale;
        //     ApplyVisualEffect();
        // }
        ApplyVisualEffect();
    }
    public override void OnBeforeDealDamage(CharacterBase owner, ref int damage)
    {
        if (Stacks <= 0) return;
        damage = (int)Math.Floor(damage * (1f - DamageReduction));
        base.OnBeforeDealDamage(owner, ref damage);
    }


    private void ApplyVisualEffect()
    {
        // float scale = 1f - 0.15f * Stacks;
        // _enemyView.transform.localScale = _originalScale * Mathf.Max(0.5f, scale);
    }

}