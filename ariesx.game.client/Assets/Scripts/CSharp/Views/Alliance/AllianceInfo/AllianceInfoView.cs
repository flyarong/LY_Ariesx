using Protocol;

namespace Poukoute {
    public class AllianceInfoView : BaseView {
        private AllianceInfoViewModel viewModel;
        private AllianceInfoViewPreference viewPref;

        /* Other data */
        private bool allianceMemberFull = false;
        private bool townhallLevelNotValide = false;
        private bool isInAlliance = false;
        private bool allianceJoinClose = false;
        private bool rejoinCoolingDone = true;
        private bool canJoinDirectly = false;
        private bool reachAllianceForceLimit = false;

        protected override void OnUIInit() {
            this.ui = UIManager.GetUI("UIAllianceInfo");
            this.viewModel = this.GetComponent<AllianceInfoViewModel>();
            this.viewPref = this.ui.transform.GetComponent<AllianceInfoViewPreference>();
            this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnJoin.onClick.AddListener(this.OnBtnJoinClick);
            this.viewPref.btnInfo.onClick.AddListener(this.ShowBtnAllianceHintClick);
            this.viewPref.btnMember.onClick.AddListener(this.ShowMemberClick);
            this.viewPref.sliderAllianceExp.onValueChanged.AddListener(this.OnSliderValueChange);
        }

        private void ShowBtnAllianceHintClick() {
            this.viewModel.ShowAllianceDisplayBoard(DisplayType.AllianceDisplayBoard);
        }

        private void OnSliderValueChange(float value) {
            this.viewPref.txtExperience.text = string.Concat(
                value - 1, "/", this.viewPref.sliderAllianceExp.maxValue);
        }

        public void SetAllianceInfo() {
            this.InitUI();
            int allianceLevel = AllianceLevelConf.GetAllianceLevelByExp(this.viewModel.ViewAlliance.Exp);

            this.viewPref.txtAllianceName.text = this.viewModel.ViewAlliance.Name;
            this.viewPref.txtAllianceLevel.text = GameHelper.GetLevelLocal(allianceLevel);
            this.viewPref.imgAllianceEmblem.sprite =
                ArtPrefabConf.GetAliEmblemSprite(this.viewModel.ViewAlliance.Emblem);
            bool isReachMaxAlyLevel = allianceLevel >= AllianceLevelConf.GetAllianceMaxLevel();
            this.viewPref.pnlAllianceExp.gameObject.SetActiveSafe(!isReachMaxAlyLevel);
            if (!isReachMaxAlyLevel) {
                int allianceExp = AllianceLevelConf.GetAllianceUpgradeExpByLevel(allianceLevel);
                this.viewPref.sliderAllianceExp.maxValue = allianceExp;
                this.viewPref.sliderAllianceExp.value = this.viewModel.ViewAlliance.Exp + 1;
            } else {
                this.viewPref.sliderAllianceExp.maxValue = 1;
                this.viewPref.sliderAllianceExp.value = 1;
            }

            AllianceLevelConf allianceConf = AllianceLevelConf.GetConf(allianceLevel.ToString());
            this.viewPref.txtDesc.text = this.viewModel.ViewAlliance.Description.CustomIsEmpty() ?
                LocalManager.GetValue(LocalHashConst.alliance_default_desc) :
                this.viewModel.ViewAlliance.Description;
            this.viewPref.txtInflunce.text =
                this.viewModel.ViewAlliance.Exp.ToString();
            this.viewPref.txtMembers.text =
                string.Concat(this.viewModel.ViewAlliance.MemberCount, "/", allianceConf.maxMember);
            this.viewPref.txtLanguage.text = LocalManager.GetValue(GameConst.ALLIANCE_LANGUAGE, AllianceLanguageConf.GetAllianceLanguage(this.viewModel.ViewAlliance.Language.ToString()));
            this.SetAllianceResourceBonus(allianceConf);
            this.SetBtnJoinStatus(this.viewModel.ViewAlliance);
        }

        private void SetAllianceResourceBonus(AllianceLevelConf allianceConf) {
            this.viewPref.txtLumber.text = string.Concat("+", (allianceConf.lumberbuff * 100), "%");
            this.viewPref.txtSteel.text = string.Concat("+", (allianceConf.steelbuff * 100), "%");
            this.viewPref.txtMarble.text = string.Concat("+", (allianceConf.marblebuff * 100), "%");
            this.viewPref.txtFood.text = string.Concat("+", (allianceConf.foodbuff * 100), "%");
        }

