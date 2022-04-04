using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;


namespace Poukoute {
    public enum HeroStatus {
        OnTroop,
        OnHeal,
        OnFatigue,
        None
    }

    public class HeroHeadView: BaseItemViewsHolder, IPoolHandler {
        #region ui element
        [SerializeField]
        private CustomDrag customDrag;
        [SerializeField]
        private CustomButton btnHead;
        [SerializeField]
        private RectTransform pnlContentRT;
        [SerializeField]
        private CanvasGroup heroCG;
        [SerializeField]
        private CanvasGroup heroSliderCG;
        [SerializeField]
        private Slider sliFragment;
        [SerializeField]
        private Image imgMaxLevel;
        [SerializeField]
        private TextMeshProUGUI txtFragment;
        [SerializeField]
        private Slider sliTroop;
        [SerializeField]
        private TextMeshProUGUI txtTroop;
        [SerializeField]
        private Slider sliEnergy;
        [SerializeField]
        private TextMeshProUGUI txtEnergy;
        [SerializeField]
        private Image pnlTierUp;
        [SerializeField]
        private Image imgHeroAvatar;
        [SerializeField]
        private Transform pnlAttributes;
        [SerializeField]
        private CanvasGroup heroLevelCG;
        [SerializeField]
        private TextMeshProUGUI txtLevel;
        [SerializeField]
        private CanvasGroup heroStatusCG;
        [SerializeField]
        private TextMeshProUGUI txtHeroStatus;
        [SerializeField]
        private Transform pnlPower;
        [SerializeField]
        private TextMeshProUGUI txtPower;
        [SerializeField]
        private TextMeshProUGUI txtChest;
        [SerializeField]
        private Image imgRole;
        [SerializeField]
        private Transform pnlStars;
        [SerializeField]
        private GameObject[] heroRarity;
        [SerializeField]
        private CanvasGroup heroNewCG;
        [SerializeField]
        private Image fragmentFill;
        [SerializeField]
        public Animator animUpgrade;
        [SerializeField]
        public Animator animSliderFill;
        #endregion

        public float Height {
            get {
                if (this.ShowFragment) {
                    return 267;
                } else {
                    return 230;
                }
            }
        }

        private string heroName;
        private HeroAttributeConf heroConf;
        private bool showFragment;
        private bool showEnergy;
        private bool showTroop;
        private HeroStatus heroStatus = HeroStatus.None;
        public HeroStatus HeroStatus {
            get {
                return this.heroStatus;
            }
            set {
                // if (this.heroStatus != value) {
                this.heroStatus = value;
                this.OnHeroStatusChange();
                // }
            }
        }
        private bool isDragable = false;
        private Vector3 dragOrigin;

        [HideInInspector]
        public bool canLevelUp;
        public class OnDragEvent: UnityEvent<PointerEventData> { }

        public string HeroName {
            get {
                return this.heroName;
            }
        }

        private bool ShowFragment {
            get {
                return this.showFragment;
            }
            set {
                if (this.showFragment != value) {
                    this.showFragment = value;
                }
                this.sliFragment.gameObject.SetActiveSafe(showFragment);
            }
        }

        public float Percent {
            get {
                return this.sliFragment.value / this.sliFragment.maxValue;
            }
            private set {
                this.sliFragment.value = value * this.sliFragment.maxValue;
            }
        }

        public HeroAttributeConf HeroConf {
            get {
                return this.heroConf;
            }
        }

        private int level;
        public int Level {
            get {
                return this.level;
            }
        }

        public string FragmentSliderText {
            get {
                return this.txtFragment.text;
            }
            private set {
                this.txtFragment.text = value;
            }
        }

        public bool Visible {
            get {
                return this.heroCG.alpha > 0;
            }
            set {
                this.heroCG.alpha = value ? 1 : 0;
            }
        }

        public UnityEvent OnPress {
            get {
                this.btnHead.onPressing.RemoveAllListeners();
                return this.btnHead.onPressing;
            }
        }

        public UnityEvent OnHeroClick {
            get {
                this.btnHead.onClick.RemoveAllListeners();
                return this.btnHead.onClick;
            }
        }

        public CustomDrag.DragEvent OnHeroBeginDrag {
            get {
                this.customDrag.onBeginDrag.RemoveAllListeners();
                return this.customDrag.onBeginDrag;
            }
        }

