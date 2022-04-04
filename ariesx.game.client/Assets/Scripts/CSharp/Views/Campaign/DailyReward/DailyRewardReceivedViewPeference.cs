using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Poukoute {
    public class DailyRewardReceivedViewPeference: BaseViewPreference {
        
       [Tooltip("UIToDeyOnlineRewardReceived/BtnBackground")]
        public CustomButton btnBackground;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/pnlTitleList/RawImage")]
        public Transform pnlReceiveRawImage;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/PnlRewardContent/pnlHero")]
        public Transform pnlReceivedHero;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/PnlRewardContent/pnlResouceList")]
        public Transform pnlReceivedResouceList;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/PnlRewardContent/pnlHero/ImgHero")]
        public Image imgReceivedHero;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/PnlRewardContent/pnlHero/ImgHero/pnlHeroNumber")]
        public TextMeshProUGUI txtReceivedHeroNumber;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/PnlRewardContent/pnlHero/HeroName")]
        public TextMeshProUGUI txtReceivedHeroName;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/pnlTitleList/TxtTitle")]
        public TextMeshProUGUI txtReceivedTitle;
        
       [Tooltip("UIToDeyOnlineRewardReceived/PnlCheckReward/PnlRewardContentReceived/PnlRewardContent/BtnReceived")]
        public CustomButton btnReceiveds;
        
        public Button btnHeroClick;

        public GameObject[] objReceivedRarity;
    }
}
