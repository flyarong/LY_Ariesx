using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Poukoute {
    public class CountryViewPreference : MonoBehaviour {
        [Tooltip("ContryMapCamera")]
        public Camera countryCamera;
        [Tooltip("PnlCountryIntro")]
        public GameObject pnlCountryInfo;
        [Tooltip("PnlCountryIntro.TxtDescription")]
        public TextMeshProUGUI txtCountryDesc;
        [Tooltip("PnlCountryIntro.BtnChoose")]
        public CustomButton btnChoose;

        [Tooltip("PnlComic.Image")]
        public GameObject pnlComic;
        public GameObject pnlComicText;

        public OnCountryClick[] countryClick;
    }
}