        private void UpdateAction() {
            if (!this.rejoinCoolingDone &&
                this.viewPref.btnJoin != null &&
                this.viewPref.btnJoin.gameObject.activeSelf) {
                this.viewPref.btnJoin.Grayable = true;
                this.viewPref.txtCoolDownTime.text = this.viewModel.GetRejoinCooldownInfo();
            }
        }

        private void SetBtnJoinStatus(Alliance alliance) {
            if (!this.viewPref.btnJoin.gameObject.activeSelf) return;
            this.isInAlliance = false;
            this.allianceJoinClose = false;
            this.canJoinDirectly = false;
            this.reachAllianceForceLimit = false;
            this.townhallLevelNotValide = false;
            this.allianceMemberFull = false;

            if (!this.viewModel.ReachTownhallRequireLevel()) {
                this.townhallLevelNotValide = true;
                this.viewPref.btnJoin.Grayable = true;
                return;
            }

            this.rejoinCoolingDone = this.viewModel.RejoinAllianceFinishAt < RoleManager.GetCurrentUtcTime();
            if (!this.rejoinCoolingDone) {
                UIManager.SetUICanvasGroupVisible(this.viewPref.coolDownCG, true);
                this.viewPref.txtCoolDownTime.text = this.viewModel.GetRejoinCooldownInfo();
                this.viewPref.btnJoin.Grayable = true;
                return;
            }

            string allianceId = RoleManager.GetAllianceId();
            if (!allianceId.CustomIsEmpty()) {
                this.isInAlliance = true;
                this.viewPref.btnJoin.Grayable = true;
                UIManager.SetUICanvasGroupVisible(this.viewPref.coolDownCG, true);
                this.viewPref.txtCoolDownTime.text =
                    LocalManager.GetValue(LocalHashConst.server_player_already_in_alliance);
                return;
            }

            int allianceLevel = AllianceLevelConf.GetAllianceLevelByExp(alliance.Exp);
            AllianceLevelConf allianceConf = AllianceLevelConf.GetConf(allianceLevel.ToString());
            if (alliance.MemberCount >= allianceConf.maxMember) {
                allianceMemberFull = true;
                this.viewPref.btnJoin.Grayable = true;
                return;
            }

            JoinConditionType allianceConditionType = (JoinConditionType)alliance.JoinCondition.Type;
            this.viewPref.btnJoin.Grayable = false;
            UIManager.SetUICanvasGroupVisible(this.viewPref.coolDownCG, false);
            switch (allianceConditionType) {
                case JoinConditionType.Close:
                    this.allianceJoinClose = true;
                    UIManager.SetUICanvasGroupVisible(this.viewPref.coolDownCG, true);
                    this.viewPref.txtCoolDownTime.text =
                        LocalManager.GetValue(LocalHashConst.alliance_refrused_join);
                    this.viewPref.btnJoin.Grayable = true;
                    break;
                case JoinConditionType.Limit:
                    bool reachForchLimit = RoleManager.GetForce() >= alliance.JoinCondition.ForceLimit;
                    this.viewPref.btnJoin.Grayable = !reachForchLimit;
                    UIManager.SetUICanvasGroupVisible(this.viewPref.coolDownCG, !reachForchLimit);
                    this.reachAllianceForceLimit = reachForchLimit;
                    this.canJoinDirectly = reachForchLimit;
                    if (!reachForchLimit) {
                        this.viewPref.txtCoolDownTime.text =
                            LocalManager.GetValue(LocalHashConst.alliance_force_not_reach);
                    }
                    break;
                case JoinConditionType.Apply:
                    break;
                case JoinConditionType.Free:
                    this.canJoinDirectly = true;
                    break;
                default:
                    Debug.LogError("Notice!!! could not come here");
                    break;
            }
        }

        private void OnBtnCloseClick() {
            this.viewModel.Hide();
        }

        private void OnBtnJoinClick() {
            this.viewModel.ApplyToJoinAlliance(this.allianceMemberFull,
                                                this.townhallLevelNotValide,
                                                this.isInAlliance,
                                                this.allianceJoinClose,
                                                this.rejoinCoolingDone,
                                                this.canJoinDirectly,
                                                this.reachAllianceForceLimit);
        }

        private void ShowMemberClick() {
            this.viewModel.ShowMembersList();
        }

        protected override void OnVisible() {
            UpdateManager.Regist(UpdateInfo.AllianceInfoView, this.UpdateAction);
        }

        protected override void OnInvisible() {
            UpdateManager.Unregist(UpdateInfo.AllianceInfoView);
        }
    }
}
