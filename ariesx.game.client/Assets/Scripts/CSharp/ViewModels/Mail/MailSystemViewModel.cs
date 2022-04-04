using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using Protocol;
using System;
using UnityEngine.UI;

namespace Poukoute {
    public enum SystemBodyTpl {
        mail_marchfailed_ownerchanged,
        mail_marchfailed_protection,
        mail_siegereward,
        mail_alliancerole,
        server_player_has_been_kicked_body,
        server_alliance_auto_dissolved_body,
        server_alliance_fall_npc_city_body,
        server_alliance_fall_pass_body,
        server_alliance_lose_npc_city_body,
        server_alliance_lose_pass_body,
        server_was_fallen_body,
        server_free_for_help_body,
        server_free_for_pay_body,
        server_was_approval_join_alliance_body,
        server_alliance_dissolved_body,
        server_first_alliance_fall_npc_city_subject,
        server_first_alliance_fall_npc_city_body,
        server_login_reward_title,
        server_login_reward_content,
        server_alliance_fall_npc_city_body_normal,
        server_force_level_downgraded_via_tile_abandon_body,
        server_force_level_downgraded_via_tile_beoccupied_body,
        none
    }

    public class MailSystemViewModel: BaseViewModel {
        private MailViewModel parent;
        private MailModel model;
        private MailSystemView view;
        /* Model data get set */

        public MailChannel Channel {
            get {
                return this.model.system;
            }
        }

        public List<SystemMessage> SystemMailList {
            get {
                return this.model.systemList;
            }
        }

        public Dictionary<string, SystemMessage> SystemMailDict {
            get {
                return this.model.systemDict;
            }
        }

        public bool IsLoadAll {
            get {
                return this.model.system.isLoadAll;
            }
            set {
                this.model.system.isLoadAll = value;
            }
        }

        private SystemMessage systemMail = null;
        public SystemMessage SystemMail {
            get {
                return this.systemMail;
            }
            set {
                if (this.systemMail != value) {
                    this.systemMail = value;
                    this.OnMailChange();
                }
            }
        }
        /**********************/

        /* Other members */
        public bool NeedRefresh { get; set; }
        public bool IsVisible { get { return this.view.IsVisible; } }
        private bool isRequestPage = false;
        /*****************/

        void Awake() {
            this.view = this.gameObject.AddComponent<MailSystemView>();
            this.parent = this.transform.parent.GetComponent<MailViewModel>();
            this.model = ModelManager.GetModelData<MailModel>();
            this.NeedRefresh = true;
        }

        public void NewSystemMail() {
            this.NeedRefresh = true;
            if (this.view.IsVisible) {
                this.Show();
            }
        }

        public void Show() {
            this.view.Show();
            if (this.NeedRefresh) {
                this.IsLoadAll = false;
                this.Channel.page = 0;
                this.GetMailReq();
            }
        }

        public void Hide() {
            if (this.view.IsVisible) {
                this.view.Hide();
                this.view.HideSystemMailDetail();
                this.NeedRefresh = true;
            }
        }

        // To do: set needrefresh.
        protected override void OnReLogin() {
            this.NewSystemMail();
        }

        public void HideMail() {
            this.parent.Hide();
            this.view.HideSystemMailDetail();
        }

        public void OnHeroClick(string heroName) {
            this.parent.OnHeroClick(heroName);
        }

