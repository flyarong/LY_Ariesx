using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;

namespace Poukoute {
    public class HouseKeeperResourcesInfoView: BaseView {
        private HouseKeeperResourcesInfoViewModel viewModel;
        private HouseKeeperResourcesPreference viewPref;

        private const int PRODUCTION_TIMES = 10;

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<HouseKeeperResourcesInfoViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIHouseKeeperResources");
            this.viewPref = this.ui.transform.GetComponent<HouseKeeperResourcesPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.btnBackClose.onClick.AddListener(this.OnCloseBtnClick);
        }

        public override void PlayShow() {
            base.PlayShow(() => {
                this.SetDetail();
            });
        }

        public override void PlayHide() {
            base.PlayHide();
        }

        private void SetDetail() {
            this.SetAllRes();
            this.SetAllianceRes();
            this.SetBuildRes();
            this.SetNpcRes();
            this.SetRes();
            this.SetStorageRes();
        }

        private void SetStorageRes() {
            this.viewPref.txtLumberStorage.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum((long)RoleManager.GetResource(Resource.Lumber)
                , maxLength: 4, decLength: 1),
                 GameHelper.GetFormatNum((long)RoleManager.GetResourceLimit(Resource.Lumber)
                , maxLength: 4, decLength: 1));
            this.viewPref.txtSteelStorage.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum((long)RoleManager.GetResource(Resource.Steel)
                , maxLength: 4, decLength: 1),
                 GameHelper.GetFormatNum((long)RoleManager.GetResourceLimit(Resource.Steel)
                , maxLength: 4, decLength: 1));
            this.viewPref.txtMarbleStorage.text = string.Format("{0}/{1}",
               GameHelper.GetFormatNum((long)RoleManager.GetResource(Resource.Marble)
                , maxLength: 4, decLength: 1),
                 GameHelper.GetFormatNum((long)RoleManager.GetResourceLimit(Resource.Marble)
                , maxLength: 4, decLength: 1));
            this.viewPref.txtFoodStorage.text = string.Format("{0}/{1}",
               GameHelper.GetFormatNum((long)RoleManager.GetResource(Resource.Food)
                , maxLength: 4, decLength: 1),
                 GameHelper.GetFormatNum((long)RoleManager.GetResourceLimit(Resource.Food)
                , maxLength: 4, decLength: 1));
        }

        private void SetAllRes() {
            this.viewPref.txtLumberNum.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Lumber
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtSteelNum.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Steel
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtMarbleNum.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Marble
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtFoodNum.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.AllResourcesProduction.Food
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
        }

        private void SetNpcRes() {
            this.viewPref.txtLumberCity.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.NpcResourcesProduction.Lumber
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtSteelCity.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.NpcResourcesProduction.Steel
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtMarbleCity.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.NpcResourcesProduction.Marble
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtFoodCity.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.NpcResourcesProduction.Food
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
        }

        private void SetRes() {
            this.viewPref.txtLumberManor.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.ResourcesProduction.Lumber
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtSteelManor.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.ResourcesProduction.Steel
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtMarbleManor.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.ResourcesProduction.Marble
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtFoodManor.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.ResourcesProduction.Food
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
        }

        private void SetBuildRes() {
            this.viewPref.txtLumberBuild.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.BuildingResourcesProduction.Lumber
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtSteelBuild.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.BuildingResourcesProduction.Steel
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtMarbleBuild.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.BuildingResourcesProduction.Marble
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
            this.viewPref.txtFoodBuild.text = string.Format("{0}/{1}",
                GameHelper.GetFormatNum(this.viewModel.BuildingResourcesProduction.Food
                * PRODUCTION_TIMES, maxLength: 4, decLength: 1), LocalManager.GetValue(LocalHashConst.time_hour));
        }

        private void SetAllianceRes() {
            Dictionary<Resource, float> allianceResDic = new Dictionary<Resource, float>();
            allianceResDic = this.viewModel.GetAllianceModelBuff();
            this.viewPref.txtLumberAlliance.text = string.Concat((allianceResDic[Resource.Lumber] * 100), "%");
            this.viewPref.txtSteelAlliance.text = string.Concat((allianceResDic[Resource.Steel] * 100), "%");
            this.viewPref.txtMarbleAlliance.text = string.Concat((allianceResDic[Resource.Marble] * 100), "%");
            this.viewPref.txtFoodAlliance.text = string.Concat((allianceResDic[Resource.Food] * 100), "%");
        }

        private void OnCloseBtnClick() {
            this.viewModel.Hide();
        }
    }
}
