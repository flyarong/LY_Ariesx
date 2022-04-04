using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;

namespace Poukoute {
    public class CampaignCitySelectViewModel : BaseViewModel {
        private CampaignCitySelectView view;
        private DemonShadowDetailsViewModel demonShadowViewModel;
        public NPCCityConf cityConf;
        private CampaignModel model;

        /* Model data get set */

        public Vector2 CurrentCoord {
            get; set;
        }

        public bool IsCity {
            get; set;
        }
        /**********************/

        void Awake() {
            this.view = this.gameObject.AddComponent<CampaignCitySelectView>();
            this.demonShadowViewModel = GameObject.FindObjectOfType<DemonShadowDetailsViewModel>();
            this.model = ModelManager.GetModelData<CampaignModel>();
        }

        public void Show() {
            if (!this.view.IsVisible) {
                this.view.PlayShow();
            }
            this.view.SetCitiesList(model.NPCCityConfList);
        }

        internal void ShowCityConfirmView() {
            if (IsCity) {
                UIManager.ShowConfirm(
                   LocalManager.GetValue(LocalHashConst.notice_title_warning),
                  (string.Format(LocalManager.GetValue(LocalHashConst.domination_summon_confirm_final),
                   string.Concat(NPCCityConf.GetNpcCityLocalName(cityConf.name, true)),
                   cityConf.level, string.Concat(cityConf.GetNpcCityMapZoneLocal()),
                   string.Concat(this.CurrentCoord.x, ",", this.CurrentCoord.y),
                   LocalManager.GetValue(LocalHashConst.domination_name), cityConf.level)), () => {
                       this.GenerateBoss(this.CurrentCoord);//召唤恶魔
                   }, () => { });
            } else {
                UIManager.ShowTip(
                    LocalManager.GetValue(LocalHashConst.domination_notice_choose_city), TipType.Info);
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.PlayHide();
            }
            this.IsCity = false;
        }

        /// <summary>
        /// 召唤恶魔
        /// </summary>
        public void GenerateBoss(Coord coord) {
            GetGenerateBossReq(coord);
        }
        private void GetGenerateBossReq(Coord coord) {
            GenerateBossReq generateBossReq = new GenerateBossReq();
            generateBossReq.Coord = coord;
            NetManager.SendMessage(generateBossReq,
                typeof(GenerateBossAck).Name, this.GetGenerateBossAck);
        }

        private void GetGenerateBossAck(IExtensible message) {
            GenerateBossAck generateBossAck = message as GenerateBossAck;
            //Debug.LogError("召唤BOssAck");
            if (generateBossAck.Info != null) {
                this.Hide();
                Debug.LogError(this.cityConf.name);
                this.demonShadowViewModel.ShowPnlAgainstOpen(generateBossAck.Info, this.cityConf);
            }
        }

        /// <summary>
        /// 选择城市,填充将要召唤的恶魔所在位置
        /// </summary>
        /// <param name="cityConf"></param>
        public void OnCityItemInfoClick(NPCCityConf cityConf) {
            this.CurrentCoord = MiniMapCityConf.GetConf(cityConf.id).coordinate;
            this.cityConf = cityConf;
            this.IsCity = true;
        }
        /* Add 'NetMessageAck' function here*/

        /***********************************/
    }
}
