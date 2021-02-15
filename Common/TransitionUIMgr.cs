using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TransitionUIMgr : SingleTon<TransitionUIMgr>
{
    [SerializeField] GameObject         m_SimpleMsgObj;
    [SerializeField] GameObject         m_FadeObj;
    [SerializeField] GameObject         m_BlurObj;
    [SerializeField] GameObject         m_TransitionBoxObj;

    private Image                       m_FadeImg;
    private RectTransform               m_TransitionBoxRectTr;

    private Color                       m_FadeImgColor = new Color(0, 0, 0, 0.6f);


    private void Start() {
        m_FadeImg = m_FadeObj.GetComponent<Image>();
        m_TransitionBoxRectTr = m_TransitionBoxObj.GetComponent<RectTransform>();
    }


    #region OnOff

    public void OnOffSimpleMsg(bool isActive) {
        m_SimpleMsgObj.SetActive(isActive);
    }

    public void OnOffFade(bool isActive) {
        m_FadeObj.SetActive(isActive);
    }

    public void OnOffBlur(bool isActive) {
        m_BlurObj.SetActive(isActive);
    }

    #endregion OnOff

    #region Fade

    public void FadeIn(System.Action onFinished, float time, bool moveScene = false, Color? color = null) {
        m_FadeImg.color = color ?? m_FadeImgColor;
        m_FadeObj.SetActive(true);
        m_FadeImg.DOFade(0f, time).SetEase(Ease.OutQuad).OnComplete(() => {
            onFinished();
            if (!moveScene) {
                m_FadeObj.SetActive(false);
                m_FadeImg.color = m_FadeImgColor;
            }
            else {
                SystemMgr.Instance._OnSceneChangeOnce += () => {
                    m_FadeImg.DOFade(1f, time).SetEase(Ease.OutQuad).OnComplete(() => {
                        m_FadeObj.SetActive(false);
                        m_FadeImg.color = m_FadeImgColor;
                    });
                };
            }
        });
    }

    public void FadeOut(System.Action onFinished, float time, bool moveScene = false, Color? color = null) {
        m_FadeImg.color = color ?? m_FadeImgColor;
        m_FadeObj.SetActive(true);
        m_FadeImg.DOFade(1f, time).SetEase(Ease.OutQuad).OnComplete(() => {
            onFinished();
            if (!moveScene) {
                m_FadeObj.SetActive(false);
                m_FadeImg.color = m_FadeImgColor;
            }
            else {
                SystemMgr.Instance._OnSceneChangeOnce += () => {
                    m_FadeImg.DOFade(0f, time).SetEase(Ease.OutQuad).OnComplete(() => {
                        m_FadeObj.SetActive(false);
                        m_FadeImg.color = m_FadeImgColor;
                    });
                };
            }
        });
    }

    #endregion Fade

    #region Move

    /// <summary>
    /// 트랜지션 박스 이동 RectTransform의 position으로
    /// </summary>
    /// <param name="dir">움직일 방향</param>
    /// <param name="moveScene">씬 이동을 하는지 -> 씬 이동 끝나면 함수 실행</param>
    public void TransitionBox(System.Action onFinished, Vector2 dir, float time, bool moveScene = false) {
        Vector2 dest = Vector2.zero;
        if(dir == Vector2.left) {
            dest = new Vector2(-1280f, 0f);
        }
        else if (dir == Vector2.right) {
            dest = new Vector2(1280f, 0f);
        }
        else if (dir == Vector2.up) {
            dest = new Vector2(0f, 720f);
        }
        else if (dir == Vector2.down) {
            dest = new Vector2(0f, -720f);
        }

        m_TransitionBoxRectTr.anchoredPosition = dest * -1f;

        m_TransitionBoxObj.SetActive(true);
        m_TransitionBoxRectTr.DOLocalMove(Vector2.zero, time * 0.5f).SetEase(Ease.OutSine).OnComplete(() => {
            onFinished();
            if(!moveScene) {
                m_TransitionBoxObj.transform.DOLocalMove(dest, time * 0.5f).SetEase(Ease.InCubic).OnComplete(() => {
                    m_TransitionBoxObj.SetActive(false);
                });
            }
            else {
                SystemMgr.Instance._OnSceneChangeOnce += () => {
                    m_TransitionBoxObj.transform.DOLocalMove(dest, time * 0.5f).SetEase(Ease.InCubic).OnComplete(() => {
                        m_TransitionBoxObj.SetActive(false);
                    });
                };
            }
        });
    }

    #endregion Move

}
