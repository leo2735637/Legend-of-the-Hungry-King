using UnityEngine;

public class Boss : Enemy
{
    [Header("進入第二階段的血量")]
    public float secondHp = 300;
    [Header("魔王攻擊：第二階段特效")]
    public ParticleSystem psAttackSecond;
    [Header("第二階段攻擊力"), Range(0, 1000)]
    public float attackSecond = 10;

    /// <summary>
    /// 魔王的狀態
    /// </summary>
    public StateBoss stateBoss;
    public override void Hit(float damage)
    {
        base.Hit(damage);            

        
        if (hp <= secondHp)
        {
            radiusAttack = 7;
            stateBoss = StateBoss.second;
        }
    }

    protected override void AttackState()
    {
        switch (stateBoss)
        {
            case StateBoss.first:
                base.AttackState();
                break;
            case StateBoss.second:
                timer = 0;
                ani.SetTrigger("攻擊觸發");
                psAttackSecond.transform.position = transform.position + transform.right * 3 + transform.up * -3f;
                psAttackSecond.transform.eulerAngles = transform.eulerAngles + new Vector3(0, 180, 0);
                psAttackSecond.Play();
                break;
        }
    }

    protected override void Dead()
    {
        base.Dead();

        StartCoroutine(player.GetComponent<Player>().GameOver("You Win"));
    }
    protected override void Start()
    {
        base.Start();

        psAttackSecond.GetComponent<ParticleSystemData>().attack = attackSecond;
    }
}

/// <summary>
/// 魔王狀態：第一階段、第二階段
/// </summary>
public enum StateBoss
{
    first, second
}
