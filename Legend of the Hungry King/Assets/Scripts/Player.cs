using UnityEngine;
using UnityEngine.UI;                   //引用 介面 API
using UnityEngine.SceneManagement;
using System.Collections;               //引用 系統.集合 API - 集合與協同程序

public class Player : MonoBehaviour
{
    #region 欄位

    [Header("移動速度"), Range(0, 5000)]
    public float speed = 10.5f;
    [Header("跳躍高度"), Range(0, 3000)]
    public int jump = 100;
    [Header("血量"), Range(0, 200)]
    public float hp = 100f; 
    [Header("生命數量")]
    public static int life = 3;
    [Header("是否在地板上"), Tooltip("地板")]
    public bool isGrounded;
    [Header("判斷地板碰撞的位移與半徑")]
    public Vector3 groundOffset;
    public float groundRadius = 0.2f;       
   

    private AudioSource aud;
    private Rigidbody2D rig;
    private Animator ani;
    private ParticleSystem ps;

    /// <summary>
    /// 紀錄按下左鍵的計時器
    /// </summary>
    private float timer;
    /// <summary>
    /// 攻擊力
    /// </summary>
    private float attack = 10;
    /// <summary>
    /// 圖片：血條
    /// </summary>
    private Image imgHp;
    /// <summary>
    /// 文字：生命
    /// </summary>
    private Text textHp;
    private float hpMax;
    #endregion

    #region 事件

    ///<summary>
    ///結束標題
    ///</summary>
    private Text textFinalTitle;
    ///<summary>
    ///結束畫面
    ///</summary>
    private CanvasGroup groupFinal;
    
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        Physics2D.IgnoreLayerCollision(9, 10, true);

        imgHp = GameObject.Find("血條").GetComponent<Image>();
        textHp = GameObject.Find("生命").GetComponent<Text>();        
        textHp.text = life.ToString();
        hpMax = hp;

        groupFinal = GameObject.Find("結束畫面").GetComponent<CanvasGroup>();
        textFinalTitle = GameObject.Find("結束標題").GetComponent<Text>();
    }

    private void Update()
    {
        if (Dead()) return;

        Move();
        Jump();        
    }
    private void FixedUpdate()
    {
        MoveFixed();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position + transform.right * groundOffset.x + transform.up * groundOffset.y, groundRadius);
        //Gizmos.color = new Color(0, 0, 1, 0.5f);
       // Gizmos.DrawSphere(transform.position + transform.right * posBullet.x + transform.up * posBullet.y, 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EatProp(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "死亡區域") hp = 0;
    }
        
    private void OnParticleCollision(GameObject other)
    {
        Hit(other.GetComponent<ParticleSystemData>().attack);
    }


    #endregion

    #region 方法

    /// <summary>
    /// 物理移動
    /// </summary>
    private void MoveFixed()
    {       
        rig.velocity = new Vector2(h * speed * Time.deltaTime, rig.velocity.y);
    }

    private float h;

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {       
        h = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }        
        ani.SetBool("走路開關", h != 0);
    }    

    /// <summary>
    /// 跳躍
    /// </summary>
    private void Jump()
    {        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {           
            rig.AddForce(new Vector2(0, jump));
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position + transform.right * groundOffset.x + transform.up * groundOffset.y, groundRadius, 1 << 8);
        
        if (hit && (hit.name == "地板" || hit.name == "跳台"))
        {
            isGrounded = true;
        }        
        else
        {
            isGrounded = false;
        }
       
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">造成的傷害</param>
    public void Hit(float damage)
    {
        hp -= damage;
        imgHp.fillAmount = hp / hpMax; 

        if (hp <= 0) Dead();
    }

    /// <summary>
    /// 死亡
    /// </summary>
    /// <returns>是否死亡</returns>
    private bool Dead()
    {        
        if (!ani.GetBool("死亡開關") && hp <= 0)
        {
            ani.SetBool("死亡開關", hp <= 0);
            life--;                                
            textHp.text = life.ToString();         

            if (life > 0) Invoke("Replay", 1.5f);     
            else StartCoroutine(GameOver());         
        }
        return hp <= 0;
    }

    public IEnumerator GameOver(string finalTitle = "GameOver")
    {
        textFinalTitle.text = finalTitle;

        while (groupFinal.alpha < 1)                   
        {
            groupFinal.alpha += 0.05f;                        
            yield return new WaitForSeconds(0.02f);     
        }
        groupFinal.interactable = true;                    
        groupFinal.blocksRaycasts = true;               
    }

    /// <summary>
    /// 重新遊戲
    /// </summary>    
    private void Replay()
    {
        SceneManager.LoadScene("遊戲畫面");
    }

    /// 吃道具
    /// </summary>
    /// <param name="prop">道具的名稱</param>
    private void EatProp(GameObject prop)
    {
        if (prop.tag == "道具")
        {
            print(prop.name.Remove(2));

            switch (prop.name.Remove(2))
            {
                case "補血":

                    hp += 30;
                    hp = Mathf.Clamp(hp, 0, hpMax);
                    imgHp.fillAmount = hp / hpMax;
                    break;
            }
            Destroy(prop);
        }
    }

    #endregion

}