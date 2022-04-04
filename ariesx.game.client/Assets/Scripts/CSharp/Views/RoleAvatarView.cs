using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using ProtoBuf;

namespace Poukoute {
    public class RoleAvatarView : BaseView {
        private RoleAvatarViewModel viewModel;
        private RoleAvatarViewPreference viewPref;
        private PlayerInfoStateViewModel parent;
        /* UI Members*/

        /*************/

        void Awake() {
            this.viewModel = this.gameObject.GetComponent<RoleAvatarViewModel>();
            this.parent = this.transform.parent.GetComponent<PlayerInfoStateViewModel>();
        }

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIRoleAvatar");
            this.viewPref = this.ui.transform.GetComponent<RoleAvatarViewPreference>();
            this.viewPref.btnClose.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.btnBackClose.onClick.AddListener(this.OnCloseBtnClick);
            this.viewPref.btnChangeAvatar.onClick.AddListener(this.OnChangeAvatarClick);
            this.viewPref.btnSelectedAvatar.onClick.AddListener(this.OnSelectedAvatarClick);
            /* Cache the ui components here */
        }

        private void OnSelectedAvatarClick() {
            this.viewPref.btnSelectedAvatar.gameObject.SetActiveSafe(false);
            this.viewModel.currAvatar = -1;
        }

        private void OnChangeAvatarClick() {
            if (this.viewModel.currAvatar == -1) {
                ShowHint(LocalManager.GetValue(LocalHashConst.player_hero_avater_choose_one));
                return;
            }
            ChangeAvatar();
        }

        private void ChangeAvatar() {
            this.viewPref.btnChangeAvatar.interactable = false;
            ChangeAvatarReq changeAvatarReq = new ChangeAvatarReq();
            changeAvatarReq.Avatar = this.viewModel.currAvatar;
            NetManager.SendMessage(changeAvatarReq, typeof(ChangeAvatarAck).Name, this.ChangeAvatarAck);
        }

        private void ChangeAvatarAck(IExtensible message) {
            this.viewPref.btnChangeAvatar.interactable = true;
            RoleManager.SetRoleAvatar(this.viewModel.currAvatar);
            this.viewModel.Hide(() => {
                this.ClearDetail();
                this.parent.ChangeAvatar(LocalManager.GetValue(LocalHashConst.player_hero_avater_set_success));
            });
            this.viewModel.IsChangeAvatarSuccess = true;
        }

        private void OnCloseBtnClick() {
            this.viewModel.Hide(() => {
                this.ClearDetail();
            });
        }

        /* Propert change function */

        /***************************/

        public void ShowHint(string hintDes) {
            this.viewPref.txtHint.text = hintDes;
            AnimationManager.Animate(this.viewPref.pnlHint.gameObject, "Show");
        }

        public override void PlayShow() {
            base.PlayShow(() => {
                this.SetDetail();
            });
        }

        private void SetDetail() {
            this.viewModel.currAvatar = -1;
            this.viewModel.RefershAvatarConfs();
            GameObject avatarObj = null;
            SetRoleAvatar(avatarObj, this.viewModel.unlockAvatarConfs);
            SetRoleAvatar(avatarObj, this.viewModel.lockAvatarConfs);
        }

        private GameObject SetRoleAvatar(GameObject avatarObj, Dictionary<string, BaseConf> avatarConfs) {
            foreach (var pair in avatarConfs) {
                PlayerHeroAvatarConf avatar = pair.Value as PlayerHeroAvatarConf;
                avatarObj = PoolManager.GetObject(
                    PrefabPath.roleAvatar,
                    this.viewPref.glgContentAvatar.transform
                );
                RoleAvatarItemView avatarItem = avatarObj.GetComponent<RoleAvatarItemView>();

                bool isCurrentAvatar = pair.Key == RoleManager.GetRoleAvatarKey().ToString();
                bool isUnlockAvatar = this.viewModel.AllExistingHero.ContainsKey(avatar.avatarName);
                //if (RoleManager.GetRoleAvatarKey().ToString() == "0" &&
                //this.viewModel.unlockAvatarConfs.ContainsKey("0")) {
                //    avatarItem.SetAvatarCurrentShow(true);
                //    Debug.LogError("第一个英雄");
                //}
                avatarItem.SetRoleAvatar(
                    ArtPrefabConf.GetRoleAvatarSprite(avatar.avatarName),
                    isCurrentAvatar, isUnlockAvatar, () => {
                        if (!isUnlockAvatar) {
                            ShowHint(string.Format(
                                LocalManager.GetValue(LocalHashConst.player_hero_avater_locked),
                                HeroAttributeConf.GetLocalName(avatar.avatarName))
                            );
                            return;
                        }
                        this.viewModel.currAvatar = int.Parse(avatar.avatarId);
                        if (!isCurrentAvatar) {
                            this.viewPref.imgSelectedAvatar.sprite = avatarItem.GetAvatarSprite();
                            this.viewPref.btnSelectedAvatar.transform.parent = avatarItem.transform;
                            this.viewPref.btnSelectedAvatar.transform.localPosition = Vector3.zero;
                            this.viewPref.btnSelectedAvatar.gameObject.SetActiveSafe(true);
                        } else {
                            this.viewModel.currAvatar = -1;
                            ShowHint(LocalManager.GetValue(LocalHashConst.player_hero_avater_using));
                        }
                    }
                );
            }

            return avatarObj;
        }

        public override void PlayHide() {
            base.PlayHide();
        }

        private void ClearDetail() {
            this.viewPref.btnSelectedAvatar.transform.parent = this.transform;
            foreach (Transform pair in this.viewPref.glgContentAvatar.transform) {
                GameObject.Destroy(pair.gameObject);
            }
        }
    }
}
