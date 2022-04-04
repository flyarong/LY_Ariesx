using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class FteRect {
        public Vector2 position;
        public Rect rect;

        public FteRect(Vector2 position, Rect rect) {
            this.position = position;
            this.rect = rect;
        }
    }

    public enum FteChatRole {
        None,
        Demon,
        Elf,
        Human,
        Dwarf
    }

    public class FteView : BaseView {
        private static FteView self;
        private FteViewPreference viewPref;
        private Image imgOther;
        private GameObject dragon;
        private GameObject demon;
        private GameObject elf;
        public Camera fteCamera;
        private int chatDirectin;
        private const float stepInterval = 1f;
        private const float txtInterval = 0.03f;
        /*************/

        UnityEvent onAreaClick = new UnityEvent();
        UnityAction MaskCallBack = null;
        UnityEvent onAreaMove = new UnityEvent();
        private float chatOffset = 0;
        //private int demonCount = 0;
        private bool isNewChat = true;

        void Awake() {
            self = this;
            this.ui = GameObject.Find("UIFte");
            this.viewPref = this.ui.transform.GetComponent<FteViewPreference>();
            this.viewPref.canvase.worldCamera =
            this.fteCamera =
                GameObject.FindGameObjectWithTag("FteCamera").GetComponent<Camera>();
            this.viewPref.canvase.planeDistance = 1;
            this.viewPref.canvaseScaler.matchWidthOrHeight = UIManager.AdaptiveParam;
            UpdateManager.Regist(UpdateInfo.FteView, this.UpdateAction);
        }

        private void UpdateAction() {
            this.onAreaMove.InvokeSafe();
        }

        public void ShowFteMask() {
            this.viewPref.background.gameObject.SetActiveSafe(true);
        }

        public void EndFte() {
            this.HideFteMask();
            this.HideArrow();
            AnimationManager.Stop(this.viewPref.arrowImg);
            this.viewPref.pnlChat.gameObject.SetActiveSafe(false);
            this.viewPref.pnlDark.gameObject.SetActiveSafe(false);
            this.onAreaClick.InvokeSafe();
            this.onAreaClick.RemoveAllListeners();
            this.ClearClickable();
            if (this.imgOther != null) {
                this.imgOther.gameObject.SetActiveSafe(false);
            }
        }

        public void SetImageOther(FteChatRole role) {
            if (imgOther != null) {
                this.imgOther = this.viewPref.imgSelf;
                this.imgOther.gameObject.SetActiveSafe(true);
            }
        }

        public void HideFteMask() {
            this.viewPref.background.gameObject.SetActiveSafe(false);
        }

        // To do: move after show.
        public void ShowComic(int step) {
            this.viewPref.pnlComic.gameObject.SetActiveSafe(true);
            this.viewPref.clickComic.onClick.RemoveAllListeners();
            GameObject comic = this.viewPref.pnlComic.Find("Images").Find("StepComic" + step).gameObject;
            Transform pnlText = comic.transform.Find("PnlText");
            pnlText.gameObject.SetActiveSafe(false);
            comic.gameObject.SetActiveSafe(true);
            AnimationManager.Animate(
                comic, "Move", isOffset: false,
                finishCallback: () => {
                    pnlText.gameObject.SetActiveSafe(true);
                    this.viewPref.clickComic.gameObject.SetActiveSafe(true);
                    this.ShowComicText(pnlText, 0, pnlText.childCount);
                }
            );
        }

        public void HideComic(Transform pnlText) {
            StartCoroutine(this.HideComicDelay(pnlText));
        }

        private IEnumerator HideComicDelay(Transform pnlText) {
            yield return YieldManager.GetWaitForSeconds(1.5f);
            GameObject comic = pnlText.parent.gameObject;
            AnimationManager.Animate(comic, "Hide", () => {
                comic.SetActiveSafe(false);
                this.viewPref.pnlComic.gameObject.SetActiveSafe(false);
                FteManager.EndFte(true);
            });
        }

        private void ShowComicText(Transform pnlText, int index, int length) {
            GameObject text = pnlText.GetChild(index++).gameObject;
            text.gameObject.SetActiveSafe(true);
            AnimationManager.Animate(text.gameObject, "Show", () => {
                if (index >= length) {
                    this.HideComic(pnlText);
                } else {
                    this.ShowComicText(pnlText, index, length);
                }
            });
        }

        public void ShowBanner(UnityAction callback) {
            this.viewPref.pnlBanner.gameObject.SetActiveSafe(true);
            this.viewPref.bannerCanvasGroup.alpha = 1;
            AnimationManager.Animate(this.viewPref.pnlBannerUp.gameObject, "Move", isOffset: false);
            AnimationManager.Animate(this.viewPref.pnlBannerBelow.gameObject, "Move", callback, isOffset: false);
        }

        public void HideBanner(UnityAction callback, bool immediatly = false) {
            if (immediatly) {
                this.viewPref.pnlBanner.gameObject.SetActiveSafe(false);
            } else {
                AnimationManager.Animate(this.viewPref.pnlBanner.gameObject, "Hide", () => {
                    this.viewPref.pnlBanner.gameObject.SetActiveSafe(false);
                    callback.InvokeSafe();
                }, isOffset: false);
            }
        }

        public void ShowDragon() {
            this.InitDragon();
            AudioManager.Stop(AudioType.Background);
            AudioManager.PlayBg(AudioScene.None);
            this.ShowBanner(() => {
                AudioManager.Play("dragon_roar", AudioType.Action, AudioVolumn.High);
                StartCoroutine(this.DelayShowDragon());
            });
        }

        private IEnumerator DelayShowDragon() {
            this.InitDragon();
            yield return YieldManager.GetWaitForSeconds(2.5f);
            AudioManager.Play("dragon_fly", AudioType.Show, AudioVolumn.High, true);
            this.dragon.SetActiveSafe(true);
            this.dragon.GetComponent<Animator>().SetTrigger(GameConst.animMoveRight);
            AnimationManager.Animate(this.dragon.gameObject, "Move",
                GameManager.MainCamera.transform.position,
                GameManager.MainCamera.transform.position,
                () => {
                    this.dragon.gameObject.SetActiveSafe(false);
                    AudioManager.Stop(AudioType.Action);
                    AudioManager.Stop(AudioType.Show);
                    this.HideBanner(() => {
                        FteManager.EndFte(true);
                    });
                }, isOffset: false, space: PositionSpace.SelfWorld);
        }

        public void ShowDragonAgain() {
            this.InitDragon();
            AudioManager.Play("dragon_roar", AudioType.Action, AudioVolumn.High);
            StartCoroutine(this.DelayShowDragonAgain());
        }

        private IEnumerator DelayShowDragonAgain() {
            this.InitDragon();
            yield return YieldManager.GetWaitForSeconds(2.5f);
            AudioManager.Play("dragon_fly", AudioType.Show, AudioVolumn.High, true);
            this.dragon.gameObject.SetActiveSafe(true);
            this.dragon.transform.eulerAngles = Vector3.up * 180;
            this.dragon.GetComponent<Animator>().SetTrigger(GameConst.animMoveRight);
            Vector2 position =
                MapUtils.CoordinateToPosition(FteManager.Instance.DemonTarget);
            AnimationManager.Animate(
                this.dragon,
                "MoveDemon",
                start: GameManager.MainCamera.transform.position,
                target: position,
                finishCallback: () => {
                    AudioManager.Stop(AudioType.Action);
                    AudioManager.Stop(AudioType.Show);
                    FteManager.EndFte(true);
                }, isOffset: false, space: PositionSpace.World);
        }

        public void ShowDragonDisappear() {
            this.InitDragon();
            AudioManager.Play("dragon_injure", AudioType.Action, AudioVolumn.High);
            StartCoroutine(this.DelayShowDragonDisappear());
        }

        private IEnumerator DelayShowDragonDisappear() {
            this.InitDragon();
            yield return YieldManager.GetWaitForSeconds(2.5f);
            AudioManager.Play("dragon_fly", AudioType.Show, AudioVolumn.High, true);
            AudioManager.Stop(AudioType.Action);
            this.dragon.SetActiveSafe(true);
            this.dragon.transform.eulerAngles = Vector3.zero;
            AnimationManager.Animate(
                this.dragon.gameObject,
                "MoveOut",
                start: this.dragon.transform.position,
                target: GameManager.MainCamera.transform.position,
                finishCallback: () => {
                    this.dragon.gameObject.SetActiveSafe(false);
                    FteManager.EndFte(true);
                    AudioManager.Stop(AudioType.Show);
                }, isOffset: false, space: PositionSpace.World
            );
        }

        public void ShowDemonDisappear() {
            this.InitDemon();
            this.demon.SetActiveSafe(false);
        }

        public void ShowDemonAppear(Vector2 coordinate, bool needAnimation) {
            this.InitDemon();
            Vector2 position = MapUtils.CoordinateToPosition(coordinate);
            this.demon.SetActiveSafe(true);
            GameObject model = demon.transform.Find("demon").gameObject;
            FteDemonAppear demonAppear = this.demon.GetComponent<FteDemonAppear>();
            if (needAnimation) {
                this.demon.transform.localPosition = position;
                demonAppear.onEnd = () => {
                    FteManager.EndFte(true);
                    model.SetActive(false);
                };
                demonAppear.StartShow();
            } else {
                model.SetActive(true);
                model.transform.position = position;
                model.transform.localPosition = new Vector3(
                    model.transform.localPosition.x,
                    model.transform.localPosition.y,
                    1
                );
                demonAppear.StartMove();
            }
        }

        private IEnumerator DelayDemonAppearEnd() {
            this.InitDemon();
            yield return YieldManager.GetWaitForSeconds(2f);
        }

        public void SetDemonVisible(bool visible) {
            this.InitDemon();
            this.demon.gameObject.SetActiveSafe(visible);
        }

        public void ShowElfAppear(Vector2 coordinate, bool needAnimation) {
            this.InitElf();
            Vector2 position = MapUtils.CoordinateToPosition(coordinate);
            this.elf.gameObject.SetActiveSafe(true);
            this.elf.GetComponent<Animator>().SetTrigger(GameConst.animMoveRight);
            this.elf.transform.localPosition = position;
            if (needAnimation) {
                AnimationManager.Animate(this.elf, "Show", () => {
                    FteManager.EndFte(true);
                }, space: PositionSpace.World);
            }
        }

        public void SetElfVisible(bool visible) {
            this.InitElf();
            this.elf.SetActiveSafe(visible);
        }

        private void InitDragon() {
            if (this.dragon == null) {
                this.dragon = PoolManager.GetObject(PrefabPath.fteDragon, GameManager.Instance.transform);
                this.dragon.transform.localScale = new Vector3(3.2f, 3.2f, 1);
            }
        }

        private void InitDemon() {
            if (this.demon == null) {
                this.demon = PoolManager.GetObject(PrefabPath.fteDemon, GameManager.Instance.transform);
                this.demon.transform.localScale = new Vector3(1.8f, 1.8f, 1);
            }
        }

        private void InitElf() {
            if (this.elf == null) {
                this.elf = PoolManager.GetObject(PrefabPath.fteElf, GameManager.Instance.transform);
                this.elf.transform.localScale = new Vector3(1.8f, 1.8f, 1);
            }
        }

        public void DestroyRole(string index) {
            if (this.dragon != null) {
                Destroy(this.dragon);
            }
            if (this.demon != null) {
                Destroy(this.demon);
            }
            if (this.elf != null) {
                Destroy(this.elf);
            }
        }

        public void SetClickableAreaWithMask(Transform fteTrans, bool isNext,
            bool autoNext, bool isButton, Transform arrowParent, Vector2 offset, float rotation) {
            Debug.Log("SetClickableAreaWithMask");
            Debug.Log(fteTrans);
            this.viewPref.pnlDark.gameObject.SetActiveSafe(true);
            RectTransform fteRectTrans = fteTrans.GetComponent<RectTransform>();

            Vector2 rectPos = MapUtils.WorldToUIPoint(
                fteTrans.position,
                this.fteCamera
            );
            FteRect fteRect = new FteRect(rectPos, fteRectTrans.rect);

            if (isNext) {
                this.SetArrow(fteRect, arrowParent ?? fteTrans, offset, rotation);
            }
            Transform originParent = fteTrans.parent;
            int originIndex = fteTrans.GetSiblingIndex();
            GameObject replaceObj = PoolManager.GetObject(PrefabPath.pnlFteEmpty, originParent);
            RectTransform replaceRectTrans = replaceObj.GetComponent<RectTransform>();
            replaceObj.transform.SetSiblingIndex(fteTrans.GetSiblingIndex());
            this.onAreaClick.AddListener(() => {
                PoolManager.RemoveObject(replaceObj);
                fteTrans.SetParent(originParent);
                fteTrans.SetSiblingIndex(originIndex);
            });
            GameHelper.CopyRectTransform(fteRectTrans, replaceRectTrans);
            fteTrans.SetParent(this.viewPref.pnlShow);
            // To do: Add a empty rect to 'originParent'.
            GameObject pnlFteRect = PoolManager.GetObject(PrefabPath.pnlFteRect, this.viewPref.pnlClickable);
            RectTransform rectTransform = pnlFteRect.GetComponent<RectTransform>();

            CustomButton btnRect = pnlFteRect.GetComponent<CustomButton>();
            btnRect.pnlContent = fteTrans;
            btnRect.onClick.RemoveAllListeners();
            btnRect.onClick.AddListener(() => {
                this.ClearClickable();
                this.viewPref.pnlDark.gameObject.SetActiveSafe(false);
                FteManager.EndFte(isNext, autoNext);
            });
            rectTransform.anchoredPosition = fteRect.position;
            rectTransform.sizeDelta = fteRect.rect.size;
            this.onAreaMove.AddListener(() => {
                fteRectTrans.position =
                rectTransform.position = replaceObj.transform.position;
            });
        }

        public void SetClickableArea(Transform fteTrans, bool isNext, bool hasArrow, bool autoNext,
            bool isButton, Transform arrowParent, Vector2 offset, float rotation) {
            RectTransform fteRectTrans = fteTrans.GetComponent<RectTransform>();
            FteRect fteRect = new FteRect(
                MapUtils.WorldToUIPoint(fteTrans.position, this.fteCamera),
                fteRectTrans.rect
            );
            if (isNext && hasArrow) {
                this.SetArrow(fteRect, arrowParent ?? fteTrans, offset, rotation);
            }
            GameObject pnlFteRect = PoolManager.GetObject(PrefabPath.pnlFteRect, this.viewPref.pnlClickable);
            RectTransform rectTransform = pnlFteRect.GetComponent<RectTransform>();
            this.onAreaMove.AddListener(() => {
                rectTransform.position = fteRectTrans.transform.position;
            });
            CustomButton btnRect = pnlFteRect.GetComponent<CustomButton>();
            if (isButton) {
                btnRect.pnlContent = fteTrans;
            }
            btnRect.onClick.RemoveAllListeners();
            btnRect.onClick.AddListener(() => {
                this.ClearClickable();
                FteManager.EndFte(isNext, autoNext);
            });
            rectTransform.sizeDelta = fteRect.rect.size;
            rectTransform.anchoredPosition = fteRect.position;
        }

        public void SetDragMask(Transform from, Transform to, CustomDrag fromDrag, CustomDrop toDrop) {
            // Set rect.
            this.ClearClickable();
            RectTransform fromRectTrans = from.GetComponent<RectTransform>();
            RectTransform toRectTrans = to.GetComponent<RectTransform>();
            GameObject dragObj = PoolManager.GetObject(PrefabPath.pnlFteDrag, this.viewPref.pnlClickable);
            RectTransform dragRectTrans = dragObj.GetComponent<RectTransform>();
            dragRectTrans.anchoredPosition = MapUtils.WorldToUIPoint(from.position, this.fteCamera);
            dragRectTrans.sizeDelta = fromRectTrans.rect.size;
            GameObject dropObj = PoolManager.GetObject(PrefabPath.pnlFteDrop, this.viewPref.pnlClickable);
            RectTransform dropRectTrans = dropObj.GetComponent<RectTransform>();
            dropRectTrans.anchoredPosition = MapUtils.WorldToUIPoint(to.position);
            dropRectTrans.sizeDelta = toRectTrans.rect.size;
            this.SetArrow(
                new FteRect(
                    MapUtils.WorldToUIPoint(fromRectTrans.position, this.fteCamera),
                    fromRectTrans.rect
                )
            );
            Transform originParent = from.parent;
            CustomDrag drag = dragObj.GetComponent<CustomDrag>();
            CustomDrop drop = dropObj.GetComponent<CustomDrop>();
            Vector2 originPosition = dragRectTrans.anchoredPosition;
            drag.onBeginDrag.RemoveAllListeners();
            drag.onBeginDrag.AddListener((position) => {
                fromDrag.onBeginDrag.Invoke(position);
            });

            drag.onDrag.RemoveAllListeners();
            drag.onDrag.AddListener((position) => {
                Vector2 curPosition = MapUtils.ScreenToUIPoint(position);
                dragRectTrans.anchoredPosition = curPosition;
                fromDrag.onDrag.Invoke(position);
            });

            drag.onEndDrag.RemoveAllListeners();
            drag.onEndDrag.AddListener((position) => {
                dragRectTrans.anchoredPosition = originPosition;
                fromDrag.onEndDrag.Invoke(position);
            });

            drop.onDrop.RemoveAllListeners();
            drop.onDrop.AddListener(() => {
                this.ClearClickable();
                FteManager.EndFte(true, false);
                toDrop.onDrop.InvokeSafe();
            });
        }

        public void RemoveMask() {
            this.MaskCallBack = null;
            this.onAreaClick.InvokeSafe();
            this.ClearClickable();
            this.viewPref.pnlDark.gameObject.SetActiveSafe(false);
        }

        public void SetMask(Transform nextTrans, Transform prevTrans, bool hasArrow, bool isButton, bool autoNext,
            bool isEnforce, bool isHighlight, Transform arrowParent, Vector2 offset, float rotation,UnityAction AfterCallBack) {
            this.MaskCallBack = AfterCallBack;
            this.chatDirectin = 0;
            this.onAreaClick.RemoveAllListeners();
            this.viewPref.background.gameObject.SetActiveSafe(isEnforce);
            if (isHighlight) {
                Transform nextOrigin = nextTrans.parent;
                int nextIndex = nextTrans.GetSiblingIndex();
                this.onAreaClick.AddListener(() => {
                    this.MaskCallBack.InvokeSafe();
                    nextTrans.SetParent(nextOrigin);
                    nextTrans.localScale = Vector3.one;
                    if (!isButton) {
                        nextTrans.SetSiblingIndex(nextIndex);
                    }
                });
                this.SetClickableAreaWithMask(nextTrans, hasArrow, autoNext,
                    isButton, arrowParent, offset, rotation);
            } else {
                this.SetClickableArea(nextTrans, true, hasArrow, autoNext,
                    isButton, arrowParent, offset, rotation);
            }

            if (prevTrans != null) {
                if (isHighlight) {
                    this.SetClickableAreaWithMask(prevTrans, false, autoNext,
                        isButton, arrowParent, offset, rotation);
                } else {
                    this.SetClickableArea(prevTrans, false, false, autoNext,
                        isButton, arrowParent, offset, rotation);
                }
            }
        }

        public void SetArrow(FteRect clickRect, Transform parent = null,
            Vector2 offset = default(Vector2), float rotation = 0) {
            clickRect.rect.width = Mathf.Max(50, clickRect.rect.width);
            clickRect.rect.height = Mathf.Max(50, clickRect.rect.height);
            this.viewPref.pnlArrow.SetParent(this.ui.transform);
            this.viewPref.pnlArrow.gameObject.SetActiveSafe(true);
            this.viewPref.pnlArrow.localScale = Vector3.one;
            this.viewPref.arrowRT.anchoredPosition =
                clickRect.position + Vector2.up * clickRect.rect.height / 2 + offset;
            this.viewPref.pnlArrow.eulerAngles = Vector3.forward * rotation;
            if (parent != null) {
                this.viewPref.pnlArrow.SetParent(parent);
            }
            this.viewPref.arrowRT.anchoredPosition3D =
                (Vector3)this.viewPref.arrowRT.anchoredPosition;
            AnimationManager.Animate(this.viewPref.arrowImg,
                "Show", loop: true, needRestart: true, isOffset: false);

        }

        public void HideArrow() {
            this.viewPref.pnlArrow.SetParent(this.ui.transform);
            this.viewPref.pnlArrow.gameObject.SetActiveSafe(false);
        }

        public bool HasArrow() {
            Transform pnlArrow = this.ui.transform.Find("PnlFteArrow");
            return (pnlArrow == null);
        }

        public void SetChat(float offset, string text, int subIndex,
            bool needButton, bool needBackground, int direction, bool transparent) {
            this.viewPref.btnChat.gameObject.SetActiveSafe(needButton);
            this.chatOffset = offset;
            this.viewPref.chatImage.enabled = needBackground;
            this.viewPref.chatImage.color = new Color(0, 0, 0, transparent ? 0 : 0.5f);
            this.viewPref.clickChat.onClick.RemoveAllListeners();
            bool isSelf = direction == -1;
            this.viewPref.imgSelf.gameObject.SetActiveSafe(isSelf);
            if (imgOther != null) {
                this.imgOther.gameObject.SetActiveSafe(!isSelf);
            }
            bool needRefresh = subIndex == 1 || this.chatDirectin != direction;
            this.chatDirectin = direction;
            string animation = "Left";
            if (!isSelf) {
                animation = "Right";
            }
            this.viewPref.pnlChat.gameObject.SetActiveSafe(true);
            UnityAction action = null;
            UnityAction callBack = () => {
                FteManager.SetChat(offset, ++subIndex, transparent: transparent);
            };
            if (needButton) {
                action = () => {
                    this.SetArrow(new FteRect(
                        MapUtils.WorldToUIPoint(this.viewPref.btnChat.transform.position, this.fteCamera),
                        this.viewPref.chatBtnGoRT.rect),
                        offset: Vector2.down * 60, rotation: needButton ? 180f : 0f);
                };
                this.viewPref.btnChat.onClick.AddListener(callBack);
            } else if (needBackground) {
                this.viewPref.clickChat.onClick.AddListener(callBack);
            }
            this.viewPref.txtChat.text = text;
            if (needRefresh) {
                this.viewPref.chatClick.interactable = false;
                AudioManager.Play("act_button", AudioType.Action, AudioVolumn.High);
                StartCoroutine(DelayPlayAudio(FteManager.Instance.curStep));
                if (this.isNewChat) {
                    AnimationManager.Animate(this.viewPref.pnlChatContent.gameObject, animation,
                        Vector2.up * offset, Vector2.up * offset, () => {
                            AnimationManager.Animate(this.viewPref.pnlText.gameObject, "Show"
                                , finishCallback: () => {
                                    this.viewPref.chatClick.interactable = true;
                                    action.InvokeSafe();
                                    this.isNewChat = false;
                                }
                                );
                        }, isOffset: false);
                } else {
                    AnimationManager.Animate(this.viewPref.pnlText.gameObject, "Show"
                                , finishCallback: () => {
                                    this.viewPref.chatClick.interactable = true;
                                    action.InvokeSafe();
                                }
                                );
                }
            }
        }

        private IEnumerator DelayPlayAudio(string audio) {
            yield return YieldManager.GetWaitForSeconds(0.15f);
            AudioManager.Play("chat_" + audio, AudioType.Action, AudioVolumn.High);
        }

        public void SetLeftChat(string text,bool isHigh,UnityAction afterCallBack) {
            Vector3 hight = new Vector3(397.6f, isHigh ? -56f : -129.5142f, 0);
            this.viewPref.pnlText.GetComponent<RectTransform>().anchoredPosition3D 
                = hight;
            this.viewPref.clickChat.onClick.RemoveAllListeners();

            this.viewPref.imgSelf.gameObject.SetActiveSafe(true);

            string animation = "Left";
            this.chatDirectin = -1;
            this.viewPref.pnlChat.gameObject.SetActiveSafe(true);
            UnityAction action = null;
            UnityAction callBack = () => {
                HideChat(() => {
                    //this.viewPref.pnlChat.gameObject.SetActiveSafe(false);
                    //this.viewPref.imgSelf.gameObject.SetActiveSafe(false);
                    afterCallBack.InvokeSafe();
                }, true);
            };
            this.viewPref.clickChat.onClick.AddListener(callBack);
            this.viewPref.btnChat.gameObject.SetActiveSafe(false);
            this.viewPref.txtChat.text = text;
            AnimationManager.Animate(this.viewPref.pnlChatContent.gameObject, animation,
               () => {
                   AnimationManager.Animate(this.viewPref.pnlText.gameObject, "Show"
                            , finishCallback: () => {
                                this.viewPref.chatClick.interactable = true;
                                action.InvokeSafe();
                                this.isNewChat = false;
                            }
                            );
               }, isOffset: false);
        }

        public void SetRightChat(string text, UnityAction afterCallback) {
            this.viewPref.clickChat.onClick.RemoveAllListeners();

            this.viewPref.imgRight.gameObject.SetActiveSafe(true);

            string animation = "Right";
            this.chatDirectin = 0;
            this.viewPref.pnlChat.gameObject.SetActiveSafe(true);
            UnityAction action = null;
            UnityAction callBack = () => {
                HideChat(() => {
                    this.viewPref.pnlChat.gameObject.SetActiveSafe(false);
                    this.viewPref.imgRight.gameObject.SetActiveSafe(false);
                    afterCallback.InvokeSafe();
                }, true);
            };
            this.viewPref.clickChat.onClick.AddListener(callBack);
            this.viewPref.txtRightChat.text = text;
            AnimationManager.Animate(this.viewPref.pnlChatContent.gameObject, animation,
               () => {
                   AnimationManager.Animate(this.viewPref.pnlRightText.gameObject, "Show"
                            , finishCallback: () => {
                                this.viewPref.chatClick.interactable = true;
                                action.InvokeSafe();
                                this.isNewChat = false;
                            }
                            );
               }, isOffset: false);
        }

        public void HideRightChat() {
            HideChat(() => {
                //this.viewPref.pnlChat.gameObject.SetActiveSafe(false);
                //this.viewPref.imgRight.gameObject.SetActiveSafe(false);
            }, true);
        }

        public void HideChat(UnityAction callback, bool isLastChat) {
            string animation = this.chatDirectin == -1 ? "Left" :"Right";
            GameObject objTxt = this.chatDirectin == -1 ? 
                this.viewPref.pnlText.gameObject : 
                this.viewPref.pnlRightText.gameObject;
            this.viewPref.chatClick.interactable = false;
                if (isLastChat) {
                    AnimationManager.Animate(objTxt, "Hide",
                        finishCallback: () => {
                            AnimationManager.Animate(this.viewPref.pnlChatContent.gameObject, animation + "Back", Vector2.up * this.chatOffset,
                               Vector2.up * this.chatOffset, () => {
                                   this.viewPref.chatClick.interactable = true;
                                   callback.InvokeSafe();
                                   this.viewPref.pnlChat.gameObject.SetActiveSafe(false);
                                   this.viewPref.imgSelf.gameObject.SetActiveSafe(false);
                                   this.viewPref.imgRight.gameObject.SetActiveSafe(false);
                                   this.isNewChat = true;
                               }, isOffset: false, isReverse: true);
                        });
                } else {
                    AnimationManager.Animate(objTxt, "Hide",
                        finishCallback: () => {
                            this.viewPref.chatClick.interactable = true;
                            callback.InvokeSafe();
                        });
                }
        }

        public void ShowDesc(int index, int subIndex, UnityAction callback, float interval) {
            this.viewPref.pnlDesc.gameObject.SetActiveSafe(true);
            int count = 0;
            string descStr = LocalManager.GetValue(
                string.Format("fte_normal_{0}_{1}", index, subIndex)
            );
            this.viewPref.txtDesc.text = string.Empty;
            AnimationManager.AnimateEvent(this.viewPref.pnlDesc.gameObject, descStr.Length, "Show",
                () => {
                    count++;
                    if (count <= descStr.Length) {
                        this.viewPref.txtDesc.text = descStr.Substring(0, count);
                    }
                },
                () => {
                    StartCoroutine(YieldManager.DelayCallAction(callback, stepInterval));
                },
                isFix: false, target: (descStr.Length + 1) * interval, interval: interval
            );

        }

        public void HideDesc() {
            this.viewPref.pnlDesc.gameObject.SetActiveSafe(false);
        }

        public void ClearClickable() {
            GameHelper.ClearChildren(this.viewPref.pnlClickable);
            this.onAreaMove.RemoveAllListeners();
        }

        public static void SetCloudsUIVisible(bool show, UnityAction action) {
            if ((!self.viewPref.pnlCloud.gameObject.activeSelf && show) ||
                (self.viewPref.pnlCloud.gameObject.activeSelf && !show)) {
                if (show) {
                    AudioManager.Play("show_cloud_in", AudioType.Show, AudioVolumn.High);
                } else {
                    AudioManager.Play("show_cloud_out", AudioType.Show, AudioVolumn.High);
                }
                self.SetCloudsVisible(show, action);
            } else {
                action.InvokeSafe();
            }
        }

        public void SetCloudsVisible(bool show, UnityAction action) {
            if (show) {
                this.viewPref.pnlCloud.gameObject.SetActiveSafe(true);
            }
            int reference = 0;
            foreach (Transform part in this.viewPref.pnlCloud) {
                foreach (Transform cloud in part) {
                    reference++;
                    AnimationManager.Animate(cloud.gameObject, "Show", isReverse: !show,
                        isOffset: false, finishCallback: () => {
                            reference--;
                            if (action != null && reference == 0) {
                                if (!show) {
                                    this.viewPref.pnlCloud.gameObject.SetActiveSafe(false);
                                }
                                action.Invoke();
                            }
                        });
                }
            }
        }
    }
}
