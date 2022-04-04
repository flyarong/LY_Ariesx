using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class ChestCardView : BaseView {
        public Transform cardShow;
        public SpriteRenderer showHeadView;
        // private SpriteRenderer showRace;
        private SpriteRenderer showRole;
        public SkinnedMeshRenderer cardShowRender;
        public Transform Card;
        public ParticleSystem uieffect04;
        public ParticleSystem uieffect03;
        public ParticleSystem uieffect05;
        public ParticleSystem uieffect01;
        public ParticleSystem uieffect02;
        public GameObject uieffect06;

        public Transform cardPlay;
        private ChestCardReceiver receiver;
        public SpriteRenderer playHeadView;
        // private SpriteRenderer playRace;
        private SpriteRenderer playRole;
        private GameObject haloMask;
        private Animator animator;
        public SkinnedMeshRenderer cardPlayRender;
        public SpriteRenderer headViewBaffle;
        [SerializeField]
        public List<ParticleSystem> showRarityList = new List<ParticleSystem>(5);
        [SerializeField]
        public List<ParticleSystem> moveRarityList = new List<ParticleSystem>(5);
        private ParticleSystem curShowEffect = null;
        private ParticleSystem curMoveEffect = null;
        [HideInInspector]
        public UnityAction onPlayEnd;
        [HideInInspector]
        public UnityAction onShowDetail;
        [HideInInspector]
        public UnityAction onShowName;
        [HideInInspector]
        public UnityAction onShowSlider;
        [HideInInspector]
        public UnityAction onShowStar;
        [HideInInspector]
        public UnityAction onPlayMiddle;
        [HideInInspector]
        public UnityAction onPlayStart;

        private bool isShowuieffect = false;
        private Vector3 CardVector = new Vector3(0, -1.22f, 0.97f);
        private int rarity = -1;
        public int Rarity {
            set {
                if (this.rarity != value) {
                    this.rarity = value;

                }

            }
        }

        private string heroName = string.Empty;
        public string HeroName {
            set {
                if (this.heroName != value) {
                    this.InitUI();
                    this.heroName = value;
                    if (this.cardPlay != null) {
                        HeroAttributeConf heroConf = HeroAttributeConf.GetConf(this.heroName);
                        string spriteName = heroName.Replace(" ", string.Empty);
                        this.playHeadView.sprite =
                            ArtPrefabConf.GetSprite(spriteName + "_l");
                        // this.playRace.sprite = ArtPrefabConf.GetSprite(heroConf.RaceIcon);
                        this.playRole.sprite = ArtPrefabConf.GetSprite(heroConf.RoleIcon);
                    }
                    if (this.rarity > 1 && this.rarity < this.showRarityList.Count) {
                        //Debug.Log(rarity);
                        this.curShowEffect = this.showRarityList[this.rarity - 2];
                        if (this.cardPlay != null) {
                            this.curMoveEffect = this.moveRarityList[this.rarity - 2];
                        }
                    } else {
                        this.curShowEffect = null;
                        if (this.cardPlay != null) {
                            this.curMoveEffect = null;
                        }
                    }
                }
            }
        }

        protected override void OnUIInit() {
            this.ui = this.gameObject;
            this.cardShow = this.transform.Find("CardShow");
            this.showRole = this.showHeadView.
                transform.Find("Role").GetComponent<SpriteRenderer>();
            Transform card01 = this.cardShow.Find("card01");
            if (card01 != null) {
                this.cardShowRender = card01.GetComponent<SkinnedMeshRenderer>();
            }

            this.cardPlay = this.transform.Find("CardPlay");
            if (this.cardPlay != null) {
                this.receiver = this.cardPlay.GetComponent<ChestCardReceiver>();
                this.receiver.onPlayEnd = this.OnPlayEnd;
                this.receiver.onHaloFade = this.OnHaloFade;
                this.receiver.onShowDetail = this.OnShowDetail;
                this.receiver.onShowName = this.OnShowName;
                this.receiver.onShowSlider = this.OnShowSlider;
                this.receiver.onShowStar = this.OnShowStar;
                this.receiver.onPlayMiddle = this.OnPlayMiddle;
                this.receiver.onPlayStart = this.OnPlayStart;
                //this.playRace = this.playHeadView.
                //    transform.Find("Race").GetComponent<SpriteRenderer>();
                this.playRole = this.playHeadView.
                    transform.Find("Role").GetComponent<SpriteRenderer>();
                this.haloMask = playHeadView.transform.Find("Halo").gameObject;
                this.animator = this.cardPlay.GetComponent<Animator>();
                this.cardPlayRender = this.cardPlay.Find("card01").
                    GetComponent<SkinnedMeshRenderer>();
            }
        }

        public void Reset() {
            this.ResetShow();
            this.ResetMove();
        }

        public void ResetShow() {
            foreach (ParticleSystem paritcle in this.showRarityList) {
                paritcle.Stop();
                paritcle.gameObject.SetActive(false);
            }

        }

        public void ResetMove() {
            foreach (ParticleSystem paritcle in this.moveRarityList) {
                paritcle.Stop();
                paritcle.gameObject.SetActive(false);
            }
        }

        public void PlayShowEffect() {
            if (!IsUIInit) {
                this.InitUI();
            }
            if (this.Card != null) {
                if (isShowuieffect) {
                    this.uieffect06.SetActiveSafe(false);
                }
                this.CardShow();
            } else {
                this.cardShowRender.sharedMaterial =
                        PoolManager.GetMaterial(MaterialPath.matRarityList[this.rarity - 1]);
                if (this.rarity > 1) {
                    this.ResetShow();

                    this.curShowEffect.gameObject.SetActiveSafe(true);
                    this.curShowEffect.Stop();
                    this.curShowEffect.Play();
                }
            }
        }

        public void PlayMoveEffect() {
            this.cardPlayRender.sharedMaterial =
                    PoolManager.GetMaterial(MaterialPath.matRarityList[this.rarity - 1]);
            if (this.rarity > 1) {
                this.ResetMove();

                this.curMoveEffect.gameObject.SetActiveSafe(true);
                this.curMoveEffect.Stop();
                this.curMoveEffect.Play();
            }
        }

        public void Play() {
            this.cardPlay.gameObject.SetActiveSafe(true);
            this.PlayMoveEffect();
            AudioManager.Play("show_card_fly", AudioType.Show, AudioVolumn.High);
            this.animator.SetTrigger(GameConst.animPlay);
        }

        public void Show() {
            this.OnPlayEnd();
        }

        public void Hide() {
            this.cardShow.gameObject.SetActiveSafe(false);
            this.cardPlay.gameObject.SetActiveSafe(false);
        }

        public void OnPlayEnd() {
            this.cardShow.gameObject.SetActiveSafe(true);
            this.cardPlay.gameObject.SetActiveSafe(false);
            this.onPlayEnd.InvokeSafe();
        }

        public void OnPlayMiddle() {
            this.onPlayMiddle.InvokeSafe();
        }

        public void OnShowDetail() {
            this.onShowDetail.InvokeSafe();
        }

        public void OnShowName() {
            this.onShowName.InvokeSafe();
        }

        public void OnShowSlider() {
            this.onShowSlider.InvokeSafe();
        }

        public void OnShowStar() {
            this.onShowStar.InvokeSafe();
        }

        public void OnHaloFade() {
            this.onPlayStart.InvokeSafe();
            AnimationManager.Animate(this.haloMask, "Hide", space: PositionSpace.World);
            this.SetHeroInfo();
        }

        public void OnPlayStart() {
            Color color = this.haloMask.GetComponent<SpriteRenderer>().color;
            this.haloMask.GetComponent<SpriteRenderer>().color =
                new Color(color.r, color.g, color.b, 1);
        }

        public void SetHeroInfo() {
            HeroAttributeConf heroConf = HeroAttributeConf.GetConf(this.heroName);
            string spriteName = heroName.Replace(" ", string.Empty);
            
            if (ArtPrefabConf.GetSprite(spriteName + "_l")!=null) {
                this.showHeadView.sprite =
               ArtPrefabConf.GetSprite(spriteName + "_l");
            }
            //this.showHeadView.sprite =
             // ArtPrefabConf.GetSprite(spriteName + "_l");

            //  this.showRace.sprite = ArtPrefabConf.GetSprite(heroConf.RaceIcon);
            this.showRole.sprite = ArtPrefabConf.GetSprite(heroConf.RoleIcon);
        }

        private void CardShow() {
            isShowuieffect = true;
            AnimationManager.Animate(this.showHeadView.gameObject, "Show");
            AnimationManager.Animate(this.showRole.gameObject, "Show");
            AnimationManager.Animate(this.Card.gameObject, "Show", start: CardVector,
            space: PositionSpace.World, finishCallback: () => {
                this.uieffect01.Play();
                this.uieffect05.Play();
                AnimationManager.Animate(this.headViewBaffle.gameObject, "Show");
                AnimationManager.Animate(this.Card.gameObject, "Scale", finishCallback: () => {
                    this.uieffect02.gameObject.SetActiveSafe(true);
                    this.uieffect02.Play();
                    this.uieffect06.SetActiveSafe(true);
                    this.uieffect03.Play();
                    this.uieffect04.Play();
                    AnimationManager.Animate(this.Card.gameObject, "LoopScale");
                }, space: PositionSpace.SelfWorld);
            });
        }

        public void SetUieffectStop() {
            this.uieffect01.Stop();
            this.uieffect02.Stop(); this.uieffect03.Stop();
            this.uieffect04.Stop(); this.uieffect05.Stop();
            this.uieffect06.gameObject.SetActiveSafe(false);
            this.uieffect02.gameObject.SetActiveSafe(false);
            AnimationManager.Animate(this.Card.gameObject, "Hide", space: PositionSpace.World);
            AnimationManager.Animate(this.showHeadView.gameObject, "Hide");
            AnimationManager.Animate(this.showRole.gameObject, "Hide");
            AnimationManager.Stop(this.Card.gameObject);
        }
    }
}
