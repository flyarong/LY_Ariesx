using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

namespace Poukoute {
    public class AllianceMemberItemView : BaseItemViewsHolder {
        public UnityEvent OnInfoClick {
            get {
                this.btnItemInfo.onClick.RemoveAllListeners();
                return this.btnItemInfo.onClick;
            }
        }

        public AllianceMemberWithIndex MemberData {
            get {
                return this.memberData;
            }
            set {
                memberData = value;
                this.isLeader = (value.member.Role == (int)AllianceRole.Owner);
                this.isInAbdication =
                    value.abdicationTime * 1000 > RoleManager.GetCurrentUtcTime();
                //Debug.LogError(value.abdicationTime + " " + isInAbdication);
                this.OnMemberDataChange();
            }
        }
        private AllianceMemberWithIndex memberData;

        private bool isLeader = false;
        private bool isInAbdication = false;
        //private bool inAlliance = false;

        #region UI component cache
        //private GameObject ui;
        [SerializeField]
        private TextMeshProUGUI txtName;
        [SerializeField]
        private Image imgAvatar;
        [SerializeField]
        private TextMeshProUGUI txtOfficial;
        [SerializeField]
        private TextMeshProUGUI txtAbdication;
        [SerializeField]
        private TextMeshProUGUI txtLoginTime;
        [SerializeField]
        private Transform pnlFallenHeadMark;
        [SerializeField]
        private TextMeshProUGUI txtActivity;
        [SerializeField]
        private TextMeshProUGUI txtCrown;
        [SerializeField]
        private Button btnItemInfo;
        [SerializeField]
        private Transform pnlFallen;
        #endregion

        private long abdicationTime = 0;

        //Todo : Overload Awake cover AbstractViewsHolder.Awake
        //void Awake() {
        //    UpdateManager.Regist(UpdateInfo.AllianceMemberItemView, this.UpdateAction);
        //}

        private void UpdateAction() {
            if (this.isLeader && this.isInAbdication &&
                this.txtAbdication.gameObject.activeSelf) {
                abdicationTime = this.memberData.abdicationTime * 1000 - RoleManager.GetCurrentUtcTime();
                abdicationTime = (long)Mathf.Max(0, abdicationTime);
                this.txtAbdication.text = string.Format("{0} {1}", LocalManager.GetValue(LocalHashConst.alliance_demise),
                    GameHelper.TimeFormat(abdicationTime));
            } else {
                this.txtAbdication.gameObject.SetActiveSafe(false);
            }
        }

        private void OnMemberDataChange() {
            this.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(memberData.member.Avatar);
            this.txtName.text = memberData.member.Name;
            string role = Enum.GetName(typeof(AllianceRole), memberData.member.Role).ToLower();
            this.txtOfficial.text =
                    LocalManager.GetValue("alliance_position_", role);
            this.txtAbdication.gameObject.SetActiveSafe(this.isLeader && this.isInAbdication);
            this.txtActivity.text = memberData.member.AllianceExp.ToString();
            this.txtCrown.text = GameHelper.GetFormatNum(memberData.member.Force);
            this.txtLoginTime.text = string.Concat(
                LocalManager.GetValue(LocalHashConst.label_alliance_last_login_time),
                "  ",
            GameHelper.DateFormat(
                memberData.member.LoginTime,
                "yyyy-MM-dd HH:mm"
            )
            );

            this.pnlFallen.gameObject.SetActiveSafe(memberData.member.IsFallen);
            this.pnlFallenHeadMark.gameObject.SetActiveSafe(false);

            abdicationTime = this.memberData.abdicationTime * 1000 - RoleManager.GetCurrentUtcTime();
            if (abdicationTime > 0) {
                UpdateManager.Regist(UpdateInfo.AllianceMemberItemView, this.UpdateAction);
            }
        }
    }
}
