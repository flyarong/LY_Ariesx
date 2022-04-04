using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class GameSettingViewModel : BaseViewModel {
        private class GameSettingConst {
            public static string Music = "Music";
            public static string Sound = "Sound";
            public static string Language = "Language";
            public static string Push = "Push";
            public static string FitterTribeChage = "FitterTribeChage";
        }
        private GameSettingView view = null;


        // ------------- other data --------------------------------------

        /********************************************************************/

        private void Awake() {
            this.view = this.gameObject.AddComponent<GameSettingView>();
        }

        public void Show() {
            //this.view = new GameSettingView();
            this.view.PlayShow();
            this.UpdateGameSettingStatus();
        }

        #region Public methods
        public void SetGameMusicMute() {
            if (!PlayerPrefs.HasKey(GameSettingConst.Music) ||
                PlayerPrefs.GetInt(GameSettingConst.Music) == 1) {
                PlayerPrefs.SetInt(GameSettingConst.Music, 0);
            } else {
                PlayerPrefs.SetInt(GameSettingConst.Music, 1);
            }
            bool mute = PlayerPrefs.GetInt(GameSettingConst.Music) == 1 ? true : false;
            if (AudioManager.Instance != null) {
                AudioManager.Mute(mute);
            }
            this.view.UpdateMusicStatus(mute);
        }

        private LangChooseViewModel langChooseViewModel = null;
        public void SetGameLanguage() {
            if (this.langChooseViewModel == null) {
                this.langChooseViewModel =
                    PoolManager.GetObject<LangChooseViewModel>(this.transform);
            }
            this.langChooseViewModel.Show();
        }


        public void SetGameLanguage(SystemLanguage language) {
            LocalManager.SetLocalLanguage(language);
        }
        #endregion



        private void UpdateGameSettingStatus() {
            this.view.UpdateMusicStatus(true);
            this.view.UpdateSoundsStatus(true);
            this.view.UpdatePushStatus(true);
            this.view.UpdateFitterStatus(true);
            this.view.UpdateGooglePlayStatus(true);
            this.view.UpdateFacebookStatus(false);
            this.view.UpdateDragonnestStatus(false);
        }
    }
}
