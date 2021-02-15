using UnityEngine;
using UnityEngine.UI;

public class InventoryMgr : VisibleUIMgr<InventoryMgr>
{
    [Header("Tab")]
    [SerializeField] GameObject         m_Equip;
    [SerializeField] GameObject         m_Item;

    [Header("UI")]
    [SerializeField] Text               m_Title;
    [SerializeField] Image              m_ItemImage;
    [SerializeField] Text               m_ItemName;
    [SerializeField] Text               m_Stat;
    [SerializeField] Text               m_Desc;
    [SerializeField] Text               m_SubTitle;
    [SerializeField] Image              m_Payment;
    [SerializeField] Text               m_PaymentValue;
    [SerializeField] Button             m_LeftButton;
    [SerializeField] Button             m_RightButton;
    [SerializeField] Text               m_LeftBtnText;
    [SerializeField] Text               m_RightBtnText;

    [Header("Object")]
    [SerializeField] GameObject         m_InfoObj;
    [SerializeField] GameObject         m_SellObj;
    [SerializeField] Transform          m_StarParent;

    //[SerializeField] GameObject         m_NewObj;

    [Header("OtherScripts")]
    [SerializeField] SlideTab           m_SlideTab;

    [HideInInspector]
    public InventoryItemType            _SelectItemType;

    private int                         m_SelectedItem = -1;
    private double                      m_Price = 0;

    private PlayerMgr                   m_PlayerMgr;
    private CSVMgr                      m_CSVMgr;

    // Artifact 전용
    private int                         m_A_Awaken;
    private int                         m_A_Enchant;
    private int                         m_A_Type;
    private int                         m_EnchantPriceValue;
    private int                         m_EnchantAddPriceValue;
    private int                         m_DecompositionReward;

    protected override void SetData() {
        m_PlayerMgr = PlayerMgr.Instance;
        m_CSVMgr = CSVMgr.Instance;
    }

    public override void Show() {
        base.Show();

        m_SlideTab.SetButton(0, "EQUIPMENT", ShowEquipment, true);
        m_SlideTab.SetButton(1, "ITEM", ShowItem);
        m_SlideTab.SetButton(2, "PIECES", ShowPiece);

        _OnClose += () => {
            StopCoroutine("GetItem");
            CheckInventory();
            m_PlayerMgr.ResetArtifactAlarm();
            m_PlayerMgr.ResetResourceAlarm();
        };
    }

    public override void SetScreen() {
        m_Title.text = m_CSVMgr.GetLanguage(559);
    }

    // 적용 후 다시 세팅하는 용도 
    public void ReDoEquip() {
        m_InfoObj.SetActive(false);
        m_PaymentValue.text = string.Format("{0:n0}", m_PlayerMgr.GetResource(PaymentType.RebirthStone));

        m_Equip.SetActive(false);
        m_Equip.SetActive(true);
    }

    public void ReDoPiece() {
        m_InfoObj.SetActive(false);
        m_PaymentValue.text = string.Format("{0:n0}", m_PlayerMgr.GetResource(PaymentType.Crystal));

        m_Item.SetActive(false);
        m_Item.SetActive(true);
    }

    private void ShowEquipment() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        m_InfoObj.SetActive(false);

        _SelectItemType = InventoryItemType.Artifact;
        m_Equip.SetActive(true);
        m_SellObj.SetActive(true);
        m_Item.SetActive(false);
        m_StarParent.gameObject.SetActive(true);

        m_SubTitle.text = "Artifacts List";
        m_Payment.sprite = ResourceMgr.Instance.GetSprite("Item", (int)PaymentType.RebirthStone);
        m_PaymentValue.text = string.Format("{0:n0}", m_PlayerMgr.GetResource(PaymentType.RebirthStone));

