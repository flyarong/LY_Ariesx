using Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class NewHeroView : MonoBehaviour {
        public GameObject chestLight;
        public Transform Carmera;
        public GameObject yellowLight;
        public GameObject background;
        public TextMeshPro txtSkill;
        public GameObject pnlSkill;
        public TextMeshPro txtTitle;
        public GameObject title;
        public Transform pnlHero;
        public AnimationCurve showCanvas;
        public Camera chestCamera;

        public bool isShow = false;
        private GameObject hero;
        private string heroName;

        /*******************************************/
        void Start() {
            this.chestCamera = UIManager.ChestCamera.GetComponent<Camera>();
        }

        public void LoadHero(string heroName, UnityAction callBack, UnityAction loadBack) {
            this.heroName = heroName;
            this.hero = PoolManager.GetObject("Chest/Heroes/idle_" + heroName, this.pnlHero.transform);
            //Debug.Log(this.hero);
            if (this.hero == null) {
                this.hero = 
                    PoolManager.GetObject("Chest/Heroes/idle_hero_004", this.pnlHero.transform);
            }
            loadBack.Invoke();
            StartCoroutine(ShowHero(callBack));
        }

        public void StartShowHero(string name, string skill) {
            this.background.SetActiveSafe(true);
            this.chestLight.SetActiveSafe(true);
            this.yellowLight.SetActiveSafe(true);
            this.title.SetActiveSafe(true);
            this.txtTitle.text = name;
            this.pnlSkill.gameObject.SetActiveSafe(true);
            this.txtSkill.text = skill;
            chestCamera.cullingMask = LayerMask.GetMask("ChestHero");
            chestCamera.fieldOfView = 14.7f;
            Carmera.position = new Vector3(-30, 12, 0);
            Carmera.rotation = Quaternion.Euler(15, 90, 0);

        }


        private IEnumerator ShowHero(UnityAction callBack) {
            if (this.hero != null) {
                for (int i = 0; i <= 10; i++) {
                    // camera.fieldOfView = -(92.7f - 14.7f) * i * 2 / 19f + 92.7f;
                    chestCamera.fieldOfView = 92.7f - showCanvas.Evaluate(i * 0.01f) * ((92.7f - 14.7f));
                    yield return YieldManager.GetWaitForSeconds(0.01f);
                }
                AudioManager.PlayWithPath(
                    string.Format("Audio/Battle/{0}_show", this.heroName),
                    AudioType.Action, AudioVolumn.High
                );
            }
            yield return YieldManager.GetWaitForSeconds(0.2f);
            callBack.Invoke();
        }

        public void HideNewHero() {
            this.background.SetActiveSafe(false);
            this.chestLight.SetActiveSafe(false);
            this.yellowLight.SetActiveSafe(false);
            this.title.SetActiveSafe(false);
            this.pnlSkill.gameObject.SetActiveSafe(false);
            chestCamera.cullingMask = LayerMask.GetMask("Chest");
            chestCamera.fieldOfView = 13.44f * (1 / GameManager.MainCamera.aspect);
            Carmera.position = new Vector3(-30, 3.5f, 0);
            Carmera.rotation = Quaternion.Euler(0, 90, 0);
            Destroy(hero);
        }
    }
}