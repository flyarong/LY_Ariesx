using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class AllianceLangChooseView : BaseView {
        private AllianceLangChooseViewModel viewModel;
        private AllianceLangChooseViewPreference viewPref;
        /*************/

        //void Awake() {
        //	this.viewModel = this.gameObject.GetComponent<AllianceLangChooseViewModel>();
        //}

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceLangChoose");
            /* Cache the ui components here */
            this.viewModel = this.gameObject.GetComponent<AllianceLangChooseViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AllianceLangChooseViewPreference>();

            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClicked);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClicked);
        }

        private List<LanguageItemView> itemViews = new List<LanguageItemView>(10);
        public void SetAllLanguageInfo(Dictionary<string, BaseConf> LanguageDict, int curLanguage) {
            int langCounts = LanguageDict.Count;
            GameHelper.ResizeChildreCount(this.viewPref.PnlList,
                langCounts, PrefabPath.pnlLanguageItem);
            this.itemViews.Clear();
            int index = 0;
            foreach (var pair in LanguageDict) {
                LanguageItemView itemView = this.viewPref.PnlList.GetChild(index++).GetComponent<LanguageItemView>();
                KeyValuePair<string, BaseConf> language = pair;
                AllianceLanguageConf allianceLanguage = pair.Value as AllianceLanguageConf;
                itemView.SetContent(LocalManager.GetValue(string.Concat(GameConst.ALLIANCE_LANGUAGE, allianceLanguage.language)),
                    () => { this.OnLanguageItemClick(language); },
                    curLanguage == int.Parse(allianceLanguage.id));
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

        private void OnLanguageItemClick(KeyValuePair<string, BaseConf> language) {
            AllianceLanguageConf allianceLanguage = language.Value as AllianceLanguageConf;
            this.ResetLanguageView(allianceLanguage.language);
            this.SetGameLanguage(allianceLanguage);
            this.PlayHide();
        }

        private void ResetLanguageView(string language) {
            foreach (LanguageItemView view in this.itemViews) {
                view.SetChosenStatus(language);
            }
        }

        private void SetGameLanguage(AllianceLanguageConf language) {
            this.viewModel.SetGameLanguage(language);
        }

        private void OnBtnCloseClicked() {
            this.viewModel.Hide();
        }
    }
}
