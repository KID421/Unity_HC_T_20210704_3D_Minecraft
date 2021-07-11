using UnityEngine;

public class Player : MonoBehaviour
{
    #region 欄位
    [Header("採集範圍"), Range(0, 30)]
    public float rangeCollection = 2.5f;

    private Rigidbody rig;
    private Animator ani;
    private Transform traCamera;
    #endregion

    #region 事件
    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        // transform.Find("子物件名稱") 透過名稱搜尋子物件
        ani = transform.Find("男生").GetComponent<Animator>();
        traCamera = GameObject.Find("攝影機").transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // 圖示.繪製射線(中心點，射線方向)
        Gizmos.DrawRay(traCamera.position, traCamera.forward * rangeCollection);
    }

    private void Update()
    {
        Collection();
    }
    #endregion

    #region 方法

    /// <summary>
    /// 採集：繪製射線並判定是否攻擊以及採集物件
    /// </summary>
    private void Collection()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 射線碰撞資訊
            RaycastHit hit;
            // 物理.射線碰撞(中心點，射線方向，射線碰撞資訊，射線長度，圖層) - out 將資訊儲存在輸入的欄位內
            if (Physics.Raycast(traCamera.position, traCamera.forward, out hit, rangeCollection, 1 << 8))
            {
                // 對採集物造成傷害
                hit.collider.GetComponent<ObjectCollection>().Hit(1);
            }
        }
    }
    #endregion
}
