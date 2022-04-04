using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine.UI;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using System;

namespace Poukoute {
    public class TroopFormationView: GridAdapter<TroopFormationViewModel, HeroHeadView> {
        private TroopFormationViewPreference viewPref;

        private readonly Dictionary<string, HeroHeadView> heroHeadDict =
                                    new Dictionary<string, HeroHeadView>();

        //private float positionOffset;
        private bool isEdit;
        private bool IsEdit {
            get {
                return this.isEdit;
            }
            set {
                this.isEdit = value;
                if (!this.isEdit) {
                    this.viewPref.btnHeroGridHight.gameObject.SetActiveSafe(false);
                    this.viewPref.btnCloseSelect.gameObject.SetActiveSafe(false);
                    AnimationManager.Stop(this.viewPref.pnlSelect.gameObject);
                    this.HideFteFormation();
                    UIManager.SetUICanvasGroupEnable(this.viewPref.selectCanvasGroup, false);
                    this.viewPref.pnlHighlight.gameObject.SetActiveSafe(false);
                    HideAllHighlightViewAni();
                } else {
                    this.viewPref.pnlHighlight.gameObject.SetActiveSafe(true);
                    this.SetHighlightViewInfo();
                    this.SetHighlightScreenPoint();
                }
            }
        }
        private const int unlockedTroopAmount = 6;

        private float oldArmyAmount = 0;
        private string oldSpeed = string.Empty;
        private float oldSiege = 0;

        /****************************************************************************/
        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UITroopFormation");
            this.viewModel = this.gameObject.GetComponent<TroopFormationViewModel>();
            this.viewPref = this.ui.transform.GetComponent<TroopFormationViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);

