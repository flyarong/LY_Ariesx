using ProtoBuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Poukoute {
    public class PlayerInfoStateViewModel: BaseViewModel {
        private PlayerInfoStateView view;
        private PlayerInfoViewModel parent;
        private HouseKeeperResourcesInfoViewModel hkResModel;
        private RoleAvatarViewModel roleAvatarModel;
        private AllianceDetailModel allianceDetailModel;
        private MapModel mapModel;
        private LoginModel loginModel;
        public int tributeGole;
        private string description = string.Empty;
        public Protocol.Resources AllResourcesProduction;
        public string Description {
            get {
                return RoleManager.GetRoleDesc();
            }
            set {
                this.description = value;
                this.SetPlayerDesc();
            }
        }

        public List<object> WorldList {
            get {
                return this.loginModel.allWorldList;
            }
        }

        void Awake() {
            this.parent = this.transform.parent.GetComponent<PlayerInfoViewModel>();
            this.view = this.gameObject.AddComponent<PlayerInfoStateView>();
            this.hkResModel =
                PoolManager.GetObject<HouseKeeperResourcesInfoViewModel>(this.transform);
            this.roleAvatarModel =
                PoolManager.GetObject<RoleAvatarViewModel>(this.transform);
            this.mapModel = ModelManager.GetModelData<MapModel>();
            this.allianceDetailModel = ModelManager.GetModelData<AllianceDetailModel>();
            this.loginModel = ModelManager.GetModelData<LoginModel>();
            NetHandler.AddDataHandler(typeof(ResourcesProductionNtf).Name, this.ResourcesProductionNtf);
            NetHandler.AddDataHandler(typeof(ShortIdNtf).Name, this.ShortIdNtf);
            // NetHandler.AddDataHandler()
        }

        public void Show() {
            this.view.Show();
            this.view.SetPlayerInfo();
        }

        public void Hide(bool needRefresh = false) {
            this.view.Hide();
        }

        public void ResourcesShow() {
            this.hkResModel.Show();
        }

        public void RoleAvatarShow() {
            this.roleAvatarModel.Show();
        }

        public void GetMyAlliance() {
            GetMyAllianceReq getMyAllianceReq = new GetMyAllianceReq();
            NetManager.SendMessage(getMyAllianceReq,
                                    typeof(GetMyAllianceAck).Name,
                                    this.GetMyAllianceAck);
        }

        public string GetWorldInfo() {
            foreach (Dictionary<string, object> world in this.WorldList) {
                if (((string)world["world_id"]).Equals(RoleManager.WorldId)) {
                    string worldNum = ((System.Object)world["world_number"]).ToString();
                    string name = (string)world["name"];
                    return string.Concat(" ", worldNum, LocalManager.GetValue(
                    LocalHashConst.change_server_server), " ", name);
                    //worldNum.PadLeft(5, '0')+ " " + name;
                }
            }
            return string.Empty;
        }
        private void GetMyAllianceAck(IExtensible message) {
            GetMyAllianceAck getMyAllianceAck = message as GetMyAllianceAck;
            this.allianceDetailModel.allianceDetail.selfInfo = getMyAllianceAck.Self;
            this.allianceDetailModel.allianceDetail.alliance
                = getMyAllianceAck.Alliance;
            this.view.SetPlayerAllianceDetail();
        }

        private void ChangeNameAck(IExtensible message) {
            ChangeNameAck changeName = message as ChangeNameAck;
            this.view.OnEditNameDone(changeName.NewName);
            this.parent.RefreshPlayerName();
            TriggerManager.Invoke(Trigger.VoiceLiveUserDataChange);
        }

        public void EditNameReq(string name) {
            Debug.LogError("EditNameReq " + name);
            ChangeNameReq changNameReq = new ChangeNameReq() {
                NewName = name
            };
            NetManager.SendMessage(changNameReq, "ChangeNameAck", this.ChangeNameAck);
        }
        private void SetPlayerDesc() {
            SetPlayerDescReq setPlayerDes = new SetPlayerDescReq() {
                Desc = this.description
            };
            NetManager.SendMessage(setPlayerDes, "SetPlayerDescAck",
                this.SetPlayerDescAck);
        }

        private void SetPlayerDescAck(IExtensible message) {
            RoleManager.SetRoleDesc(this.description);
            this.view.SetRoleDesc();
        }

        public int GetTileLevel(Vector2 coordinate) {
            return this.mapModel.GetTileLevel(coordinate);
        }

        private void ResourcesProductionNtf(IExtensible message) {
            ResourcesProductionNtf resourcesProduction = message as ResourcesProductionNtf;
            this.tributeGole = resourcesProduction.TributeGoldBonus;
            this.AllResourcesProduction = resourcesProduction.AllResources;
        }

        private void ShortIdNtf(IExtensible message) {
            ShortIdNtf shordId = message as ShortIdNtf;
            Debug.Log(shordId.ShortId);
            RoleManager.ShortId = shordId.ShortId;
            if (this.view.IsVisible) {
                this.view.SetShortId();
            }
        }

        public void ChangeAvatar(string hintDes) {
            this.view.ChangeAvatar(hintDes);
            this.parent.ChangeAvatar();
        }
    }
}
