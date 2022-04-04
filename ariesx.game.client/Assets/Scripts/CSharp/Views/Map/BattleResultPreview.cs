using Poukoute;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BattleResultPreview : MonoBehaviour {
    public Transform pnlBattleResult;
    public GameObject chestObject;
    public ChestSetting ChestItem;
    //public GameObject collectMailObject;
    //public Transform pnlResources;
    public Transform pnlLumber;
    public Transform pnlSteel;
    public Transform pnlMarble;
    public Transform pnlFood;
    public Transform pnlGold;
    public Transform pnlForce;
    public Transform pnlPlaceHolder;
    public TextMeshProUGUI txtLumberNumer;
    public TextMeshProUGUI txtSteelNumer;
    public TextMeshProUGUI txtMarbleNumer;
    public TextMeshProUGUI txtFoodNumer;
    public TextMeshProUGUI txtGoldNumer;
    public TextMeshProUGUI txtChestNumer;

    public Transform pnlLottery;
    //public CustomButton btnHero;
    public CustomButton btnMail; //BtnMail.

    private TroopArrivedNtf troopArriveNtf;
    //private UnityAction AfterBtnHeroPalyCallback;
    private UnityAction AfterShowCallback;
    private string chestName;
    private bool hasChest;
    private bool hasResourceRewared;
    private bool isInStep61 = false;

    private Transform getRewaredPos;
    private bool isWithin = false;

    /**************************************************/
    private void Awake() {
        FteManager.SetStartCallback(GameConst.NORMAL, 59, this.OnFteStep59Start);
        FteManager.SetEndCallback(GameConst.NORMAL, 61, this.OnFteStep61End);
        this.getRewaredPos = new GameObject("GetRwardPos").transform;
    }

    public void CollectLottery(TroopArrivedNtf message,
                               Dictionary<Vector2, GameObject> tileDict,
                               //UnityAction btnHeroCallback,
                               UnityAction afterShowCallback) {
        this.troopArriveNtf = message;

        IsRangeOfVisibility(message, tileDict);
        this.AfterShowCallback = afterShowCallback;
        if (troopArriveNtf.Reward.Chests.Count > 0) {
            this.chestName = troopArriveNtf.Reward.Chests[0].Name;
            //this.AfterBtnHeroPalyCallback = btnHeroCallback;
        } else {
            this.chestName = string.Empty;
        }
        this.hasChest = !chestName.CustomIsEmpty() && troopArriveNtf.IsWin;
        this.chestObject.gameObject.SetActiveSafe(hasChest);
        if (hasChest) {
            ChestItem.SetChestContent(this.chestName);
            this.txtChestNumer.text =
                "x" + GameHelper.GetFormatNum(this.troopArriveNtf.Reward.Chests[0].Count);
        }
        this.hasResourceRewared = this.HasAnyResourceRerward();
        this.HasAnyResource();
        if (this.hasResourceRewared) {
            this.SetResourcesNumer();
            TriggerManager.Invoke(Trigger.ResourceChange);
        }

        //this.pnlResources.gameObject.SetActiveSafe(hasResourceRewared);
        if (!isInStep61) {
            if (this.isWithin) {
                StartCoroutine(this.PlayGetReward());
            } else {
                AnimationManager.Animate(this.pnlBattleResult.gameObject, "Show", () => {
                    StartCoroutine(this.HideBattleResultPreView());
                });
            }
        }
    }

    private void IsRangeOfVisibility(TroopArrivedNtf message, Dictionary<Vector2, GameObject> tileDict) {
        Vector2 center = MapUtils.PositionToCoordinate(GameManager.MainCamera.transform.position);
        Coord collectCoord = message.PointInfo.Coord;
        float offsetX = center.x - collectCoord.X;
        float offsetY = center.y - collectCoord.Y;
        if (UnityEngine.Mathf.Abs(offsetX) <= 3 && UnityEngine.Mathf.Abs(offsetY) <= 1) {
            this.getRewaredPos.position = tileDict[message.PointInfo.Coord].transform.position -
                GameManager.MainCamera.transform.position;
            this.isWithin = true;
        } else {
            this.getRewaredPos.position = this.pnlBattleResult.transform.position;
            this.isWithin = false;
        }
    }

    private IEnumerator PlayGetReward() {
        yield return YieldManager.GetWaitForSeconds(0.3f);
        this.GetBattleReward(this.isWithin);
        if (this.hasChest && !this.isInStep61) {
            this.PlayGetChest();
        } else {
            this.AfterShowCallback.InvokeSafe();
        }
    }

    private IEnumerator HideBattleResultPreView() {
        yield return YieldManager.GetWaitForSeconds(0.3f);
        AnimationManager.Animate(this.pnlBattleResult.gameObject, "Hide", null);
        this.GetBattleReward(this.isWithin);
        if (this.hasChest && !this.isInStep61) {
            this.PlayGetChest();
        } else {
            this.AfterShowCallback.InvokeSafe();
        }
    }

    private void SetResourcesNumer() {
        this.txtLumberNumer.text =
            GameHelper.GetFormatNum(this.troopArriveNtf.Reward.Resources.Lumber);
        this.txtSteelNumer.text =
            GameHelper.GetFormatNum(this.troopArriveNtf.Reward.Resources.Steel);
        this.txtMarbleNumer.text =
            GameHelper.GetFormatNum(this.troopArriveNtf.Reward.Resources.Marble);
        this.txtFoodNumer.text =
            GameHelper.GetFormatNum(this.troopArriveNtf.Reward.Resources.Food);
        this.txtGoldNumer.text =
            GameHelper.GetFormatNum(this.troopArriveNtf.Reward.Currency.Gold);
    }

    private List<int> DropOutRandomList = null;
    private void PlayGetChest() {
        GachaGroupConf chestName = GachaGroupConf.GetConf(this.chestName);
        if (isWithin) {
            GameHelper.ChestCollect(this.chestObject.transform.position,
         chestName,
         CollectChestType.collectWithDropOut,
         this.AfterShowCallback);
        } else {
            StartCoroutine(CollectActionResource());
        }
    }

    private IEnumerator CollectActionResource() {
        yield return new WaitForSeconds(0.5f);
        GameHelper.ChestCollect(this.chestObject.transform.position,
            GachaGroupConf.GetConf(this.chestName),
            CollectChestType.normalCollect,
            this.AfterShowCallback);
    }

    private void HasAnyResource() {
        pnlForce.gameObject.SetActiveSafe(this.isWithin);
        pnlLumber.gameObject.SetActiveSafe(this.troopArriveNtf.Reward.Resources.Lumber > 0);
        pnlSteel.gameObject.SetActiveSafe(this.troopArriveNtf.Reward.Resources.Steel > 0);
        pnlMarble.gameObject.SetActiveSafe(this.troopArriveNtf.Reward.Resources.Marble > 0);
        pnlFood.gameObject.SetActiveSafe(this.troopArriveNtf.Reward.Resources.Food > 0);
        pnlGold.gameObject.SetActiveSafe(this.troopArriveNtf.Reward.Currency.Gold > 0);
        pnlForce.gameObject.SetActiveSafe(this.troopArriveNtf.Reward.Resources.Lumber > 0);
    }


    private bool HasAnyResourceRerward() {
        return this.troopArriveNtf.Reward.Resources.Lumber > 0 ||
            this.troopArriveNtf.Reward.Resources.Steel > 0 ||
            this.troopArriveNtf.Reward.Resources.Food > 0 ||
            this.troopArriveNtf.Reward.Resources.Marble > 0 ||
            this.troopArriveNtf.Reward.Currency.Gem > 0 ||
            this.troopArriveNtf.Reward.Currency.Gold > 0;
    }

    private void GetBattleReward(bool isWithin) {
        if (hasResourceRewared) {
            if (!isInStep61) {
                Dictionary<Resource, Transform> resourceDict =
                    new Dictionary<Resource, Transform>(5);
                if (isWithin) {
                    resourceDict[Resource.Lumber] = this.getRewaredPos;
                    resourceDict[Resource.Steel] = this.getRewaredPos;
                    resourceDict[Resource.Marble] = this.getRewaredPos;
                    resourceDict[Resource.Food] = this.getRewaredPos;
                    resourceDict[Resource.Gold] = this.getRewaredPos;
                    GameHelper.ForceCollect(this.getRewaredPos.position, true);
                } else {
                    resourceDict[Resource.Lumber] = this.pnlLumber;
                    resourceDict[Resource.Steel] = this.pnlSteel;
                    resourceDict[Resource.Marble] = this.pnlMarble;
                    resourceDict[Resource.Food] = this.pnlFood;
                    resourceDict[Resource.Gold] = this.pnlGold;
                    GameHelper.ForceCollect(this.pnlForce.position, false);
                }

                Protocol.Resources addResources = this.troopArriveNtf.Reward.Resources;
                Protocol.Currency addCurrency = this.troopArriveNtf.Reward.Currency;
                Protocol.Resources resources = this.troopArriveNtf.Resources;
                Protocol.Currency currency = this.troopArriveNtf.Currency;
                //RoleManager.Instance.NeedResourceAnimation = true;
                //RoleManager.Instance.NeedCurrencyAnimation = true;
                GameHelper.CollectResources(addResources, addCurrency,
                        resources, currency, resourceDict, isWithin);
            }
        }
    }
    #region Fte
    private void OnFteStep59Start(string index) {
        this.isInStep61 = true;
    }

    private void OnFteStep61End() {
        this.isInStep61 = false;
    }


    #endregion
}
