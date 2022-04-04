using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class LangChooseViewModel : BaseViewModel {
        private GameSettingViewModel parent;
        private LangChooseView view;


        private void Awake() {
            this.view = this.gameObject.AddComponent<LangChooseView>();
            this.parent = this.transform.parent.GetComponent<GameSettingViewModel>();
        }


        public void Show() {
            this.view.PlayShow();
            this.view.SetAllLanguageInfo(LocalManager.GetGameLanguageDict());
        }

        public void Hide() {
            this.view.PlayHide();
        }

        public void SetGameLanguage(SystemLanguage language) {
            if (this.parent == null) {
                this.parent =
                    PoolManager.GetObject<GameSettingViewModel>(this.transform);
            }
            this.parent.SetGameLanguage(language);
        }
    }
}