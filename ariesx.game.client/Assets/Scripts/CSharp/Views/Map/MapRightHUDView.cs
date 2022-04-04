using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class MapRightHUDView : BaseView {
        private MapRightHUDViewModel viewModel;
        //private GameObject ui;
        /* UI Members*/
        private Transform pnlNotice;

        /**********************************/

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<MapRightHUDViewModel>();
            this.ui = UIManager.GetUI("UIMapRightHUD");
            this.group = UIGroup.MapNotice;
            this.pnlNotice = this.ui.transform.Find("PnlNotice");
        }

        public int GetNoticeItemCount() {
            return this.pnlNotice.childCount;
        }

        #region Display notices
        public void ShowMapRecruitDoneNotice(bool isHeroLevelUp) {
            GameObject pnlNoticeItem =
                PoolManager.GetObject(PrefabPath.pnlRecruitNoticeItem, this.pnlNotice);
            MapRecruitNoticeItemView noticeItemView =
                    pnlNoticeItem.GetComponent<MapRecruitNoticeItemView>();
            noticeItemView.SetItemContent(this.viewModel.CurrentHero, isHeroLevelUp);
            string heroName = this.viewModel.CurrentHero.Name;
            noticeItemView.OnClick.AddListener(() => this.OnHeroClick(heroName));
            noticeItemView.PlayShow();
            this.StartCoroutine(this.HideMapNotice(noticeItemView));
        }

        private IEnumerator HideMapNotice(MapNoticeItemView noticeItemView) {
            yield return YieldManager.GetWaitForSeconds(2f);
            noticeItemView.PlayHide(() => {
                PoolManager.RemoveObject(noticeItemView.gameObject);
                this.viewModel.ShowNoticesItemInCache();
            });
        }


        public void ShowBuildingLevelUp() {
            GameObject pnlNoticeItem =
                PoolManager.GetObject(PrefabPath.pnlBuildingNoticeItem, this.pnlNotice);
            MapBuildingNoticeItemView noticeItemView =
                    pnlNoticeItem.GetComponent<MapBuildingNoticeItemView>();
            noticeItemView.SetItemContent(this.viewModel.CurrentBuilding);
            string buildingName = this.viewModel.CurrentBuilding.Name;
            noticeItemView.OnClick.AddListener(() => this.OnBuildingClick(buildingName));
            noticeItemView.PlayShow();
            this.StartCoroutine(this.HideMapNotice(noticeItemView));
        }
        #endregion

        #region FTE

        public void OnFteStep7End() {
            this.pnlNotice.gameObject.SetActiveSafe(false);
        }

        #endregion

        private void OnBuildingClick(string buildingName) {
            this.viewModel.MoveToBuilding(buildingName);
        }

        private void OnHeroClick(string heroName) {
            this.viewModel.ShowHero(heroName);
        }
    }
}
