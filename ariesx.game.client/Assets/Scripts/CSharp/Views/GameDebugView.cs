using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoBuf;
using TMPro;

namespace Poukoute {
    public enum NetGate {
        None,
        LYtest,
        developer,
        googleplay,
        taptap
    }

    public class GameDebugView : MonoBehaviour {
        #region UI element
        [SerializeField]
        private RectTransform settingRT;
        [SerializeField]
        private Button btnFold;
        [SerializeField]
        private Button btnNewPlayer;
        [SerializeField]
        private Button btnSwitchGate;
        [SerializeField]
        private Button btnMute;
        [SerializeField]
        private TextMeshProUGUI txtMute;
        [SerializeField]
        private Button btnCopy;
        [SerializeField]
        private GameObject pnlCode;
        [SerializeField]
        private Button btnAddResource;
        [SerializeField]
        private Button btnAddMoney;
        [SerializeField]
        private Button btnTroopEnergy;
        [SerializeField]
        private Button btnFinishEvent;
        [SerializeField]
        private Button btnUpgradeBuild;
        [SerializeField]
        private Button btnShowServerVersion;
        [SerializeField]
        private Button btnGetAllHero;
        [SerializeField]
        private Button btnHeroFullLevel;
        [SerializeField]
        private Button btnHero12Level;
        [SerializeField]
        private Button btnSkipFte;
        [SerializeField]
        private Button btnRestart;
        [SerializeField]
        private Button btnEnableAccount;
        #endregion

        void Awake() {
            //this.ui = this.gameObject;
            this.btnFold.onClick.AddListener(this.OnBtnFoldClick);
            this.btnSwitchGate.onClick.AddListener(this.OnBtnSwitchClick);
            this.btnMute.onClick.AddListener(this.OnBtnMuteClick);
            bool mute = !(!PlayerPrefs.HasKey("mute") ||
                          (PlayerPrefs.GetInt("mute") == 0));
            this.txtMute.text = string.Concat(LocalManager.GetValue(LocalHashConst.music),
                ":", mute ? LocalManager.GetValue(LocalHashConst.off) :
               LocalManager.GetValue(LocalHashConst.on));
#if UNITY_EDITOR || DEVELOPER
            this.btnCopy.onClick.AddListener(this.OnBtnCopyClick);
            this.btnNewPlayer.onClick.AddListener(this.OnBtnNewPlayer);
            this.btnAddResource.onClick.AddListener(this.OnBtnAddResourceClick);
            this.btnAddMoney.onClick.AddListener(this.OnBtnAddMoneyClick);
            this.btnTroopEnergy.onClick.AddListener(this.OnBtnTroopEnergyClick);
            this.btnFinishEvent.onClick.AddListener(this.OntnFinishEventClick);
            this.btnUpgradeBuild.onClick.AddListener(this.OnBtnUpgradeBuildClick);
            this.btnGetAllHero.onClick.AddListener(this.OnBtnGetAllHeroClick);
            this.btnHeroFullLevel.onClick.AddListener(this.OnBtnHeroFullLevelClick);
            this.btnHero12Level.onClick.AddListener(this.OnBtnHero12LevelClick);
            this.btnShowServerVersion.onClick.AddListener(this.OnBtnShowServerVersionClick);
            this.btnSkipFte.onClick.AddListener(this.OnBtnSkipFteClick);
            this.btnRestart.onClick.AddListener(this.OnBtnRestartClick);
            this.btnEnableAccount.onClick.AddListener(this.OnBtnEnableAccountClick);
            this.pnlCode.SetActive(true);
#else
            this.pnlCode.SetActive(false);
#endif
        }

        private void OnBtnAddResourceClick() {
            this.WorldChatReq("greedisgood");
        }

        private void OnBtnAddMoneyClick() {
            this.WorldChatReq("showmethemoney");
        }

        private void OnBtnTroopEnergyClick() {
            this.WorldChatReq("iamtheking");
        }

