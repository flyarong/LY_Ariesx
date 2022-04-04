using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class PayPduItemViewPriference : BaseViewPreference {
        
        [Tooltip("Pnl2lPduItem")]
        public CustomButton btnPay;
        
        [Tooltip("Pnl2lPduItem")]
        public GameObject pnlBtnPay;
        
       [Tooltip("Pnl2lPduItem/PnlAmount")]
        public GameObject pnlAmount;
        
       [Tooltip("Pnl2lPduItem/PnlAmount/TxtAmount")]
        public TextMeshProUGUI txtAmount;
        
       [Tooltip("Pnl2lPduItem/PnlGoldAmount")]
        public GameObject pnlGoldAmount;
        
       [Tooltip("Pnl2lPduItem/PnlGoldAmount/TxtAmount")]
        public TextMeshProUGUI txtGoldAmount;
        
       [Tooltip("Pnl2lPduItem/PnlGemAmount")]
        public GameObject pnlGemAmount;
        
       [Tooltip("Pnl2lPduItem/PnlGemAmount/TxtAmount")]
        public TextMeshProUGUI txtGemAmount;
        
       [Tooltip("Pnl2lPduItem/PnlDiamond")]
        public GameObject pnlDiamond;
        
       [Tooltip("Pnl2lPduItem/PnlDiamond/ImgPdu")]
        public Image imgDimPdu;
        
       [Tooltip("Pnl2lPduItem/PnlDiamond/TxtNum")]
        public TextMeshProUGUI txtDimPduNum;
        
       [Tooltip("Pnl2lPduItem/PnlFirstTopup")]
        public GameObject pnlFirstTopup;
        
       [Tooltip("Pnl2lPduItem/PnlFirstTopup/ImgChest")]
        public Image imgChest;
        
       [Tooltip("Pnl2lPduItem/PnlFirstTopup/ImgChest/ImgWight/TxtNum")]
        public TextMeshProUGUI txtChestNum;
        
       [Tooltip("Pnl2lPduItem/PnlFirstTopup/ImgPduBG/ImgPdu")]
        public Image imgFTPdu;
        
       [Tooltip("Pnl2lPduItem/PnlFirstTopup/ImgPduBG/ImgPdu/TxtNum")]
        public TextMeshProUGUI txtFBPduNum;
        
       [Tooltip("Pnl2lPduItem/PnlSellOut")]
        public GameObject pnlSellOut;
        
       [Tooltip("Pnl2lPduItem/PnlDiamond/TxtNum")]
        public GameObject imgNum;
        
       [Tooltip("Pnl2lPduItem/TxtDis")]
        public TextMeshProUGUI txtDis;
        
       [Tooltip("Pnl2lPduItem/PnlHero")]
        public Transform pnlHero;
        
       [Tooltip("Pnl2lPduItem/PnlHero/ImgHero")]
        public Image imgHero;
        
       [Tooltip("Pnl2lPduItem/PnlHero/TxtNum")]
        public TextMeshProUGUI txtHeroNum;
        
       [Tooltip("Pnl2lPduItem/PnlHero/PnlStars")]
        public Transform pnlStars;
        
       [Tooltip("Pnl2lPduItem/PnlHero/SliFragment")]
        public Slider sliFragment;
        
       [Tooltip("Pnl2lPduItem/PnlHero/SliFragment/TxtAmount")]
        public TextMeshProUGUI txtFragment;
        
       [Tooltip("Pnl2lPduItem/PnlHero/SliFragment/ImgMaxLevel")]
        public Image imgMaxLevel;
        
       [Tooltip("Pnl2lPduItem/PnlHero/SliFragment/ImgUpgrade")]
        public Image pnlTierUp;
        
        public Image[] heroRarity;
        
       [Tooltip("Pnl2lPduItem/PnlSellOut")]
        public CustomButton btnSellOut;
        
       [Tooltip("Pnl2lPduItem/PnlSellOut/ImgSellOut/TextMeshPro Text")]
        public TextMeshProUGUI txtSellOut;
    }
}
