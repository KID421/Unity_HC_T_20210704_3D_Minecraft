using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    #region 欄位
    [Header("地形大小")]
    public int x;
    public int y;

    // 陣列
    // 語法：類型後加上中括號[]
    // 用途：存放相同類型的多筆資料
    // 陣列內的資料都有編號，並且從 0 開始
    /// <summary>
    /// 用於存放地形物件，按順序為：0 草地、1 泥土、2 石頭
    /// </summary>
    [Header("地形物件：0 草地、1 泥土、2 石頭")]
    public GameObject[] objTerrains;

    /// <summary>
    /// 地形群組：用來放生成的地形物件
    /// </summary>
    private Transform traTerrainGroup;
    #endregion

    #region 事件
    private void Start()
    {
        traTerrainGroup = GameObject.Find("地形群組").transform;
    }
    #endregion

    #region 方法
    private void Generate()
    {

    }
    #endregion
}