        private void OntnFinishEventClick() {
            this.WorldChatReq("asap");
        }

        private void OnBtnUpgradeBuildClick() {
            this.WorldChatReq("ianno8");
        }

        private void OnBtnGetAllHeroClick() {
            this.WorldChatReq("fillme");
        }

        private void OnBtnHeroFullLevelClick() {
            this.WorldChatReq("iamthequeen");
        }

        private void OnBtnHero12LevelClick() {
            this.WorldChatReq("12level");
        }

        private void OnBtnShowServerVersionClick() {
            this.WorldChatReq("whoami");
        }

        private void OnBtnRemoveFreshProtectionClick() {
            this.WorldChatReq("iamadept");
        }

        private void OnBtnRemoveTileProtectionClick() {
            this.WorldChatReq("pointisold x,y");
        }

        private void OnBtnSkipFteClick() {
            PlayerPrefs.SetInt(RoleManager.Udid + "skip_fte", 1);
            GameManager.RestartGame();
        }

        private void OnBtnRestartClick() {
            GameManager.RestartGame();
        }

        private void OnBtnEnableAccountClick() {
            if (PlayerPrefs.HasKey("enable_new_account")) {
                int enableNewAccount = PlayerPrefs.GetInt("enable_new_account");
                if (enableNewAccount == 1) {
                    PlayerPrefs.SetInt("enable_new_account", 0);
                } else {
                    PlayerPrefs.SetInt("enable_new_account", 1);
                }
            } else {
                PlayerPrefs.SetInt("enable_new_account", 1);
            }
            GameManager.RestartGame();
        }

        private void WorldChatReq(string input) {
            WorldChatReq worldChat = new WorldChatReq() {
                Content = input
            };
            NetManager.SendMessage(worldChat, string.Empty, null);
        }

        private void OnBtnNewPlayer() {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("udid", System.Guid.NewGuid().ToString());
            GameManager.RestartGame();
        }

        private void OnBtnSwitchClick() {
            if (!PlayerPrefs.HasKey("net_gate")) {
                PlayerPrefs.SetString("net_gate", NetGate.developer.ToString());
            } else if (PlayerPrefs.GetString("net_gate") == NetGate.LYtest.ToString()) {
                PlayerPrefs.SetString("net_gate", NetGate.developer.ToString());
            } else {
                PlayerPrefs.SetString("net_gate", NetGate.LYtest.ToString());
            }
            GameManager.RestartGame();
        }

        private void OnBtnMuteClick() {
            if (!PlayerPrefs.HasKey("mute") || PlayerPrefs.GetInt("mute") == 1) {
                PlayerPrefs.SetInt("mute", 0);
            } else {
                PlayerPrefs.SetInt("mute", 1);
            }
            bool mute = PlayerPrefs.GetInt("mute") == 1 ? true : false;
            this.txtMute.text = string.Concat(LocalManager.GetValue(LocalHashConst.music),
                ":", mute ? LocalManager.GetValue(LocalHashConst.off) :
               LocalManager.GetValue(LocalHashConst.on));
            if (AudioManager.Instance != null) {
                AudioManager.Mute(mute);
            }
        }

        private void OnBtnCopyClick() {
            GUIUtility.systemCopyBuffer = RoleManager.Udid;
        }

        private void OnBtnFoldClick() {
            this.btnFold.transform.eulerAngles = -this.btnFold.transform.eulerAngles;
            int sign = this.btnFold.transform.eulerAngles.z == 90 ? 1 : -1;
            //if (!VersionConst.CanLinkFacebook()) {
            //this.settingRT.anchoredPosition = new Vector2(sign * 134, 288);
            this.settingRT.anchoredPosition = new Vector2(sign * 198, 288);


            //} else {
            //    this.settingRT.anchoredPosition = new Vector2(sign * 198, 288);
            //}

        }
    }
}