        public string GetSystemContent(SystemMessage chosenMail) {
            //Debug.LogError(chosenMail.Body);
            string localContent = LocalManager.GetValue(chosenMail.Body);
            SystemBodyTpl bodyTpl = SystemBodyTpl.none;
            if (Enum.IsDefined(typeof(SystemBodyTpl), chosenMail.Body)) {
                bodyTpl = (SystemBodyTpl)Enum.Parse(typeof(SystemBodyTpl), chosenMail.Body);
            }
            switch (bodyTpl) {
                case SystemBodyTpl.mail_marchfailed_ownerchanged:
                case SystemBodyTpl.mail_marchfailed_protection:
                    string troopId = "1"; //this.mail.Params[0];
                    string coorStr = chosenMail.Params[0];
                    return string.Format(localContent, troopId, coorStr);
                case SystemBodyTpl.mail_siegereward:
                    string cityName = string.Concat("<color=#239e16>",
                        NPCCityConf.GetConf(chosenMail.Params[0]).name, "</color>");
                    string level = chosenMail.Params[1];
                    return string.Format(localContent, cityName, level);
                case SystemBodyTpl.mail_alliancerole:
                    string userName = chosenMail.Params[0];
                    string role = chosenMail.Params[1];
                    return string.Format(localContent, userName, role);
                case SystemBodyTpl.server_player_has_been_kicked_body:
                case SystemBodyTpl.server_alliance_auto_dissolved_body:
                case SystemBodyTpl.server_free_for_pay_body:
                case SystemBodyTpl.server_alliance_dissolved_body:
                    return string.Format(localContent, chosenMail.Params[0]);
                case SystemBodyTpl.server_alliance_fall_npc_city_body:
                    string allianceName = chosenMail.Params[0];
                    level = chosenMail.Params[1];
                    NPCCityConf cityConf = NPCCityConf.GetConf(chosenMail.Params[2]);
                    cityName = string.Concat("<color=#239e16>",
                        NPCCityConf.GetNpcCityLocalName(cityConf.name, cityConf.isCenter), "</color>");
                    return string.Format(localContent, allianceName, level, cityName, cityName);
                case SystemBodyTpl.server_alliance_fall_pass_body:
                    allianceName = chosenMail.Params[0];
                    level = chosenMail.Params[1];
                    cityName = string.Concat("<color=#239e16>",
                        LocalManager.GetValue("pass_", chosenMail.Params[2]), "</color>");
                    return string.Format(localContent, allianceName, level, cityName);
                case SystemBodyTpl.server_alliance_lose_npc_city_body:
                    allianceName = chosenMail.Params[0];
                    level = chosenMail.Params[1]; cityConf = NPCCityConf.GetConf(chosenMail.Params[2]);
                    cityName = string.Concat("<color=#239e16>",
                        NPCCityConf.GetNpcCityLocalName(cityConf.name, cityConf.isCenter), "</color>");
                    string enemyAllianceName = chosenMail.Params[3];
                    return string.Format(localContent, allianceName, level, cityName, enemyAllianceName);
                case SystemBodyTpl.server_alliance_lose_pass_body:
                    allianceName = chosenMail.Params[0];
                    level = chosenMail.Params[1];
                    cityName = string.Concat("<color=#239e16>",
                        LocalManager.GetValue("pass_", chosenMail.Params[2]), "</color>");
                    enemyAllianceName = chosenMail.Params[3];
                    return string.Format(localContent, allianceName, level, cityName, enemyAllianceName);
                case SystemBodyTpl.server_alliance_fall_npc_city_body_normal:
                    allianceName = chosenMail.Params[0];
                    level = chosenMail.Params[1]; cityConf = NPCCityConf.GetConf(chosenMail.Params[2]);
                    cityName = string.Concat("<color=#239e16>",
                        NPCCityConf.GetNpcCityLocalName(cityConf.name, cityConf.isCenter), "</color>");
                    return string.Format(localContent, allianceName, level, cityName, cityName);
                case SystemBodyTpl.server_first_alliance_fall_npc_city_body:
                    allianceName = chosenMail.Params[0];
                    level = chosenMail.Params[1]; cityConf = NPCCityConf.GetConf(chosenMail.Params[2]);
                    cityName = string.Concat("<color=#239e16>",
                        NPCCityConf.GetNpcCityLocalName(cityConf.name, cityConf.isCenter), "</color>");
                    string rewardGem = chosenMail.Params[3];
                    return string.Format(localContent, allianceName, level, cityName, rewardGem.ToString());
                case SystemBodyTpl.server_was_fallen_body:
                case SystemBodyTpl.server_free_for_help_body:
                    string attackerName = chosenMail.Params[0];
                    string attackerAllianceName = chosenMail.Params[1];
                    return string.Format(localContent, attackerName, attackerAllianceName);
                case SystemBodyTpl.server_was_approval_join_alliance_body:
                    string playerName = chosenMail.Params[0];
                    allianceName = chosenMail.Params[1];
                    return string.Format(localContent, playerName, allianceName);
                case SystemBodyTpl.server_force_level_downgraded_via_tile_abandon_body:
                    int tileType = int.Parse(chosenMail.Params[0]);
                    level = chosenMail.Params[1]; string coord = chosenMail.Params[2];
                    string forcelevel = chosenMail.Params[3];
                    ElementType pointType = (ElementType)tileType;
                    string tileName = LocalManager.GetValue("name_" + pointType.ToString());
                    return string.Format(localContent, tileName, level, coord, forcelevel);
                case SystemBodyTpl.server_force_level_downgraded_via_tile_beoccupied_body:
                    tileType = int.Parse(chosenMail.Params[0]);
                    level = chosenMail.Params[1]; coord = chosenMail.Params[2];
                    playerName = chosenMail.Params[3]; allianceName = chosenMail.Params[4];
                    forcelevel = chosenMail.Params[5]; pointType = (ElementType)tileType;
                    tileName = LocalManager.GetValue("name_" + pointType.ToString());
                    return string.Format(localContent, tileName, level, coord, playerName,
                        allianceName, forcelevel);
                default:
                    return string.Format(localContent, chosenMail.Params.ToArray());
            }
        }

