using UnityEngine.Events;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using UnityEngine;

namespace Poukoute {
    public class HouseKeeperResourcesInfoViewModel: BaseViewModel {
        //private HouseKeeperStateViewModel parent;
        private HouseKeeperResourcesInfoView view;
        private AllianceDetailModel allianceModel;
        private BuildModel buileModel;
        public Protocol.Resources AllResourcesProduction;
        public Protocol.Resources ResourcesProduction;
        public Protocol.Resources NpcResourcesProduction;
        public Protocol.Resources BuildingResourcesProduction;
        public Dictionary<Resource, int> buildResDic = new Dictionary<Resource, int>();
        public Dictionary<Resource, float> allianceResDic = new Dictionary<Resource, float>();

        void Awake() {
            //this.parent = this.transform.parent.GetComponent<HouseKeeperStateViewModel>();
            this.view = this.gameObject.AddComponent<HouseKeeperResourcesInfoView>();
            this.allianceModel = ModelManager.GetModelData<AllianceDetailModel>();
            this.buileModel = ModelManager.GetModelData<BuildModel>();
            NetHandler.AddDataHandler(typeof(ResourcesProductionNtf).Name, this.ResourcesProductionNtf);
            this.SetDict();
        }

        public void Show() {
            this.GetAllianceModelBuff();
            this.view.PlayShow();
        }

        public void Hide() {
            this.view.PlayHide();
        }

        private void SetDict() {
            buildResDic.Add(Resource.Lumber, 0);
            buildResDic.Add(Resource.Steel, 0);
            buildResDic.Add(Resource.Marble, 0);
            buildResDic.Add(Resource.Food, 0);
            allianceResDic.Add(Resource.Lumber, 0);
            allianceResDic.Add(Resource.Steel, 0);
            allianceResDic.Add(Resource.Marble, 0);
            allianceResDic.Add(Resource.Food, 0);
        }

        public Dictionary<Resource, int> GetBuildResources() {
            buildResDic[Resource.Lumber] = 0;
            buildResDic[Resource.Steel] = 0;
            buildResDic[Resource.Marble] = 0;
            buildResDic[Resource.Food] = 0;
            foreach (var item in this.buileModel.buildingDict) {
                if (item.Value.Name.Contains("produce")) {
                    ResourceProduceConf currentLevelConf =
                        ResourceProduceConf.GetConf(item.Value.Name, item.Value.Level);
                    try {
                        buildResDic[currentLevelConf.type] += currentLevelConf.bonus;
                    } catch { Debug.LogError("building..."); }
                }
            }
            return buildResDic;
        }

        public Dictionary<Resource, float> GetAllianceModelBuff() {
            bool inAllinance = RoleManager.GetAllianceId().CustomIsEmpty();
            string masterAllianceName = RoleManager.GetMasterAllianceName();
            bool isPlayerFalled = string.IsNullOrEmpty(masterAllianceName);
            if (inAllinance && isPlayerFalled) {
                allianceResDic[Resource.Lumber] = 0;
                allianceResDic[Resource.Steel] = 0;
                allianceResDic[Resource.Marble] = 0;
                allianceResDic[Resource.Food] = 0;
            } else {
                int allianceLevel = AllianceLevelConf.GetAllianceLevelByExp(allianceModel.allianceDetail.alliance.Exp);
                AllianceLevelConf allianceConf = AllianceLevelConf.GetConf(allianceLevel.ToString());
                allianceResDic[Resource.Lumber] = allianceConf.lumberbuff;
                allianceResDic[Resource.Steel] = allianceConf.steelbuff;
                allianceResDic[Resource.Marble] = allianceConf.marblebuff;
                allianceResDic[Resource.Food] = allianceConf.foodbuff;
            }
            return allianceResDic;

        }

        private void ResourcesProductionNtf(IExtensible message) {
            ResourcesProductionNtf resourcesProduction = message as ResourcesProductionNtf;
            this.AllResourcesProduction = resourcesProduction.AllResources;
            this.ResourcesProduction = resourcesProduction.Resources;
            this.NpcResourcesProduction = resourcesProduction.NpcResources;
            this.BuildingResourcesProduction = resourcesProduction.BuildingResources;
        }

    }
}