        public CustomDrag.DragEvent OnHeroDrag {
            get {
                this.customDrag.onDrag.RemoveAllListeners();
                return this.customDrag.onDrag;
            }
        }

        public CustomDrag.DragEvent OnHeroEndDrag {
            get {
                this.customDrag.onEndDrag.RemoveAllListeners();
                return this.customDrag.onEndDrag;
            }
        }

        public bool IsEnable {
            set {
                this.imgHeroAvatar.material = value ? null : PoolManager.GetMaterial(MaterialPath.matGray);
                this.OnHeroEnableChange(value);
            }
        }

        public bool IsShowUnlockChest {
            set {
                this.txtChest.gameObject.SetActiveSafe(value);
            }
        }

        // Methods
        private void OnHeroEnableChange(bool isEnable) {
            string chestName = string.Empty;
            if (!isEnable) {
                int unlockMaxChestLevel = RoleManager.GetFDRecordMaxLevel();
                int level = GachaGroupConf.GetUnlockChestIndex(this.heroName);
                if (level > 0 && level > unlockMaxChestLevel) {
                    chestName = string.Concat(
                        LocalManager.GetValue(LocalHashConst.unlock_by),
                        LocalManager.GetValue("dragon_chest_", level.ToString()));
                }
            }
            bool heroNotUnlocked = !chestName.CustomIsEmpty();
            this.txtChest.gameObject.SetActiveSafe(heroNotUnlocked);
            this.txtPower.gameObject.SetActiveSafe(!heroNotUnlocked);
            if (heroNotUnlocked) {
                this.txtChest.text = chestName;
            }
        }

        public void AddListener(UnityAction call) {
            this.btnHead.onClick.RemoveAllListeners();
            this.btnHead.onClick.AddListener(call);
        }

        private void LateResize() {
            float scale = 1.0f;
            GridLayoutGroup grid =
                this.transform.parent.GetComponent<GridLayoutGroup>();
            if (grid == null) {
                RectTransform parentRect = this.transform.parent.GetComponent<RectTransform>();
                if (parentRect != null) {
                    root.offsetMin = Vector2.zero;
                    root.offsetMax = Vector2.zero;
                    root.anchorMin = Vector2.zero;
                    root.anchorMax = Vector2.one;
                    scale = parentRect.rect.width / this.pnlContentRT.rect.width;
                }
            } else {
                scale = grid.cellSize.x / this.pnlContentRT.rect.width;
            }

            scale = scale > 1.0f ? 1.0f : scale;
            this.pnlContentRT.localScale = Vector2.one * scale;
            this.pnlContentRT.anchoredPosition = Vector2.zero;
        }

        public void HideHero() {
            this.sliEnergy.gameObject.SetActiveSafe(false);
            this.sliFragment.gameObject.SetActiveSafe(false);
            this.sliTroop.gameObject.SetActiveSafe(false);
            this.imgHeroAvatar.gameObject.SetActiveSafe(false);
            this.pnlAttributes.gameObject.SetActiveSafe(false);
        }

        public void ShowHero() {
            this.sliEnergy.gameObject.SetActiveSafe(this.showEnergy);
            this.sliFragment.gameObject.SetActiveSafe(this.showFragment);
            this.sliTroop.gameObject.SetActiveSafe(this.showTroop);
            this.imgHeroAvatar.gameObject.SetActiveSafe(true);
            this.pnlAttributes.gameObject.SetActiveSafe(true);
        }

        public void SetHero(Battle.Hero battleHero, bool showFragment = false,
            bool showInfo = true, bool showLevel = true,
            bool showStar = true, bool showPower = true, bool showRole = true,
            bool isDragable = false, bool showArmyAmount = false, bool showHeroStatus = true) {
            //Debug.LogError("Set Battle Hero " + battleHero.Name + " " + battleHero.ArmyAmountBonus);
            this.SetHero(new Hero {
                Name = battleHero.Name,
                Level = battleHero.Level,
                ArmyAmount = battleHero.ArmyAmount,
                armyCoeff = GameHelper.NearlyEqual(battleHero.ArmyAmountBonus, 0d, 0.001d) ? 1 : battleHero.ArmyAmountBonus
            }, showFragment: showFragment, showInfo: showInfo, showArmyAmount: showArmyAmount,
            showLevel: showLevel, showStar: showStar,
            showPower: showPower, showRole: showRole, isDragable: isDragable);
        }

