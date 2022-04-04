using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class OpenChestView : BaseView {
        private OpenChestViewModel viewModel;
        private OpenChestViewPreference viewPref;
        /* UI Members*/

        // chest
        private GameObject lottery;
        private GameObject newHero;
        private GameObject chestRoot;
        private Animator chestAnimator;
        private ChestView chestView;
        private GameObject chestEffect;
        private ChestEffectView chestEffectView;
        private GameObject chestCard;
        private ChestCardView chestCardView;

        private Transform pnlLeft;
        private Material pnlLeftMat;
        private Image imgPnlLeft;
        private TextMeshProUGUI txtLeft;

        private Transform pnlHero;
        private OpenChestHeroInfoViewPreference heroInfoViewPref;

        private bool isNewHero;
        private bool isInNewHero = false;
        private bool isChanging;
        private float add;
        private float current;
        private float target;
        private float origin;
        private float max;
        private int heroCount;
        private bool isReachMaxLevel = false;
        private bool showChestStart = false;
        private bool showChestMiddle = false;
        private bool showChestFinal = false;
        private bool needSkipAnimation = false;
        private UnityAction openCallback = null;
        private UnityAction hideCallback = null;
        private int sliderValue = 0;
        private int SliderAnimationCount = 0;
        private int addCount = 1;
        private Vector3 chestEffectPosition;
        private string chestName = string.Empty;

        private Material starMat;

        // Tmp
        public static GameObject tmpLottey;

        private bool IsChapterReward {
            get {
                return this.viewModel.CurrentGroupName.CustomEquals(GameConst.GIFT_GROUP_1);
            }
        }

        /*******************************************************************************/
        protected override void OnUIInit() {
            this.lottery = tmpLottey;
            this.newHero = GameObject.FindGameObjectWithTag("NewHero");
            this.chestRoot = this.lottery.transform.Find("Chest").gameObject;
            this.chestEffect = this.lottery.transform.Find("Effect").gameObject;
            this.chestEffectPosition = this.chestEffect.transform.position;
            this.chestEffectView = this.chestEffect.GetComponent<ChestEffectView>();
            this.chestCard = this.lottery.transform.Find("Card").gameObject;
            this.chestCardView = this.chestCard.GetComponent<ChestCardView>();
            this.chestCardView.onShowDetail = this.OnShowDetail;
            this.chestCardView.onShowName = this.OnShowName;
            this.chestCardView.onShowSlider = this.OnShowSlider;
            this.chestCardView.onShowStar = this.OnShowStar;
            this.chestCardView.onPlayMiddle = this.OnPlayMiddle;
            this.chestCardView.onPlayEnd = this.OnPlayEnd;

            this.pnlHero = UIManager.GetUI("PnlHero", UICanvas.Above).transform;
            this.heroInfoViewPref = this.pnlHero.
                GetComponent<OpenChestHeroInfoViewPreference>();

            // Chest count
            this.pnlLeft = UIManager.GetUI("PnlLeft", UICanvas.Above).transform;
            this.pnlLeftMat = Instantiate(
                pnlLeft.Find("ImgCardFlash").GetComponent<Image>().material
            );
            this.imgPnlLeft = pnlLeft.Find("ImgCardFlash").GetComponent<Image>();
            this.txtLeft = this.pnlLeft.Find("TxtNumber").GetComponent<TextMeshProUGUI>();

            this.ui = UIManager.GetUI("UIOpenChest");
            this.viewModel = this.gameObject.GetComponent<OpenChestViewModel>();
            this.group = UIGroup.Hero;
            this.viewPref = this.ui.transform.GetComponent<OpenChestViewPreference>();
        }

        private void UpdateAction() {
            if (this.isChanging) {
                this.current = Mathf.Lerp(this.current,
                    this.target, 3 * Time.unscaledDeltaTime);
                if ((this.target - this.current) < this.add / 20) {
                    this.current = this.target;
                    this.isChanging = false;
                    AudioManager.Stop(AudioType.Show);
                }
                if (this.isReachMaxLevel) {
                    this.heroInfoViewPref.sldTier.value = 1;
                    this.heroInfoViewPref.txtShowAmount.text =
                        Mathf.RoundToInt(this.current).ToString();
                } else {
                    float trueValue = (this.origin + this.add -
                        (this.target - this.current));
                    int tmpValue = Mathf.RoundToInt(trueValue);
                    if (tmpValue != this.sliderValue) {
                        if (this.SliderAnimationCount == 0) {
                            this.SliderAnimationCount = 10;
                            AnimationManager.Animate(
                                this.heroInfoViewPref.sldTier.gameObject,
                                "Beat", needRestart: true
                            );
                            StartCoroutine(Flash());
                        }
                    }
                    this.sliderValue = tmpValue;
                    //   Debug.LogError(sliderValue);
                    this.heroInfoViewPref.txtShowAmount.text =
                        sliderValue + "/" +
                        GameHelper.GetFormatNum((long)this.max, maxLength: 3);
                    this.heroInfoViewPref.sldTier.value = trueValue / this.max;
                }
                if (this.target >= this.max && !this.isReachMaxLevel) {
                    //this.heroInfoViewPref.imgTierUp.gameObject.SetActiveSafe(true);
                    this.heroInfoViewPref.txtTierUp.gameObject.SetActiveSafe(true);
                    this.heroInfoViewPref.imgSliderFill.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix, "hightlightgreen");
                    this.heroInfoViewPref.imgTierUp.sprite =
                    ArtPrefabConf.GetSprite("upgrade_arrow_green");
                } else {
                    this.heroInfoViewPref.txtTierUp.gameObject.SetActiveSafe(false);
                    this.heroInfoViewPref.imgSliderFill.sprite =
                   ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix, "hightlightblue");
                    this.heroInfoViewPref.imgTierUp.sprite =
                    ArtPrefabConf.GetSprite("upgrade_arrow_blue");
                }
                if (this.SliderAnimationCount != 0) {
                    this.SliderAnimationCount--;
                }
            }
        }

        private IEnumerator Flash() {
            Image image = this.heroInfoViewPref.ImgFlash.GetComponent<Image>();
            image.material.SetFloat("_Exposure", 0.8f);
            yield return YieldManager.GetWaitForSeconds(0.1f);
            image.material.SetFloat("_Exposure", 0);
            yield return YieldManager.GetWaitForSeconds(0.1f);
        }

        public void SetUIVisibleForChest(bool isVisible) {
            if (this.IsVisible) {
                this.ui.GetComponent<CanvasGroup>().alpha = isVisible ? 1 : 0;
            }
            UIManager.ChestCamera.SetActiveSafe(isVisible);
            UIManager.SetChestAboveUIVisible(isVisible);
        }

        public void ShowStart() {
            this.heroCount = this.viewModel.LotteryResultList.Count;
            UIManager.ChestCamera.SetActiveSafe(true);
            if (this.viewModel.CurrentResult != null) {
                this.viewModel.IsNewInPool =
                    !this.viewModel.HeroDict.ContainsKey(this.viewModel.CurrentResult.Name) &&
                    !HeroBaseConf.HeroList.Contains(this.viewModel.CurrentResult.Name);
            }
            this.heroInfoViewPref.txtNewPoolName.gameObject.SetActive(false);
            string chestModelName = this.viewModel.CurrentGroupName;
            if (this.viewModel.CurrentGroupName.CustomStartsWith(GameConst.GIFT_GROUP_PREFIX)) {
                chestModelName = GameConst.GIFT_GROUP_PREFIX;
            } else {
                int index = chestModelName.LastIndexOf("_");
                chestModelName = this.viewModel.CurrentGroupName.Substring(0, index);
            }
            this.chestName = GachaGroupConf.GetConf(this.viewModel.CurrentGroupName).chest;
            if (chestName.Contains(GameConst.GIFT_GROUP_PREFIX)) {
                chestName = "free_1";
            }
            string chestAnimatorPath = ArtPrefabConf.GetValue(chestModelName);
            GameObject chest3DModel = PoolManager.GetObject(chestAnimatorPath,
                this.chestRoot.transform);
            this.chestEffect.transform.parent = chest3DModel.transform.Find("Dummy001");
            this.chestEffect.transform.position = this.chestEffectPosition +
                new Vector3(0, 0.2f, 0);
            this.chestAnimator = chest3DModel.transform.GetComponent<Animator>();
            this.chestView = chest3DModel.transform.GetComponent<ChestView>();
            this.chestView.onOpenEnd.RemoveAllListeners();
            this.chestView.onOpenEnd.AddListener(this.OnChestOpenEnd);
            this.chestView.onStartEnd.RemoveAllListeners();
            this.chestView.onStartEnd.AddListener(this.OnChestStartEnd);
            this.chestView.onOpenPlay.RemoveAllListeners();
            this.chestView.onOpenPlay.AddListener(this.OnChestOpenPlay);
            this.chestAnimator.gameObject.SetActiveSafe(true);
            this.chestAnimator.SetTrigger(GameConst.animStart);
            AudioManager.Play("show_chest_open", AudioType.Show, AudioVolumn.High, isAdditive: true);
        }

        public void ShowNextHero() {
            this.chestAnimator.SetTrigger(GameConst.animRepeat);
            this.chestCardView.Rarity = this.viewModel.HeroConf.rarity;
            this.isNewHero =
                !this.viewModel.HeroDict.ContainsKey(this.viewModel.CurrentResult.Name);
            this.chestCardView.HeroName = this.viewModel.CurrentResult.Name;
        }

        private void ShowHero() {
            this.chestCardView.Play();
            UIManager.ShowUI(this.pnlLeft.gameObject);
            if (!this.isInNewHero) {
                this.pnlLeft.gameObject.SetActiveSafe(true);
            }
        }

        public void ShowHeroDetail(bool skipAnim) {
            int rarity = this.viewModel.HeroConf.rarity;
            AudioManager.Play(
                AudioPath.showPrefix + "lottery_rarity_" + rarity,
                AudioType.Show,
                AudioVolumn.High,
                isAdditive: true
            );
            if (this.isReachMaxLevel) {
                this.heroInfoViewPref.imgSliderFill.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.heroReachMaxLevel);
            } else {
                this.heroInfoViewPref.imgSliderFill.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix + "green");
            }

            UIManager.ShowUI(this.pnlHero.gameObject);
            if (!this.isInNewHero) {
                this.heroInfoViewPref.pnlHeroDetail.gameObject.SetActiveSafe(true);
            }
            this.heroInfoViewPref.PnlSeperate.SetActiveSafe(false);
            this.heroInfoViewPref.sldTier.gameObject.SetActiveSafe(false);
            this.heroInfoViewPref.txtAddAmount.gameObject.SetActiveSafe(false);
            this.heroInfoViewPref.txtAddAmount.text = "X" + this.add.ToString();
            for (int i = 1; i <= 5; i++) {
                GameObject star = this.heroInfoViewPref.pnlStars.GetChild(i - 1).gameObject;
                star.gameObject.SetActive(false);
            }

            AnimationManager.Animate(this.heroInfoViewPref.txtHeroLevel.gameObject, "Show", isOffset: false);



            if (skipAnim) {
                AnimationManager.Finish(this.heroInfoViewPref.txtHeroName.gameObject);
                AnimationManager.Finish(this.heroInfoViewPref.txtHeroLevel.gameObject);
                AnimationManager.Finish(this.heroInfoViewPref.sldTier.gameObject);
                AnimationManager.Finish(this.heroInfoViewPref.txtAddAmount.gameObject);
                AnimationManager.Finish(this.heroInfoViewPref.pnlStars.gameObject);
            }
        }

        /* Propert change function */
        public void OnChestOpenPlay() {
            this.chestEffectView.ShowNewHero();
            this.chestEffectView.ShowOpenLight(0f);
        }

        public void OnChestOpenEnd() {
            this.chestEffectView.ShowOpenLightGrade(this.viewModel.HeroConf.rarity);
            this.chestEffectView.ShowSmoke();
            this.ShowHero();
        }

        public void OnChestStartEnd() {
            this.showChestStart = true;
            this.showChestFinal = true;
            this.chestEffectView.ShowSlowHalo();
            this.InitUI();
            this.ShowHeroNormal();
        }

        private void ShowHeroNormal() {
            this.ShowChestNormal(true);
            this.viewPref.btnChest.onClick.RemoveAllListeners();
            this.viewPref.btnChest.onClick.AddListener(this.OnBtnChestClick);
        }


        public void OnShowDetail() {
            this.ShowHeroDetail(false);
        }

        public void OnShowSlider() {
            this.ShowSlider(false);
        }

        public void OnShowStar() {
            this.ShowStar(false);
        }

        public void OnShowName() {
            AnimationManager.Animate(this.heroInfoViewPref.PnlHeroName, "Show", isOffset: false, finishCallback: () => {
                AnimationManager.Animate(this.heroInfoViewPref.PnlHeroName, "Show1", isOffset: false
                    , finishCallback: () => {
                        this.openCallback.InvokeSafe();
                        this.openCallback = null;
                    }
                    );
            });
        }

        public void ShowSlider(bool skipAnim) {
            if (this.isNewHero) {
                this.heroInfoViewPref.PnlSeperate.SetActiveSafe(true);
                AnimationManager.Animate(this.heroInfoViewPref.PnlSeperate, "Fade");
                if (this.viewModel.IsNewInPool) {
                    this.heroInfoViewPref.txtNewPoolName.text = string.Format(
                        LocalManager.GetValue(LocalHashConst.hero_join_card_pool),
                        HeroAttributeConf.GetLocalName(this.viewModel.CurrentResult.Name)
                    );
                    this.heroInfoViewPref.txtNewPoolName.gameObject.SetActive(true);
                    AnimationManager.Animate(
                        this.heroInfoViewPref.txtNewPoolName.gameObject,
                        "Show",
                        isOffset: false
                    );
                }
            } else {
                this.heroInfoViewPref.sldTier.gameObject.SetActive(true);
                this.heroInfoViewPref.sldTier.value = this.current / this.max;
                this.heroInfoViewPref.txtShowAmount.text = Mathf.RoundToInt(this.current) + "/" + Mathf.RoundToInt(this.max);
                if (this.current > this.max) {
                    this.heroInfoViewPref.imgSliderFill.sprite =
                    ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix + "hightlightgreen");
                    this.heroInfoViewPref.imgTierUp.sprite =
                    ArtPrefabConf.GetSprite("upgrade_arrow_green");
                } else {
                    this.heroInfoViewPref.imgSliderFill.sprite =
                   ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix + "hightlightblue");
                    this.heroInfoViewPref.imgTierUp.sprite =
                    ArtPrefabConf.GetSprite("upgrade_arrow_blue");
                }
                AnimationManager.Animate(this.heroInfoViewPref.sldTier.gameObject, "Show", isOffset: false, finishCallback: () => {
                    AnimationManager.Animate(this.heroInfoViewPref.sldTier.gameObject, "Show1", isOffset: false, finishCallback: () => {
                        this.isChanging = true;
                    });
                });
                this.heroInfoViewPref.txtAddAmount.gameObject.SetActive(true);
                AnimationManager.Animate(this.heroInfoViewPref.txtAddAmount.gameObject, "Show", isOffset: false,
                    finishCallback: () => {
                        StartCoroutine(TxtAddAmountBeat());
                        if (skipAnim) {
                            this.current = this.target;
                        }
                        AudioManager.Play(AudioPath.showPrefix + "hero_amount_change",
                            AudioType.Show, AudioVolumn.High, loop: true, isAdditive: true);
                    }
                );
            }
        }

        private IEnumerator TxtAddAmountBeat() {
            float addAmount = this.add / 3 * this.addCount;
            if (Mathf.Abs(addAmount - this.add) < 1 || addCount == 3) {
                this.heroInfoViewPref.txtAddAmount.text = "X" + add.ToString();
                AnimationManager.Animate(this.heroInfoViewPref.txtAddAmount.gameObject, "Beat");
                this.addCount = 1;
                yield return YieldManager.GetWaitForSeconds(0.1f);
            } else {
                this.addCount++;
                this.heroInfoViewPref.txtAddAmount.text = "X" + Mathf.Ceil(addAmount).ToString();
                AnimationManager.Animate(this.heroInfoViewPref.txtAddAmount.gameObject, "Beat");
                yield return YieldManager.GetWaitForSeconds(0.1f);
                StartCoroutine(TxtAddAmountBeat());
            }
        }

        public void ShowStar(bool skipAnim) {
            int rarity = this.viewModel.HeroConf.rarity;
            this.heroInfoViewPref.pnlStars.GetComponent<CanvasGroup>().alpha = 1;
            for (int i = 1; i <= rarity; i++) {
                GameObject star = this.heroInfoViewPref.pnlStars.GetChild(i - 1).gameObject;
                star.gameObject.SetActive(true);
                if (!skipAnim) {
                    Color color = star.transform.GetChild(0).gameObject.GetComponent<Image>().color;
                    color.a = 0;
                    star.transform.GetChild(0).gameObject.GetComponent<Image>().color = color;
                    StartCoroutine(DelayStartStar(i, star.transform.GetChild(0).gameObject));
                }
            }
        }

        private IEnumerator DelayStartStar(int time, GameObject star) {
            yield return YieldManager.GetWaitForSeconds(0.15f * time);
            Image image = star.GetComponent<Image>();
            Color color = image.color;
            color.a = 255;
            image.color = color;
            Vector3 position = star.transform.GetComponent<RectTransform>().localPosition;
            AnimationManager.Animate(star, "Step0",
                start: position + new Vector3(0, 20, 0), target: position,
                finishCallback: () => {
                    AnimationManager.Animate(star, "Step1", space: PositionSpace.UI,
                        start: position, target: position + new Vector3(0, 3, 0),
                        finishCallback: () => {
                            AnimationManager.Animate(
                                star, "Step2",
                                start: position + new Vector3(0, 3, 0), target: position,
                                space: PositionSpace.UI
                            );
                            StartCoroutine(this.StarFlashStep2(image));
                        }
                    );
                    StartCoroutine(this.StarFlashStep1(image));
                },
                space: PositionSpace.UI
            );
        }

        private IEnumerator StarFlashStep1(Image image) {
            Material mat = Instantiate(image.material);
            mat.SetFloat("_Exposure", 1);
            image.material = mat;
            yield return YieldManager.GetWaitForSeconds(0.1f);
        }

        private IEnumerator StarFlashStep2(Image image) {
            yield return YieldManager.GetWaitForSeconds(0.1f);
            Material mat = Instantiate(image.material);
            mat.SetFloat("_Exposure", 0);
            image.material = mat;
        }

        public IEnumerator ShowHide() {
            this.heroInfoViewPref.PnlSeperate.gameObject.SetActive(false);
            AnimationManager.Animate(this.heroInfoViewPref.PnlHeroName, "Hide");
            StartCoroutine(SliderHide(this.heroInfoViewPref.sldTier.gameObject));
            StartCoroutine(StarHide(this.heroInfoViewPref.pnlStars.gameObject));
            yield return YieldManager.GetWaitForSeconds(0f);
            if (this.viewModel.LotteryResultList.Count > 0) {
                this.viewModel.CurrentResult = this.viewModel.LotteryResultList[0];
                this.ShowNextHero();
                this.viewModel.LotteryResultList.RemoveAt(0);
                this.viewModel.IsNewInPool = !this.viewModel.HeroDict.ContainsKey(this.viewModel.CurrentResult.Name) &&
                    !HeroBaseConf.HeroList.Contains(this.viewModel.CurrentResult.Name);
                this.heroInfoViewPref.txtNewPoolName.gameObject.SetActive(false);
            } else {
                this.viewPref.btnChest.onClick.RemoveAllListeners();
                NewHeroView newHeroView = this.newHero.GetComponent<NewHeroView>();
                newHeroView.HideNewHero();
                this.viewModel.Hide();
            }
        }

        public IEnumerator SliderHide(GameObject slider) {
            yield return YieldManager.GetWaitForSeconds(0f);
            Vector3 position = slider.GetComponent<RectTransform>().anchoredPosition;
            AnimationManager.Animate(slider, "Hide",
                start: position, target: position + new Vector3(0, 14, 0), space: PositionSpace.UI
                );
        }

        private IEnumerator StarHide(GameObject star) {
            yield return YieldManager.GetWaitForSeconds(0.1f);
            Vector3 position = star.GetComponent<RectTransform>().anchoredPosition;
            AnimationManager.Animate(star, "Hide",
                start: position, target: position + new Vector3(0, 15, 0), space: PositionSpace.UI
                , finishCallback: () => {
                    star.GetComponent<RectTransform>().anchoredPosition = position;
                }
                );
        }

        public void OnPlayEnd() {
            this.showChestFinal = true;
            this.chestCardView.PlayShowEffect();
            this.showChestMiddle = false;
        }

        public void OnPlayMiddle() {
            this.OnHeroChange();
            //this.chestCardView.onPlayMiddle.RemoveAllListeners();
            if (this.needSkipAnimation) {
                this.ShowChestImmediatly();
            } else {
                this.showChestMiddle = true;
                //this.chestCardView.ResetShow();外发光不需要
                UIManager.HideUI(this.pnlHero.gameObject);
                this.chestCardView.cardShow.gameObject.SetActiveSafe(false);
            }
        }

        //  public void OnPlayStart() {
        //      this.chestCardView.onPlayMiddle.AddListener(this.OnPlayMiddle);
        //  }

        private void OnHeroChange() {
            LotteryResult result = this.viewModel.CurrentResult;
            this.viewModel.IsNewInPool = !this.viewModel.HeroDict.ContainsKey(result.Name) &&
                !HeroBaseConf.HeroList.Contains(result.Name);
            HeroAttributeConf heroConf = this.viewModel.HeroConf;
            //int rarity = heroConf.rarity;
            Hero hero;
            this.add = result.FragmentCount;
            if (!this.viewModel.HeroDict.TryGetValue(result.Name, out hero)) {
                //this.isNewHero = true;
                hero = new Hero {
                    Name = result.Name,
                    Level = 1,
                    IsNew = true,
                    FragmentCount = -1,
                    ArmyAmount = heroConf.GetAttribute(1, HeroAttribute.ArmyAmount),
                    Energy = GameConst.HERO_ENERGY_MAX
                };
                hero.Skills.Clear();
                foreach (Protocol.Skill skill in result.Skills) {
                    hero.Skills.Add(skill);
                }
                this.heroInfoViewPref.txtSkillName.text = LocalManager.GetValue(
                    SkillConf.GetConf(this.viewModel.HeroConf.skills).name
                );
                this.viewModel.HeroDict.Add(result.Name, hero);
                this.viewModel.UnlockHeroDict.Remove(result.Name);
                this.viewModel.NewHeroCount++;
            }
            this.origin = hero.FragmentCount;
            this.isReachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(hero);
            this.heroInfoViewPref.pnlNewHeroTitle.gameObject.SetActiveSafe(this.isNewHero);
            this.viewModel.AddFramgent(result.Name, (int)this.add);
            this.max = HeroLevelConf.GetHeroUpgradFragments(hero);
            if (hero.Level == 1) {
                this.max += 1;
                this.origin += 1;
            }
            this.current = this.origin;
            this.target = this.origin + this.add;
            this.heroInfoViewPref.sldTier.value = 0;
            this.heroInfoViewPref.sldTier.maxValue = 1;
            if (this.current >= this.max && !this.isReachMaxLevel) {
                // this.heroInfoViewPref.imgTierUp.gameObject.SetActiveSafe(true);
                this.heroInfoViewPref.txtTierUp.gameObject.SetActiveSafe(true);

            } else {
                // this.heroInfoViewPref.imgTierUp.gameObject.SetActiveSafe(false);
                this.heroInfoViewPref.txtTierUp.gameObject.SetActiveSafe(false);
                this.heroInfoViewPref.txtShowAmount.gameObject.SetActiveSafe(true);
            }
            this.sliderValue = Mathf.RoundToInt(this.current);
            this.heroInfoViewPref.sldTier.value = this.sliderValue / this.max;
            this.heroInfoViewPref.txtHeroName.text = HeroAttributeConf.GetLocalName(hero.GetId());
            this.heroInfoViewPref.txtHeroGrade.text = HeroAttributeConf.GetConf(hero.GetId()).rarity.ToString();
            this.heroInfoViewPref.txtHeroLevel.text = string.Format(
                LocalManager.GetValue(LocalHashConst.level), hero.Level);
            //this.heroInfoViewPref.txtHeroLevel.color = ArtConst.heroRarityColor[rarity];
            this.SetLeftHeroCount();
        }
        /***************************/

        private void OnBtnChestClick() {
            //  to do : stop animation not hide this view\
            //Debug.Log("新英雄?"+isNewHero);
            if (isNewHero) {
                //this.heroInfoViewPref.pnlHeroDetail.gameObject.SetActiveSafe(false);
                NewHeroView newHeroView = this.newHero.GetComponent<NewHeroView>();
                if (!newHeroView.isShow) {
                    this.heroInfoViewPref.pnlHeroDetail.gameObject.SetActiveSafe(false);
                    this.pnlLeft.gameObject.SetActiveSafe(false);
                    this.isInNewHero = true;
                    this.viewPref.btnChest.onClick.RemoveAllListeners();
                    UnityAction callBack = () => {
                        this.heroInfoViewPref.txtSkill.text =
                        LocalManager.GetValue(this.viewModel.CurrentResult.Name, "_skill_1_intro");
                        this.heroInfoViewPref.txtSkill.gameObject.SetActiveSafe(false);
                        this.heroInfoViewPref.PnlSkill.SetActiveSafe(true);
                        StartCoroutine(OpenSkillBack());
                    };
                    this.pnlLeft.gameObject.SetActiveSafe(false);

                    // this.viewPref.imgBack.color = new Color(0, 0, 0, 1);
                    UnityAction loadBack = () => {
                        newHeroView.StartShowHero("",
                            HeroAttributeConf.GetLocalName(this.viewModel.CurrentResult.Name)
                        );
                    };
                    AudioManager.Play("show_new_hero", AudioType.Show, AudioVolumn.High, isAdditive: true);
                    newHeroView.LoadHero(this.viewModel.CurrentResult.Name, callBack, loadBack);

                    newHeroView.isShow = true;
                    return;
                } else {

                    this.heroInfoViewPref.PnlSkill.SetActiveSafe(false);
                    newHeroView.HideNewHero();

                    newHeroView.isShow = false;
                    this.hideCallback.InvokeSafe();
                    this.hideCallback = null;
                    this.isNewHero = false;
                    this.heroInfoViewPref.pnlHeroDetail.gameObject.SetActiveSafe(true);
                    bool leftVisible = this.heroCount > 0;
                    this.pnlLeft.gameObject.SetActiveSafe(leftVisible);
                    this.isInNewHero = false;
                    return;
                }
            }

            if (this.showChestMiddle) {
                this.ShowChestImmediatly();
            } else if (this.showChestFinal) {
                this.ShowChestNormal(false);
            } else if (this.showChestStart) {
                this.needSkipAnimation = true;
            }
        }

        private IEnumerator OpenSkillBack() {
            for (int i = 0; i <= 10; i++) {
                this.heroInfoViewPref.imgSkillLeft.sizeDelta =
                    new Vector2(heroInfoViewPref.skillBackCurve.
                    Evaluate(i * 0.02f) * (291.9f - 44) + 44, 183);
                this.heroInfoViewPref.imgSkillRight.sizeDelta =
                    new Vector2(heroInfoViewPref.skillBackCurve.
                    Evaluate(i * 0.02f) * (291.9f - 44) + 44, 183);
                yield return YieldManager.GetWaitForSeconds(0.005f);
            }
            yield return YieldManager.GetWaitForSeconds(0.05f);
            this.heroInfoViewPref.txtSkill.gameObject.SetActiveSafe(true);
            AnimationManager.Animate(this.heroInfoViewPref.txtSkill.gameObject, "Fade", finishCallback: () => {
                this.viewPref.btnChest.onClick.AddListener(this.OnBtnChestClick);
            });
        }

        private void SetLeftHeroCount() {
            bool leftVisible = --this.heroCount > 0;
            if (!this.isInNewHero) {
                this.pnlLeft.gameObject.SetActiveSafe(leftVisible);
            }
            if (leftVisible) {
                UIManager.ShowUI(this.pnlLeft.gameObject);
                AnimationManager.Animate(this.pnlLeft.gameObject, "Flash");
                StartCoroutine(this.LeftFlash());
                this.txtLeft.text = this.heroCount.ToString();
            }
        }

        private IEnumerator LeftFlash() {
            this.pnlLeftMat.SetFloat("_Exposure", 1);
            this.imgPnlLeft.material = this.pnlLeftMat;
            yield return YieldManager.GetWaitForSeconds(0.1336979f);
            this.pnlLeftMat.SetFloat("_Exposure", 0);
            this.imgPnlLeft.material = this.pnlLeftMat;
        }

        private void ShowChestImmediatly() {
            this.chestCardView.SetHeroInfo();
            this.ShowHeroDetail(true);
            this.OnShowName();
            this.ShowSlider(true);
            this.ShowStar(true);
            this.showChestMiddle = false;
            this.chestCardView.Show();
            this.needSkipAnimation = false;
        }

        private void ShowChestNormal(bool isFirst) {
            this.chestEffectView.StopOpenLight();
            this.chestEffectView.ShowFastHalo();
            this.viewModel.Next(isFirst);
            this.showChestFinal = false;
            this.needSkipAnimation = false;
        }

        //private void OnBtnHeroPoolClick() {
        //    if (!this.showChestFinal) {
        //        return;
        //    }
        //    this.viewModel.HideOpenChest();
        //    this.viewModel.ShowHeroPoolView();
        //}

        private void OnBtnStopOpenChestClick() {
            this.viewModel.HideOpenChest();
        }

        #region FTE

        public void SetHeroPoolAcvive() {
            //this.viewPref.pnlHeroPool.gameObject.SetActiveSafe(true);
            this.openCallback = null;
        }

        public void OnFteStepOpenStart(UnityAction callback, bool isForceFte) {
            InitUI();
            this.viewModel.Show();
            //this.viewPref.btnChest.onClick.AddListener(this.OnBtnChestClick);
            //this.chestView.onStartEnd.AddListener(this.ShowChestNormal);
            if (callback == null) {
                if (isForceFte) {
                    this.openCallback = () => {
                        FteManager.SetMask(this.viewPref.btnChest.transform, hasArrow: false);
                    };
                } else {
                    FteManager.StopFte();
                    this.openCallback = null;
                }
            } else {
                this.openCallback = callback;
            }
        }

        public void OnFteStep128Start() {
            FteManager.SetMask(this.viewPref.btnChest.transform, hasArrow: false);
        }

        #endregion

        protected override void OnVisible() {
            this.lottery.gameObject.SetActiveSafe(true);
            UpdateManager.Regist(UpdateInfo.OpenChestView, this.UpdateAction);
            this.showChestFinal = false;
            this.chestCardView.Hide();
            UIManager.ShowFakeBack(true);
            UIManager.GetUI("UIBackground").GetComponent<Canvas>().sortingOrder =
                this.canvas.sortingOrder - 1;
            AudioManager.PlayBg(AudioScene.Lottery);
            AudioManager.Stop(AudioType.Enviroment);
        }

        protected override void OnInvisible() {
            this.viewModel.isHideViewForBattle = false;
            this.lottery.gameObject.SetActiveSafe(false);
            UpdateManager.Unregist(UpdateInfo.OpenChestView);
            UIManager.HideUI(this.pnlHero.gameObject);
            UIManager.HideUI(this.pnlLeft.gameObject);
            this.pnlLeft.gameObject.SetActiveSafe(false);
            UIManager.ChestCamera.SetActiveSafe(false);
            this.chestEffectView.Reset();
            this.chestEffect.transform.parent = this.lottery.transform;
            GameHelper.ClearChildren(this.chestRoot.transform);
            UIManager.ShowFakeBack(false);
            UIManager.GetUI("UIBackground").GetComponent<Canvas>().sortingOrder = 1;
            AudioManager.StopBg();
        }
    }
}