        m_LeftButton.gameObject.SetActive(true);
        m_RightButton.gameObject.SetActive(true);
        m_LeftBtnText.text = m_CSVMgr.GetLanguage(576);
        m_RightBtnText.text = m_CSVMgr.GetLanguage(1791);

    }

    private void ShowItem() {
        _SelectItemType = InventoryItemType.Normal;

        m_SellObj.SetActive(false);

        m_SubTitle.text = "Item List";
        ItemSetting();
    }

    private void ShowPiece() {
        _SelectItemType = InventoryItemType.Piece;

        m_SellObj.SetActive(true);

        m_SubTitle.text = "Piece List";
        ItemSetting();
    }

    private void ItemSetting() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        m_InfoObj.SetActive(false);
        m_Equip.SetActive(false);
        ReDoPiece();                // 같은것을 공유해서 끄고켜기

        m_StarParent.gameObject.SetActive(false);

        m_Payment.sprite = ResourceMgr.Instance.GetSprite("Item", (int)PaymentType.Crystal);
        m_PaymentValue.text = string.Format("{0:n0}", m_PlayerMgr.GetResource(PaymentType.Crystal));

        m_LeftButton.gameObject.SetActive(false);
        m_RightButton.gameObject.SetActive(false);
    }


    public void NewInventory() {
        //_New = true;
    }
    public void CheckInventory() {
        //_New = false;
    }


    public void SelectArtifact(int artifactNum) {
        if(!m_InfoObj.activeSelf) {
            m_InfoObj.SetActive(true);
        }

        m_SelectedItem = artifactNum;
        m_A_Type = PlayerMgr.Instance.GetArtifactType(artifactNum);
        m_A_Enchant = m_PlayerMgr.GetArtifactEnchant(artifactNum);
        m_Price = m_CSVMgr.GetArtifactSellPrice(m_A_Type);

        m_ItemImage.sprite = ResourceMgr.Instance.GetSprite("Artifact", m_A_Type);
        m_ItemName.text = m_PlayerMgr.GetArtifactName(artifactNum);
        m_Stat.text = string.Format("+{0}", m_A_Enchant);
        m_Desc.text = m_PlayerMgr.GetArtifactDescription(artifactNum);

        int awaken = m_PlayerMgr.GetArtifactAwaken(artifactNum);
        for (int i = 0, length = m_StarParent.childCount; i < length; i++) {
            m_StarParent.GetChild(i).gameObject.SetActive(i < awaken);
        }

        bool canUpgrade = m_PlayerMgr.GetArtifactEnchant(artifactNum) < 
                            m_PlayerMgr.GetArtifactMaxEnchant(artifactNum);
        m_LeftButton.interactable = canUpgrade;
        m_RightButton.interactable = !canUpgrade;
    }

    public void Click_Sell()
    {
        if (_SelectItemType == InventoryItemType.Artifact)
        {
            m_EnchantPriceValue = m_CSVMgr.GetArtifactPriceValue(m_A_Type);
            m_EnchantAddPriceValue = m_CSVMgr.GetArtifactAddPriceValue(m_A_Type);
            m_DecompositionReward = (int)m_Price + ((((2 * m_EnchantPriceValue) + ((m_A_Enchant - 1) * 
                                    m_EnchantAddPriceValue)) / 4) * m_A_Enchant);

            TwoButtonMgr.Instance.Show(null, DecompositionArtifact, 3, 2, 772,
                                       string.Format(m_CSVMgr.GetLanguage(m_A_Awaken == 0 ? 1704 : 1705), 
                                       m_DecompositionReward));
        }
        else
        {
            TwoButtonMgr.Instance.Show(() => SellMgr.Instance.Show(m_SelectedItem),
                                       3, 2, 566, TwoButtonPopupType.Sell);
        }

    }

    public void SelectItem(InventoryItemType type, int idx) {
        if (!m_InfoObj.activeSelf) {
            m_InfoObj.SetActive(true);
        }

        _SelectItemType = type;
        m_SelectedItem = idx;

        m_Price = m_CSVMgr.GetItemSellPrice(idx);

        m_ItemImage.sprite = ResourceMgr.Instance.GetSprite("Item", (idx > 13) ? 11 : idx);
        m_ItemName.text = CSVMgr.Instance.GetItemName(idx); ;
        m_Desc.text = CSVMgr.Instance.GetItemDescription(idx); ;

        if (type == InventoryItemType.Piece) {
            m_Stat.text = PlayerMgr.Instance.GetResource(idx).ToString();
        }
        else {
            m_Stat.text = PlayerMgr.Instance.GetResourceDivision(idx).ToString();
        }
    }

    public void SelectItem(InventoryItemType type, int idx, Sprite image, string name, string count, string desc) {
        if (!m_InfoObj.activeSelf) {
            m_InfoObj.SetActive(true);
        }

        _SelectItemType = type;
        m_SelectedItem = idx;

        m_Price = m_CSVMgr.GetItemSellPrice(idx);

        m_ItemImage.sprite = image;
        m_ItemName.text = name;
        m_Stat.text = count + "pcs";
        m_Desc.text = desc;
    }


    // 분해 완료
    public void DecompositionArtifact() {
        m_PlayerMgr.AddResource(PaymentType.RebirthStone, m_DecompositionReward);

        m_PlayerMgr.RemoveArtifact(m_SelectedItem);

        m_SelectedItem = -1;
        ReDoEquip();
    }

    public void SellItem() {
        m_PlayerMgr.AddResource(PaymentType.RebirthStone, m_DecompositionReward);

        m_PlayerMgr.RemoveArtifact(m_SelectedItem);

        m_SelectedItem = -1;
        ReDoPiece();
    }


    //IEnumerator GetItem()
    //{
    //    _ItemType.Clear();
    //    _ItemIDX.Clear();
    //    _ItemAlarm.Clear();

    //    yield return new WaitForEndOfFrame();

    //    if (_SelectTab == Tabs.All || _SelectTab == Tabs.Artifact)
    //    {
    //        for (int i = 0, length = _PlayerMgr.GetArtifactCnt(); i < length; i++)
    //        {
    //            int _A_IDX = _PlayerMgr.GetArtifactIDX(i);
    //            int equip = _PlayerMgr.GetArtifactEquip(_A_IDX);

    //            if (equip == 1 || equip == 0)
    //            {
    //                _ItemType.Add((int)InventoryItemType.Artifact);
    //                _ItemIDX.Add(_A_IDX);
    //                _ItemAlarm.Add(_PlayerMgr.GetArtifactAlarm(i)); 
    //            }
    //        }
    //    }

    //    if (_SelectTab == Tabs.All || _SelectTab == Tabs.Piece)
    //    {
    //        for (int i = 0, length = _CSVMgr.GetItemCnt(); i < length; i++)
    //        {
    //            if (_CSVMgr.GetItemType(i) == (int)InventoryItemType.Piece && _PlayerMgr.GetResource(i) > 0)
    //            {
    //                _ItemType.Add((int)InventoryItemType.Piece);
    //                _ItemIDX.Add(i);
    //                _ItemAlarm.Add(_PlayerMgr.GetResourceAlarm(i));
    //            }
    //        }
    //    }

    //    if (_SelectTab == Tabs.All || _SelectTab == Tabs.Normal)
    //    {
    //        for (int i = 0, length = _CSVMgr.GetItemCnt(); i < length; i++)
    //        {
    //            int itemType = _CSVMgr.GetItemType(i);
    //            if (PlayerMgr.Instance.GetResource(i) > 0 && (itemType == (int)InventoryItemType.Normal || 
    //                                                          itemType == (int)InventoryItemType.Ticket))
    //            {
    //                _ItemType.Add(itemType);
    //                _ItemIDX.Add(i);
    //                _ItemAlarm.Add(_PlayerMgr.GetResourceAlarm(i));
    //            }
    //        }
    //    }

    //    SetScroll();
    //}

    //void SetScroll()
    //{
    //    _DynamicScroll[0].SetCnt(_ItemType.Count);
    //    _DynamicScroll[0].SetList();
    //    _DynamicScroll[0].PositionReset();

    //    if (_ItemType.Count > 0) {
    //        SelectItem(0);
    //        _Obj[(int)Objects.Main].SetActive(true);
    //        _Obj[(int)Objects.Empty].SetActive(false);
    //    }
    //    else {
    //        SelectItem(-1);
    //        _Obj[(int)Objects.Main].SetActive(false);
    //        _Obj[(int)Objects.Empty].SetActive(true);
    //    }
    //}

    //  public void SelectItem(int _Num)
    //  {
    //      // **
    //      if (_OnItemTouch != null && _SelectedItem != _Num) {
    //          _OnItemTouch();
    //          _OnItemTouch = null;
    //      }

    //      _SelectedItem = _Num;

    //      if (_SelectedItem == -1)
    //{
    //          _Obj[(int)Objects.Artifact].SetActive(false);
    //          _Obj[(int)Objects.Item].SetActive(false);
    //          _Obj[(int)Objects.Use].SetActive(false);
    //          _Obj[(int)Objects.Sell].SetActive(false);

    //          _Sprite[(int)Sprites.Item].spriteName = "Empty";
    //          _Sprite[(int)Sprites.Artifact].spriteName = "Empty";
    //          _Sprite[(int)Sprites.Price].spriteName = "Empty";
    //          _Sprite[(int)Sprites.Star].spriteName = "Empty";
    //          _Sprite[(int)Sprites.Grade].spriteName = "Empty";

    //          _Label[(int)Labels.Count].text = "";
    //	_Label[(int)Labels.Name].text = "";
    //	_Label[(int)Labels.Enchant].text = "";
    //          _Label[(int)Labels.Price].text = "";
    //	_Label[(int)Labels.Info].text = "";
    //}
    //else
    //{
    //          _SelectItemType = (InventoryItemType)_ItemType[_SelectedItem];

    //          if (_SelectItemType == InventoryItemType.Artifact) {
    //              _A_Type = _PlayerMgr.GetArtifactType(_ItemIDX[_SelectedItem]);
    //              _A_Awaken = _PlayerMgr.GetArtifactAwaken(_ItemIDX[_SelectedItem]);
    //              _A_Enchant = _PlayerMgr.GetArtifactEnchant(_ItemIDX[_SelectedItem]);
    //              _Price = _CSVMgr.GetArtifactSellPrice(_A_Type);
    //              _IDX = _PlayerMgr.GetArtifactSellIDX(_ItemIDX[_SelectedItem]);

    //              _Obj[(int)Objects.Artifact].SetActive(true);
    //              _Obj[(int)Objects.Item].SetActive(false);
    //              _Obj[(int)Objects.Use].SetActive(true);
    //              _Obj[(int)Objects.Sell].SetActive(true);

    //              _Label[(int)Labels.Name].text = _PlayerMgr.GetArtifactName(_ItemIDX[_SelectedItem]); // 아이템 이름
    //              _Label[(int)Labels.Info].text = _PlayerMgr.GetArtifactDescription(_ItemIDX[_SelectedItem]); // 아이템 설명
    //              _Label[(int)Labels.Enchant].text = string.Format("+{0}", _A_Enchant);
    //              _Label[(int)Labels.Count].text = "";
    //              _Label[(int)Labels.Use].text = _PlayerMgr.GetArtifactEquip(_ItemIDX[_SelectedItem]) > 0 ?
    //                                             _CSVMgr.GetLanguage(574) : _CSVMgr.GetLanguage(567);
    //              _Label[(int)Labels.Sell].text = _CSVMgr.GetLanguage(1703);

    //              _Sprite[(int)Sprites.Artifact].spriteName = "Artifact" + _A_Type;
    //              _Sprite[(int)Sprites.Grade].spriteName = string.Format("Grade_0{0}", _PlayerMgr.GetArtifactGrade(_ItemIDX[_SelectedItem]));
    //              int starNum = _A_Awaken;
    //              int starWidth = starNum <= 3 ? starNum * 21 : starNum * 18;
    //              _Sprite[(int)Sprites.Star].spriteName = string.Format("UI_Star{0}", starNum);
    //              _Sprite[(int)Sprites.Star].SetDimensions(starWidth, 21);
    //          }

    //          else if (_SelectItemType == InventoryItemType.Piece) {
    //              _Price = _CSVMgr.GetItemSellPrice(_ItemIDX[_SelectedItem]);
    //              _IDX = _CSVMgr.GetItemSellIDX(_ItemIDX[_SelectedItem]);

    //              _Obj[(int)Objects.Artifact].SetActive(false);
    //              _Obj[(int)Objects.Item].SetActive(true);
    //              _Obj[(int)Objects.Piece].SetActive(true);
    //              _Obj[(int)Objects.Use].SetActive(false);
    //              _Obj[(int)Objects.Sell].SetActive(true);

    //              _Label[(int)Labels.Name].text = _CSVMgr.GetItemName(_ItemIDX[_SelectedItem]);
    //              _Label[(int)Labels.Info].text = _CSVMgr.GetItemDescription(_ItemIDX[_SelectedItem]);
    //              //_Label[(int)Labels.Count].text = string.Format("[FCF06DFF]{0} [B28021FF]{1}", 
    //              _Label[(int)Labels.Count].text = string.Format("{0} {1}",
    //                                               _PlayerMgr.GetResource(_ItemIDX[_SelectedItem]), _CSVMgr.GetLanguage(560));
    //              _Label[(int)Labels.Sell].text = _CSVMgr.GetLanguage(566);

    //              _Sprite[(int)Sprites.Item].spriteName = "Item" + _ItemIDX[_SelectedItem];
    //          }

    //          else if (_SelectItemType == InventoryItemType.Normal) {
    //              _Price = _CSVMgr.GetItemSellPrice(_ItemIDX[_SelectedItem]);
    //              _IDX = _CSVMgr.GetItemSellIDX(_ItemIDX[_SelectedItem]);

    //              _Obj[(int)Objects.Artifact].SetActive(false);
    //              _Obj[(int)Objects.Item].SetActive(true);
    //              _Obj[(int)Objects.Piece].SetActive(false);
    //              _Obj[(int)Objects.Use].SetActive(false);
    //              _Obj[(int)Objects.Sell].SetActive(true);

    //              _Label[(int)Labels.Name].text = _CSVMgr.GetItemName(_ItemIDX[_SelectedItem]);
    //              if (_ItemIDX[_SelectedItem] == (int)PaymentType.Medal) {
    //                  int _Grade = _PlayerMgr.GetTempleGrade();
    //                  int _MedalCnt = _CSVMgr.GetTempleStep(_PlayerMgr.GetTemple(), _Grade);

    //                  _Label[(int)Labels.Info].text = string.Format(_CSVMgr.GetItemDescription(_ItemIDX[_SelectedItem]), _MedalCnt, _Grade + 1);
    //              }
    //              else {
    //                  _Label[(int)Labels.Info].text = _CSVMgr.GetItemDescription(_ItemIDX[_SelectedItem]);
    //              }
    //              //_Label[(int)Labels.Count].text = string.Format("[FCF06DFF]{0} [B28021FF]{1}",
    //              _Label[(int)Labels.Count].text = string.Format("{0} {1}",
    //                                              _PlayerMgr.GetResource(_ItemIDX[_SelectedItem]), _CSVMgr.GetLanguage(560));

    //              _Label[(int)Labels.Sell].text = _CSVMgr.GetLanguage(566);

    //              _Sprite[(int)Sprites.Item].spriteName = "Item" + _ItemIDX[_SelectedItem];
    //          }

    //          else if (_SelectItemType == InventoryItemType.Ticket) {
    //              _Price = _CSVMgr.GetItemSellPrice(_ItemIDX[_SelectedItem]);
    //              _IDX = _CSVMgr.GetItemSellIDX(_ItemIDX[_SelectedItem]);

    //              _Obj[(int)Objects.Artifact].SetActive(false);
    //              _Obj[(int)Objects.Item].SetActive(true);
    //              _Obj[(int)Objects.Piece].SetActive(false);
    //              _Obj[(int)Objects.Use].SetActive(true);
    //              _Obj[(int)Objects.Sell].SetActive(false);

    //              _Label[(int)Labels.Name].text = _CSVMgr.GetItemName(_ItemIDX[_SelectedItem]);
    //              _Label[(int)Labels.Info].text = _CSVMgr.GetItemDescription(_ItemIDX[_SelectedItem]);
    //              //_Label[(int)Labels.Count].text = string.Format("[FCF06DFF]{0} [B28021FF]{1}",
    //              _Label[(int)Labels.Count].text = string.Format("{0} {1}", 
    //                                              _PlayerMgr.GetResourceDivision(_ItemIDX[_SelectedItem]), _CSVMgr.GetLanguage(560));
    //              _Label[(int)Labels.Use].text = CSVMgr.Instance.GetLanguage(1767);
    //              _Sprite[(int)Sprites.Item].spriteName = "Item" + _ItemIDX[_SelectedItem];
    //          }


    //          if(_IDX == -1) {
    //              _Label[(int)Labels.Price].text = "";
    //              _Sprite[(int)Sprites.Price].spriteName = "Empty";
    //          }
    //          else if( _IDX == (int)PaymentType.Gold) {
    //              _Label[(int)Labels.Price].text = SystemMgr.Instance.GetDoubleString(_Price);
    //              _Sprite[(int)Sprites.Price].spriteName = string.Format("Item{0}", _IDX);
    //          }
    //          else {
    //              _Label[(int)Labels.Price].text = _Price.ToString();
    //              _Sprite[(int)Sprites.Price].spriteName = string.Format("Item{0}", _IDX);
    //          }

    //          if(_IDX ==  -1) {
    //              _Label[(int)Labels.Payment].text = "";
    //          }
    //          else if(_IDX == (int)PaymentType.Gold) {
    //              _Payment = PlayerMgr.Instance.GetResource(_IDX);
    //              _Label[(int)Labels.Payment].text = SystemMgr.Instance.GetDoubleString(_Payment);
    //          }
    //          else { 
    //              _Payment = PlayerMgr.Instance.GetResource(_IDX);
    //              _Label[(int)Labels.Payment].text = string.Format("{0:n0}", _Payment);
    //          }
    //          _Sprite[(int)Sprites.Payment].spriteName = _Sprite[(int)Sprites.Price].spriteName;

    //      }
    //  }

    //public void Click_Use()
    //{

    //    if (m_SelectItemType == InventoryItemType.Ticket) {
    //        switch (_ItemIDX[_SelectedItem]) {
    //            case 8:
    //                QuickBossMgr.Instance.Show();
    //                break;
    //            case 0:
    //            case 1:
    //            case 11:
    //                Click_Close();
    //                StoreMgr.Instance.Show(StoreType.CashShop, (int)CashTabs.Dia);
    //                break;
    //            case 13:
    //                Click_Close();

    //                SystemMgr.Instance.MoveScene("Temple");
    //                break;
    //        }
    //    }
    //}
}