        public void GetMailReq() {
            if (this.IsLoadAll || this.isRequestPage || !this.view.IsVisible) {
                return;
            }
            this.isRequestPage = true;
            GetSystemMessagesReq getMails = new GetSystemMessagesReq() {
                Page = ++this.Channel.page
            };
            NetManager.SendMessage(getMails, typeof(GetSystemMessagesAck).Name,
                this.GetMailAck);
        }

        private bool mailIdEmpty = false;
        public void MarkMailIsRead(string mailId = "") {
            this.mailIdEmpty = mailId.CustomIsEmpty();
            MarkSystemMessageReadReq markReadReq = new MarkSystemMessageReadReq();
            if (!mailIdEmpty && !this.systemMail.IsRead) {
                markReadReq.Id.Add(mailId);
            }
            NetManager.SendMessage(markReadReq, typeof(MarkSystemMessageReadAck).Name,
                this.OnMarkSystemMessageRead);
        }

        private void OnMarkSystemMessageRead(IExtensible message) {
            if (this.mailIdEmpty) {
                foreach (SystemMessage systemMsg in this.SystemMailList) {
                    systemMsg.IsRead = true;
                }
                this.view.Refresh();
            } else {
                this.systemMail.IsRead = true;
            }
        }


        private bool gettingAttachMent = false;
        public void GetMailAttachMent() {
            if (this.SystemMail.IsCollect) {
                UIManager.ShowTip(LocalManager.GetValue(
                    LocalHashConst.server_attachment_has_been_collected), TipType.Info);
                return;
            }

            if (!this.gettingAttachMent) {
                this.gettingAttachMent = true;
                OpenAttachmentReq collectReq = new OpenAttachmentReq() {
                    Id = this.systemMail.Id
                };
                NetManager.SendMessage(collectReq,
                    typeof(OpenAttachmentAck).Name,
                    this.OnReceiveRewardAck);
            }
        }

        public Dictionary<Resource, Transform> resourceTransDict =
            new Dictionary<Resource, Transform>();
        private void OnReceiveRewardAck(IExtensible message) {
            OpenAttachmentAck reward = message as OpenAttachmentAck;
            //Debug.LogError(reward.Attachment.Fragments.Count + " " +
            //    reward.Attachment.Chests.Count);
            //foreach (var item in reward.Attachment.Fragments) {
            //    Debug.LogError(item.Count);
            //}

            this.gettingAttachMent = false;
            this.systemMail.IsCollect = true;
            this.view.SetBtnReceiveReward(true);

            if (reward.Results.Count > 0) {
                this.parent.ShowOpenChest(reward.Results, () => this.CollectResources(reward));
            } else {
                this.CollectResources(reward);
            }

        }

        private void CollectResources(OpenAttachmentAck reward) {
            Protocol.Resources resources = reward.Resources;
            Protocol.Currency currency = reward.Currency;
            GameHelper.CollectResources(reward.Attachment,
                resources, currency, this.resourceTransDict);
        }


        /* Add 'NetMessageAck' function here*/
        private void GetMailAck(IExtensible message) {
            GetSystemMessagesAck getMailsAck = message as GetSystemMessagesAck;
            this.isRequestPage = false;
            if (getMailsAck.Messages.Count < this.Channel.pageCount) {
                this.IsLoadAll = true;
            }

            if (this.NeedRefresh) {
                this.SystemMailList.Clear();
                this.SystemMailDict.Clear();
            }

            int originCount = this.SystemMailList.Count;
            int newDataCount = getMailsAck.Messages.Count;
            foreach (SystemMessage mail in getMailsAck.Messages) {
                if (!this.SystemMailDict.ContainsKey(mail.Id)) {
                    this.SystemMailList.Add(mail);
                    this.SystemMailDict[mail.Id] = mail;
                }
#if UNITY_EDITOR
                else {
                    --newDataCount;
                    Debug.LogError(mail.Id + " already contained!!");
                }
#endif
            }
            this.view.SetMail();

            if (this.Channel.page == 1 && this.view.IsVisible) {
                this.NeedRefresh = false;
                this.view.ResetItems(newDataCount);
                if (!this.IsLoadAll) {
                    this.view.DataRequestAction += this.GetMailReq;
                }
            } else {
                this.view.InsertItems(originCount, newDataCount);
            }
        }

        private void OnMailChange() {
            if (!this.systemMail.IsRead) {
                this.MarkMailIsRead(this.SystemMail.Id);
            }
            this.view.ShowSystemMailDetail();
        }

        private void OnInvisible() {
            this.isRequestPage = false;
            this.NeedRefresh = true;
            this.IsLoadAll = false;
            this.Channel.page = 0;
        }
    }
}
