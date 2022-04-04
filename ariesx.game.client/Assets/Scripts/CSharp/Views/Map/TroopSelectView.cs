using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using TMPro;
using frame8.ScrollRectItemsAdapter.Util.GridView;

namespace Poukoute {
    public enum TroopChoseAction {
        Format,
        Recruit,
        Return,
        None
    }

    public class TroopSelectView : GridAdapter<TroopSelectViewModel, TroopSelectItemView> {
        private Transform pnlLabel;
        private Transform pnlTroop;
        private CanvasGroup troopCanvasGroup;
        private CustomScrollRect scrollRect;
        private CustomGridLayoutGroup gridLayoutGroup;
        //private TextMeshProUGUI txtNoTroop;

        private AnimationCurve positionCurve;

        private TroopSelectItemView previouseItem;
        private TroopSelectItemView currentItem;
        private TroopSelectItemView CurrentItem {
            get {
                return this.currentItem;
            }
            set {
                if (this.currentItem != value) {
                    this.currentItem = value;
                    this.OnCurrentItemChange();
                }
            }
        }
        //private bool canChangeItem = true;
        //private float dFactor = 0.2f;
        //private int count = 0;
        //private int startCenterIndex = 0;
        //private float currentDuration = 0;
        //private float centerOffset = 0.5f;
        //private int depthFactor = 5;
        //private float yFixedPositionValue = 0;
        //private float totalHorizontalWidth = 500.0f;

        //private float cellWidth = 150f;
        //private float lerpDuration = 0.2f;

        //private List<TroopSelectItemView> troopItemList = new List<TroopSelectItemView>();

        private UnityAction<Troop> choseAction = null;

        void Start() {
            this.viewModel = this.gameObject.GetComponent<TroopSelectViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UITile.PnlTile.PnlBottom.PnlTroops");
            this.scrollRect = this.ui.GetComponent<CustomScrollRect>();
            this.pnlLabel = this.ui.transform.Find("PnlLabel");
            this.pnlTroop = this.ui.transform.Find("PnlTroop");
            this.troopCanvasGroup = this.pnlTroop.GetComponent<CanvasGroup>();
            this.gridLayoutGroup = this.pnlTroop.GetComponent<CustomGridLayoutGroup>();
            //this.txtNoTroop = this.ui.transform.Find("TxtNoTroop").GetComponent<TextMeshProUGUI>();
            this.GridAdapterInit(this.scrollRect, this.gridLayoutGroup, PrefabPath.pnlTroopSelectItem);
        }

        protected override void UpdateCellViewsHolder(TroopSelectItemView itemView) {
            if (itemView.ItemIndex < this.viewModel.CurrentTroopCount) {
                itemView.SetSelfTroopInfo(
                    this.viewModel.IsAttack,
                    this.viewModel.Target,
                    this.viewModel.CurrentTroopList[itemView.ItemIndex],
                    () => this.OnTroopItemClike(itemView)
                );
            } else {
                GetPointPlayerTroopsAck.Troop troop = this.viewModel.PlayerTroopList[
                    itemView.ItemIndex - this.viewModel.CurrentTroopCount
                ];
                itemView.SetOtherTroopInfo(troop.PlayerName, troop.PlayerAvatar,
                    troop.AllianceId, troop.MasterAllianceId, () => this.OnOtherTroopItemClick(troop.PlayerId));
            }
        }

        private void OnItemContentChange(TroopSelectItemView itemView, Troop troop) {

        }

        public void HideTroop() {
            UIManager.SetUICanvasGroupEnable(this.troopCanvasGroup, false);
        }

        public void ShowTroop() {
            UIManager.SetUICanvasGroupEnable(this.troopCanvasGroup, true);
        }

        private void OnTroopItemClike(TroopSelectItemView itemView) {
            Vector2 position = itemView.Troop.Coord;
            float sqrDistance = (position - itemView.target).sqrMagnitude;
            if (this.viewModel.Target == RoleManager.GetRoleCoordinate() ||
                sqrDistance < GameConst.TROOP_FAR * GameConst.TROOP_FAR) {
                this.OnItemClick(itemView);
            } else {
                UIManager.ShowTip(LocalManager.GetValue(
                LocalHashConst.warning_troop_distance_300), TipType.Info);
            }
        }

        private void OnOtherTroopItemClick(string playerId) {
            this.viewModel.ShowPlayerInfo(playerId);
        }

        public void SetHighlight(string id) {
            TroopSelectItemView itemView;
            foreach (CellGroupViewsHolder cellGroup in this._VisibleItems) {
                for (int i = 0; i < cellGroup.NumActiveCells; i++) {
                    itemView = cellGroup.ContainingCellViewsHolders[i] as TroopSelectItemView;
                    if (itemView.Troop.Id == id) {
                        this.CurrentItem = itemView;
                        itemView.DisableHighlight();
                        return;
                    }
                }
            }
        }

        public void EnableShining() {
            TroopSelectItemView itemView;
            foreach (CellGroupViewsHolder cellGroup in this._VisibleItems) {
                for (int i = 0; i < cellGroup.NumActiveCells; i++) {
                    itemView = cellGroup.ContainingCellViewsHolders[i] as TroopSelectItemView;
                    Vector2 position = new Vector2(itemView.Troop.Coord.X, itemView.Troop.Coord.Y);
                    float sqrDistance = (position - itemView.target).sqrMagnitude;
                    if (sqrDistance >= GameConst.TROOP_FAR * GameConst.TROOP_FAR &&
                        this.viewModel.Target != RoleManager.GetRoleCoordinate()) {
                        itemView.DisableShining();
                    } else {
                        itemView.EnableShining();
                    }
                }
            }
        }

