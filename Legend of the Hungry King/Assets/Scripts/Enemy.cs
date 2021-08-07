using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 100)]
    public float speed = 50f;
    [Header("攻擊力"), Range(0, 100)]
    public float attack = 10f;
    [Header("攻擊冷卻"), Range(0, 30)]
    public float cd = 3;
    [Header("血量"), Range(0, 500)]
    public float hp = 200f;
    [Header("追蹤範圍"), Range(0, 50)]
    public float radiusTrack = 5;
    [Header("攻擊範圍"), Range(0, 30)]
    public float radiusAttack = 2;
    [Header("偵測地板的位移與半徑")]
    public Vector3 groundOffset;
    public float groundRadius = 0.1f;
    [Header("掉落道具")]
    public GameObject prop;
    [Header("掉落機率"), Range(0f, 1f)]
    public float propProbilty = 0.5f;
    [Header("攻擊區域位移尺寸")]
    public Vector3 attackoffset;
    public Vector3 attackSize;

    private Rigidbody2D rig;
    protected Transform player;
    /// <summary>
    /// 原始速度
    /// </summary>
    private float speedOriginal;

    protected Animator ani;
    /// <summary>
    /// 計時器：記錄攻擊冷卻
    /// </summary>
    protected float timer;
    #endregion

    #region 事件
    protected virtual void Start()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
                
        player = GameObject.Find("玩家").transform;

        timer = cd;                       
        speedOriginal = speed;          
    }

    private void OnDrawGizmos()
    {
        #region 繪製距離與檢查地板

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusTrack);

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusAttack);

        Gizmos.color = new Color(0.6f, 0.9f, 1, 0.7f);
        Gizmos.DrawSphere(transform.position + transform.right * groundOffset.x + transform.up * groundOffset.y, groundRadius);
        #endregion

        #region 繪製攻擊區域
        Gizmos.color = new Color(0.3f, 0.3f, 1, 0.8f);
        Gizmos.DrawCube(transform.position + transform.right * attackoffset.x + transform.up * attackoffset.y, attackSize);

        #endregion
    }
    private void Update()
    {
        Move();
    }

    #endregion

    #region 方法

    /// <summary>
    /// 移動：偵測是否進入追蹤範圍
    /// </summary>
    private void Move()
    {       
        if (ani.GetBool("死亡開關")) return;
                
        float dis = Vector3.Distance(player.position, transform.position);
                
        if (dis <= radiusAttack)
        {
            Attack();
            LookAtplayer();
        }
        
        else if (dis <= radiusTrack)
        {
            rig.velocity = transform.right * speed * Time.deltaTime;
            ani.SetBool("走路開關", speed != 0);   
            LookAtplayer();
            CheckGround();
        }
        else
        {
            ani.SetBool("走路開關", false);
        }
    }

    /// <summary>
    /// 攻擊
    /// </summary> 
    private void Attack()
    {
        ani.SetBool("走路開關", false);
                
        if (timer <= cd)
        {
            timer += Time.deltaTime;
        }       
        else AttackState();
    }
    protected virtual void AttackState()
    {
        timer = 0;
        ani.SetTrigger("攻擊觸發");
       
        Collider2D hit = Physics2D.OverlapBox(transform.position + transform.right * attackoffset.x + transform.up * attackoffset.y, attackSize, 0);
      
        if (hit && hit.name == "玩家") hit.GetComponent<Player>().Hit(attack);
    }

    /// <summary>
    /// 面向玩家
    /// </summary>
    private void LookAtplayer()
    {           
        if (transform.position.x > player.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

    /// <summary>
    /// 檢查前方是否有地板
    /// </summary>
    private void CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position + transform.right * groundOffset.x + transform.up * groundOffset.y, groundRadius, 1 << 8);
               
        if (hit && (hit.name == "地板" || hit.name == "跳台")) speed = speedOriginal;
        else speed = 0;
    }

    /// <summary>
    /// 死亡
    /// </summary>
    protected virtual void Dead()
    {
        ani.SetBool("死亡開關", true);
        rig.Sleep();                                            
        rig.constraints = RigidbodyConstraints2D.FreezeAll;       
        GetComponent<CapsuleCollider2D>().enabled = false;        
        Destroy(gameObject, 2);                                 
        Prop();
    }

    /// <summary>
    /// 掉落道具
    /// </summary>
    private void Prop()
    {
        float r = Random.value; 

        if (r <= propProbilty) Instantiate(prop, transform.position, Quaternion.identity);
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">接受到的傷害</param>>
    public virtual void Hit(float damage)
    {
        hp -= damage;
        
        if (hp <= 0) Dead();
    }

    #endregion
}
