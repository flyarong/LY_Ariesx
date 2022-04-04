using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class BannaerView : BaseView {
        private BannaerViewModel viewModel;
        private BannaerViewPreference viewPref;
        private ChapterCompletePreference ChapterCompletePref;
        private Protocol.CommonReward reward;
        private Protocol.Resources resources;
        private Protocol.Currency currency;
        private UnityAction ShowNextChapterAction;
        // Format
        public bool needRefresh;
        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<BannaerViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIBanner");
            this.viewPref = this.ui.transform.GetComponent<BannaerViewPreference>();
            this.ChapterCompletePref = this.ui.transform.Find("PnlChapterComplete").
                GetComponent<ChapterCompletePreference>();
            this.ChapterCompletePref.background.onClick.
                AddListener(this.OnBtnBackgroundClick);
            this.ChapterCompletePref.btnReceive.onClick.AddListener(this.OnBtnReceiveClick);
        }

        public void ShowBanner(Protocol.CommonReward reward, Protocol.Resources resources
            , Protocol.Currency currency, bool needNext) {
            this.reward = reward;
            this.resources = resources;
            this.currency = currency;
            base.Show();
            this.SetChapterComplete();
            this.ShowChapterComplete();
            ShowNextChapterAction = () => {
                if (needNext) {
                    StartCoroutine(this.DelayShowNext());
                } else {
                    UIManager.HideUI(this.ChapterCompletePref.pnlChapterComplete.gameObject);
                    base.Hide();
                }
            };

        }

        public void ShowNextChapterImmediately() {
            base.Show();
            this.ShowNextChapter();
        }

        private void ShowNextChapter() {
            DramaConf dramaConf = DramaConf.GetConf(this.viewModel.DramaList[0].Id.ToString());
            this.viewPref.txtBanner.text =
                LocalManager.GetValue("chapter_title_", dramaConf.chapter.ToString());
            this.viewPref.txtBannerSub.text = dramaConf.GetTitle();
            AudioManager.Play("show_chapter_change", AudioType.Show, AudioVolumn.High);
            this.viewPref.imgLeft.fillAmount = 0;
            this.viewPref.imgRight.fillAmount = 0;
            StartCoroutine(this.DelayShowBanner());
        }

        private IEnumerator DelayShowBanner() {
            this.viewPref.imgRight.fillOrigin = 0;
            this.viewPref.imgLeft.fillOrigin = 1;
            AudioManager.Play("show_chapter_change", AudioType.Show, AudioVolumn.High);
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgRight.fillAmount =
                    this.viewPref.showCurve.Evaluate(this.viewPref.showCurve.keys[1].time / 13 * i) * 2;
                if (i == 13) {
                    AnimationManager.Animate(this.viewPref.pnlBannerContent.gameObject, "Show");
                }
                yield return YieldManager.GetWaitForSeconds(this.viewPref.showCurve.keys[1].time / 13);
            }
            
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgLeft.fillAmount =
                    (this.viewPref.showCurve.Evaluate((this.viewPref.showCurve.keys[2].time -
                        this.viewPref.showCurve.keys[1].time)
                            / 13 * i + this.viewPref.showCurve.keys[1].time) - 0.5f) * 2f;

                yield return YieldManager.GetWaitForSeconds
                    ((this.viewPref.showCurve.keys[2].time
                        - this.viewPref.showCurve.keys[1].time) / 13);
            }
            yield return YieldManager.GetWaitForSeconds(1f);
            this.viewPref.imgRight.fillOrigin = 1;
            this.viewPref.imgLeft.fillOrigin = 0;
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgRight.fillAmount =
                    1 - this.viewPref.hideCurve.Evaluate(
                        this.viewPref.hideCurve.keys[1].time / 13 * i) * 2;
                yield return YieldManager.GetWaitForSeconds(
                    this.viewPref.hideCurve.keys[1].time / 13);
                if (i == 0) {
                    AnimationManager.Animate(this.viewPref.pnlBannerContent.gameObject, "Hide");
                }
            }
            for (int i = 0; i <= 13; i++) {
                this.viewPref.imgLeft.fillAmount = 1 -
                    (this.viewPref.hideCurve.Evaluate((this.viewPref.hideCurve.keys[2].time
                         - this.viewPref.hideCurve.keys[1].time) / 13 * i
                            + this.viewPref.hideCurve.keys[1].time) - 0.5f) * 2f;
                yield return YieldManager.GetWaitForSeconds((
                    this.viewPref.hideCurve.keys[2].time
                        - this.viewPref.hideCurve.keys[1].time) / 13);
            }
            yield return YieldManager.GetWaitForSeconds(0.2f);
            this.viewModel.ShowDramaView();
            base.Hide();
        }

        //public void ShowBannerEx() {
        //    base.Show();
        //    DramaConf dramaConf = DramaConf.GetConf(this.viewModel.DramaList[0].Id.ToString());
        //    this.viewPref.txtBanner.text =
        //        LocalManager.GetValue("chapter_title_", dramaConf.chapter.ToString());
        //    this.viewPref.txtBannerSub.text = dramaConf.GetTitle();
        //    AudioManager.Play("show_chapter_change", AudioType.Show, AudioVolumn.High);
        //    AnimationManager.Animate(this.viewPref.pnlBanner.gameObject, "Show", () => {
        //        base.Hide();
        //    });
        //}
        private void SetChapterComplete() {
            Debug.Log(this.ChapterCompletePref.pnlChapterComplete);
            this.ChapterCompletePref.pnlCard.gameObject.SetActiveSafe(false);
            if (this.reward.Currency.Gold != 0) {
                this.ChapterCompletePref.pnlGold.gameObject.SetActiveSafe(true);
                this.ChapterCompletePref.txtGold.text =
                    GameHelper.GetFormatNum(this.reward.Currency.Gold);
            } else {
                this.ChapterCompletePref.pnlGold.gameObject.SetActiveSafe(false);
            }
            if (this.reward.Resources.Food != 0) {
                this.ChapterCompletePref.pnlFood.gameObject.SetActiveSafe(true);
                this.ChapterCompletePref.txtFood.text = 
                    GameHelper.GetFormatNum(this.reward.Resources.Food);
            } else {
                this.ChapterCompletePref.pnlFood.gameObject.SetActiveSafe(false);
            }
            if (this.reward.Resources.Lumber != 0) {
                this.ChapterCompletePref.pnlLumber.gameObject.SetActiveSafe(true);
                this.ChapterCompletePref.txtLumber.text =
                    GameHelper.GetFormatNum(this.reward.Resources.Lumber);
            } else {
                this.ChapterCompletePref.pnlLumber.gameObject.SetActiveSafe(false);
            }
            if (this.reward.Resources.Marble != 0) {
                this.ChapterCompletePref.pnlMarble.gameObject.SetActiveSafe(true);
                this.ChapterCompletePref.txtMarble.text =
                    GameHelper.GetFormatNum(this.reward.Resources.Marble);
            } else {
                this.ChapterCompletePref.pnlMarble.gameObject.SetActiveSafe(false);
            }
            if (this.reward.Resources.Steel != 0) {
                this.ChapterCompletePref.pnlSteel.gameObject.SetActiveSafe(true);
                this.ChapterCompletePref.txtSteel.text =
                    GameHelper.GetFormatNum(this.reward.Resources.Steel);
            } else {
                this.ChapterCompletePref.pnlSteel.gameObject.SetActiveSafe(false);
            }
            this.ChapterCompletePref.btnReceive.gameObject.SetActiveSafe(false);
        }

        private void ShowChapterComplete() {
            UIManager.ShowUI(this.ChapterCompletePref.pnlChapterComplete.gameObject);
            AnimationManager.Animate(
                this.ChapterCompletePref.pnlChapterComplete.gameObject,
                "Fade",finishCallback:()=> {
                    this.ChapterCompletePref.btnReceive.gameObject.SetActiveSafe(true);
                });
        }

        private void OnBtnBackgroundClick() {
            AnimationManager.Stop(this.ChapterCompletePref.pnlChapterComplete.gameObject);
            UIManager.ShowUI(this.ChapterCompletePref.pnlChapterComplete.gameObject);
            this.ChapterCompletePref.btnReceive.gameObject.SetActiveSafe(true);
        }

        private void OnBtnReceiveClick() {
            Dictionary<Resource, Transform> resourceDict =
                new Dictionary<Resource, Transform>();
            if (this.reward.Currency.Gold != 0) {
                resourceDict.Add(Resource.Gold, this.ChapterCompletePref.pnlGold);
            } 
            if (this.reward.Resources.Food != 0) {
                resourceDict.Add(Resource.Food, this.ChapterCompletePref.pnlFood);
            } 
            if (this.reward.Resources.Lumber != 0) {
                resourceDict.Add(Resource.Lumber, this.ChapterCompletePref.pnlLumber);
            } 
            if (this.reward.Resources.Marble != 0) {
                resourceDict.Add(Resource.Marble, this.ChapterCompletePref.pnlMarble);
            }
            if (this.reward.Resources.Steel != 0) {
                resourceDict.Add(Resource.Steel, this.ChapterCompletePref.pnlSteel);
            }
            GameHelper.CollectResources(this.reward, this.resources, this.currency
                , resourceDict);
            this.ShowNextChapterAction.InvokeSafe();
        }
        private IEnumerator DelayShowNext() {
            yield return YieldManager.GetWaitForSeconds(1);
            UIManager.HideUI(this.ChapterCompletePref.pnlChapterComplete.gameObject);
            this.ShowNextChapter();
        }
    }
}