        public void DisableShining() {
            TroopSelectItemView itemView;
            foreach (CellGroupViewsHolder cellGroup in this._VisibleItems) {
                for (int i = 0; i < cellGroup.NumActiveCells; i++) {
                    itemView = cellGroup.ContainingCellViewsHolders[i] as TroopSelectItemView;
                    itemView.DisableShining();
                }
            }
        }

        private TroopChoseAction troopChosenAction = TroopChoseAction.None;
        public void SetChosenAction(TroopChoseAction type) {
            this.troopChosenAction = type;
            switch (type) {
                case TroopChoseAction.Format:
                    this.choseAction = this.OnItemChosenFormat;
                    break;
                case TroopChoseAction.Recruit:
                    this.choseAction = this.OnItemChosenRecruit;
                    break;
                case TroopChoseAction.Return:
                    this.choseAction = this.OnItemChosenReturn;
                    break;
                case TroopChoseAction.None:
                default:
                    break;
            }
        }

        public void ResetChoseCallback() {
            this.choseAction = null;
        }

        static public int SortPosition(TroopSelectItemView a, TroopSelectItemView b) {
            return a.transform.localPosition.x.CompareTo(b.transform.localPosition.x);
        }

        private void OnCurrentItemChange() {
            if (this.CurrentItem == null) {
                return;
            }
            this.CurrentItem.EnableHighlight();
            if (!this.viewModel.IsAttack) {
                return;
            }
            //this.viewModel.ShowMarchLine();
        }

        private void OnItemClick(TroopSelectItemView itemView) {
            TroopSelectItemView curItemView = this.CurrentItem;
            if (this.CurrentItem != itemView && !itemView.IsShining) {
                if (this.CurrentItem != null) {
                    this.CurrentItem.DisableHighlight();
                }
                this.CurrentItem = itemView;
                this.viewModel.ShowTroopOverview(itemView.Troop);
                if (this.viewModel.IsAttack) {
                    return;
                }
            }
            //if (!this.canChangeItem)
            //    return;
            if ((curItemView != null && curItemView == itemView) || itemView.IsShining) {
                if (choseAction == null) {
                    this.OnItemChosenNormal(itemView.Troop);
                } else {
                    this.choseAction.Invoke(itemView.Troop);
                    this.choseAction = null;
                }
            }
        }

        private void OnItemChosenNormal(Troop troop) {
            if (this.viewModel.IsAttack) {
                this.viewModel.ShowTroop(troop);
            }
        }

        private void OnItemChosenFormat(Troop troop) {
            this.viewModel.ShowTroopFormation(troop.Id);
        }

        private void OnItemChosenRecruit(Troop troop) {
            this.viewModel.ShowRecruit(troop.Id);
        }

        private void OnItemChosenReturn(Troop troop) {
            this.viewModel.TroopReturnReq(troop.Id);
        }

        #region FTE

        public void OnTroopStep2Process() {
            TroopSelectItemView itemView;
            string fteCurrentTroop = FteManager.GetCurrentTroop();
            foreach (CellGroupViewsHolder cellGroup in this._VisibleItems) {
                for (int i = 0; i < cellGroup.NumActiveCells; i++) {
                    itemView = cellGroup.ContainingCellViewsHolders[i] as TroopSelectItemView;
                    if (itemView.Troop.Id.CustomEquals(fteCurrentTroop)) {
                        itemView.EnableHighlight();
                        return;
                    }
                }
            }
        }

        public void OnResourceStep3Start(bool isEnforce) {
            if (this._VisibleItems.Count == 0) {
                FteManager.StopFte();
                this.viewModel.StartChapterDailyGuid();
            }
            
            TroopSelectItemView itemView;
            foreach (CellGroupViewsHolder cellGroup in this._VisibleItems) {
                for (int i = 0; i < cellGroup.NumActiveCells; i++) {
                    itemView = cellGroup.ContainingCellViewsHolders[i] as TroopSelectItemView;
                    if (itemView.Troop.Positions.Count > 0) {
                        StartCoroutine(this.InnerOnResourceStep3Start(itemView, isEnforce));
                        return;
                    }
                }
            }
            FteManager.StopFte();
            this.viewModel.StartChapterDailyGuid();
        }

        private IEnumerator InnerOnResourceStep3Start(TroopSelectItemView itemView, bool isEnforce) {
            yield return YieldManager.EndOfFrame;
            FteManager.SetCurrentTroop(itemView.Troop.Id, Vector2.zero, string.Empty);
            FteManager.SetMask(itemView.transform, offset: Vector2.up * 50, isEnforce: isEnforce,
                arrowParent: UIManager.GetUI("UITile.PnlTile.PnlBottom").transform);
        }
        #endregion

        protected override void OnVisible() {
            this.pnlLabel.gameObject.SetActiveSafe(true);
        }

        protected override void OnInvisible() {
            this.pnlLabel.gameObject.SetActiveSafe(false);
            this.DisableShining();
            this.currentItem = null;
        }
    }
}
