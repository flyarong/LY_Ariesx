using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class CampaignCitySelectView : BaseView {
        private CampaignCitySelectViewModel viewModel;
        private CampaignCitySelectViewPreference viewPref;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<CampaignCitySelectViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UICampaignCitySelect");
            /* Cache the ui components here */
            this.viewPref = this.ui.transform.GetComponent<CampaignCitySelectViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnNext.onClick.AddListener(() => {
                this.viewModel.ShowCityConfirmView();
            });
        }

        public void SetCitiesList(List<NPCCityConf> citiesList) {
            this.viewModel.CurrentCoord = new Vector2();
            GameHelper.ClearChildren(this.viewPref.pnlList);
            AllianceCityItemView itemView;
            int listCount = citiesList.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlList,
               listCount, PrefabPath.pnlAllianceCityItem);
            if (listCount > 0) {
                for (int i = 0; i < listCount; i++) {
                    NPCCityConf cityConf;
                    int index = i;
                    itemView = this.viewPref.pnlList.GetChild(index).
                        GetComponent<AllianceCityItemView>();
                    cityConf = citiesList[index];
                    Vector2 coord = MiniMapCityConf.GetConf(cityConf.id).coordinate;
                    itemView.SetItem(cityConf.id, ElementType.npc_city, string.Empty, () => {
                        this.OnCityItemInfoClick(cityConf, index, citiesList);
                    }, coord == this.viewModel.CurrentCoord);
                }
            }
        }
        
        private void OnCityItemInfoClick(NPCCityConf cityConf, int index
            , List<NPCCityConf> citiesList) {
            AllianceCityItemView itemView;
            int listCount = citiesList.Count;
            for (int i = 0; i < listCount; i++) {
                itemView = this.viewPref.pnlList.GetChild(i).
                GetComponent<AllianceCityItemView>();
                itemView.SetItemIsChosen(i == index);
            }
            this.viewModel.OnCityItemInfoClick(cityConf);
        }
        /* Propert change function */

        /***************************/

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
