using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 切換道具管理器、合成系統
/// 切換道具：裝備與道具欄
/// 點擊後將道具資訊存放到【選取中的道具】
/// 並判定與另一個欄位切換
/// 切換道具至合成素材區後匹配有沒有對應到合成表物件
/// </summary>
public class InventorySwitch : MonoBehaviour
{
    #region 欄位
    [Header("選取中的道具")]
    public Transform traChooseProp;
    [Header("要判斷的畫布：圖像射線碰撞")]
    public GraphicRaycaster graphic;
    [Header("事件系統：EventSystem")]
    public EventSystem eventSystem;
    [Header("素材 1 ~ 4")]
    public Item[] itemMerge;
    [Header("合成表")]
    public MergeTable mergeTable;

    /// <summary>
    /// 當前合成區的組合
    /// </summary>
    private GameObject[] goMergeCurrent = new GameObject[4];
    /// <summary>
    /// 事件碰撞座標資訊
    /// </summary>
    private PointerEventData dataPointer;
    /// <summary>
    /// 是否選到道具
    /// </summary>
    private bool chooseItem;
    #endregion

    #region 事件
    private InventoryItem inventoryItemChooseProp;
    private Item itemChooseProp;

    private void Start()
    {
        inventoryItemChooseProp = traChooseProp.GetComponent<InventoryItem>();
        itemChooseProp = traChooseProp.GetComponent<Item>();
    }

    private void Update()
    {
        CheckMousePositionItem();
    }
    #endregion

    #region 方法
    /// <summary>
    /// 檢查滑鼠座標上的道具
    /// </summary>
    private void CheckMousePositionItem()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            #region 檢查滑鼠有沒有碰到介面
            // 取得滑鼠座標
            Vector3 posMouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            // 新增事件碰撞座標資訊 並 指定 事件系統
            dataPointer = new PointerEventData(eventSystem);
            // 指定為滑鼠座標
            dataPointer.position = posMouse;

            // 碰撞清單
            List<RaycastResult> result = new List<RaycastResult>();

            // 圖形.射線碰撞(碰撞座標資訊，碰撞清單)
            graphic.Raycast(dataPointer, result);
            #endregion

