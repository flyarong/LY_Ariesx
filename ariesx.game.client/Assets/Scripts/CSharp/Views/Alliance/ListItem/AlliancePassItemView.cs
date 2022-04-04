using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AlliancePassItemView : BaseItemViewsHolder {
        [HideInInspector]
        public float Height {
            get {
                return 92;
            }
        }
        //public int index = 0;

        public UnityEvent OnInfoClick {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();

                return this.btnItemInfo.onClick;
            }
        }
        public UnityAction jumb = null;
        public UnityAction OnInfo {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();
                return this.jumb;
            }
        }

        //private FallenTarget cityData;
        //private NPCCityConf cityConf;

        // ui
        #region ui element
        [SerializeField]
        private GameObject pnlChosen;
        [SerializeField]
        private TextMeshProUGUI passInfo;
        [SerializeField]
        private TextMeshProUGUI txtAllianceName;
        [SerializeField]
        private TextMeshProUGUI passAddr;
        [SerializeField]
        private Button btnItemInfo;
        public Coord coord = new Coord();
        #endregion

        private MiniMapPassConf pass;
        public MiniMapPassConf Pass {
            get {
                return pass;
            }
            set {
                if (this.pass != value) {
                    this.pass = value;
                    this.OnPassChange();
                }
            }
        }

        private string allianceName;
        public string AllianceName {
            get {
                return this.allianceName;
            }
            set {
                if (this.allianceName != value) {
                    this.allianceName = value;
                    this.OnAllianceNameChanged();
                }
            }
        }

        public void SetItemViewInfo(MiniMapPassConf pass, UnityAction callback, bool isChosen) {
            this.Pass = pass;
            this.btnItemInfo.onClick.RemoveAllListeners();
            this.btnItemInfo.onClick.AddListener(callback);
            this.btnItemInfo.onClick.AddListener(() => this.SetItemIsChosen(true));
            this.SetItemIsChosen(isChosen);
        }
        //跳转
        public void SetItemIsChosen(bool isChosen) {
            this.pnlChosen.SetActiveSafe(isChosen);
        }
        public void SetItemIsChosen() {
            UIManager.ShowConfirm(
                LocalManager.GetValue(LocalHashConst.tip_error),
                string.Empty,/*MapViewModel.MoveWithClick(pass.coordinate)*/ jumb, () => { },
                tips: string.Format("是否要跳转到{0}",
                pass.coordinate.x.ToString() + "/" + pass.coordinate.y.ToString())
            );
        }

        private void OnAllianceNameChanged() {
            this.txtAllianceName.StripLengthWithSuffix(this.allianceName);
            if (!this.allianceName.CustomIsEmpty()) {
                if (this.allianceName == RoleManager.GetAllianceName()) {
                    this.txtAllianceName.color = Color.blue;
                } else {
                    this.txtAllianceName.color = Color.red;
                }
            }
        }

        private void OnPassChange() {
         
            this.AllianceName = pass.allianceName == null ? string.Empty : pass.allianceName;
            this.EnableClick();
        }

        private void EnableClick() {
            if (!this.btnItemInfo.IsInteractable()) {
                this.btnItemInfo.interactable = true;
            }
        }
    }
}
