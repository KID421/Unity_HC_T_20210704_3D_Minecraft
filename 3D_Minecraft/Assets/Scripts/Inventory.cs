using UnityEngine;
using System.Linq;                  // 引用 查詢語言 LinQ API
using System.Collections.Generic;   // 引用 系統 集合 一般 API

/// <summary>
/// 道具欄管理系統
/// 吃到道具後累加
/// 道具欄顯示系統
/// 裝備道具欄介面
/// 將資訊儲存到項目 Item - 物件與數量
/// </summary>
public class Inventory : MonoBehaviour
{
    #region 欄位
    [Header("道具清單")]
    /// <summary>
    /// 道具清單
    /// </summary>
    public List<Prop> props = new List<Prop>();
    [Header("道具欄")]
    public GameObject goInventory;
    [Header("裝備的道具欄 - 5 個")]
    public InventoryItem[] itemEquipment;
    [Header("道具欄 - 24 個")]
    public InventoryItem[] itemProp;
    [Header("裝備的道具資訊 - 5 個")]
    public Item[] itemDataEquipment;
    [Header("道具欄的道具資訊 - 24 個")]
    public Item[] itemDataProp;
    #endregion

    #region 事件
    private void Start()
    {
        // 將道具欄放回原位並隱藏 - 避免隱藏物件導致的錯誤
        goInventory.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        goInventory.SetActive(false);
    }

    private void Update()
    {
        SwitchInventory();
    }
    #endregion

    #region 方法
    /// <summary>
    /// 切換道具欄介面顯示與隱藏
    /// </summary>
    private void SwitchInventory()
    {
        // 如果 按下 E 道具就設定為相反的顯示狀態
        if (Input.GetKeyDown(KeyCode.E)) goInventory.SetActive(!goInventory.activeInHierarchy);
    }

    /// <summary>
    /// 添加道具：玩家吃到道具後呼叫
    /// </summary>
    public void AddProp(Prop prop)
    {
        props.Add(prop);                    // 添加吃到的道具名稱到清單內
        ObjectPoolUsing(prop.gameObject);   // 丟進物件池
        ShowPropInInventory(prop);
    }

    /// <summary>
    /// 顯示道具欄與裝備欄上的道具 - 先處理裝備再處理道具
    /// </summary>
    private void ShowPropInInventory(Prop prop)
    {
        if (UpdateItem(prop, itemEquipment, itemDataEquipment))
        UpdateItem(prop, itemProp, itemDataProp);
    }

    /// <summary>
    /// 更新裝備與道具欄每一格欄位
    /// </summary>
    /// <param name="pror">吃到的道具資訊</param>
    /// <param name="items">道具欄陣列 - 裝備或者道具</param>
    /// <returns>是否道具已放滿</returns>
    private bool UpdateItem(Prop prop, InventoryItem[] items, Item[] itemData)
    {
        for (int i = 0; i < items.Length; i++)                                          // 迴圈執行 裝備道具欄 - 5 個
        {
            if (items[i].hasProp && items[i].imgProp.sprite == prop.sprProp)            // 如果 格子內有道具 並且 跟當前吃到的道具相同 就累加
            {
                // (x => ***) Lambda 簡寫
                // 數量 = 道具清單.查找(查找與 當前道具.圖片 相同的 道具資料).轉清單().數量
                int count = props.Where(x => x.sprProp == prop.sprProp).ToList().Count;

                items[i].textProp.text = count + "";                                    // 更新數量
                UpdateItemData(i, prop, itemData, count);
                return false;
            }
            else if (!items[i].hasProp)                                                 // 如果 裝備道具欄 沒有道具 才可以放道具
            {
                items[i].hasProp = true;                                                // 已經放道具
                items[i].imgProp.enabled = true;                                        // 更新圖片
                items[i].imgProp.sprite = prop.sprProp;                                 // 放入圖片
                items[i].textProp.text = 1 + "";                                        // 更新數量
                UpdateItemData(i, prop, itemData, 1);
                return false;                                                           // 跳出 break 僅跳出迴圈，return 跳出此方法
            }
        }

        return true;                                                                    // 已經塞滿道具
    }

    /// <summary>
    /// 更新裝備與道具的 Item Class 物件與數量資訊
    /// </summary>
    private void UpdateItemData(int index, Prop prop, Item[] itemData, int count)
    {
        itemData[index].goItem = prop.goProp;
        itemData[index].count = count;
    }

    /// <summary>
    /// 物件池使用中的道具：放在遠處
    /// </summary>
    private void ObjectPoolUsing(GameObject obProp)
    {
        obProp.transform.position = Vector3.one * -99999;

        #region 關閉元件
        obProp.GetComponent<Collider>().enabled = false;
        obProp.GetComponent<ConstantForce>().enabled = false;
        obProp.GetComponent<Rigidbody>().useGravity = false;
        obProp.GetComponent<Rigidbody>().Sleep();
        #endregion
    }
    #endregion
}
