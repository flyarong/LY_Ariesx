using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Protocol;

namespace Poukoute {
    public class TypeChooseContent {
        public List<string> typeList;
        public List<string> typeLocalList = null;

        public TypeChooseContent(
                            List<string> typeList,
                            List<string> typeLocalList) {
            this.typeList = typeList;
            this.typeLocalList = typeLocalList;
        }

    }
    public class TypeChooseView : MonoBehaviour {
        public UnityAction TypeValueChangeCallback;
        public int TypeIndex {
            get {
                return this.typeIndex;
            }
            set {
                this.typeIndex = value;
                this.OnTypeIndexChange();
            }
        }

        public TypeChooseContent TypeInfo {
            get {   
                return typeInfo;
            }
            set {
                if (typeInfo != value) {
                    typeInfo = value;
                    this.OnTypeInfoChange();
                }
            }
        }
        private TypeChooseContent typeInfo;

        #region UI Element
        public TextMeshProUGUI txtType;
        public TextMeshProUGUI txtTitle;
        public Button btnLeftButton;
        public Image imgLeftArrow;
        public Button btnRightButton;
        public Image imgRightArrow;
        public GameObject imgGrayMask;
        #endregion
        private int typeIndex = 0;


        private void Start() {
            btnLeftButton.onClick.AddListener(this.OnBtnLeftClick);
            btnRightButton.onClick.AddListener(this.OnBtnRightClick);
        }

        private void OnTypeInfoChange() {
            this.TypeIndex = 0;
            this.SetBtnVisibile();
        }

        private void OnBtnLeftClick() {
            this.TypeIndex = Mathf.Clamp(
                this.TypeIndex - 1, 0, typeInfo.typeList.Count - 1);
        }

        private void OnBtnRightClick() {
            this.TypeIndex = Mathf.Clamp(
                this.TypeIndex + 1, 0, typeInfo.typeList.Count - 1);
        }

        private void SetBtnVisibile() {
            if (this.TypeIndex > 0) {
                btnLeftButton.interactable = true;
                imgLeftArrow.material = btnLeftButton.image.material = null;
            }
            else {
                btnLeftButton.interactable = false;
                imgLeftArrow.material = btnLeftButton.image.material
                    = PoolManager.GetMaterial(MaterialPath.matGray);

            }
            if ((this.TypeIndex + 1) < typeInfo.typeList.Count) {
                btnRightButton.interactable = true;
                imgRightArrow.material = btnRightButton.image.material = null;
            }
            else {
                btnRightButton.interactable = false;
                imgRightArrow.material = btnRightButton.image.material 
                    = PoolManager.GetMaterial(MaterialPath.matGray);
            }
        }

        private void OnTypeIndexChange() {
            if (typeInfo.typeLocalList != null) {
                this.txtType.text = typeInfo.typeLocalList[typeIndex];
            }
            else {
                this.txtType.text = typeInfo.typeList[typeIndex];
            }
            this.SetBtnVisibile();
            this.TypeValueChangeCallback.InvokeSafe();
        }
    }
}