            #region 點擊後並且有介面就顯示選取中的道具
            // 如果 碰撞清單數量 > 零
            if (result.Count > 0)
            {
                Item item = result[0].gameObject.GetComponent<Item>();

                #region 判定是否為合成區域的素材，以名稱來判定是否包含素材兩個字
                if (item.gameObject.name.Contains("素材"))
                {
                    UpdateMergeItem(inventoryItemChooseProp, item.GetComponent<InventoryItem>(), itemChooseProp, item);
                    return;
                }
                #endregion

                // 判斷 如果 道具有 Item 並且 不是空值道具欄在做切換道具處理
                if (!chooseItem && item && item.propType != PropType.None)
                {
                    chooseItem = true;                          // 已經選到道具
                    traChooseProp.gameObject.SetActive(true);
                    // 更新道具(點到的道具，選取中的道具，點到的道具 item，選取中的道具 item)
                    // 將第一個選到的道具 更新到 選取中的道具 上
                    UpdateItem(item.GetComponent<InventoryItem>(), inventoryItemChooseProp, item, itemChooseProp);
                }
                else if (item && chooseItem)
                {
                    chooseItem = false;                          // 已經選到道具
                    traChooseProp.gameObject.SetActive(false);
                    UpdateItem(inventoryItemChooseProp, item.GetComponent<InventoryItem>(), itemChooseProp, item);
                }
            }
            #endregion
        }

        #region 選到道具後：選取中的道具跟著滑鼠移動
        if (chooseItem)
        {
            dataPointer.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            traChooseProp.position = dataPointer.position;
        }
        #endregion
    }

    /// <summary>
    /// 是否選取合成結果道具
    /// </summary>
    private bool chooseResult;

    /// <summary>
    /// 更新道具資訊：圖片、數量、道具物件、類型
    /// </summary>
    private void UpdateItem(InventoryItem chooseInventory, InventoryItem updateInvemtoyr, Item chooseItem, Item updateItem)
    {
        updateInvemtoyr.imgProp.sprite = chooseInventory.imgProp.sprite;        // 更新 道具 圖片
        updateInvemtoyr.textProp.text = chooseInventory.textProp.text;          // 更新 道具 數量
        updateInvemtoyr.imgProp.enabled = true;                                 // 啟動 更新 道具 圖片
        chooseInventory.imgProp.enabled = false;                                // 關閉 選到的道具圖片
        chooseInventory.imgProp.sprite = null;                                  // 刪除 選到的道具圖片
        chooseInventory.textProp.text = "";                                     // 刪除 選到的道具數量

        // 更新道具資訊 Item 將選中道具的資訊更新 並刪除 原本的
        updateItem.count = chooseItem.count;
        updateItem.goItem = chooseItem.goItem;
        updateItem.propType = chooseItem.propType;
        chooseItem.count = 0;
        chooseItem.goItem = null;
        chooseItem.propType = PropType.None;

        // 通知裝備管理器
        EquipmentManager.instance.ShowEquipment();

        // 判斷 第一次點到的為合成結果，設定為點選到合成結果
        if (chooseInventory.gameObject.name == "合成結果")
        {
            chooseResult = true;
        }
        // 判斷 第二次點到 不是合成結果 就 把素材清除
        else if (chooseResult && updateInvemtoyr.gameObject.name != "合成結果")
        {
            chooseResult = false;
            ClearItemMerge();
        }
    }

    /// <summary>
    /// 清空素材 1 ~ 4 的資料
    /// </summary>
    private void ClearItemMerge()
    {
        for (int i = 0; i < itemMerge.Length; i++)
        {
            InventoryItem inventory = itemMerge[i].GetComponent<InventoryItem>();
            inventory.imgProp.sprite = null;
            inventory.imgProp.enabled = false;
            inventory.textProp.text = "";
            itemMerge[i].count = 0;
            itemMerge[i].goItem = null;
            itemMerge[i].propType = PropType.None;
        }
    }

    /// <summary>
    /// 更新合成區域的道具，合成區放進一個，並更新手上的數量減一
    /// </summary>
    private void UpdateMergeItem(InventoryItem chooseInventory, InventoryItem updateInvemtoyr, Item chooseItem, Item updateItem)
    {
        if (chooseItem.count > 0)
        {
            updateInvemtoyr.imgProp.sprite = chooseInventory.imgProp.sprite;        // 更新 道具 圖片
            updateInvemtoyr.textProp.text = "1";                                    // 素材區放入一個道具
            updateInvemtoyr.imgProp.enabled = true;                                 // 啟動 更新 道具 圖片

            int chooseCount = chooseItem.count - 1;

            chooseInventory.textProp.text = chooseCount.ToString();                 // 更新 選到的道具數量 減一
            updateItem.count = 1;
            updateItem.goItem = chooseItem.goItem;
            updateItem.propType = chooseItem.propType;
            chooseItem.count = chooseCount;
        }
        if (chooseItem.count == 0)
        {
            chooseInventory.imgProp.sprite = null;
            chooseInventory.imgProp.enabled = false;
            chooseInventory.textProp.text = "";
            chooseItem.count = 0;
            chooseItem.goItem = null;
            chooseItem.propType = PropType.None;
            traChooseProp.gameObject.SetActive(false);
            this.chooseItem = false;                        // 手上沒有道具 - this 指此類別，可以存取類別定義的欄位或方法
        }

        CheckMergeData();
        EquipmentManager.instance.ShowEquipment();
    }

    [Header("合成結果資料")]
    public InventoryItem resultInventoryItem;
    public Item resultItem;

    /// <summary>
    /// 每次放完素材後處理
    /// 檢查目前合成素材在合成表內是否有相同的資料
    /// </summary>
    private void CheckMergeData()
    {
        // 將合成區素材存放到陣列內
        for (int i = 0; i < itemMerge.Length; i++)
        {
            goMergeCurrent[i] = itemMerge[i].goItem;
        }

        // var 無類型，可存放任何類型資料 - 不嚴謹的類型，容易導致錯誤
        // 合成表.所有合成資料.尋找(資料 => 連續檢查 - 檢查兩筆陣列是否相等
        var result = mergeTable.allMergeData.Where(x => Enumerable.SequenceEqual(x.goMerge, goMergeCurrent));

        // 遍尋迴圈(類型 取得資料 在指定陣列集合內 陣列集合名稱)
        // 此處的 取得資料 會抓取 在 指定陣列集合內的 每一筆資料
        foreach (var mergeResult in result)
        {
            print("匹配的合成結果：" + mergeResult.goMergeResult.name);

            Prop mergeResultProp = mergeResult.goMergeResult.GetComponent<Prop>();

            resultInventoryItem.hasProp = true;
            resultInventoryItem.imgProp.sprite = mergeResultProp.sprProp;
            resultInventoryItem.imgProp.enabled = true;
            resultInventoryItem.textProp.text = "1";

            resultItem.goItem = mergeResultProp.goProp;
            resultItem.count = 1;
            resultItem.propType = mergeResultProp.propType;
        }
    }
    #endregion
}
