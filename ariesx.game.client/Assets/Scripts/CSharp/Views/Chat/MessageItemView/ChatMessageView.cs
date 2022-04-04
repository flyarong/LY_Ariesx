using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Protocol;
using TMPro;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;

namespace Poukoute {
    public class ChatMessageView : BaseItemViewsHolder {
        #region ui define
        [SerializeField]
        private Transform pnlSystemMsg;
        [SerializeField]
        private TextMeshProUGUI txtSystemMsg;
        [SerializeField]
        private Transform pnlContent;
        [SerializeField]
        private Transform pnlMessage;
        [SerializeField]
        private Image imgBubbleBG;
        [SerializeField]
        private VerticalLayoutGroup bubbleLayout;
        [SerializeField]
        private Transform pnlPlayer;
        [SerializeField]
        private Button btnAvatar;
        [SerializeField]
        private TextMeshProUGUI txtMessage;
        [SerializeField]
        private Image imgAvatar;
        [SerializeField]
        private Transform pnlTime;
        [SerializeField]
        private TextMeshProUGUI txtTime;
        [SerializeField]
        private TextMeshProUGUI txtPlayerInfo;
        [SerializeField]
        private VerticalLayoutGroup verticalLayoutGroup;
        #endregion

        [HideInInspector]
        public float Height {
            get; set;
        }
        private static string OTHER = "other";
        private static string SELF = "self";
        private long TIME_OFFSET = 5 * 60;

        private int personalMessageAvater;
        private PersonalMessage.Message personalMessage;
        public int PersonalMessageAvater {
            get {
                return personalMessageAvater;
            }

            set {
                personalMessageAvater = value;
            }
        }
        public PersonalMessage.Message PersonalMessage {
            get {
                return this.personalMessage;
            }
            set {
                if (this.personalMessage != value) {
                    this.personalMessage = value;
                    this.OnPersonalMessageChange();
                }
            }
        }

        private ChatMessage worldChatMessage;
        public ChatMessage WorldChatMessage {
            get {
                return this.worldChatMessage;
            }
            set {
                if (this.worldChatMessage != value) {
                    this.worldChatMessage = value;
                    this.OnWorldChatMessageChange();
                }
            }
        }

        private AllianceMessage allianceMessage;
        public AllianceMessage AllianceMessage {
            get {
                return this.allianceMessage;
            }
            set {
                if (this.allianceMessage != value) {
                    this.allianceMessage = value;
                    this.OnAllianceMessageChange();
                }
            }
        }


        public long AllianceMessageTimestamp {
            get; set;
        }

        public UnityEvent OnAvatarClick {
            get {
                this.btnAvatar.onClick.RemoveAllListeners();
                return this.btnAvatar.onClick;
            }
        }
        /******************************************************/

        public override void MarkForRebuild() {
            GameHelper.ForceLayout(this.bubbleLayout);
            GameHelper.ForceLayout(this.verticalLayoutGroup);
        }

        public void ResetTimeVisible(long curMsgTime, long preMsgTime) {
            long timeOffset = curMsgTime - preMsgTime;
            //Debug.LogError("ResetTimeVisible " + this.ItemIndex + " " + timeOffset);
            this.pnlTime.gameObject.SetActiveSafe(timeOffset > TIME_OFFSET);
        }