        // To do: Use bit to set show attribute or not.
        public void SetHero(Hero hero, bool showFragment = false, bool showArmyAmount = false,
            bool showEnergy = false, bool showInfo = true, bool showLevel = true,
            bool showStar = true, bool showPower = true,
            bool showRole = true, bool isDragable = false, bool isShowNewMark = false,
            bool showHeroStatus = true) {
            HeroAttributeConf heroAttributeConf = HeroAttributeConf.GetConf(hero.GetId());
            int maxArmyAmount = heroAttributeConf != null ?
                heroAttributeConf.GetAttribute(hero.Level, HeroAttribute.ArmyAmount, hero.armyCoeff) : 0;
            HeroStatus heroStatus = HeroStatus.None;
            int newEnergy = hero.GetNewEnergy();
            if (!showHeroStatus) {
                heroStatus = HeroStatus.None;
            } else if (hero.IsRecruiting) {
                heroStatus = HeroStatus.OnHeal;
            } else if (newEnergy < GameConst.HERO_ENERGY_COST) {
                heroStatus = HeroStatus.OnFatigue;
            } else if (hero.OnTroop) {
                heroStatus = HeroStatus.OnTroop;
            } else {
                heroStatus = HeroStatus.None;
            }
            this.SetHeroDetail(hero.Name, hero.Level, hero.FragmentCount,
                showFragment, maxArmyAmount, hero.ArmyAmount, showArmyAmount, newEnergy, showEnergy,
                showInfo, showLevel, showStar, showPower, showRole, isDragable,
                hero.IsNew, heroStatus);
            if (this.heroNewCG != null) {
                UIManager.SetUICanvasGroupVisible(this.heroNewCG, hero.IsNew && isShowNewMark);
            }
        }

        public void SetHero(HeroAttributeConf heroAttributeConf) {
            this.SetHeroDetail(heroAttributeConf.name, showLevel: false);
            if (this.heroNewCG != null) {
                UIManager.SetUICanvasGroupVisible(this.heroNewCG, false);
            }
        }

        public void SetUnlockHero(HeroAttributeConf heroAttributeConf) {
            this.SetHeroDetail(heroAttributeConf.name, showLevel: false);
            if (this.heroNewCG != null) {
                UIManager.SetUICanvasGroupVisible(this.heroNewCG, false);
            }
            this.IsEnable = false;
        }

