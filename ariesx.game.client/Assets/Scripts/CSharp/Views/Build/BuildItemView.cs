using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;
using System.Text;

namespace Poukoute {
    public class BuildItemView : BaseView {
        //private GameObject ui;.
        #region UI component cache
        // public Transform hightlight;
        // public Transform pnlInfo;
        public Image imgBoard;
        public Image imgBuild;
        public Transform pnlTime;
        public TextMeshProUGUI txtTime;
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtDesc;
        public TextMeshProUGUI txtNumber;
        public Transform pnlResource;
        public Transform pnlQueueFull;
        public Button btnItem;
        public Button btnDetail;
     //   public Transform pnlNeedRepaire;
        public Image imgNotUnlock;
        [Tooltip("PnlBuildItem/PnlUnlockShow")]
        public Transform pnlUnlockShow;
        [Tooltip("PnlBuildItem/PnlUnlockShow/TxtUnlockShow")]
        public TextMeshProUGUI txtUnlockShow;
        [Tooltip("PnlBuildItem/PnlCost")]
        public Transform pnlCost;
        #endregion

        private BuildModel model;
        public enum Background {
            Normal = 1,
            Broken,
        }

        public class BuildEvent : UnityEvent<bool, bool> { }
        private BuildEvent OnClick = new BuildEvent();
        private BuildEvent OnClickDetail = new BuildEvent();
        // private 
        private string buildingId;
        //public string buildingName;
        //[HideInInspector]
        private bool canBuild;
        private bool isLock;

        public string ID {
            get {
                return this.buildingId;
            }
            private set {
                if (this.buildingId != value) {
                    this.buildingId = value;
                }
            }
        }

        private Dictionary<string, ElementBuilding> BuildingDict {
            get {
                return this.model.buildingDict;
            }
        }

        //private bool isRotated = false;
        //private BuildInfoViewModel buildInfoViewModel;

        private void Awake() {
            this.model = ModelManager.GetModelData<BuildModel>();
            //this.buildInfoViewModel = PoolManager.GetObject<BuildInfoViewModel>(this.transform);
        }
        /***************************/

        private void OnBtnItemClick() {
            this.OnClick.Invoke(this.canBuild, this.isLock);
        }

        private void OnBtnDetailClick() {
            this.OnClickDetail.Invoke(true, true);
            //this.isRotated = !this.isRotated;
            //this.SetRotateInfo();
            //AnimationManager.Animate(this.gameObject, "Rotate", null);
        }

        public void SetBuildItemDeatil(string id, UnityAction<bool, bool> callback) {
            this.OnClickDetail.RemoveAllListeners();
            this.OnClickDetail.AddListener(callback);
        }

        public void SetBuildItem(string id, UnityAction<bool, bool> callback) {
            this.ID = id;
            this.OnClick.RemoveAllListeners();
            this.OnClick.AddListener(callback);
        }

        public void SetBuilding(BuildingConf buildingConf, int buildingCount, bool isBroken, bool isUnlockShow = false, string unlockShowLevel = null) {
            this.InnerSetBuilding(
                buildingConf.type,
                buildingConf.buildingName,
                buildingConf.buildingLevel,
                buildingConf.resourceDict,
                buildingConf.duration,
                buildingCount,
                isBroken,
                isUnlockShow,
                unlockShowLevel
            );
        }

