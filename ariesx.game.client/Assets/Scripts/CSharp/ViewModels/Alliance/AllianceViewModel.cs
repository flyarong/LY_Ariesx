using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceViewModel : BaseViewModel, IViewModel {
        private MapViewModel parent;
        private AllianceDetailModel allianceDetailModel;
        private AllianceInfoModel allianceInfoModel;
        private AllianceView view;
        private BuildModel buildModel;
        private MapMarkModel markModel;

        /* Model data get set */
        public List<MapMark> MarkList {
            get {
                return this.markModel.markList;
            }
        }

        public Dictionary<Vector3, MapMark> MarkDict {
            get {
                return this.markModel.markDict;
            }
        }

        public System.Type PreviouseView {
            get; set;
        }

        public string SelfAllianceId {
            get {
                return RoleManager.GetAllianceId();
            }
        }

        private int TownhallRequireLevel {
            get {
                return AllianceModel.townhallRequireLevel;
            }
        }

        private Alliance CurrentViewAlliance {
            get {
                return this.allianceInfoModel.viewAlliance;
            }
        }
        /**********************/
        public bool isInitiativeQuitAlliance = false;
        private AllianceRole preAllianceRole = AllianceRole.None;
        /* Other members */
        private AllianceCreateOrJoinViewModel createOrJoinViewModel = null;
        private AllianceCreateOrJoinViewModel CreateOrJoinViewModel {
            get {
                return this.createOrJoinViewModel ?? (this.createOrJoinViewModel =
                           PoolManager.GetObject<AllianceCreateOrJoinViewModel>(this.transform));
            }
        }
        private AllianceDetailViewModel detailViewModel = null;
        private AllianceDetailViewModel DetailViewModel {
            get {
                return this.detailViewModel ??
                       (this.detailViewModel = PoolManager.GetObject<AllianceDetailViewModel>(this.transform));
            }
        }
        private AllianceSubWindowsViewModel subWindowsViewModel = null;
        private AllianceSubWindowsViewModel SubWindowsViewModel {
            get {
                return this.subWindowsViewModel ?? (this.subWindowsViewModel =
                           PoolManager.GetObject<AllianceSubWindowsViewModel>(this.transform));
            }
        }

        private AllianceMembersViewModel allianceMembersViewModel;
        private AllianceInfoViewModel allianceInfoViewModel;
        private AllianceDisplayBoardViewModel allianceDisplayBoardViewModel;

        private string oldAllianceId = string.Empty;
        private string curAllianceId = string.Empty;

        /*****************/
        void Awake() {
            //ConfigureManager.Instance.LoadConfigure<AllianceLanguageConf>("alliance_language");
            this.view = this.gameObject.AddComponent<AllianceView>();
            this.parent = this.transform.parent.GetComponent<MapViewModel>();
            this.allianceDetailModel = ModelManager.GetModelData<AllianceDetailModel>();
            this.allianceInfoModel = ModelManager.GetModelData<AllianceInfoModel>();
            this.buildModel = ModelManager.GetModelData<BuildModel>();
            this.markModel = ModelManager.GetModelData<MapMarkModel>();
            this.allianceDisplayBoardViewModel =
                           PoolManager.GetObject<AllianceDisplayBoardViewModel>(this.transform);
            //this.allianceDisplayBoardViewModel
            FteManager.SetStartCallback(GameConst.JOIN_ALLIANCE, 2, this.OnAllianceStep2Start);
            FteManager.SetEndCallback(GameConst.JOIN_ALLIANCE, 2, this.OnAllianceStep2End);
            NetHandler.AddDataHandler(typeof(RelationNtf).Name, this.OnRelationChangeNtf);
            NetHandler.AddDataHandler(typeof(AllianceMarksNtf).Name, this.AllAllianceMarksNtf);
            NetHandler.AddDataHandler(typeof(AllianceMarkNtf).Name, this.AllianceMarkNtf);
        }

        public void Show() {
            bool isInAlliance = !this.SelfAllianceId.CustomIsEmpty();
            FteManager.HideFteMask();
            if (isInAlliance) {
                this.ShowAllianceDetail();
            } else {
                this.CreateOrJoinViewModel.Show();
            }
        }

        public void ShowAllianceDisplayBoard(DisplayType tape) {
            this.allianceDisplayBoardViewModel.Show(tape);
        }

        public void ShowAllianceMembersList(string allianceId) {
            if (this.allianceMembersViewModel == null) {
                this.allianceMembersViewModel =
                    PoolManager.GetObject<AllianceMembersViewModel>(this.transform);
            }
            this.allianceMembersViewModel.ShowAllianceMembersList(allianceId);
        }

        public void ShowAllianceInfo(string allianceId = "") {
            if (allianceId.CustomIsEmpty() ||
                allianceId.CustomEquals(RoleManager.GetAllianceId())) {
                this.Show();
            } else {
                if (this.allianceInfoViewModel == null) {
                    this.allianceInfoViewModel =
                        PoolManager.GetObject<AllianceInfoViewModel>(this.transform);
                }
                this.allianceInfoViewModel.ShowAllianceInfo(allianceId);
            }
        }

        public void ShowSubWindowByType(AllianceSubWindowType type) {
            this.SubWindowsViewModel.ShowSubWindow(type);
        }

        public void RefreshAllianeDetailByType(AllianceViewType type) {
            this.DetailViewModel.RefreshAllianeDetailByType(type);
        }

        public void HideAllianceDetail() {
            this.DetailViewModel.Hide();
        }

        public void HideSubWindows() {
            this.SubWindowsViewModel.HideAllSubView();
        }

        public void ApplyJoinAlliance(string message) {
            this.CreateOrJoinViewModel.ApplyToJoinAlliance(
                this.CurrentViewAlliance.Id, message);
        }

        public void OnAllianceLogoChange(int logoId) {
            this.CreateOrJoinViewModel.OnAllianceLogoChange(logoId);
        }

        public void ShowAllianceChat() {
            this.view.afterHideCallback = this.parent.ShowAllianceChatroom;
            this.Hide();
        }

        public void RefreshMarkInTile(Vector2 coord, MapMarkType type, bool isAdd) {
            this.parent.RefreshMarkInTile(coord, type, isAdd);
        }

        public void ShowPlayerDetailInfo(string playerId) {
            this.parent.ShowPlayerDetailInfo(playerId);
        }

        public void ShowAllianceMemOperate(PlayerPublicInfo playerInfo,
            ButtonClickWithLabel greenBtnInfo, ButtonClickWithLabel redBtnInfo) {
            this.parent.ShowAllianceMemOperate(playerInfo, greenBtnInfo, redBtnInfo);
        }

        public bool CanCreateAlliance() {
            return this.SelfAllianceId.CustomIsEmpty() &&
           buildModel.GetBuildLevelByName(ElementName.townhall) >=
           this.TownhallRequireLevel;

        }

        private void ShowAllianceDetail() {
            this.view.PlayShow(() => {
                this.parent.OnAddViewAboveMap(this);
            }, true);
            this.DetailViewModel.Show();
        }

        public void Hide() {
            this.view.PlayHide(() => {
                this.HideAllSubView();
                this.OnHideCallback();
            });
        }

        public void HideImmediatly() {
            this.view.HideImmediatly(this.OnHideCallback);
        }

        private void OnHideCallback() {
            this.parent.OnRemoveViewAboveMap(this);
        }

        public void Move(Vector2 coordinate) {
            this.parent.Move(coordinate);
        }

        public void AddAllianceMark(string markName, Vector2 coordinate) {
            AddAllianceMarkReq addAllianceMarkReq = new AddAllianceMarkReq() {
                Name = markName,
                Coord = coordinate
            };
            NetManager.SendMessage(addAllianceMarkReq, string.Empty, null);
        }

        public void DeleteAllianceMarkReq(Coord coordinate) {
            DelAllianceMarkReq deleteAllianceMark = new DelAllianceMarkReq() {
                Coord = coordinate
            };
            NetManager.SendMessage(deleteAllianceMark, string.Empty, null);
        }

        public void HideCurrentPanel() {
            this.view.HideCurrentPanel();
        }

        public void ShowSearch(bool visible) {
            this.CreateOrJoinViewModel.ShowSearch(visible);
        }

        public void RefreshAfterJoinAlliance() {
            if (string.IsNullOrEmpty(this.SelfAllianceId)) {
                this.Hide();
            }
        }

        public void RefreshAfterQuitAlliance() {
            this.view.afterHideCallback = this.CreateOrJoinViewModel.Show;
            this.Hide();
        }

        public void HideAllSubView() {
            this.DetailViewModel.Hide();
        }

        public void OnAddViewAboveMap(IViewModel baseViewModel) {
            this.parent.OnAddViewAboveMap(baseViewModel);
        }

        public void OnRemoveViewAboveMap(IViewModel baseViewModel) {
            this.parent.OnRemoveViewAboveMap(baseViewModel);
        }

        private void OnBeenKickcedOutALliance() {
            if (!this.isInitiativeQuitAlliance) {
                string tips = (this.preAllianceRole != AllianceRole.Owner) ?
                    LocalManager.GetValue(LocalHashConst.kicked_out_alliance) :
                    LocalManager.GetValue(LocalHashConst.alliance_dissolve_done);
                UIManager.ShowTip(tips, TipType.Info);
                this.isInitiativeQuitAlliance = false;
            }
            this.isBeExcludedALliance = true;
        }

        private bool isBeExcludedALliance = false;
        private void NoticeAllianceChange(RelationNtf relationNtf) {
            this.isBeExcludedALliance = false;
            if (this.curAllianceId.CustomIsEmpty() &&
                !this.oldAllianceId.CustomIsEmpty()) {
                this.DetailViewModel.ResetUserAllianceInfo();
                this.parent.NoticeAllianceStatusChange();
                this.OnBeenKickcedOutALliance();
                TriggerManager.Invoke(Trigger.BeenKickedOutAlliance);
            } else {
                if (this.view.IsVisible) {
                    //Debug.LogError(relationNtf.AllianceName);
                    AllianceRole currentAllianceRole = (AllianceRole)relationNtf.AllianceRole;
                    bool roleStatusChange = currentAllianceRole != AllianceRole.None &&
                        currentAllianceRole != this.preAllianceRole;
                    if (roleStatusChange) {
                        this.DetailViewModel.GetMyAlliance();
                    }

                    if (!this.oldAllianceId.CustomEquals(this.SelfAllianceId)) {
                        this.parent.NoticeAllianceStatusChange();
                    }
                }
                this.parent.ChatAllianceMessagesReq();
            }
        }

        private void OnRelationChangeNtf(IExtensible message) {
            RelationNtf relationNtf = message as RelationNtf;
            this.preAllianceRole = RoleManager.GetAllianceRole();
            this.oldAllianceId = RoleManager.GetAllianceId();
            this.curAllianceId = relationNtf.AllianceId;
            string oldMasterId = RoleManager.GetMasterAllianceId();
            string curMasterId = relationNtf.MasterAllianceId;
            this.NoticeAllianceChange(relationNtf);
            if (RoleManager.GetAlliance() == null) {
                this.allianceDetailModel.Refresh(relationNtf);
            } else {
                RoleManager.GetAlliance().Id = relationNtf.AllianceId;
                RoleManager.GetAlliance().Name = relationNtf.AllianceName;
            }
            RoleManager.ResetOwnAllianceRole(relationNtf.AllianceRole);
            RoleManager.SetMasterAllianceInfo(relationNtf.MasterAllianceName,
                relationNtf.MasterAllianceId);
            if (!oldMasterId.CustomIsEmpty()) {
                this.parent.OnAllianceChange(oldMasterId);
            }
            if (!curMasterId.CustomIsEmpty()) {
                this.parent.OnAllianceChange(curMasterId);
            }
            if ((!oldAllianceId.CustomIsEmpty() &&
                  this.curAllianceId.CustomIsEmpty())) {
                this.parent.OnAllianceChange(this.oldAllianceId);
            } else if (this.oldAllianceId.CustomIsEmpty() &&
                     !this.curAllianceId.CustomIsEmpty()) {
                this.parent.OnAllianceChange(this.curAllianceId);
            }

            if (this.view.IsVisible) {
                if (this.isBeExcludedALliance) {
                    this.view.afterHideCallback = this.CreateOrJoinViewModel.Show;
                    this.Hide();
                } else {
                    this.Show();
                }
            }
        }

        public void JumbCityItemCood(Coord coord) {
            this.Hide();
            this.parent.MoveWithClick(coord);
        }

        private void AllAllianceMarksNtf(IExtensible message) {
            AllianceMarksNtf allianceMarksNtf = message as AllianceMarksNtf;
            if (allianceMarksNtf.Marks.Count <= 0) {
                return;
            }

            foreach (Mark mark in allianceMarksNtf.Marks) {
                string markKey = string.Concat(AllianceModel.AllianceMarkPre, mark.Coord.X, mark.Coord.Y);
                bool isMarkRecorded = PlayerPrefs.GetString(markKey) == AllianceModel.AllianceMarkRecorde;
                MapMark mapMark = new MapMark {
                    mark = mark,
                    type = MapMarkType.Alliance,
                    isNew = !isMarkRecorded
                };
                Vector3 coordinate = new Vector3(mark.Coord.X, mark.Coord.Y, (int)MapMarkType.Alliance);
                if (!this.MarkDict.ContainsKey(coordinate)) {
                    this.MarkList.Insert(0, mapMark);
                    this.MarkDict.Add(coordinate, mapMark);
                };
                this.RefreshMarkInTile(coordinate, MapMarkType.Alliance, true);
            }
        }

        private void AddAllianceMark(Coord coord, MapMark mark) {
            Vector3 coordinate = new Vector3(
                coord.X,
                coord.Y,
                (int)MapMarkType.Alliance
            );
            if (!this.MarkDict.ContainsKey(coordinate)) {
                this.MarkList.Insert(0, mark);
                this.MarkDict.Add(coordinate, mark);

                string markKey = string.Concat(AllianceModel.AllianceMarkPre, coord.X, coord.Y);
                PlayerPrefs.SetString(markKey, AllianceModel.AllianceMarkRecorde);
                PlayerPrefs.Save();
                this.RefreshMarkInTile(coordinate, MapMarkType.Alliance, true);
            }
        }

        private void DeleteAllianceMark(Coord coord) {
            Vector3 coordinate = new Vector3(
                coord.X, coord.Y, (int)MapMarkType.Alliance
            );
            MapMark mapMark;
            if (this.MarkDict.TryGetValue(coordinate, out mapMark)) {
                this.MarkDict.Remove(coordinate);
                this.MarkList.Remove(mapMark);
                string markKey = string.Concat(
                    AllianceModel.AllianceMarkPre, coordinate.x, coordinate.y);
                PlayerPrefs.DeleteKey(markKey);
                this.RefreshMarkInTile(coordinate, MapMarkType.Alliance, false);
            }
        }

        private void AllianceMarkNtf(IExtensible message) {
            AllianceMarkNtf allianceMarkNtf = message as AllianceMarkNtf;
            if (allianceMarkNtf.Method.CustomEquals("new")) {
                MapMark mapMark = new MapMark {
                    mark = allianceMarkNtf.Mark,
                    type = MapMarkType.Alliance
                };
                this.AddAllianceMark(allianceMarkNtf.Mark.Coord, mapMark);
            } else if (allianceMarkNtf.Method.CustomEquals("del")) {
                this.DeleteAllianceMark(allianceMarkNtf.Mark.Coord);
            }
        }

        #region FTE

        private void OnAllianceStep2Start(string index) {
            this.view.afterHideCallback = FteManager.StopFte;
            // this.view.afterShowCallback = this.
            this.Show();
        }

        private void OnAllianceStep2End() {
            this.view.afterHideCallback = null;
        }

        #endregion
    }
}
