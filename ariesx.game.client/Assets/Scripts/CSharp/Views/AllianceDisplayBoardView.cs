using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
public enum DisplayType {
    AllianceMembers,
    AllianceDisplayBoard,
    None
}

namespace Poukoute {
    public class AllianceDisplayBoardView : BaseView {
        private AllianceDisplayBoardViewModel viewModel;
        private AllianceDisplayBoardViewPeference viewPref;
        private string allianceIntroTitle = string.Empty;
        /* UI Members*/
        #region TxtKey
        public List<string> AllianceMembersTitleList;
        public List<string> AllianceMembersContentList;
        public List<string> AllianceDisplayBoardTitleList;
        public List<string> AllianceDisplayBoardContentList;
        #endregion

        /****************************/

        protected override void OnUIInit() {
            this.viewModel = this.gameObject.GetComponent<AllianceDisplayBoardViewModel>();
            this.ui = UIManager.GetUI("UIAllianceDisplayBoard");
            this.viewPref = this.ui.transform.transform.GetComponent<AllianceDisplayBoardViewPeference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);

            AllianceMembersTitleList = new List<string>() {
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_owner_title),
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_leader_title),
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_elder_title),
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_contribution_title)
            };

            AllianceMembersContentList = new List<string>() {
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_owner_detail),
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_leader_detail),
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_elder_detail),
                LocalManager.GetValue(LocalHashConst.alliance_member_intro_content_contribution_detail)
            };
            AllianceDisplayBoardTitleList = new List<string>() {
                LocalManager.GetValue(LocalHashConst.alliance_intro_content_level_title),
                LocalManager.GetValue(LocalHashConst.alliance_intro_content_exp_title),
                LocalManager.GetValue(LocalHashConst.alliance_intro_content_contribution_title),
                LocalManager.GetValue(LocalHashConst.alliance_intro_content_multiplies_title)
            };

            AllianceDisplayBoardContentList = new List<string>() {
                string.Format(LocalManager.GetValue(LocalHashConst.alliance_intro_content_level_detail),
                AllianceLevelConf.GetAllianceMaxLevel(), AllianceLevelConf.GetAllianceMaxMembers()),
                LocalManager.GetValue(LocalHashConst.alliance_intro_content_exp_detail),
                LocalManager.GetValue(LocalHashConst.alliance_intro_content_contribution_detail),
                LocalManager.GetValue(LocalHashConst.alliance_intro_content_multiplies_detail)
            };
        }

        public void SetDisplayBoradContent(DisplayType tape) {
            this.allianceIntroTitle = null;
            if (string.IsNullOrEmpty(allianceIntroTitle) &&
                tape == DisplayType.AllianceDisplayBoard) {
                this.allianceIntroTitle = LocalManager.GetValue(LocalHashConst.alliance_intro_title);
            } else if (string.IsNullOrEmpty(allianceIntroTitle) &&
                tape == DisplayType.AllianceMembers) {
                this.allianceIntroTitle = LocalManager.GetValue(LocalHashConst.alliance_member_intro_title);
            }
            this.viewPref.txtTitle.text = allianceIntroTitle;
            switch (tape) {
                case DisplayType.AllianceMembers:
                    this.SetAllianceMembers(DisplayType.AllianceMembers);
                    break;
                case DisplayType.AllianceDisplayBoard:
                    this.SetAllianceMembers(DisplayType.AllianceDisplayBoard);
                    break;
            }
        }

        private void SetAllianceMembers(DisplayType type) {
            GameHelper.ClearChildren(viewPref.PnlDescList);
            int count = this.AllianceMembersTitleList.Count;
            GameHelper.ResizeChildreCount(this.viewPref.PnlDescList,
                count, PrefabPath.pnlAllianceDescTypeItem);
            if (type == DisplayType.AllianceMembers) {
                for (int i = 0; i < count; i++) {
                    AllianceDescTypeViewItem viewItem = this.viewPref.PnlDescList.GetChild(i).
                        GetComponent<AllianceDescTypeViewItem>();
                    viewItem.SetCount(AllianceMembersTitleList[i], AllianceMembersContentList[i]);
                }
            } else {
                for (int i = 0; i < count; i++) {
                    AllianceDescTypeViewItem viewItem = this.viewPref.PnlDescList.GetChild(i).
                        GetComponent<AllianceDescTypeViewItem>();
                    viewItem.SetCount(AllianceDisplayBoardTitleList[i], AllianceDisplayBoardContentList[i]);
                }
            }
        }

        private void Format() {
            this.viewPref.pnlDescListLayoutGroup.enabled = true;
            this.viewPref.customContentSizeFitter.onSetLayoutVertical.AddListener(() => {
                this.viewPref.pnlDescListLayoutGroup.
                  GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                this.viewPref.pnlDescListLayoutGroup.enabled = false;
            });
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }
    }
}
