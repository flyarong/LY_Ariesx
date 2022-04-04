using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace Poukoute {
    public class DisplayBoardView : BaseView {
        private DisplayBoardViewModel viewModel;
        private DisplayBoardViewPreference viewPref;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<DisplayBoardViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIDisplayBoard");
            this.viewPref = this.ui.transform.transform.GetComponent<DisplayBoardViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
        }

        public void SetDisplayBoradContent(string title, string content) {
            this.viewPref.txtTitle.text = title;
            this.viewPref.txtDesc.text = content;
            //this.viewPref.contentSizeFiltter.onSetLayoutVertical.AddListener(() => {
            //    this.viewPref.rectTransform.anchoredPosition = new Vector2(
            //        0,
            //        -this.viewPref.rectTransform.rect.height / 2
            //        );
            //});
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
