using UnityEngine;
using UnityEngine.Events;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceLogoView : BaseView {
        private AllianceLogoViewModel viewModel;
        private AllianceLogoViewPreference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<AllianceLogoViewModel>();
            //this.InitUi();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceWindows.PnlWindows.PnlLogo");
            this.viewPref = this.ui.transform.GetComponent<AllianceLogoViewPreference>();

        }

        public void SetLogo() {
            GameHelper.ClearChildren(this.viewPref.pnlLogoGrid);
            int allianceElmIndex = ArtPrefabConf.GetAlliEmblemCount();
            for (int i = 1; i < allianceElmIndex; i++) {
                GameObject imgLogo = PoolManager.GetObject(PrefabPath.pnlAllianceLogo, this.viewPref.pnlLogoGrid);
                AllianceLogoItemView imgLogoView = imgLogo.GetComponent<AllianceLogoItemView>();
                int emblem = i;
                imgLogoView.SetLogoInfo(i, () => {
                    this.OnBtnImageClick(emblem);
                });
            }
            this.FormatPnlLogoGrid();
        }

        protected void OnBtnCloseClick() {
            this.viewModel.HideWindow();
        }

        private void OnBtnImageClick(int emblem) {
            this.viewModel.SetLogo(emblem);
        }

        public void FormatPnlLogoGrid() {
            this.viewPref.logoGridLayoutGroup.CalculateLayoutInputHorizontal();
            this.viewPref.logoGridLayoutGroup.CalculateLayoutInputVertical();
            this.viewPref.logoRectTransform.anchoredPosition = new Vector2(
                this.viewPref.logoRectTransform.rect.width / 2,
                -this.viewPref.logoRectTransform.rect.height / 2
            );
        }
    }
}
