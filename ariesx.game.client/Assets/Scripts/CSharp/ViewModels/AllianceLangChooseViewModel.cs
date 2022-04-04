using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;

namespace Poukoute {
    public class AllianceLangChooseViewModel : BaseViewModel {
        private AllianceCreateViewModel parentAllianceCreateViewModel;
        private AllianceEditViewModel parentAllianceEditViewModel;
        private AllianceLangChooseView view;

        void Awake() {
            this.view = this.gameObject.AddComponent<AllianceLangChooseView>();
            this.parentAllianceCreateViewModel = this.transform.parent.GetComponent<AllianceCreateViewModel>();
            this.parentAllianceEditViewModel = this.transform.parent.GetComponent<AllianceEditViewModel>();
        }

        public void Show() {
            this.view.PlayShow();

            if (parentAllianceCreateViewModel != null) {
                this.view.SetAllLanguageInfo(AllianceModel.AllianceLanguageConf, parentAllianceCreateViewModel.Language);
            }
            if (parentAllianceEditViewModel != null) {
                this.view.SetAllLanguageInfo(AllianceModel.AllianceLanguageConf, parentAllianceEditViewModel.Language);
            }
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void SetGameLanguage(AllianceLanguageConf language) {
            if (parentAllianceCreateViewModel != null)
                this.parentAllianceCreateViewModel.SetGameLanguage(language);
            if (parentAllianceEditViewModel != null)
                this.parentAllianceEditViewModel.SetAllianceLanguage(language);
        }
    }
}