        private void SetHeroDetail(string heroName, int level = 1, int fragmentCount = 0, bool showFragment = false,
            int troopMaxAmount = 0, int troopAmount = 0, bool showTroopAmount = false, int energy = 0, bool showEnergy = false,
            bool showInfo = true, bool showLevel = true, bool showStar = true,
            bool showPower = true, bool showRole = true, bool isDragable = false, bool isNew = false,
            HeroStatus heroStatus = HeroStatus.None) {
            this.heroName = heroName;
            //int sprite = Mathf.CeilToInt(1 / 5.0f);
            this.level = level;
            string spriteName = heroName.Replace(" ", string.Empty);
            this.imgHeroAvatar.sprite = ArtPrefabConf.GetSprite(spriteName + "_l");

            this.heroConf = HeroAttributeConf.GetConf(heroName);
            this.imgRole.sprite = ArtPrefabConf.GetSprite(heroConf.RoleIcon);
            this.txtLevel.text = string.Format(
                LocalManager.GetValue(LocalHashConst.hero_heroinfo_level), level);
            this.ShowFragment = showFragment;
            if (this.ShowFragment) {
                int heroFragments = HeroLevelConf.GetHeroUpgradFragments(heroName, level);
                bool reachMaxLevel = HeroLevelConf.GetHeroReachMaxLevel(heroName, level);
                this.imgMaxLevel.gameObject.SetActiveSafe(reachMaxLevel);
                this.fragmentFill.gameObject.SetActiveSafe(!reachMaxLevel);
                this.canLevelUp = (fragmentCount >= heroFragments) && !reachMaxLevel;
                this.animUpgrade.gameObject.SetActiveSafe(canLevelUp);
                this.animSliderFill.gameObject.SetActiveSafe(canLevelUp);
                this.pnlTierUp.gameObject.SetActiveSafe(
                    reachMaxLevel ? false : canLevelUp ? false : true);
                if (reachMaxLevel) {
                    this.FragmentSliderText = fragmentCount.ToString();
                    this.Percent = 1.0f;
                } else {
                    if (level == 1) {
                        this.FragmentSliderText =
                            string.Concat(fragmentCount + 1, "/", heroFragments + 1);
                        this.Percent = 1.0f * (fragmentCount + 1) / (heroFragments + 1);
                    } else {
                        this.FragmentSliderText =
                            string.Concat(fragmentCount, "/", heroFragments);
                        this.Percent = fragmentCount / (heroFragments * 1.0f);
                    }
                    if (canLevelUp) {
                        this.animSliderFill.SetBool("IsShow", true);
                        this.animUpgrade.SetBool("IsShow", true);
                    }
                    this.fragmentFill.sprite = ArtPrefabConf.GetSprite(SpritePath.resouceSliderPrefix,
                        this.canLevelUp ? "hightlightgreen" : "hightlightblue");
                }
            }
            this.sliEnergy.gameObject.SetActiveSafe(showEnergy);
            this.showEnergy = showEnergy;
            if (showEnergy) {
                this.sliEnergy.maxValue = GameConst.HERO_ENERGY_MAX;
                this.sliEnergy.value = energy;
                this.txtEnergy.text = string.Concat(energy, "/", GameConst.HERO_ENERGY_MAX);
            }

            this.sliTroop.gameObject.SetActiveSafe(showTroopAmount);
            this.showTroop = showTroopAmount;
            if (showTroopAmount) {
                this.sliTroop.maxValue = troopMaxAmount;
                this.sliTroop.value = troopAmount;
                this.txtTroop.text = string.Concat(troopAmount, "/", troopMaxAmount);
            }

            this.pnlAttributes.gameObject.SetActiveSafe(showInfo);
            if (showInfo) {
                this.pnlStars.gameObject.SetActiveSafe(showStar);
                UIManager.SetUICanvasGroupEnable(this.heroLevelCG, showLevel);
                this.pnlPower.gameObject.SetActiveSafe(showPower);
                if (showPower) {
                    this.txtPower.text =
                        string.Format("<size=12>{0}</size>{1}",
                        LocalManager.GetValue(LocalHashConst.hero_power),
                        this.heroConf.GetPower(level));
                }
                this.imgRole.gameObject.SetActiveSafe(showRole);
            }
            this.isDragable = isDragable;
            if (this.customDrag != null) {
                this.customDrag.enabled = this.isDragable;
            }
            this.IsEnable = true;
            //HeroStatus
            this.HeroStatus = heroStatus;
            this.SetHeroRarityInfo(heroName);
            this.LateResize();
        }

        private void SetHeroRarityInfo(string heroName) {
            HeroAttributeConf heroAttribute = HeroAttributeConf.GetConf(heroName);
            int i = 0;
            for (; i < heroAttribute.rarity; i++) {
                this.heroRarity[i].SetActiveSafe(true);
            }
            int rarityCount = this.heroRarity.Length;
            for (; i < rarityCount; i++) {
                this.heroRarity[i].SetActiveSafe(false);
            }
        }

        private void OnHeroStatusChange() {
            UIManager.SetUICanvasGroupVisible(this.heroStatusCG, (heroStatus != HeroStatus.None));
            switch (this.heroStatus) {
                case HeroStatus.OnHeal:
                    this.txtHeroStatus.text = string.Concat(
                        "<color=#61d7f8ff>",
                        LocalManager.GetValue(LocalHashConst.troop_recruit),
                        "</color>");
                    break;
                case HeroStatus.OnFatigue:
                    this.txtHeroStatus.text = string.Concat(
                        "<color=#ff7d7dff>",
                        LocalManager.GetValue(LocalHashConst.troop_fatigued),
                        "</color>");
                    break;
                case HeroStatus.OnTroop:
                    TroopModel troopModel = ModelManager.GetModelData<TroopModel>();
                    string troopName = troopModel.GetHeroTroopName(heroName);
                    if (troopName.CustomIsEmpty())
                        UIManager.SetUICanvasGroupVisible(this.heroStatusCG, false);
                    else
                        this.txtHeroStatus.text = string.Concat(
                             "<color=#ffffffff>",
                             string.Format(LocalManager.GetValue(LocalHashConst.troop_equipped),
                             GameHelper.GetBuildIndex(troopName)),
                             "</color>");
                    break;
                case HeroStatus.None:
                default:
                    break;
            }
        }


        public void OnInPool() {
            this.btnHead.onClick.RemoveAllListeners();
        }

        public void OnOutPool() {

        }
    }
}
