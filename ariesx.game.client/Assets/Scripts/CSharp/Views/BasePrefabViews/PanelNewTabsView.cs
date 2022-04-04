using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Protocol;

namespace Poukoute {
    public enum TabNoticePointType {
        Red,
        Yellow,
        Green
    }

    public class TabInfo {
        public string tabLabel;
        public Transform tabPanel;
        public UnityAction<bool> tabAction;

        public TabInfo(string label, Transform panel, UnityAction<bool> action) {
            this.tabLabel = label;
            this.tabPanel = panel;
            this.tabAction = action;
        }
    }

    public class PanelNewTabsView: MonoBehaviour {
        [SerializeField]
        private HorizontalLayoutGroup horizontalLayoutGroup;
        [SerializeField]
        public CustomButton btnClose;
        [SerializeField]
        private ToggleGroup toggleGroup;
        private Dictionary<int, Transform> tabTransformDict =
            new Dictionary<int, Transform>(2);
        private Dictionary<int, CustomToggle> tabCustomToggleDict =
            new Dictionary<int, CustomToggle>(2);

        private const int EXIST_TABS_COUT = 2;


        void Start() {
            this.horizontalLayoutGroup.enabled = false;
        }

        public void InitTab(int count) {
            this.horizontalLayoutGroup.enabled = true;
            if (count > EXIST_TABS_COUT) {
                for (int i = EXIST_TABS_COUT; i < count; i++) {
                    GameObject toggle = PoolManager.GetObject(PrefabPath.pnlBigTogTab, this.transform);
                    toggle.transform.SetSiblingIndex(i - 1);
                    toggle.GetComponent<CustomToggle>().Toggle.group = toggleGroup;
                }
            }
            GameHelper.ForceLayout(this.horizontalLayoutGroup);
            this.horizontalLayoutGroup.enabled = false;
        }

        public void SetTab(int tabIndex, TabInfo tabInfo) {
            Transform tabTransform = this.transform.GetChild(tabIndex);
            tabTransformDict[tabIndex] = tabTransform;
            CustomToggle customToggle = tabTransform.GetComponent<CustomToggle>();
            this.tabCustomToggleDict[tabIndex] = customToggle;
            customToggle.Toggle.onValueChanged.RemoveAllListeners();
            customToggle.Toggle.onValueChanged.AddListener((state) => {
                this.OnToggleStateChange(tabIndex, tabInfo, state);
            });
            customToggle.txtLabel.text = tabInfo.tabLabel;
        }

        public Transform GetTabTransform(int index) {
            return this.tabCustomToggleDict[index].transform;
        }


        public void SetActiveTab(int index) {
            if (index > this.transform.childCount) {
                Debug.LogError("Tab index out of range, reset to 0");
                index = 0;
            }
            this.tabCustomToggleDict[index].Toggle.isOn = true;
        }

        public void SetToggleVisible(int index, bool visible) {
            this.horizontalLayoutGroup.enabled = true;
            this.tabTransformDict[index].gameObject.SetActiveSafe(visible);
            GameHelper.ForceLayout(this.horizontalLayoutGroup);
            this.horizontalLayoutGroup.enabled = false;
        }

        public void SetPointVisible(int index, bool visible, int noticeNum = 0) {
            this.tabCustomToggleDict[index].SetRedNoticeVisible(visible, noticeNum);
        }

        public void SetPointVisible(int index, int noticeNum) {
            this.tabCustomToggleDict[index].SetRedNoticeVisible(noticeNum > 0, noticeNum);
        }

        public void SetColorPoint(int index, TabNoticePointType color = TabNoticePointType.Red) {
            this.tabCustomToggleDict[index].SetNoticeColor(color);
        }

        public void SetColorPointVisible(int index, bool visible,
            TabNoticePointType type = TabNoticePointType.Red) {
            string pointName = string.Concat("ImgPoint",
                    Enum.GetName(typeof(TabNoticePointType), type));
            Transform point = this.tabTransformDict[index].Find(pointName);
            if (point != null) {
                point.gameObject.SetActiveSafe(visible);
            }
        }

        public void SetAllOff() {
            this.GetComponent<ToggleGroup>().SetAllTogglesOff();
        }

        public void SetCloseCallBack(UnityAction call) {
            this.btnClose.onClick.RemoveAllListeners();
            this.btnClose.onClick.AddListener(call);
        }

        /******** tog value change callback ***********/
        //private Vector2 togLabelOffsetMax = Vector2.zero;
        private Transform childTransform = null;
        private void OnToggleStateChange(int tabIndex, TabInfo tabInfo, bool state) {
            childTransform = tabTransformDict[tabIndex];
            if (state &&
                (childTransform.GetSiblingIndex() != (this.transform.childCount - 1))) {

                childTransform.SetAsLastSibling();
            }
            this.tabCustomToggleDict[tabIndex].imgMark.gameObject.SetActiveSafe(state);

            if (tabInfo.tabPanel != null) {
                UIManager.SetUIVisible(tabInfo.tabPanel.gameObject, state);
            }
            tabInfo.tabAction.Invoke(state);
        }

        #region FTE

        public void OnChatStep2Start() {
        }

        #endregion
    }

}