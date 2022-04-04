using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Protocol;

namespace Poukoute {
    public class TabTogsInfo {
        public string tabLabel;
        public CanvasGroup tabPanelCG;
        public UnityAction<bool> tabAction;

        public TabTogsInfo(string label, CanvasGroup panel, UnityAction<bool> action) {
            this.tabLabel = label;
            this.tabPanelCG = panel;
            this.tabAction = action;
        }
    }

    public class PanelOnlyTabsView : BaseView {
        [SerializeField]
        private ToggleGroup toggleGroup;
        [SerializeField]
        private HorizontalLayoutGroup horizontalLayoutGroup;

        private const int MAX_TABS_COUNT = 4;
        private const int EXIST_TABS_COUT = 2;
        private List<CustomToggle> toggleList = new List<CustomToggle>(2);
        /***************************/
        private void Start() {
            this.horizontalLayoutGroup.enabled = false;
        }

        public void InitTab(int count) {
            this.horizontalLayoutGroup.enabled = true;
            count = (count > MAX_TABS_COUNT) ? MAX_TABS_COUNT : count;
            for (int i = EXIST_TABS_COUT; i < count; i++) {
                GameObject toggle = PoolManager.GetObject(PrefabPath.pnlTogTab, this.transform);
                toggle.GetComponent<CustomToggle>().Toggle.group = toggleGroup;
            }
            Canvas.ForceUpdateCanvases();
            this.horizontalLayoutGroup.enabled = false;
            this.toggleList.Clear();
        }

        private Dictionary<int, Transform> tabTransformDict = new Dictionary<int, Transform>(2);
        public void SetTab(int tabIndex, TabTogsInfo tabInfo) {
            Transform tabTransform = this.transform.GetChild(tabIndex);
            tabTransformDict[tabIndex] = tabTransform;
            CustomToggle customToggle = tabTransform.GetComponent<CustomToggle>();
            customToggle.Toggle.onValueChanged.RemoveAllListeners();
            customToggle.Toggle.onValueChanged.AddListener((state) => {
                this.OnToggleStateChange(tabIndex, tabInfo, state);
            });
            customToggle.txtLabel.text = tabInfo.tabLabel;
            this.toggleList.Add(customToggle);
        }

        public void SetAllOff() {
            toggleGroup.SetAllTogglesOff();
            this.horizontalLayoutGroup.enabled = false;
        }

        public void SetActiveTab(int index) {
            if (index > this.transform.childCount) {
                Debug.LogError("Tab index out of range, reset to 0");
                index = 0;
            }
            Toggle toggle = tabTransformDict[index].GetComponent<CustomToggle>().Toggle;
            toggle.isOn = true;
        }

        public void SetToggleInteractable(bool isEnable) {
            foreach (CustomToggle toggle in this.toggleList) {
                toggle.SetToggleInteractable(isEnable);
            }
        }

        /******** tog value change callback ***********/
        private Vector2 togLabelOffsetMax = Vector2.zero;
        private Transform childTransform = null;
        private void OnToggleStateChange(int tabIndex, TabTogsInfo tabInfo, bool state) {
            childTransform = tabTransformDict[tabIndex];
            if (state &&
                (childTransform.GetSiblingIndex() != (this.transform.childCount - 1))) {

                childTransform.SetAsLastSibling();
            }
            CustomToggle customToggle = childTransform.GetComponent<CustomToggle>();
            togLabelOffsetMax.y = state ? 0 : -9;// To do : Change the magic number
            customToggle.labelRectTransform.offsetMax = togLabelOffsetMax;

            if (tabInfo.tabPanelCG != null) {
                UIManager.SetUICanvasGroupEnable(tabInfo.tabPanelCG, state);
            }
            tabInfo.tabAction.Invoke(state);
        }
    }

}