        private void OnPersonalMessageChange() {
            this.pnlTime.gameObject.SetActiveSafe(true);
            this.txtMessage.text = this.personalMessage.Content;
            this.txtMessage.color =
                this.personalMessage.IsSend ? ArtConst.ChatMsgOwnColor : ArtConst.ChatMsgOtherColor;
            this.txtTime.text = GameHelper.HistoryTimeFormat(this.personalMessage.Timestamp);
            this.txtPlayerInfo.text = string.Empty;
            this.pnlPlayer.gameObject.SetActiveSafe(true);
            this.pnlSystemMsg.gameObject.SetActiveSafe(false);
            this.pnlContent.gameObject.SetActiveSafe(true);
            if (this.personalMessage.IsSend) {
                this.imgAvatar.sprite = RoleManager.GetRoleAvatar();
                this.pnlMessage.SetAsFirstSibling();
                this.imgBubbleBG.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.chatMessageBackgroundPrefix, SELF);
                verticalLayoutGroup.childAlignment = TextAnchor.UpperRight;
                this.bubbleLayout.padding.left = 15;
                this.bubbleLayout.padding.right = 30;
            } else {
                this.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(PersonalMessageAvater);
                this.pnlMessage.SetAsLastSibling();
                this.imgBubbleBG.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.chatMessageBackgroundPrefix, OTHER);
                verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                this.bubbleLayout.padding.left = 30;
                this.bubbleLayout.padding.right = 15;
            }
        }

        private void OnWorldChatMessageChange() {
            this.pnlTime.gameObject.SetActiveSafe(true);
            this.txtTime.text = GameHelper.HistoryTimeFormat(this.worldChatMessage.Timestamp);

            bool isSystemMsg = !this.worldChatMessage.Template.CustomIsEmpty();
            this.pnlSystemMsg.gameObject.SetActiveSafe(isSystemMsg);
            this.pnlContent.gameObject.SetActiveSafe(!isSystemMsg);
            this.pnlPlayer.gameObject.SetActiveSafe(!isSystemMsg);
            if (isSystemMsg) {
                //this.pnlContent.gameObject.SetActiveSafe(false);
                //Debug.LogError(this.worldChatMessage.Template);
                string[] infoArgs = this.worldChatMessage.Params.ToArray();
                NPCCityConf npcCityConf = NPCCityConf.GetConf(infoArgs[0]);
                infoArgs[0] = NPCCityConf.GetNpcCityLocalName(npcCityConf.name, npcCityConf.isCenter);
                this.txtSystemMsg.text = string.Format(
                    LocalManager.GetValue(this.worldChatMessage.Template), infoArgs);
            } else {
                this.txtMessage.text = this.worldChatMessage.Content;
                bool isOwnMsg = this.worldChatMessage.PlayerId.CustomEquals(RoleManager.GetRoleId());
                this.txtMessage.color =
                    isOwnMsg ? ArtConst.ChatMsgOwnColor : ArtConst.ChatMsgOtherColor;
                string allianceName = string.Empty;
                if (!this.worldChatMessage.AllianceName.CustomIsEmpty()) {
                    allianceName = string.Concat(" [", this.worldChatMessage.AllianceName, "] ");
                }
                string playerName = string.Format("<color=#777056FF>{0}</color>", this.worldChatMessage.PlayerName);
                string NPCCityName = string.Empty;
                NPCCityName = string.Concat(" [", NPCCityConf.GetMapSNLocalName(this.worldChatMessage.MapSN), "]");
                if (isOwnMsg) {
                    this.imgAvatar.sprite = RoleManager.GetRoleAvatar();
                    this.txtPlayerInfo.text = string.Format(
                    "<size=22><color=#9C9480FF>{0}{1}</color></size> {2}", allianceName, NPCCityName, playerName);
                    this.pnlMessage.SetAsFirstSibling();
                    //this.txtPlayerInfo.alignment = TextAlignmentOptions.BaselineRight;
                    this.txtPlayerInfo.alignment = TextAlignmentOptions.BottomRight;
                    this.imgBubbleBG.sprite =
                        ArtPrefabConf.GetSprite(SpritePath.chatMessageBackgroundPrefix, SELF);
                    verticalLayoutGroup.childAlignment = TextAnchor.UpperRight;
                    this.bubbleLayout.padding.left = 15;
                    this.bubbleLayout.padding.right = 30;
                } else {
                    this.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(this.worldChatMessage.Avatar);
                    this.txtPlayerInfo.text = string.Format("{0} <size=22><color=#9C9480FF>{1}{2}</color></size>",
                       playerName, allianceName, NPCCityName);
                    this.pnlMessage.SetAsLastSibling();
                    this.txtPlayerInfo.alignment = TextAlignmentOptions.BottomLeft;
                    this.imgBubbleBG.sprite =
                        ArtPrefabConf.GetSprite(SpritePath.chatMessageBackgroundPrefix, OTHER);
                    verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                    this.bubbleLayout.padding.left = 30;
                    this.bubbleLayout.padding.right = 15;
                }
            }
        }

        private void OnAllianceMessageChange() {
            this.txtTime.text = GameHelper.HistoryTimeFormat(this.allianceMessage.Timestamp);
            bool isAllianceChatMsg = (this.allianceMessage.Chat != null);
            this.pnlSystemMsg.gameObject.SetActiveSafe(!isAllianceChatMsg);
            this.pnlContent.gameObject.SetActiveSafe(isAllianceChatMsg);
            this.pnlPlayer.gameObject.SetActiveSafe(isAllianceChatMsg);
            if (isAllianceChatMsg) {
                this.OnAllianceChatMessageChange();
            } else {
                this.OnAllianceSystemMsgChange();
            }
        }

        private void OnAllianceChatMessageChange() {
            AllianceMessageChat allianceChatMessage = this.allianceMessage.Chat;
            bool isOwnMsg = allianceChatMessage.PlayerId.CustomEquals(RoleManager.GetRoleId());
            this.txtMessage.text = allianceChatMessage.Content;
            string playerName =
                string.Format("<color=#777056FF>{0}</color>", allianceChatMessage.PlayerName);
            if (isOwnMsg) {
                this.imgAvatar.sprite = RoleManager.GetRoleAvatar();
                this.txtPlayerInfo.text = string.Format("<size=22><color=#9C9480FF>[{0}]</color></size> {1}",
               NPCCityConf.GetMapSNLocalName(allianceChatMessage.PlayerMapSN), playerName);
                this.pnlMessage.SetAsFirstSibling();
            } else {
                this.imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(allianceChatMessage.Avatar);
                this.txtPlayerInfo.text = string.Format("{0} <size=22><color=#9C9480FF>[{1}]</color></size>", playerName,
               NPCCityConf.GetMapSNLocalName(allianceChatMessage.PlayerMapSN));
                this.pnlMessage.SetAsLastSibling();
            }

            this.txtMessage.color =
                    isOwnMsg ? ArtConst.ChatMsgOwnColor : ArtConst.ChatMsgOtherColor;
            this.txtPlayerInfo.alignment =
                isOwnMsg ? TextAlignmentOptions.BottomRight : TextAlignmentOptions.BottomLeft;
            this.imgBubbleBG.sprite = ArtPrefabConf.GetSprite(
                    SpritePath.chatMessageBackgroundPrefix, isOwnMsg ? SELF : OTHER);
            verticalLayoutGroup.childAlignment =
                isOwnMsg ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
            this.bubbleLayout.padding.left = isOwnMsg ? 15 : 30;
            this.bubbleLayout.padding.right = isOwnMsg ? 30 : 15;
        }

        private void OnAllianceSystemMsgChange() {
            if (this.allianceMessage.Info != null) {
                AllianceMessageInfo allianceMsginfo = this.allianceMessage.Info;
                ulong tmplateHashCode = allianceMsginfo.Template.CustomGetHashCode();
                string[] infoArgs = allianceMsginfo.Args.ToArray();
                if (tmplateHashCode == LocalHashConst.tpl_member_role_up ||
                    tmplateHashCode == LocalHashConst.tpl_member_role_down) {
                    infoArgs[1] = LocalManager.GetValue(
                        string.Concat("alliance_position_",
                        Enum.Parse(typeof(AllianceRole), allianceMsginfo.Args[1])).ToString().ToLower());
                }
                this.txtSystemMsg.text = string.Format(
                        LocalManager.GetValue(allianceMsginfo.Template), infoArgs);
            } else if (this.allianceMessage.JoinRequest != null) {
                AllianceMessageJoinRequest allianceMsgjoinRequest = this.allianceMessage.JoinRequest;
                string localTemplate = allianceMsgjoinRequest.IsApproval ?
                    LocalManager.GetValue(LocalHashConst.accept_request) :
                    LocalManager.GetValue(LocalHashConst.refuse_request);

                this.txtSystemMsg.text = string.Format(localTemplate,
                    allianceMsgjoinRequest.OperatorName);
            }
        }
    }
}