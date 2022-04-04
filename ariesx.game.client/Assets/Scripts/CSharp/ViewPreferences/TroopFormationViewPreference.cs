using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System;
using TMPro;

namespace Poukoute {
    public class TroopFormationViewPreference : BaseViewPreference {
        [Tooltip("UITroopFormation.PnlFormation")]
        public Transform pnlFormation;
        [Tooltip("UITroopFormation.BtnBackground CustomButton")]
        public CustomButton btnBackground;
        [Tooltip("UITroopFormation.PnlFormation.PnlTitle.BtnClose CustomButton")]
        public CustomButton btnClose;

        [Tooltip("UITroopFormation.PnlFormation.PnlTitle.Text")]
        public TextMeshProUGUI txtTitle;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlTroopGrid")]
        public Transform pnlTroopGrid;
        public Transform[] heroTransform = new Transform[6];
        public GameObject[] backImage = new GameObject[6];
        public GameObject[] backText = new GameObject[6];
        public CustomClick[] backCustomClick = new CustomClick[6];
        public Transform[] pnlLockedDetail = new Transform[6];
        public TextMeshProUGUI[] txtLockeDetail = new TextMeshProUGUI[6];

        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlTroopGrid.PnlAttributes")]
        public TextMeshProUGUI[] txtTroopAttributes;

        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlTroopGrid.PnlHighlight")]
        public Transform pnlHighlight;
        public Transform[] highlightTransform = new Transform[6];
        
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.ScrollView CustomScrollRect")]
        public CustomScrollRect scrollRect;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.ScrollView CanvasGroup")]
        public CanvasGroup scrollViewCanvasGroup;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.ScrollView.PnlContent GridLayoutGroup")]
        public GridLayoutGroup gridLayoutGroup;
        [Tooltip("UITroopFormation.PnlFormation.HeroGridHight")]
        public CustomDrop dropHeroGridHight;
        [Tooltip("UITroopFormation.PnlFormation.HeroGridHight")]
        public Button btnHeroGridHight;
        [Tooltip("UITroopFormation.PnlFormation.BtnCloseSelect")]
        public Button btnCloseSelect;

        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlSelect")]
        public Transform pnlSelect;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlSelect")]
        public CanvasGroup selectCanvasGroup;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlSelect RectTransform")]
        public RectTransform selectRectTransform;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlSelect.PnlAvatar")]
        public Transform pnlSelectHeroAvatar;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlSelect.PnlAvatar.PnlHeroBig HeroHeadView")]
        public HeroHeadView selectHeroHead;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlSelect.PnlButtons.BtnInfo")]
        public CustomButton btnInfo;

        public Transform pnlMove;
        public CanvasGroup CGMove;
        public RectTransform RTMove;
        public HeroHeadView HHVMove;


        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlOrder.BtnOrder")]
        public Button btnOrder;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlOrder.BtnOrder.PnlContent.Text")]
        public TextMeshProUGUI txtOrderLabel;
        [Tooltip("UITroopFormation.PnlFormation.PnlContent.PnlOrder.BtnRecruit")]
        public CustomButton btnRecruit;
        [Tooltip("UITroopFormation.PnlFormation.PnlTroopGrid.PnlDragHint")]
        public RectTransform dragHint;
    }
}
