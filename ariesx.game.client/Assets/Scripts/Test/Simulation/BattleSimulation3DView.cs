using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {

    public class BattleSimulation3DView : BaseView {
        private BattleSimulation3DViewModel viewModel;
        private BattleViewPreference viewPref;
        private Transform skillRoot;
        private Camera battleCamera;
        private BattleCamera cameraScript;

        private Dictionary<string, Transform> battleRootDict =
            new Dictionary<string, Transform>();

        private Dictionary<string, Dictionary<int, BattlePositionView>> battleCampDict =
            new Dictionary<string, Dictionary<int, BattlePositionView>>();

        private Dictionary<string, BattlePositionView> battlePositionDict =
            new Dictionary<string, BattlePositionView>();

        public List<string> needHoldPostionList = new List<string> { };
        private List<float> playSpeedList = new List<float>() { 0.5f, 1, 2, 4 };
        private GameObject pnlReward;
        int playSpeedIndex = 1;
        int playSpeedCount = 0;

        int curRoundIndex = 0;
        int curActionIndex = 0;

        private bool hasReward = false;

        private List<Transform> HPList = new List<Transform>();

        void Awake() {
            this.viewModel = this.GetComponent<BattleSimulation3DViewModel>();
            this.ui = GameObject.Find("UI/UIBattle");
            this.battleCamera = GameObject.FindWithTag("BattleCamera").GetComponent<Camera>();
            this.cameraScript = this.battleCamera.transform.Find("DragMask").GetComponent<BattleCamera>();
            this.cameraScript.onDrag = this.OnCameraMove;
            this.ui.transform.parent.GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
            //UIManager.AdaptiveParam;
            this.viewPref = this.ui.transform.GetComponent<BattleViewPreference>();

            this.InitBattleUI();
            this.InitPlayer();
            this.InitResult();
            this.InitBattle();

            this.playSpeedCount = playSpeedList.Count;
        }
        public void SetUICanvasGroupEnable(CanvasGroup canvasGroup, bool isEnable) {
            canvasGroup.alpha = isEnable ? 1 : 0;
            canvasGroup.interactable = isEnable ? true : false;
            canvasGroup.blocksRaycasts = isEnable ? true : false;
        }
        private void InitBattleUI() {
            this.viewPref.btnFteMask.gameObject.SetActive(false);
            this.viewPref.btnPlaySpeed.onClick.AddListener(this.OnBtnPlaySpeedClick);
            this.viewPref.btnStart.onClick.AddListener(this.OnBtnStartClick);
            this.viewPref.btnPause.onClick.AddListener(this.OnBtnPauseClick);
            this.viewPref.btnExit.onClick.AddListener(this.OnBtnCloseClick);

            UIManager.SetUICanvasGroupEnable(this.viewPref.cgEnd, false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgHead, false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgReward, false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgNoreward, false);
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgLost, false);

            this.viewPref.btnClose.onClick.AddListener(this.OnBtnCloseClick);
            this.viewPref.btnPlaySpeed.gameObject.SetActive(true);
            this.skillRoot = this.transform.Find("Skill");
            this.skillRoot.gameObject.SetActiveSafe(false);
        }

        private void InitPlayer() {
            this.SetPlayerInfo(this.viewModel.Attacker.BasicInfo, this.viewPref.pnlAttackerInfo);
            this.SetPlayerInfo(this.viewModel.Defender.BasicInfo, this.viewPref.pnlDefenderInfo);
        }

        private void InitResult() {
            // Win or lose
            this.SetBattleResultInfo();
            // Reward
            string chestName = this.viewModel.BattleResult.ChestName;
            bool hasChest = !chestName.CustomIsEmpty();
            Vector2 rewardRT = this.viewPref.rewardRT.sizeDelta;
            rewardRT.y = hasChest ? 265 : 175;
            this.viewPref.rewardRT.sizeDelta = rewardRT;
            this.viewPref.pnlChest.gameObject.SetActiveSafe(hasChest);
            if (hasChest) {
                this.hasReward = true;
                this.viewPref.txtChestName.text =
                    string.Concat(LocalManager.GetValue(chestName), "x1");
                this.viewPref.imgChest.sprite =
                    ArtPrefabConf.GetChestSprite(chestName);
            }
            this.SetBattleBonusInfo();
            this.viewPref.rewardRT.gameObject.SetActiveSafe(hasReward);
            if (hasReward) {
                this.pnlReward = this.viewPref.rewardRT.gameObject;
            }
            else if (this.viewModel.Win) {
                this.pnlReward = this.viewPref.rctNoRewards.gameObject;
            }
            else {
                this.viewPref.rewardRT.gameObject.SetActiveSafe(true);
            }
            this.viewPref.rctNoRewards.gameObject.SetActiveSafe(!this.hasReward && this.viewModel.Win);


            // Lost
            this.SetBattleLostInfo();
        }

        private void SetBattleLostInfo() {
            this.viewPref.txtOverview.text = this.viewModel.TotalLost.ToString();
            int heroesCount = this.viewModel.Attacker.AfterHeroes.Count;
            GameHelper.ResizeChildreCount(this.viewPref.pnlItems,
                heroesCount, PrefabPath.pnlBattleResultItem);
            this.viewPref.pnlOverview.gameObject.SetActive(true);
            this.viewPref.pnlDetail.gameObject.SetActive(false);
            this.viewPref.btnLostDetail.onClick.RemoveAllListeners();
            this.viewPref.btnLostDetail.onClick.AddListener(() => {
                this.viewPref.pnlOverview.gameObject.SetActive(false);
                this.viewPref.pnlDetail.gameObject.SetActive(true);
            });
            BattleResultItemView itemView = null;
            for (int index = 0; index < heroesCount; index++) {
                itemView = this.viewPref.pnlItems.GetChild(index).GetComponent<BattleResultItemView>();
                itemView.Hero = this.viewModel.Attacker.AfterHeroes[index];
            }
        }

        private void SetBattleResultInfo() {
            UIManager.SetUICanvasGroupVisible(this.viewPref.battleWinCG, this.viewModel.Win);
            UIManager.SetUICanvasGroupVisible(this.viewPref.battleLostCG, !this.viewModel.Win);
            string pointStatus = this.viewModel.PointOccupied ?
            LocalManager.GetValue(LocalHashConst.mail_battle_report_occupied) :
                (this.viewModel.PointLosed ?
                LocalManager.GetValue(LocalHashConst.mail_battle_report_territory) : string.Empty);
            if (!pointStatus.CustomIsEmpty()) {
                string resultFormat = string.Concat("<color=#",
                    this.viewModel.Win ? "F6FF63FF" : "59EDFFFF", ">{0}</color>");
                pointStatus = string.Format(resultFormat, pointStatus);
            }
            this.viewPref.txtBattleResult.text = pointStatus;
        }

        private void SetBattleBonusInfo() {
            //bool hasReward = false;
            Protocol.Resources resources = this.viewModel.BattleResult.Resources;
            Protocol.Currency currency = this.viewModel.BattleResult.Currency;
            GameHelper.ClearChildren(this.viewPref.pnlResources);
            this.InitResource(Resource.Lumber, resources.Lumber);
            this.InitResource(Resource.Steel, resources.Steel);
            this.InitResource(Resource.Marble, resources.Marble);
            this.InitResource(Resource.Food, resources.Food);
            this.InitResource(Resource.Gold, currency.Gold);
        }

        private bool InitResource(Resource type, int value) {
            //Debug.LogError(type + " " + value);
            if (value != 0) {
                this.hasReward = true;
                GameObject resourceObj = PoolManager.GetObject(
                    PrefabPath.pnlItemWithCount, this.viewPref.pnlResources);
                resourceObj.GetComponent<ItemWithCountView>().SetResourceInfo(type, value);
                return true;
            }
            else {
                return false;
            }
        }

        private void InitBattle() {
            List<int> positionList = new List<int> { 1, 2, 3, 4, 5, 6 };
            Transform format = this.transform.Find("Format");
            this.battleRootDict.Add(BattleConst.attack, format.Find("Attacker"));
            this.battleRootDict.Add(BattleConst.defense, format.Find("Defender"));

            this.battleCampDict[BattleConst.attack] =
                new Dictionary<int, BattlePositionView>();
            this.battleCampDict[BattleConst.defense] =
                new Dictionary<int, BattlePositionView>();

            foreach (Battle.Hero hero in this.viewModel.Attacker.BeforeHeroes) {
                if (hero.ArmyAmount > 0) {
                    this.SetPosition(BattleConst.attack, hero.Position, hero);
                    positionList.Remove(hero.Position);
                }
            }
            foreach (int position in positionList) {
                this.SetPosition(BattleConst.attack, position);
            }

            positionList = new List<int> { 1, 2, 3, 4, 5, 6 };
            foreach (Battle.Hero hero in this.viewModel.Defender.BeforeHeroes) {
                if (hero.ArmyAmount > 0) {
                    this.SetPosition(BattleConst.defense, hero.Position, hero);
                    positionList.Remove(hero.Position);
                }
            }
            foreach (int position in positionList) {
                this.SetPosition(BattleConst.defense, position);
            }
        }

        private IEnumerator PlayStart(int framCount) {
            yield return new WaitUntil(() => { return Time.frameCount - framCount > 2; });
            AnimationManager.Animate(this.viewPref.pnlAttackerInfo.gameObject, "Show", isOffset: false);
            AudioManager.Play(AudioPath.showPrefix + "battle_start", AudioType.Show, AudioVolumn.High);
            AnimationManager.Animate(this.viewPref.pnlDefenderInfo.gameObject, "Show",
                finishCallback: () => {
                    AnimationManager.Animate(this.viewPref.pnlAttackerInfo.gameObject, "Hide", isOffset: false);
                    AnimationManager.Animate(this.viewPref.pnlDefenderInfo.gameObject, "Hide", isOffset: false);
                    this.PlayStart(this.viewPref.pnlAttack, this.viewPref.rectAttack, 1);
                    this.PlayStart(this.viewPref.pnlDefense, this.viewPref.rectDefense, -1);
                }, isOffset: false
            );
            AnimationManager.Animate(this.viewPref.cameraRoot, "Start",
                finishCallback: () => {
                    try {
                        StartCoroutine(this.PlayAction(this.viewModel.BattleRounds, 0, 0));
                    }
                    catch (System.Exception e) {
                        Debug.LogWarningf("finishCallback action error: {0}", e);
                    }
                },
                frameCallback: () => {
                    this.SetHealthView();
                }, space: PositionSpace.SelfWorld
            );
        }

        private void PlayStart(Transform pnl, RectTransform rectTrans, int sign) {
            float height = rectTrans.rect.height;
            AnimationManager.Animate(pnl.gameObject, "Hide",
                sign * height / 2 * Vector2.up,
                -sign * (height / 2 - 100) * Vector2.up,
                finishCallback: () => {
                    if (pnl == null) {
                        return;
                    }
                }, isOffset: false
            );
        }

        private void PlayEnd() {
            string camp = BattleConst.defense;
            if (this.viewModel.Win) {
                camp = BattleConst.attack;
            }
            foreach (BattlePositionView positionView in this.battleCampDict[camp].Values) {
                positionView.Victor();
            }
            this.StartCoroutine(this.PlayEndDelay());

        }

        private IEnumerator PlayEndDelay() {
            yield return YieldManager.GetWaitForSeconds(1.5f);
            this.viewPref.cgEnd.gameObject.SetActive(true);
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgEnd, true);
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgHead, true);
            if (this.viewModel.Win) {
                AudioManager.Play(AudioPath.showPrefix + "battle_win",
                    AudioType.Show, AudioVolumn.High);
            }
            else {
                AudioManager.Play(AudioPath.showPrefix + "battle_lose",
                    AudioType.Show, AudioVolumn.High);
            }
            Vector2 headPosition = this.viewPref.cgHead.GetComponent<RectTransform>().anchoredPosition;
            AnimationManager.Animate(this.viewPref.cgHead.gameObject, "Show",
                headPosition - Vector2.up * 410,
                headPosition,
                finishCallback: () => {
                    if (this.viewModel.Win) {
                        UIManager.SetUICanvasGroupEnable(this.viewPref.cgReward, true);
                        if (this.viewPref.cgReward != null) {
                            Vector2 rewardPosition =
                                this.viewPref.cgReward.GetComponent<RectTransform>().anchoredPosition;
                            AnimationManager.Animate(this.pnlReward, "Show",
                                rewardPosition, rewardPosition + Vector2.up * 160,
                            isOffset: false);
                        }
                    }
                    if (this.viewPref.cgLost != null) {
                        this.viewPref.btnBackground.onClick.RemoveAllListeners();
                        this.viewPref.btnBackground.onClick.AddListener(this.OnBtnCloseClick);
                        UIManager.SetUICanvasGroupEnable(this.viewPref.cgLost, true);
                        Vector2 lostPosition =
                            this.viewPref.cgLost.GetComponent<RectTransform>().anchoredPosition;
                        AnimationManager.Animate(
                            this.viewPref.cgLost.gameObject, "Show",
                            start: lostPosition, target: lostPosition,
                            delay: 0.3f, isOffset: false, finishCallback: () => {
                                //FteManager.EndFte(true, false);
                            });
                    }
                }, isOffset: false);
        }

        // To do: alliance icon id.
        private void SetPlayerInfo(Battle.BasicInfo player, Transform pnlPlayerInfo) {
            //  pnlPlayerInfo.parent.GetComponent<Image>().enabled = true;
            TextMeshProUGUI txtAlliance = pnlPlayerInfo.Find("TxtAlliance").GetComponent<TextMeshProUGUI>();
            //bool isInAlliance = !player.AllianceName.CustomIsEmpty();
            txtAlliance.text = player.AllianceName.CustomIsEmpty() ?
                string.Empty : player.AllianceName;
            Image imgAvatar = pnlPlayerInfo.Find("PnlAvatar").GetComponent<Image>();
            TextMeshProUGUI txtName = pnlPlayerInfo.Find("TxtName").GetComponent<TextMeshProUGUI>();
            bool isMonster = player.Id.Contains("monster");
            bool isBoss = player.Id.Contains("boss");
            if (isMonster) {
                if (CampaignModel.monsterType.Equals(CampaignModel.nian)) {
                    imgAvatar.sprite = ArtPrefabConf.GetSprite("hero_407_s");
                }
                else {
                    imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(42);
                }
            }
            else if (isBoss) {
                imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(28);
            }
            else {
                imgAvatar.sprite = RoleManager.GetRoleAvatarByKey(player.Avatar);
            }

            if (isMonster) {
                int monsterLevel = int.Parse(player.Id.CustomSplit('+')[1]);
                txtName.text = string.Concat(
                    CampaignModel.MonsterLocalName, string.Format(
                        LocalManager.GetValue(LocalHashConst.melee_map_level),
                        monsterLevel
                    )
                );
            }
            else if (isBoss) {
                int bossLevel = int.Parse(player.Id.CustomSplit('+')[1]);
                txtName.text = string.Format(
                    LocalManager.GetValue(LocalHashConst.domination_detail_name), bossLevel);
            }
            else if (player.Name.CustomIsEmpty()) {
                txtName.text = this.viewModel.BattleResult.Report.PointInfo.GetBattleOccureTileName();
            }
            else {
                if (RoleManager.GetRoleName().CustomEquals(player.Name)) {
                    txtName.text = LocalManager.GetValue(LocalHashConst.mail_battle_report_me);
                }
                else {
                    txtName.text = player.Name;
                }
            }
        }

        private void SetPosition(string camp, int position, Battle.Hero hero = null) {
            GameObject troopObj;
            BattlePositionView positionView = null;
            int armyAmount = 0;
            string name = string.Empty;
            if (hero != null) {
                name = hero.Name;
                troopObj = PoolManager.GetObject(
                    BattleConst.positionPrefix + hero.Name,
                    this.battleRootDict[camp]
                );
                if (troopObj.GetComponent<BattlePositionView>() == null)
                    troopObj.AddComponent<BattlePositionView>();
                positionView = troopObj.GetComponent<BattlePositionView>();
                this.battlePositionDict.Add(hero.Id, positionView);
                armyAmount = hero.ArmyAmount;
            }
            else {
                troopObj = PoolManager.GetObject(
                    PrefabPath.battle_hero_empty,
                    this.battleRootDict[camp]
                );
                if (troopObj.GetComponent<BattlePositionView>() == null)
                    troopObj.AddComponent<BattlePositionView>();
                positionView = troopObj.GetComponent<BattlePositionView>();
            }

            this.battleCampDict[camp].Add(
                position, positionView
            );

            Transform hpTF = positionView.SetPosition(
                position, camp, name,
                armyAmount, this.skillRoot,
                this.battlePositionDict,
                this.battleCampDict,
                this.battleCamera
            );
            if (hpTF != null) {
                Image imgRole = hpTF.Find("HealthBg").Find("ImgRole").GetComponent<Image>();
                HeroAttributeConf heroConf = HeroAttributeConf.GetConf(hero.Name);
                imgRole.sprite = ArtPrefabConf.GetSprite(heroConf.RoleIcon);
                imgRole.gameObject.SetActive(true);
                this.HPList.Add(hpTF);
            }
        }

        public void SetHealthView() {
            foreach (BattlePositionView view in this.battlePositionDict.Values) {
                view.SetUIPosition();
            }
        }

        public void Play() {
            StartCoroutine(this.PlayStart(Time.frameCount));
        }

        private IEnumerator PlayAction(Battle.ReportRounds rounds, int roundIndex, int actionIndex) {
            if (this.gameObject != null && !this.viewModel.IsPause) {
                yield return YieldManager.GetWaitForSeconds(0.2f / this.viewModel.PlaySpeed);
                int roundsCount = rounds.Rounds.Count;
                if (roundIndex < rounds.Rounds.Count) {
                    Battle.Round round = rounds.Rounds[roundIndex];
                    if (round.Actions.Count > 0) {
                        Battle.Action action = round.Actions[actionIndex];
                        BattlePositionView positionView =
                            this.battlePositionDict[action.HeroId];
                        UnityAction actionEndCallback = null;
                        if (++actionIndex >= round.Actions.Count) {
                            if (++roundIndex < rounds.Rounds.Count) {
                                actionIndex = 0;
                                this.OnRoundEnd();
                                actionEndCallback = () => {
                                    if (this != null)
                                        StartCoroutine(this.PlayAction(rounds, roundIndex, actionIndex));
                                };
                            }
                            else {
                                actionEndCallback = this.PlayEnd;
                            }
                        }
                        else {
                            actionEndCallback = () => {
                                if (this != null)
                                    StartCoroutine(this.PlayAction(rounds, roundIndex, actionIndex));
                            };
                        }

                        positionView.ActMultiple(action, 0, (var) => {
                            actionEndCallback.InvokeSafe();
                        });
                    }
                    else {
                        if (this != null)
                            StartCoroutine(this.PlayAction(rounds, ++roundIndex, actionIndex));
                    }
                }
                else {
                    this.PlayEnd();
                }
            }
            else {
                this.curRoundIndex = roundIndex;
                this.curActionIndex = actionIndex;
            }
        }

        private void OnRoundEnd() {
            foreach (BattlePositionView positionView in this.battlePositionDict.Values) {
                positionView.OnRoundEnd();
            }
        }

        protected void OnBtnCloseClick() {
            this.viewModel.Close();
        }

        public void OnPauseStatusChange() {
            this.viewPref.btnStart.gameObject.SetActiveSafe(this.viewModel.IsPause);
            this.viewPref.btnPause.gameObject.SetActiveSafe(!this.viewModel.IsPause);
            if (!this.viewModel.IsPause) {
                StartCoroutine(
                    this.PlayAction(this.viewModel.BattleRounds,
                    this.curRoundIndex, this.curActionIndex));

            }
        }

        private void OnBtnPauseClick() {
            this.viewModel.IsPause = true;
        }

        private void OnBtnStartClick() {
            this.viewModel.IsPause = false;
        }

        private void OnBtnPlaySpeedClick() {
            playSpeedIndex += 1;
            if (playSpeedIndex >= playSpeedCount) {
                playSpeedIndex = 0;
            }
            this.viewModel.PlaySpeed = playSpeedList[playSpeedIndex];
        }

        public void OnSpeedChange() {
            if (this.viewModel.PlaySpeed < 1) {
                int denominator = Mathf.RoundToInt(1 / this.viewModel.PlaySpeed);
                this.viewPref.txtPlaySpeed.text = "x 1/" + denominator;
            }
            else {
                this.viewPref.txtPlaySpeed.text = string.Format("x {0}", this.viewModel.PlaySpeed);
            }
        }

        public void OnCameraMove() {
            this.SetHealthView();
        }

        private void OnEnable() {
            this.ShowButtons(true);
        }

        void OnDisable() {
            this.StopAllCoroutines();
            this.ShowButtons(false);
            this.ShowEnd(false);
            this.ClearGameObject();
            PoolManager.ClearPool(PoolType.Normal);
        }

        public void ShowButtons(bool b) {
            this.viewPref.pnlButtons.gameObject.SetActive(b);
        }

        public void ShowEnd(bool b) {
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgEnd, b);
            UIManager.SetUICanvasGroupEnable(this.viewPref.cgHead, b);
        }

        public void ClearGameObject() {
            Transform format = this.transform.Find("Format");
            GameHelper.ClearChildren(format.Find("Attacker"));
            GameHelper.ClearChildren(format.Find("Defender"));
            foreach (var item in this.HPList) {
                if (item != null)
                    GameObject.Destroy(item.gameObject);
            }
        }
    }
}
