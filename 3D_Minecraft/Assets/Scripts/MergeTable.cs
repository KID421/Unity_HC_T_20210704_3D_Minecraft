using UnityEngine;

/// <summary>
/// 合成表：儲存所有合成方式
/// </summary>
[CreateAssetMenu(fileName = "合成表", menuName = "KID/合成表")]
public class MergeTable : ScriptableObject
{
    [Header("所有合成資料")]
    public MergeData[] allMergeData;
}

/// <summary>
/// 合成資料：儲存每一筆合成資料
/// </summary>
// Serializable 序列化：將類別顯示在屬性面板上
[System.Serializable]
public class MergeData
{
    /// <summary>
    /// 素材 1 ~ 4 所需配置的物件
    /// </summary>
    [Header("素材 1 ~ 4 所需配置的物件")]
    public GameObject[] goMerge = new GameObject[4];
    [Header("合成後的物件")]
    public GameObject goMergeResult;
}