        private void InnerSetBuilding(string buildingType, string buildingName,
            int buildingLevel, Dictionary<Resource, int> resourceDict,
            long duration, int buildingCount, bool isBroken, bool isLockShow = false, string unlockShowLevel = null) {
            //this.isRotated = false;
            this.SetRotateInfo();
            //this.buildingName = buildingName;
            string buildingLocalName = MapUtils.GetBuildingLocalName(buildingType, buildingName);
            this.txtDesc.text =
               LocalManager.GetValue(string.Concat("brief_", buildingType));
            this.imgBuild.sprite = ArtPrefabConf.GetSprite(
                string.Concat(SpritePath.tileLayerAbovePrefix, buildingType, buildingLevel));
            string imgBackgroundPath =
                string.Concat(SpritePath.buildItemBackgroundPrefix, (int)Background.Normal);

           // this.pnlNeedRepaire.gameObject.SetActiveSafe(isBroken);
            if (isBroken) {
                StringBuilder s = new StringBuilder();
                s.AppendFormat(
                    LocalManager.GetValue(LocalHashConst.building_upgrade_previouse_building),
                    buildingLocalName, buildingLevel
                );
                this.txtName.text = s.ToString();
                this.txtNumber.text = string.Empty;
                imgBackgroundPath = string.Concat(
                    SpritePath.buildItemBackgroundPrefix,
                    (int)Background.Broken
                );

            } else {
                int buildingNumber = this.model.GetHavedBuilding(buildingType);
                this.txtName.text = buildingLocalName;
                this.txtNumber.text = string.Format(" {0}:{1}/{2}",
                    LocalManager.GetValue(LocalHashConst.building_builded),
                    buildingNumber,
                    buildingCount + buildingNumber);
            }
            this.imgBoard.sprite = ArtPrefabConf.GetSprite(imgBackgroundPath);

            this.canBuild = this.SetResourceInfo(resourceDict);
            this.SetBuildInfo(duration, buildingType, buildingCount, isBroken);
            this.isLock = isLockShow;
            if (isLockShow) {
                //this.imgBoard.material = PoolManager.GetMaterial(MaterialPath.matGray);
                this.imgBuild.material = PoolManager.GetMaterial(MaterialPath.matGray);
                this.imgNotUnlock.gameObject.SetActiveSafe(isLockShow);
                this.pnlCost.gameObject.SetActiveSafe(false);
                this.pnlUnlockShow.gameObject.SetActiveSafe(true);
                string upgradeDesc = string.Empty;
                string unlockBuildRealName = string.Empty;
                string level = string.Empty;
                BuildingConf buildConf =
                    ConfigureManager.GetConfById<BuildingConf>(
                                string.Concat(buildingName, "_", buildingLevel));
                bool showImg = false;
                string unlockBuildName = string.Empty;
                this.model.GetUpgradeConditionConf(out upgradeDesc, buildConf, out showImg,
                    out unlockBuildName, out unlockBuildRealName, out level);
                this.txtUnlockShow.text = upgradeDesc;
                this.txtName.text = buildingLocalName;
                this.txtNumber.text = string.Empty;
            } else {
                this.imgBoard.material = null;
                this.imgBuild.material = null;
                this.pnlCost.gameObject.SetActiveSafe(true);
                this.pnlUnlockShow.gameObject.SetActiveSafe(false);
                this.imgNotUnlock.gameObject.SetActiveSafe(false);
            }
        }

        private bool SetResourceInfo(Dictionary<Resource, int> resourceDict) {
            GameHelper.ClearChildren(this.pnlResource);
            bool isEnough = true;
            foreach (var pair in resourceDict) {
                GameObject resourceItem =
                    PoolManager.GetObject(PrefabPath.pnlBuildResourceItem, this.pnlResource);
                resourceItem.transform.Find("Image").GetComponent<Image>().sprite =
                    ArtPrefabConf.GetSprite(SpritePath.resourceIconPrefix,
                    pair.Key.ToString().ToLower());
                TextMeshProUGUI txtAmount = resourceItem.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                txtAmount.text = GameHelper.GetFormatNum(pair.Value);
                if (pair.Value > RoleManager.GetResource(pair.Key)) {
                    txtAmount.color = Color.red;
                    isEnough = false;
                } else {
                    txtAmount.color = Color.white;
                }
            }

            return isEnough;
        }

        private void SetBuildInfo(long duration,
            string buildType, int buildingCount, bool isBroken) {
            // To do: May provide the more detail of build event.
            bool buildQueueMax = EventManager.IsBuildEventMaxFull();
            this.pnlQueueFull.gameObject.SetActive(buildQueueMax);
            this.pnlResource.gameObject.SetActive(!buildQueueMax);
            if (buildQueueMax) {
                this.btnItem.onClick.RemoveAllListeners();
                //this.btnDetail.onClick.RemoveAllListeners();
            } else {
                this.txtTime.text = GameHelper.TimeFormat(duration * 1000);
                //int buildingNumber = model.GetHavedBuilding(buildType);
                this.btnItem.onClick.RemoveAllListeners();
                this.btnItem.onClick.AddListener(this.OnBtnItemClick);
                //this.btnDetail.onClick.RemoveAllListeners();
                // this.btnDetail.onClick.AddListener(this.OnBtnDetailClick);
            }

        }

        private void SetRotateInfo() {
        }


        #region FTE
        public void HideBtnDetail() {
        }

        public void ShowBtnDetail() {
        }

        #endregion
    }

}
