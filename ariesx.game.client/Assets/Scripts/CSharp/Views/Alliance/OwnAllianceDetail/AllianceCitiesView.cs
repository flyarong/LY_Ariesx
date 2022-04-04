using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Protocol;

namespace Poukoute {
    public class AllianceCitiesView: BaseView {
        private AllianceCitiesViewModel viewModel;
        private AllianceCitiesViewViewPreference viewPref;
        /*************/

        protected override void OnUIInit() {
            GameObject viewHoldler = UIManager.GetUI("UIAlliance.PnlAlliance.PnlCitiesHoldler");
            PrefabLoader viewLoadler = viewHoldler.GetComponent<PrefabLoader>();
            this.ui = viewLoadler.LoadSubPrefab();
            //this.ui = UIManager.GetUI("UIAlliance.PnlAlliance.PnlCities");
            this.viewModel = this.gameObject.GetComponent<AllianceCitiesViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AllianceCitiesViewViewPreference>();
        }

        public void RefreshCitiesList() {
            this.RefreshList();
            this.SetCityHeadContent();
        }

        // to do : need set city head content
        private void SetCityHeadContent() {
            this.viewPref.cityHeadPre.txtCityCount.text = this.viewModel.CitiesCount.ToString();
            this.viewPref.cityHeadPre.txtPassCount.text = this.viewModel.PassesCount.ToString();

            Dictionary<Resource, int> resourceBuff =
                this.viewModel.GetCityResourceBuffAddition();
            this.SetResourceBonus(this.viewPref.cityHeadPre.txtLumber,
                                  resourceBuff[Resource.Lumber]);
            this.SetResourceBonus(this.viewPref.cityHeadPre.txtSteel,
                                  resourceBuff[Resource.Steel]);
            this.SetResourceBonus(this.viewPref.cityHeadPre.txtMarble,
                                  resourceBuff[Resource.Marble]);
            this.SetResourceBonus(this.viewPref.cityHeadPre.txtFood,
                                  resourceBuff[Resource.Food]);
        }

        private void SetResourceBonus(TextMeshProUGUI resource, int amount) {
            resource.text = string.Concat("+", (amount > 0 ? amount.ToString() : "0/ "),
                    LocalManager.GetValue(LocalHashConst.time_hour));
        }

        private void FormatList() {
            this.viewPref.listVerticalLayoutGroup.CalculateLayoutInputHorizontal();
            this.viewPref.listVerticalLayoutGroup.CalculateLayoutInputVertical();
            this.viewPref.listContentSizeFitter.SetLayoutVertical();
            this.viewPref.listRectTransform.anchoredPosition = new Vector2(
                this.viewPref.listRectTransform.rect.width / 2,
                -this.viewPref.listRectTransform.rect.height / 2
            );
        }

        private void RefreshList() {
            GameObject pnlAllianceListItem;
            AllianceCityItemView itemCityView;
            List<NPCCityConf> citiesList = this.viewModel.AllianceCities;
            int listCount = citiesList.Count;
            foreach (Transform child in this.viewPref.pnlList) {
                GameObject.Destroy(child.gameObject);
            }
            if (listCount > 0) {
                NPCCityConf cityConf;
                for (int i = 0; i < listCount; i++) {
                    int index = i;
                    pnlAllianceListItem = PoolManager.GetObject(PrefabPath.pnlAllianceCityItem, this.viewPref.pnlList);
                    itemCityView = pnlAllianceListItem.GetComponent<AllianceCityItemView>();
                    cityConf = citiesList[index];
                    Vector2 coord = MiniMapCityConf.GetConf(cityConf.id).coordinate;
                    itemCityView.SetItem(cityConf.id, ElementType.npc_city, "", () => {
                        this.JumbCityItemCood(coord);
                    }, false, false);
                }
            }

            //AlliancePassItemView itemPassView;
            List<MiniMapPassConf> passesList = this.viewModel.AlliancePasses;
            listCount = passesList.Count;
            if (listCount > 0) {
                MiniMapPassConf passConf;
                for (int j = 0; j < listCount; j++) {
                    int index = j;
                    pnlAllianceListItem = PoolManager.GetObject(PrefabPath.pnlAllianceCityItem, this.viewPref.pnlList);
                    itemCityView = pnlAllianceListItem.GetComponent<AllianceCityItemView>();
                    passConf = passesList[index];
                    Vector2 coord = MiniMapPassConf.GetConf(passConf.id).coordinate;
                    itemCityView.SetItem(passConf.id, ElementType.pass, "", () => {
                        this.JumbCityItemCood(coord);
                    }, false, false);

                }
            }
            this.FormatList();
        }


        /*************** btn callback ***************************/

        public void JumbCityItemCood(Coord coord) {
            string content = string.Format(LocalManager.GetValue(
                LocalHashConst.coordinate_jump_confirm),coord.ToString());
            UIManager.ShowConfirm(LocalManager.GetValue(LocalHashConst.tip_error),
            content, () => { this.viewModel.JumbCityItemCood(coord); }, () => { });
        }

        /********************************************************/
    }
}
