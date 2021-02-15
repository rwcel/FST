using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;

public class LobbyCharacter : MonoBehaviour
{
    #region def
    private enum LobbyAnimations
    {
        Idle, Attack, Skill, Ready, 
        Run = 10,
    }

    // 애니메이션
    [Header("Animation")]
    [SerializeField] Transform      m_ModelParent;

    private GameObject              m_Model;
    private Animator                m_Animator;
    private SkeletonMecanim         m_SkeletonMecanim;
    private string                  m_CurAnimName;
    private bool                    m_IsActing;                 // 애니메이션 특별 모션(Walk제외) 작동중인가
    private float                   m_MoveMinX;
    private float                   m_MoveMaxX;


    private const int               _Anim_Random_Num = 4;       // 로비에서 행동할 때 
    private const int               _Anim_Run_Num = 10;         // InGame 진입시


    // 하트
    [Header("Heart")]
    [SerializeField] Transform      m_HeartTr;

    private float                   m_RandomPosX = 20f;
    private float                   m_RandomPosY = 20f;
    private float                   m_MinAnimDuration = 0.6f;
    private float                   m_MaxAnimDuration = 1.5f;

    private string                  m_CharImageName;

    // **Data 필요
    private int                     m_Amount = 3;               // 얻을 개수
    private float                   m_HeartCooltime = 180f;     // 3분

    #endregion


    private void Start() {
        int slotNum = int.Parse(gameObject.name.Substring(name.Length - 2, 1));
        int heroNum = PlayerMgr.Instance.GetHeroSlot(slotNum);
        LobbyUIMgr lobbyUIMgr = LobbyUIMgr.Instance;

        if (heroNum == -1) {
            gameObject.SetActive(false);
            return;
        }

        m_Model = ResourceMgr.Instance.GetPrefab("LobbyHero", slotNum, m_ModelParent);
        m_Animator = m_Model.GetComponent<Animator>();
        m_SkeletonMecanim = m_Model.GetComponent<SkeletonMecanim>();

        lobbyUIMgr._OnMoveInGame += () => { Click_InGame(); };

        // -450~-350 -200~-100 50~150 300~400
        m_MoveMinX = lobbyUIMgr._WindowCharMinX + (slotNum * lobbyUIMgr._BetweenCharDistance);
        m_MoveMaxX = m_MoveMinX + lobbyUIMgr._CharRandomValue;

        float randomX = Random.Range(m_MoveMinX, m_MoveMaxX);
        float randomY = Random.Range(-lobbyUIMgr._WindowCharLimitY, lobbyUIMgr._WindowCharLimitY);
        
        transform.localPosition = new Vector3(randomX, 250 + randomY, 0);
        if (Random.Range(0f,1f) >= 0.5f) {
            m_SkeletonMecanim.skeleton.ScaleX = -1;
        }

        HeartSetting();
    }


    private IEnumerator FreeAction() {
        m_IsActing = Random.Range(0f,1f) >= 0.5f;

        while (true) {
            m_Animator.SetBool("Walk", m_IsActing);

            if (m_IsActing) {    // 해제 -> Walk
                yield return StartCoroutine("RandomMove");
            }
            else {
                //    m_CurAnimName = ((LobbyAnimations)Random.Range(0, _Anim_Random_Num)).ToString();
                m_IsActing = true;
            }

            yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
        }
    }

    /// <summary>
    /// Walk 상태에서 랜덤 움직임
    /// 수정 필요
    /// </summary>
    private IEnumerator RandomMove() {
        float dest = Random.Range(m_MoveMinX, m_MoveMaxX);

        int normalDir = transform.localPosition.x > dest ? -1 : 1;
        m_SkeletonMecanim.skeleton.ScaleX = normalDir;

        while (transform.localPosition.x - dest <= 1f) {
            transform.position += Vector3.right * normalDir * Time.deltaTime * 30f;
            yield return null;
        }
        transform.localPosition = new Vector2(dest, transform.localPosition.y);

        m_IsActing = false;
        m_Animator.SetBool("Walk", m_IsActing);
    }

    // 재접속 시간에 따른 설정
    private void HeartSetting() {
        m_CharImageName = m_SkeletonMecanim.skeletonDataAsset.name;
        if (PrefsMgr.Instance.GetLobbyCharHeartTime(m_CharImageName) == 0f) {
            PrefsMgr.Instance.SetLobbyCharHeartTime(m_CharImageName);
        }
        m_HeartTr.gameObject.SetActive(SystemMgr.Instance.GetTimestamp() -
                                        PrefsMgr.Instance.GetLobbyCharHeartTime(m_CharImageName) >= m_HeartCooltime);
    }

    public void Click_Heart()
    {
        Vector3 targetPos = MyInfo.Instance.RebirthStoneImageTr.position;

        for (int i = 0; i < m_Amount; i++)
        {
            GameObject item = ItemPool.Instance.DeQueue(PaymentType.RebirthStone.ToString());
            // null 처리
            item.transform.position = m_HeartTr.position +
                    new Vector3(Random.Range(-m_RandomPosX, m_RandomPosX), Random.Range(-m_RandomPosY, m_RandomPosY));
            item.transform.SetParent(transform);

            item.transform.DOMove(targetPos, Random.Range(m_MinAnimDuration, m_MaxAnimDuration)).SetEase(Ease.InBack).
                OnComplete(() => {
                ItemPool.Instance.EnQueue(item);
                PlayerMgr.Instance.AddResource(PaymentType.RebirthStone, 1);
                });
        }
        PrefsMgr.Instance.SetLobbyCharHeartTime(m_CharImageName);
        m_HeartTr.gameObject.SetActive(false);
    }



    /// <summary>
    /// 인게임 들어가면 현재 Anim 멈추고 Run으로 이동해야함
    /// </summary>
    public void Click_InGame() {
        StartCoroutine("InGameMove");

        m_HeartTr.gameObject.SetActive(false);
        m_SkeletonMecanim.skeleton.ScaleX = 1;

        m_Animator.SetBool(LobbyAnimations.Run.ToString(), true);
    }


    private IEnumerator InGameMove() {
        StopCoroutine("FreeAction");
        while(true) {
            transform.localPosition += Vector3.right * 200f * Time.deltaTime;
            yield return null;
        }
    }

}
