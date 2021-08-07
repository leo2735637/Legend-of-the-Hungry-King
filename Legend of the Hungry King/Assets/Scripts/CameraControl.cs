using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region 欄位
    [Header("玩家的變形元件")]
    public Transform player;
    [Header("追蹤的速度"), Range(0, 100)]
    public float speed = 30;        
    [Header("上下邊界")]
    public Vector2 limtY = new Vector2(0.4f, 0f);   
    [Header("左右邊界")]
    public Vector2 limtX = new Vector2(-0.55f, 147f);

    #endregion

    #region 方法
    /// <summary>
    /// 攝影機追蹤玩家的座標
    /// </summary>
    private void Track()
    {
        Vector3 vCam = transform.position;     
        Vector3 vPla = player.position;        

        
        vCam = Vector3.Lerp(vCam, vPla, 0.5f * speed * Time.deltaTime);
       
        vCam.z = -10;
        
        vCam.x = Mathf.Clamp(vCam.x, limtX.x, limtX.y);
        vCam.y = Mathf.Clamp(vCam.y, limtY.x, limtY.y);
               
        transform.position = vCam;
    }

    #endregion

    #region 事件
    //延遲更新事件
    //在Update後執行
    //官方建議攝影機追蹤行為可在此事件呼叫執行
    private void LateUpdate()
    {
        Track();
    }


    #endregion
}
