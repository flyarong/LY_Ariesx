using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Protocol;
using UnityEngine.UI;

namespace Poukoute {
    public class BossInfoViewPeference: BaseViewPreference {

        [Tooltip("UIBossInfo/PnlDetail/PnlTitle/BtnClose")]
        public CustomButton BtnBackground;

        [Tooltip("UIBossInfo/PnlDetail/PnlTitle/Text")]
        public TextMeshProUGUI txtTitle;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlTitle/BtnClose")]
        public Button btnClose;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlBoss/PnlContent/ImgAvatar")]
        public Image imgBossTypeIcon;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlBoss/PnlContent/TxtPower")]
        public TextMeshProUGUI txtPower;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlBoss/PnlContent/ImgRole")]
        public Image imgRole;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlBoss/PnlHeroDetail/PnlLevel/TxtLevel")]
        public TextMeshProUGUI txtLevel;

        [Tooltip("UIBossInfo/PnlDetail/PnlBoss/PnlHeroDetail/PnlLevel/TxtLevel/PnlStars")]
        public GameObject[] imgRarity;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlBoss/PnlHeroDetail/TxtScrollRect/TxtDescription")]
        public TextMeshProUGUI txtDescription;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlAttribute/PnlTroopHp/SliTroop")]
        public Slider sliHp;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlAttribute/PnlTroopHp/SliTroop/TxtAmount")]
        public TextMeshProUGUI txtHp;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlAttribute/PnlAttack/PnlNumber/TxtNumber")]
        public TextMeshProUGUI txtAttack;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlAttribute/PnlDefence/PnlNumber/TxtNumber")]
        public TextMeshProUGUI txtDefence;
        
       [Tooltip("UIBossInfo/PnlDetail/PnlSkilList")]
        public Transform pnlSkilList;

        public RectTransform rectSkillDetail;
        public Transform pnlSkillDetail;
        public TextMeshProUGUI txtSklillDescription;
        public CanvasGroup pnlSkillDetailCG;
    }
}
