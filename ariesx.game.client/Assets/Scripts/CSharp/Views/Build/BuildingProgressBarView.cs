using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Protocol;

namespace Poukoute {

    public enum BuildBarType {
        Building,
        Upgrade,
        GiveUp
    }

    public class BuildingProgressBarView : MonoBehaviour {
        #region UI component cache
        //[SerializeField]
        //private Transform pnlIcon;
        [SerializeField]
        private TextMeshProUGUI txtTime;
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private Image imgBuildBG;
        [SerializeField]
        private Image imgUpdateBG;
        #endregion

        public string Time {
            get {
                return this.txtTime.text;
            }
            set {
                this.txtTime.text = value;
            }
        }

        public float Value {
            get {
                return this.slider.value;
            }
            set {
                this.slider.value = value;
            }
        }

        public float MaxValue {
            get {
                return this.slider.maxValue;
            }
        }

        private BuildBarType barType;
        public BuildBarType BarType {
            get {
                return this.barType;
            }
            set {
                if (this.barType != value) {
                    this.barType = value;
                    this.OnBarTypeChange();
                }
            }
        }

        public void OnBarTypeChange() {
            //bool isShowIcon = false;
            switch (this.barType) {
                case BuildBarType.Building:
                    //isShowIcon = true;
                    this.imgBuildBG.gameObject.SetActiveSafe(true);
                    this.imgUpdateBG.gameObject.SetActiveSafe(false);
                    break;
                case BuildBarType.Upgrade:
                    //isShowIcon = true;
                    this.imgBuildBG.gameObject.SetActiveSafe(false);
                    this.imgUpdateBG.gameObject.SetActiveSafe(true);
                    break;
                case BuildBarType.GiveUp:
                    //isShowIcon = true;
                    break;
                default:
                    Debug.LogError("No such bar type");
                    break;
            }
            //this.pnlIcon.gameObject.SetActiveSafe(isShowIcon);
        }
    }
}
