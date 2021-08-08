using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// <summary>
    /// 攻擊力
    /// </summary>
    public float attack;

    private void Start()
    {
        
        Physics2D.IgnoreLayerCollision(10, 10, true);
        
        Physics2D.IgnoreLayerCollision(10, 11, true);
    }    
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.tag == "敵人")
        {            
            collision.gameObject.GetComponent<Enemy>().Hit(attack);
            Destroy(gameObject);
        }                
        Destroy(gameObject);
    }
}