            this.viewPref.scrollRect.onDrag.AddListener(this.OnHeroGridDrag);
            this.viewPref.pnlHighlight.gameObject.SetActiveSafe(false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.selectCanvasGroup, false);
            this.viewPref.btnOrder.onClick.AddListener(this.OnBtnOrderClick);
            this.viewPref.btnRecruit.onClick.AddListener(this.OnBtnRecruitClick);
            this.viewPref.btnCloseSelect.onClick.AddListener(this.OnBtnCloseSelect);
            this.GridAdapterInit(this.viewPref.scrollRect, this.viewPref.gridLayoutGroup,
                PrefabPath.pnlHeroBig);
        }

        private void OnBtnCloseSelect() {
            DisableEdit();
        }

        public void RefreshHero(string heroName) {
            HeroHeadView heroHeadView;
            if (this.heroHeadDict.TryGetValue(heroName, out heroHeadView)) {
                this.UpdateCellViewsHolder(heroHeadView);
            }
        }

        #region GridAdapter implementation
        protected override void UpdateCellViewsHolder(HeroHeadView itemView) {
            if (itemView.ItemIndex >= this.viewModel.HeroList.Count)
                return;
            Hero hero = this.viewModel.HeroList[itemView.ItemIndex];
            Troop troop = this.viewModel.TroopDict[this.viewModel.CurrentTroop];
            string name = hero.Name;
            this.RefreshHero(itemView, name);
            itemView.AddListener(
                () => {
                    if (this.viewModel.GetHeroTroopName(itemView.HeroName).
                    CustomEquals(troop.ArmyCamp)) {
                        UIManager.ShowTip(
                            string.Format(
                                LocalManager.GetValue(LocalHashConst.warning_already_in_troop),
                                 GameHelper.GetBuildIndex(
                                     this.viewModel.TroopDict[this.viewModel.CurrentTroop].ArmyCamp
                                 )
                            ),
                            TipType.Warn
                        );
                        return;
                    } else {
                        this.OnHeroInGridClick(name, () => this.RefreshHero(itemView, name));
                    }
                }
            );
            itemView.OnPress.AddListener(() => this.viewModel.ShowHeroInfo(name,
                () => this.RefreshHero(itemView, name)));
            this.heroHeadDict[name] = itemView;
        }
        #endregion

        private void SetHighlightScreenPoint() {
            for (int i = 0; i < highlightScreenPoint.Length; i++) {
                highlightScreenPoint[i] = GameManager.MainCamera.WorldToScreenPoint(viewPref.highlightTransform[i].position);
            }
        }

        private ArmyCampConf armyCampConf = null;
        public void SetTroopFormat() {
            this.viewModel.RefreshGridHeroList();
            Troop troop = this.viewModel.TroopDict[this.viewModel.CurrentTroop];
            // To do : Get the local
            this.viewPref.txtTitle.text = string.Format(
                LocalManager.GetValue(LocalHashConst.troop_formation),
                TroopModel.GetTroopName(troop.ArmyCamp));
            this.armyCampConf = ArmyCampConf.GetConf(
                    this.viewModel.BuildingDict[troop.ArmyCamp].Level.ToString());
            this.SetBtnSortTitle();
            this.SetFormationDetail();
        }

        public void UpdateTroopFormation() {
            //   this.viewModel.RefreshGridHeroList();
            //   base.Refresh();
            this.SetTroopGrid(false);
            ChangeHeroFormTroopUI();
        }

        private void SetFormationDetail() {
            this.SetAttributes();
            this.SetHeroList();
            this.SetTroopGrid();
        }

        private void SetHighlightViewInfo() {
            for (int index = 0; index < unlockedTroopAmount; index++) {
                bool unlocked = armyCampConf.unlockPositionList.Contains(index + 1);
                if (index + 1 == 1) {
                    unlocked = true;
                }
                this.viewPref.highlightTransform[index].gameObject.SetActiveSafe(unlocked);
                if (unlocked) {
                    //unlockTroopAmount++;
                    SetHighLightViewAni(index, "ShowRotate");
                }
            }
        }

        private void HideAllHighlightViewAni() {
            for (int index = 0; index < unlockedTroopAmount; index++) {
                AnimationManager.Stop(this.viewPref.highlightTransform[index].gameObject);
                AnimationManager.Stop(this.viewPref.heroTransform[index].gameObject);
                SetHighlightRotateToZero(index);
            }
        }

        private void SetAllHighlightViewAni(string aniName, UnityAction callback = null) {
            for (int index = 0; index < unlockedTroopAmount; index++) {
                AnimationManager.Animate(this.viewPref.highlightTransform[index].gameObject,
                   aniName, callback);
                AnimationManager.Animate(this.viewPref.heroTransform[index].gameObject,
                    aniName, callback);
            }
        }

        private void HideHighlightViewInfo(int index) {
            AnimationManager.Stop(this.viewPref.highlightTransform[index].gameObject);
            AnimationManager.Stop(this.viewPref.heroTransform[index].gameObject);
            SetHighlightRotateToZero(index);
        }
        private void SetHighLightViewAni(int index, string aniName, UnityAction callback = null) {
            AnimationManager.Animate(this.viewPref.highlightTransform[index].gameObject,
                    aniName, callback);
            AnimationManager.Animate(this.viewPref.heroTransform[index].gameObject,
                aniName, callback);
        }

        private void SetHighlightRotateToZero(int index) {
            Quaternion originQua = this.viewPref.highlightTransform[index].transform.rotation;
            this.viewPref.highlightTransform[index].rotation = new Quaternion(originQua.x, originQua.y, 0, originQua.w);
            originQua = this.viewPref.heroTransform[index].rotation;
            this.viewPref.heroTransform[index].rotation = new Quaternion(originQua.x, originQua.y, 0, originQua.w);
        }

        private void SetHeroList() {
            _CellsCount = this.viewModel.HeroList.Count;
            base.Refresh(false, true);
        }

        private void SetAttributes() {
            float armyAmount = 0;
            float speed = Mathf.Infinity;
            float siege = 0;
            foreach (HeroPosition heroPosition in this.viewModel.Formation.Values) {
                Hero hero = this.viewModel.HeroDict[heroPosition.Name];
                HeroAttributeConf heroConf = HeroAttributeConf.GetConf(hero.GetId());
                armyAmount += hero.ArmyAmount;
                siege += heroConf.siege;
                speed = Mathf.Min(speed, heroConf.speed);
            }
            this.viewPref.txtTroopAttributes[0].text =
                GameHelper.GetFormatNum((long)armyAmount);
            string speedStr = "--";
            if (this.viewModel.Formation.Count != 0) {
                speedStr = HeroAttributeConf.GetSpeed(speed);
            }
            this.viewPref.txtTroopAttributes[1].text = speedStr;
            this.viewPref.txtTroopAttributes[2].text = siege.ToString();
            if (this.oldArmyAmount != armyAmount) {
                AnimationManager.Animate(this.viewPref.txtTroopAttributes[0].gameObject, "Show");
                this.oldArmyAmount = armyAmount;
            }
            if (!this.oldSpeed.CustomEquals(speedStr)) {
                AnimationManager.Animate(this.viewPref.txtTroopAttributes[1].gameObject, "Show");
                this.oldSpeed = speedStr;
            }
            if (this.oldSiege != siege) {
                AnimationManager.Animate(this.viewPref.txtTroopAttributes[2].gameObject, "Show");
                this.oldSiege = siege;
            }
        }

        private readonly bool[] isShowInfoPnl = new bool[] { false, false, false, false, false, false };
        private readonly bool[] isPlayingAnimation = new bool[] { false, false, false, false, false, false };
        private void OnBtnInfoClick(GameObject targetObj, int index) {
            if (!this.isPlayingAnimation[index]) {
                this.isPlayingAnimation[index] = true;
                if (!this.isShowInfoPnl[index]) {
                    AnimationManager.Animate(targetObj, "Show", () => {
                        this.isShowInfoPnl[index] = false;
                        this.isPlayingAnimation[index] = false;
                    }, () => {
                        this.isShowInfoPnl[index] = true;
                    });
                } else {
                    AnimationManager.Animate(targetObj, "Hide", () => {
                        this.isShowInfoPnl[index] = false;
                        this.isPlayingAnimation[index] = false;
                    });
                }
            }
        }

        private void SetTroopGrid(bool visible = true) {
            bool unlocked = false;
            for (int index = 0; index < unlockedTroopAmount; index++) {
                GameHelper.ClearChildren(this.viewPref.heroTransform[index]);
                unlocked = armyCampConf.unlockPositionList.Contains(index + 1);
                if (index + 1 == 1) {
                    unlocked = true;
                }
                this.viewPref.backImage[index].SetActiveSafe(!unlocked);
                this.viewPref.backText[index].SetActiveSafe(unlocked);
                this.viewPref.backCustomClick[index].onClick.RemoveAllListeners();
                int i = index;
                int unlockedLevel = this.GetUnlockLevel(i);
                this.viewPref.txtLockeDetail[i].text =
                    string.Format(LocalManager.GetValue(LocalHashConst.troop_format_unlock_detail), unlockedLevel);
                this.viewPref.backCustomClick[index].onClick.AddListener(() => {
                    for (int j = 0; j < this.isShowInfoPnl.Length; j++) {
                        if (this.isShowInfoPnl[j])
                            OnBtnInfoClick(viewPref.pnlLockedDetail[j].gameObject, j);
                    }
                    OnBtnInfoClick(viewPref.pnlLockedDetail[i].gameObject, i);
                });
            }

            for (int index = 0; index < unlockedTroopAmount; index++) {
                Transform cell = this.viewPref.highlightTransform[index];
                CustomDrop drop = cell.GetComponent<CustomDrop>();
                cell.GetComponent<Button>().onClick.RemoveAllListeners();
                int position = this.GetPosition(cell);
                drop.onDrop.RemoveAllListeners();
                drop.onDrop.AddListener(() => {
                    this.viewModel.ResetSelect();
                    this.OnHeroInTroopClick(null, string.Empty, position);
                });
                cell.GetComponent<Button>().onClick.AddListener(() => {
                    this.OnHeroInTroopClick(null, string.Empty, position);
                });
            }
            heroHeadFormationDic.Clear();
            foreach (HeroPosition hero in this.viewModel.Formation.Values) {
                heroHeadFormationDic[hero.Name] = this.SetHero(
                    hero.Position, hero,
                    visible || this.moveTargetHeroName != hero.Name
                );
            }
        }

        private int GetUnlockLevel(int troopPos) {
            if (troopPos == 2) {
                return 3;
            } else if (troopPos == 3) {
                return 5;
            } else if (troopPos == 4) {
                return 7;
            } else if (troopPos == 5) {
                return 9;
            }
            return 1;
        }

        private readonly bool[] highlightAni = new bool[] { false, false, false, false, false, false };
        private readonly Vector2[] highlightScreenPoint = new Vector2[6];
        public void ShowDragSelectHighlight(Vector2 position) {
            for (int i = 0; i < unlockedTroopAmount; i++) {
                if ((position - highlightScreenPoint[i]).sqrMagnitude < 9000) {
                    if (!highlightAni[i]) {
                        HideHighlightViewInfo(i);
                        SetHighLightViewAni(i, "ShowScale");
                        highlightAni[i] = true;
                    }

                } else {
                    if (highlightAni[i]) {
                        HideHighlightViewInfo(i);
                        int index = i;
                        SetHighLightViewAni(i, "HideScale", () => {
                            SetHighLightViewAni(index, "ShowRotate");
                        });
                        highlightAni[i] = false;
                    }
                }
            }
        }

        public void ShowDragHint() {
            AnimationManager.Animate(this.viewPref.dragHint.gameObject, "Show");
        }

        private Dictionary<string, HeroHeadView> heroHeadFormationDic = new Dictionary<string, HeroHeadView>();
        private HeroHeadView SetHero(int position, HeroPosition heroPos, bool visible = true) {
            Transform heroTransform = this.viewPref.heroTransform[position - 1];
            GameHelper.ClearChildren(heroTransform);
            GameObject heroObj = PoolManager.GetObject(PrefabPath.pnlHeroBig, heroTransform);
            HeroHeadView heroHead = heroObj.GetComponent<HeroHeadView>();
            Hero troopHero = this.viewModel.HeroDict[heroPos.Name];
            this.RefreshHero(heroHead, heroPos.Name, visible);
            UIManager.SetHeroRecoverEffectL(troopHero.IsRecruiting, heroTransform);
            heroHead.gameObject.SetActiveSafe(true);
            heroHead.AddListener(
                () => {
                    this.OnHeroInTroopClick(heroObj, heroPos.Name);
                });
            string positionName = heroHead.transform.parent.parent.name;
            CustomDrop drop = this.viewPref.pnlHighlight.Find(positionName).GetComponent<CustomDrop>();
            drop.onDrop.RemoveAllListeners();
            drop.onDrop.AddListener(() => {
                this.viewModel.ResetSelect();
                this.OnHeroInTroopClick(heroObj, heroPos.Name);
            });
            return heroHead;
        }

        public void SetBtnRecruit(bool interactable) {
            this.viewPref.btnRecruit.interactable = interactable;
        }

        private int GetPosition(Transform pnlPosition) {
            string positionName = pnlPosition.name;
            return int.Parse(positionName.Substring(8));
        }

        public void HideCurrentHero() {
            HeroHeadView heroHead = SetHeroHeadByOnClickType();
            this.viewPref.scrollViewCanvasGroup.blocksRaycasts = false;
            if (heroHead != null)
                heroHead.HideHero();
        }

        public void ShowCurrentHero() {
            HeroHeadView heroHead = SetHeroHeadByOnClickType();
            this.viewPref.scrollViewCanvasGroup.blocksRaycasts = true;
            if (heroHead != null)
                heroHead.ShowHero();
        }

        private HeroHeadView SetHeroHeadByOnClickType() {
            HeroHeadView heroHead = null;
            if (curOnClickType == OnClickType.InGrid)
                heroHead = this.heroHeadDict.ContainsKey(this.viewModel.ChosenHeroName) ?
                this.heroHeadDict[this.viewModel.ChosenHeroName] :
                null;
            else if (curOnClickType == OnClickType.InTroop)
                heroHead = this.heroHeadFormationDic.ContainsKey(this.viewModel.ChosenHeroName) ?
                    this.heroHeadFormationDic[this.viewModel.ChosenHeroName] :
                    null;
            return heroHead;
        }

        // To do: verify the troop hero amount by "army_camp" conf.
        private void AddHeroToTroop(GameObject heroObj, string name, int position = 0) {
            if (this.viewModel.IsInFormation(this.viewModel.ChosenHeroName)) {
                //int pos = position = this.GetPosition(heroObj.transform.parent.parent);
                SetAllHighlightViewAni("HideScale");
                return;
            }
            this.viewModel.IsAddHeroSuccess = true;
            this.IsEdit = false;
            bool isNameEmpty = name.CustomIsEmpty();
            if (isNameEmpty &&
                !this.viewModel.ArmyCampConf.unlockPositionList.Contains(position)) {
                UIManager.ShowAlert(
                    LocalManager.GetValue(LocalHashConst.upgrade_army_camp_tip), null, "OK");
                return;
            }
            GameObject currentHeroObj = null;
            if (!isNameEmpty) {
                if (EventManager.GetRecruitEventByHeroName(name) != null) {
                    UIManager.ShowAlert(
                        LocalManager.GetValue(LocalHashConst.hero_recruiting));
                    return;
                }
                this.viewModel.Formation.Remove(name);

                currentHeroObj = PoolManager.GetObject(
                    PrefabPath.pnlHeroBig,
                    heroObj.transform.parent
                );
                currentHeroObj.gameObject.SetActiveSafe(false);
                position = this.GetPosition(heroObj.transform.parent.parent);
                ChangeHeroFormTroopDataByName(name);
            } else {
                Transform pnlHero = this.viewPref.heroTransform[position - 1];
                if (pnlHero.childCount > 0) {
                    HeroHeadView heroHeadold = pnlHero.GetChild(0).GetComponent<HeroHeadView>();
                    this.viewModel.ReplaceHeroName = heroHeadold.HeroName;
                    if (EventManager.GetRecruitEventByHeroName(heroHeadold.HeroName) != null) {
                        UIManager.ShowAlert(
                            LocalManager.GetValue(LocalHashConst.hero_recruiting));
                        return;
                    }
                    this.RemoveHeroFromTroop(heroHeadold.transform.gameObject,
                                              heroHeadold.HeroName, false);
                }
                currentHeroObj = PoolManager.GetObject(
                    PrefabPath.pnlHeroBig, pnlHero);
                currentHeroObj.gameObject.SetActiveSafe(false);
            }
            this.viewModel.AddToFormation(
                this.viewModel.ChosenHeroName,
                position
            );
            HeroHeadView heroHead = currentHeroObj.GetComponent<HeroHeadView>();
            string currentHero = this.viewModel.ChosenHeroName;
            this.RefreshHero(heroHead, currentHero);

            heroHead.AddListener(
                () => {
                    this.OnHeroInTroopClick(currentHeroObj, currentHero);
                }
            );
            string positionName = heroHead.transform.parent.parent.name;
            CustomDrop drop = this.viewPref.pnlHighlight.Find(positionName).GetComponent<CustomDrop>();
            drop.onDrop.RemoveAllListeners();
            drop.onDrop.AddListener(() => {
                this.viewModel.ResetSelect();
                this.OnHeroInTroopClick(currentHeroObj, currentHero);
            });
            if (isNameEmpty)
                this.ChangeHeroFormTroopDataByPos(position);
            this.viewModel.EditTroopReq();
            this.OnFteDrop();

            int index = position;
            SetHighLightViewAni(index - 1, "HideScale", () => { HideAllHighlightViewAni(); });

        }

        public void RemoveHero(int position) {
            GameHelper.ClearChildren(this.viewPref.heroTransform[position - 1]);
        }

        public void RefreshHero(HeroHeadView heroHeadView, string heroName, bool visible = true) {
            heroHeadView.SetHero(
                this.viewModel.HeroDict[heroName],
                false, showEnergy: true,
                showArmyAmount: true, showLevel: true, showHeroStatus: true
            );
            heroHeadView.Visible = visible;
        }

        public void DisableEdit() {
            this.IsEdit = false;
        }

        private void RemoveHeroFromTroop(GameObject heroObj, string name, bool isSendReq = true) {
            if (EventManager.GetRecruitEventByHeroName(name) != null) {
                UIManager.ShowAlert(
                    LocalManager.GetValue(LocalHashConst.hero_recruiting));
                return;
            }
            this.viewModel.Formation.Remove(name);
            string positionName = heroObj.transform.parent.parent.name;
            Transform pnlPosition = this.viewPref.pnlHighlight.Find(positionName);
            int position = this.GetPosition(pnlPosition);
            CustomDrop drop = pnlPosition.GetComponent<CustomDrop>();
            drop.onDrop.RemoveAllListeners();
            drop.onDrop.AddListener(() => {
                this.viewModel.ResetSelect();
                this.OnHeroInTroopClick(null, string.Empty, position);
            });
            ChangeHeroFormTroopDataByName(name);
            if (isSendReq) {
                this.viewModel.EditTroopReq();
            }
        }

        Vector2 moveStartPos = Vector2.zero;
        Vector2 moveTargetPos = Vector2.zero;
        string moveTargetHeroName = string.Empty;
        string moveStartHeroName = string.Empty;
        int moveStartHeroPos = 0;
        RectTransform moveStartRTF = null;
        private void ChangeHeroFormTroopDataByName(string targetName) {
            moveTargetHeroName = targetName;
            moveStartHeroName = this.viewModel.ChosenHeroName;
            HeroHeadView heroHead = this.heroHeadFormationDic.ContainsKey(moveStartHeroName) ?
                              this.heroHeadFormationDic[moveStartHeroName] :
                              this.heroHeadDict[moveStartHeroName];
            moveStartPos = heroHead.transform.position;
            moveStartRTF = this.heroHeadFormationDic.ContainsKey(moveStartHeroName) ?
               this.heroHeadFormationDic[moveStartHeroName].GetComponent<RectTransform>() :
               this.heroHeadDict[moveStartHeroName].GetComponent<RectTransform>();
        }

        private void ChangeHeroFormTroopDataByPos(int position) {
            moveTargetHeroName = moveStartHeroName = this.viewModel.ChosenHeroName;
            moveStartHeroPos = position - 1;
            HeroHeadView heroHead = this.heroHeadFormationDic.ContainsKey(moveStartHeroName) ?
                            this.heroHeadFormationDic[moveStartHeroName] :
                            this.heroHeadDict[moveStartHeroName];
            moveStartRTF = this.viewPref.heroTransform[moveStartHeroPos].GetComponent<RectTransform>();
            moveStartPos = heroHead.transform.position;
        }

        private void ChangeHeroFormTroopUI() {
            HeroHeadView heroHead;
            bool isHeroInTroop;
            if (this.heroHeadFormationDic.TryGetValue(moveTargetHeroName, out heroHead)) {
                isHeroInTroop = true;
                moveTargetPos = heroHead.transform.position;
            } else if (this.heroHeadDict.TryGetValue(moveTargetHeroName, out heroHead)) {
                isHeroInTroop = false;
                moveTargetPos = heroHead.transform.position;
            } else {
                isHeroInTroop = false;
                moveTargetPos = new Vector2(0, 0);
            }

            this.SetPnlMoveHero(this.moveTargetHeroName);

            this.viewPref.pnlMove.transform.SetParent(this.viewPref.pnlFormation);
            if (!isHeroInTroop) {
                this.SetAttributes();
            }
            AnimationManager.Animate(
                this.viewPref.pnlMove.gameObject, "Move",
                moveStartPos, moveTargetPos,
                finishCallback: () => {
                    if (isHeroInTroop) {
                        heroHead.GetComponent<HeroHeadView>().Visible = true;
                        this.SetAttributes();
                    }
                    UIManager.SetUICanvasGroupEnable(this.viewPref.CGMove, false);
                },
                space: PositionSpace.World, isOffset: false
            );
        }

        private void SetPnlMoveHero(string name) {
            UIManager.SetUICanvasGroupEnable(this.viewPref.CGMove, true);
            this.viewPref.CGMove.transform.SetParent(this.viewPref.pnlFormation);
            this.viewPref.CGMove.transform.SetAsLastSibling();
            AnimationManager.Stop(this.viewPref.pnlMove.gameObject);
            this.viewPref.HHVMove.SetHero(this.viewModel.HeroDict[name], showFragment: false, showInfo: true,
              showLevel: true);
            this.viewPref.RTMove.position = moveStartRTF.position;
            this.viewPref.RTMove.anchoredPosition += Vector2.zero;
        }

        private enum OnClickType {
            InTroop,
            InGrid,
            InNone
        }
        private OnClickType curOnClickType = OnClickType.InNone;
        private void OnHeroInTroopClick(GameObject heroObj, string name, int position = 0) {
            this.viewPref.btnHeroGridHight.gameObject.SetActiveSafe(false);
            if (this.IsEdit) {
                if (curOnClickType == OnClickType.InGrid) {

                    if (!this.viewModel.IsHeroInTroop(this.viewModel.ChosenHeroName)) {
                        this.AddHeroToTroop(heroObj, name, position);
                    } else {
                        if (position == 0) {
                            Debug.LogError(curOnClickType);
                            this.ChangeHeroInTroopBetweenByName(heroObj, name);
                            this.ChangeHeroFormTroopDataByName(name);
                        } else {
                            this.ChangeHeroInTroopBetweenByPos(heroObj, position);
                            this.ChangeHeroFormTroopDataByPos(position);
                        }
                    }
                } else if (curOnClickType == OnClickType.InTroop) {
                    if (position == 0) {
                        this.ChangeHeroInTroopPositionByName(heroObj, name);
                        ChangeHeroFormTroopDataByName(name);
                    } else {
                        this.ChangeHeroInTroopPositionByPos(heroObj, position);
                        this.ChangeHeroFormTroopDataByPos(position);
                    }
                }
            } else if (heroObj != null) {
                this.viewPref.btnHeroGridHight.gameObject.SetActiveSafe(true);
                HeroHeadView heroHead;
                if (this.heroHeadFormationDic.TryGetValue(name, out heroHead)) {
                    this.OnHeroInGridClick(name, () => heroHead.SetHero(this.viewModel.HeroDict[name]), true);
                    this.viewModel.ChosenHeroName = name;
                    this.IsEdit = true;
                }
            }
            AddClickAndDropToHeroGridHight(heroObj, name);
            curOnClickType = OnClickType.InTroop;
        }

        private void ChangeHeroInTroopBetweenByPos(GameObject heroObj, int position) {
            this.viewModel.IsAddHeroSuccess = true;
            this.IsEdit = false;

            int chosenHeroPos = GetChosenHeroPosition();

            string name = GetTargetHeroName(position);
            if (name != null) {
                this.viewModel.ReplaceHeroName = name;
                this.viewModel.ExchangeTroopHeroReq(SwapHeroBetweenTroopByName(name, chosenHeroPos));
            } else {
                this.viewModel.ExchangeTroopHeroReq(SwapHeroBetweenTroopByPos(position, chosenHeroPos));

            }
            int index = position;
            SetHighLightViewAni(index - 1, "HideScale", () => { HideAllHighlightViewAni(); });
        }

        private void ChangeHeroInTroopBetweenByName(GameObject heroObj, string name) {
            this.viewModel.IsAddHeroSuccess = true;
            this.IsEdit = false;
            int chosenHeroPos = GetChosenHeroPosition();
            this.viewModel.ExchangeTroopHeroReq(SwapHeroBetweenTroopByName(name, chosenHeroPos));

            int index = this.viewModel.Formation[name].Position;
            SetHighLightViewAni(index - 1, "HideScale", () => { HideAllHighlightViewAni(); });
        }

        private int GetChosenHeroPosition() {
            List<HeroPosition> heroPositionList =
                this.viewModel.TroopDict[this.viewModel.GetHeroTroopByHeroName(this.viewModel.ChosenHeroName)].Positions;
            int chosenHeroPos = 0;
            for (int i = 0; i < heroPositionList.Count; i++) {
                if (this.viewModel.ChosenHeroName == heroPositionList[i].Name) {
                    chosenHeroPos = heroPositionList[i].Position;
                    break;
                }
            }

            return chosenHeroPos;
        }

        private string GetTargetHeroName(int position) {
            foreach (var pair in this.viewModel.Formation) {
                if (pair.Value.Position == position) {
                    return pair.Value.Name;
                }
            }
            return null;
        }

        private List<Formation> SwapHeroBetweenTroopByPos(int targetPosition, int chosenHeroPos) {
            return new List<Formation> {
                new Formation {
                     Id = "",
                    TroopId = this.viewModel.CurrentTroop,
                    Position = targetPosition},
                new Formation {
                    Id = this.viewModel.GetHeroIdByHeroName(this.viewModel.ChosenHeroName),
                    TroopId =  this.viewModel.GetHeroTroopByHeroName(this.viewModel.ChosenHeroName),
                    Position = chosenHeroPos},
            };
        }

        private List<Formation> SwapHeroBetweenTroopByName(string name, int chosenHeroPos) {
            return new List<Formation> {
                new Formation {
                    Id = this.viewModel.GetHeroIdByHeroName(name),
                    TroopId = this.viewModel.GetHeroTroopByHeroName(name),
                    Position = this.viewModel.Formation[name].Position},
                new Formation {
                    Id = this.viewModel.GetHeroIdByHeroName(this.viewModel.ChosenHeroName),
                    TroopId =  this.viewModel.GetHeroTroopByHeroName(this.viewModel.ChosenHeroName),
                    Position = chosenHeroPos},
            };
        }

        private void AddClickAndDropToHeroGridHight(GameObject heroObj, string name) {
            this.viewPref.btnHeroGridHight.onClick.RemoveAllListeners();
            this.viewPref.btnHeroGridHight.onClick.AddListener(() => {
                this.viewModel.ResetSelect();
                this.viewPref.btnHeroGridHight.gameObject.SetActiveSafe(false);
                this.viewModel.IsAddHeroSuccess = true;
                this.IsEdit = false;
                this.RemoveHeroFromTroop(heroObj, name);
            });

            this.viewPref.dropHeroGridHight.onDrop.RemoveAllListeners();
            this.viewPref.dropHeroGridHight.onDrop.AddListener(() => {
                this.viewModel.ResetSelect();
                this.viewModel.IsAddHeroSuccess = true;
                this.IsEdit = false;
                this.RemoveHeroFromTroop(heroObj, name);
            });
        }

        private void ChangeHeroInTroopPositionByPos(GameObject heroObj, int position) {
            string targetName = null;
            foreach (var pair in this.viewModel.Formation) {
                if (pair.Value.Position == position) {
                    targetName = pair.Key;
                    break;
                }
            }
            if (targetName == null) {
                this.viewModel.ExchangeTroopHeroReq(this.SwapHeroInTroopByPos(position));
            } else {
                this.viewModel.ExchangeTroopHeroReq(SwapHeroInTroopByName(targetName));
            }

            this.viewModel.IsAddHeroSuccess = true;
            this.IsEdit = false;
            int index = position;
            SetHighLightViewAni(index - 1, "HideScale", () => { HideAllHighlightViewAni(); });
        }

        private void ChangeHeroInTroopPositionByName(GameObject heroObj, string name) {
            this.viewModel.IsAddHeroSuccess = true;
            this.IsEdit = false;
            if (name != this.viewModel.ChosenHeroName) {
                this.viewModel.ExchangeTroopHeroReq(SwapHeroInTroopByName(name));
            }

            int index = this.viewModel.Formation[name].Position;
            SetHighLightViewAni(index - 1, "HideScale", () => { HideAllHighlightViewAni(); });
        }

        private List<Formation> SwapHeroInTroopByPos(int position) {
            return new List<Formation> {
                new Formation {
                    Id = this.viewModel.GetHeroIdByHeroName(this.viewModel.ChosenHeroName),
                    TroopId = this.viewModel.GetHeroTroopByHeroName(this.viewModel.ChosenHeroName),
                    Position = this.viewModel.Formation[this.viewModel.ChosenHeroName].Position
                },
                new Formation {
                    Id = this.viewModel.GetHeroIdByPosition(position),
                    TroopId =  this.viewModel.GetHeroTroopByHeroName(this.viewModel.ChosenHeroName),
                    Position = position
                },
            };
        }

        private List<Formation> SwapHeroInTroopByName(string name) {
            return new List<Formation> {
                new Formation {
                    Id = this.viewModel.GetHeroIdByHeroName(name),
                    TroopId = this.viewModel.GetHeroTroopByHeroName(name),
                    Position = this.viewModel.Formation[name].Position},
                new Formation {
                    Id = this.viewModel.GetHeroIdByHeroName(this.viewModel.ChosenHeroName),
                    TroopId =  this.viewModel.GetHeroTroopByHeroName(this.viewModel.ChosenHeroName),
                    Position = this.viewModel.Formation[this.viewModel.ChosenHeroName].Position},
            };
        }

        private void OnBtnRecruitClick() {
            this.viewModel.ShowRecruit();
        }

        private void OnBtnOrderClick() {
            this.OnHeroGridDrag();
            switch (this.viewModel.HeroSortBy) {
                case HeroSortType.Level:
                    this.viewModel.HeroSortBy = HeroSortType.Rarity;
                    break;
                case HeroSortType.Rarity:
                    this.viewModel.HeroSortBy = HeroSortType.Power;
                    break;
                case HeroSortType.Power:
                    this.viewModel.HeroSortBy = HeroSortType.Level;
                    break;
                default:
                    break;
            }
            this.SetTroopFormat();
        }

        private void SetBtnSortTitle() {
            switch (this.viewModel.HeroSortBy) {
                case HeroSortType.Level:
                    this.viewPref.txtOrderLabel.text =
                        LocalManager.GetValue(LocalHashConst.troop_format_order_level);
                    break;
                case HeroSortType.Rarity:
                    this.viewPref.txtOrderLabel.text =
                        LocalManager.GetValue(LocalHashConst.troop_format_order_rarity);
                    break;
                case HeroSortType.Power:
                    this.viewPref.txtOrderLabel.text =
                        LocalManager.GetValue(LocalHashConst.troop_format_order_power);
                    break;
                default:
                    break;
            }
        }

        private void OnHeroInGridClick(string name, UnityAction levelUpCallback, bool isInTroop = false) {
            this.viewPref.btnCloseSelect.gameObject.SetActiveSafe(true);
            UIManager.SetUICanvasGroupEnable(this.viewPref.selectCanvasGroup, true);
            this.viewPref.selectCanvasGroup.transform.SetParent(this.viewPref.pnlFormation);
            this.viewPref.selectCanvasGroup.transform.SetAsLastSibling();
            AnimationManager.Stop(this.viewPref.pnlSelect.gameObject);
            this.ShowFteFormation();
            this.viewPref.btnInfo.onClick.RemoveAllListeners();
            this.viewPref.btnInfo.onClick.AddListener(() => this.viewModel.ShowHeroInfo(name,
                () => {
                    this.viewPref.selectHeroHead.SetHero(this.viewModel.HeroDict[name],
                        showFragment: false, showInfo: true, showLevel: true);
                    levelUpCallback.InvokeSafe();
                }));
            this.viewModel.ChosenHeroName = name;
            RectTransform heroRectTransform;
            if (!isInTroop) {
                heroRectTransform = this.heroHeadDict[name].GetComponent<RectTransform>();
            } else {
                heroRectTransform = this.heroHeadFormationDic[name].GetComponent<RectTransform>();
            }
            this.viewPref.selectHeroHead.SetHero(
                this.viewModel.HeroDict[name],
                showFragment: false, showInfo: true,
                showLevel: true
            );

            this.viewPref.selectRectTransform.position = heroRectTransform.position;
            this.viewPref.selectRectTransform.anchoredPosition +=
                new Vector2(0, 10 + 0.5f * (heroRectTransform.rect.height - this.viewPref.selectRectTransform.rect.height));
            this.IsEdit = true;
            curOnClickType = OnClickType.InGrid;
        }

        private void OnHeroGridDrag() {
            //this.needMove = false;
            this.IsEdit = false;
            AnimationManager.Stop(this.viewPref.pnlSelect.gameObject);
            this.HideFteFormation();
            UIManager.SetUICanvasGroupEnable(this.viewPref.selectCanvasGroup, false);
            this.viewPref.selectCanvasGroup.transform.SetParent(this.viewPref.pnlFormation);


        }

        protected void OnBtnCloseClick() {
            this.viewPref.selectCanvasGroup.transform.SetParent(
               this.viewPref.showObj.transform
            );
            AnimationManager.Stop(this.viewPref.pnlSelect.gameObject);
            UIManager.SetUICanvasGroupEnable(this.viewPref.selectCanvasGroup, false);
            this.viewModel.Hide();
        }

        #region FTE
        public void OnTroopStep3Start() {
            // Judge tier.
            this.viewPref.scrollRect.vertical = false;
            this.afterShowCallback = () => {
                foreach (var pair in this.heroHeadDict) {
                    if (pair.Value.HeroStatus == HeroStatus.None &&
                        pair.Value.gameObject.activeSelf) {
                        StartCoroutine(this.DelayOnTroopStep3Start(pair.Value.transform, pair.Key));
                        return;
                    }
                }
                FteManager.StopFte();
            };
        }

        private IEnumerator DelayOnTroopStep3Start(Transform item, string heroName) {
            yield return YieldManager.EndOfFrame;
            FteManager.SetMask(item, arrowParent: this.ui.transform);
            Debug.Log("SetCurHero:" + heroName);
            FteManager.SetCurHero(heroName);
        }


        private GameObject pnlFteFormat;
        public void OnTroopStep4Start() {
            string name = FteManager.GetCurHero();
            HeroHeadView heroHead;
            if (this.heroHeadDict.TryGetValue(name, out heroHead) && heroHead.gameObject.activeSelf) {
                this.OnHeroInGridClick(name, () => heroHead.SetHero(this.viewModel.HeroDict[name], showEnergy: true, showArmyAmount: true));
                FteManager.SetArrow(this.viewPref.pnlSelect, arrowParent: this.ui.transform);
                this.pnlFteFormat = PoolManager.GetObject(PrefabPath.pnlFteFormat,
                    this.ui.transform);
                this.pnlFteFormat.transform.Find("PnlHeroBig").GetComponent<HeroHeadView>().SetHero(
                    heroHead.HeroConf
                );
                int position = this.GetEmptyPosition();
                Vector2 origin =
                    MapUtils.WorldToUIPoint(this.viewPref.pnlSelectHeroAvatar.position);
                Vector2 target =
                    MapUtils.WorldToUIPoint(this.viewPref.pnlTroopGrid.Find("Position" + position).position);
                AnimationManager.Animate(this.pnlFteFormat, "Move", origin, target, isOffset: false);
            } else {
                FteManager.StopFte();
                this.viewModel.StartChapterDailyGuid();
            }
        }

        public void OnTroopStep4End() {
            this.viewPref.scrollRect.vertical = true;
            AnimationManager.Stop(this.pnlFteFormat);
            PoolManager.RemoveObject(this.pnlFteFormat);
        }

        private int GetEmptyPosition() {
            for (int i = 1; i <= 6; i++) {
                GameObject posHighlightObj = this.viewPref.pnlHighlight.Find("Position" + i).gameObject;
                Transform posTrans = this.viewPref.pnlTroopGrid.Find("Position" + i);
                if (posHighlightObj.activeSelf && posTrans.Find("PnlHero").childCount == 0) {
                    return i;
                }
            }
            return 1;
        }

        public void HideFteFormation() {
            if (this.pnlFteFormat != null) {
                this.pnlFteFormat.gameObject.SetActiveSafe(false);
            }
        }

        public void ShowFteFormation() {
            if (this.pnlFteFormat != null) {
                this.pnlFteFormat.gameObject.SetActiveSafe(true);
            }
        }

        public void OnFteDrop() {
            if (this.pnlFteFormat != null && FteManager.Instance.curStep.Contains("chapter")) {
                FteManager.EndFte(true, false);
            }
        }

        #endregion

        protected override void OnInvisible() {
            //UpdateManager.Unregist(UpdateInfo.TroopFormationView);
            this.IsEdit = false;
            AnimationManager.Stop(this.viewPref.pnlSelect.gameObject);
            if (this.pnlFteFormat != null) {
                PoolManager.RemoveObject(this.pnlFteFormat.gameObject);
                this.pnlFteFormat = null;
            }
        }
    }
}
