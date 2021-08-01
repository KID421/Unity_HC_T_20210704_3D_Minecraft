using UnityEngine;

/// <summary>
/// 道具資訊：道具圖示、道具物件
/// </summary>
public class Prop : MonoBehaviour
{
    [Header("道具圖示")]
    public Sprite sprProp;
    [Header("道具物件")]
    public GameObject goProp;
    [Header("道具類型")]
    public PropType propType;
}

/// <summary>
/// 道具類型：無、地形物件、武器
/// </summary>
public enum PropType
{
    None, TerrainObject, Weapon
}
