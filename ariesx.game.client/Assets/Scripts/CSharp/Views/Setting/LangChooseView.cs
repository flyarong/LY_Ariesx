using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class LangChooseView : BaseView {
        private LangChooseViewModel viewModel;
        private LangChooseViewPreference viewPref;

        //private void Awake() {
        //    this.viewModel = this.gameObject.GetComponent<LangChooseViewModel>();
        //}

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UILangChoose");
            this.viewModel = this.gameObject.GetComponent<LangChooseViewModel>();
            this.viewPref = this.ui.transform.GetComponent<LangChooseViewPreference>();

            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClicked);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClicked);
        }

        private List<LanguageItemView> itemViews = new List<LanguageItemView>(10);
        public void SetAllLanguageInfo(Dictionary<SystemLanguage, string> LanguageDict) {
            int langCounts = LanguageDict.Count;
            GameHelper.ResizeChildreCount(this.viewPref.PnlList,
                langCounts, PrefabPath.pnlLanguageItem);
            this.itemViews.Clear();
            int index = 0;
            foreach (var value in LanguageDict) {
                LanguageItemView itemView = this.viewPref.PnlList.GetChild(index++).GetComponent<LanguageItemView>();
                KeyValuePair<SystemLanguage, string> language = value;
                itemView.SetContent(LocalManager.GetValue(language.Value),
                    () => { this.OnLanguageItemClick(language); });
                this.itemViews.Add(itemView);
            }
            this.FormateView();
        }

        private void FormateView() {
            this.viewPref.listContentSizeFitter.onSetLayoutVertical.AddListener(() => {
                this.viewPref.listRectTransform.anchoredPosition = new Vector2(
                        this.viewPref.listRectTransform.rect.width / 2,
                        -this.viewPref.listRectTransform.rect.height / 2
                    );
                this.viewPref.listVerticalGroup.SetOriginal();
                this.viewPref.scrollRect.velocity = Vector2.zero;
            });
        }

        private void OnLanguageItemClick(KeyValuePair<SystemLanguage, string> language) {
            this.ResetLanguageView(language.Value);
            UIManager.ShowConfirm(
                LocalManager.GetValue(LocalHashConst.button_confirm),
                string.Format(LocalManager.GetValue(LocalHashConst.language_change_confirm), language.Value),
                () => { this.SetGameLanguage(language.Key); }, () => { });

        }

        private void ResetLanguageView(string language) {
            foreach(LanguageItemView view in this.itemViews) {
                view.SetChosenStatus(language);
            }
        }

        private void SetGameLanguage(SystemLanguage language) {
            this.viewModel.SetGameLanguage(language);
        }

        private void OnBtnCloseClicked() {
            this.viewModel.Hide();
        }
    }
}