using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 내 정보 : 좌상단
/// **Dont Destroy Object로 해야할 가능성도 있습니다. (보여지는 Scene 확인 필요)
/// </summary>
public class MyInfo : SingleTon<MyInfo>
{
    #region type def

    private enum GoodsType
    {
        Dia, Rebirth, Coin
    }

    [System.Serializable]
    private struct Goods
    {
        public GameObject   Obj;
        [HideInInspector]
        public double       Value;
        public Text         ValueText;
    }

    private enum Texts
    {
        Level,
        NickName,
        Exp,
        RebirthCount,
    }

    #endregion


    #region serialized fields

    [SerializeField] private Goods[]    m_Goods;
    [SerializeField] private Text[]     m_Texts = null;
    [SerializeField] private Image      m_CharImage = null;
    [SerializeField] private Slider     m_ExpSlider = null;

    [SerializeField] Transform RebirthStoneTr;

    #endregion


    #region properties

    public Transform RebirthStoneImageTr { get {return m_Goods[(int)GoodsType.Rebirth].Obj.transform.GetChild(0); } }

    #endregion

    private void OnEnable() {
        if (PlayerMgr.Instance == null) {       // 로비부터라 문제 없을것같지만 혹시 몰라 실행
            return;
        }

        UpdateData();
        UpdateDia();
        UpdateRebirthStone();
        UpdateProfile();
    }

    #region public functions

    public void UpdateData() {
        PlayerMgr playerMgr = PlayerMgr.Instance;
        m_Texts[(int)Texts.NickName].text = playerMgr.GetName();
        m_Texts[(int)Texts.Level].text = (playerMgr.GetLv() + 1).ToString();     // **1렙부터 시작

        m_ExpSlider.value = (float)playerMgr.GetExp() / CSVMgr.Instance.GetPlayerNeedExp();
        m_Texts[(int)Texts.Exp].text = string.Format("{0}%", Mathf.FloorToInt(m_ExpSlider.value * 100));
        m_Texts[(int)Texts.RebirthCount].text = string.Format("+{0}", PlayerMgr.Instance.GetResurrection());
    }

    public void UpdateDia() {
        m_Goods[(int)GoodsType.Dia].Value = PlayerMgr.Instance.GetResource(PaymentType.Diamond);
        m_Goods[(int)GoodsType.Dia].ValueText.text = string.Format("{0:n0}", m_Goods[(int)GoodsType.Dia].Value);
    }

    public void UpdateRebirthStone() {
        m_Goods[(int)GoodsType.Rebirth].Value = PlayerMgr.Instance.GetResource(PaymentType.RebirthStone);
        m_Goods[(int)GoodsType.Rebirth].ValueText.text = string.Format("{0:n0}", m_Goods[(int)GoodsType.Rebirth].Value);
    }

    public void UpdateProfile() {
        m_CharImage.sprite = ResourceMgr.Instance.GetSprite("HeroFace", (PlayerMgr.Instance.GetHeroSlot(0) % 4));
    }
    #endregion


    #region Interaction

    public void Click_Profile() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        // **Lobby에서만..?
        //ProfileMgr.Instance.Show();
    }

    public void Click_InfoDiamond() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        ToolTipMgr.Instance.Show(m_Goods[(int)GoodsType.Dia].Obj.transform, 0, (int)PaymentType.FreeCash);
    }

    public void Click_InfoRebirth() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        ToolTipMgr.Instance.Show(m_Goods[(int)GoodsType.Rebirth].Obj.transform, 0, (int)PaymentType.RebirthStone);
    }

    public void Click_AddDiamond() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        StoreMgr.Instance.Show(StoreType.CashShop, (int)CashTabs.Dia);
    }

    public void Click_AddRebirth() {
        ResourceMgr.Instance.PlaySound("FX", 0);

        // StoreMgr.Instance.Show(StoreType.CashShop, (int)CashTabs.Dia);
    }

    #endregion

}
