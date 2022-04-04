using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poukoute {
    public class ChestEffectView : MonoBehaviour {
        public ParticleSystem slowRotate;
        public ParticleSystem fastRotate;
        public ParticleSystem openLight;
        public ParticleSystem openLightLow;
        public ParticleSystem openLightHigh;
        public ParticleSystem smoke;
        public ParticleSystem newHero;
        Vector3 OpenLightPosition;
        private void Awake() {
            this.OpenLightPosition = this.transform.position;
        }

        public void ShowSlowHalo() {
            return;
            this.fastRotate.Stop();
            this.fastRotate.gameObject.SetActiveSafe(false);
            this.slowRotate.gameObject.SetActiveSafe(true);
            this.slowRotate.Play();
        }

        public void ShowFastHalo() {
            return;
            this.slowRotate.Stop();
            this.slowRotate.gameObject.SetActiveSafe(false);
            this.fastRotate.gameObject.SetActiveSafe(true);
            this.fastRotate.Play();
        }

        public void ShowOpenLight(float offset) {
            this.StopOpenLight();
            //this.transform.position = this.OpenLightPosition ; 
            this.openLight.gameObject.SetActiveSafe(true);
            this.openLight.Play();
        }

        public void ShowOpenLightGrade(int rarity) {
            if (rarity > 3) {
                this.openLightHigh.gameObject.SetActiveSafe(true);
                this.openLightHigh.Play();
            } else if (rarity > 1) {
                this.openLightLow.gameObject.SetActiveSafe(true);
                this.openLightLow.Play();
            } 
        }

        public void ShowSmoke() {
            this.smoke.gameObject.SetActiveSafe(true);
            this.smoke.Play();
        }

        public void ShowNewHero() {
            return;
            this.StopOpenLight();
            this.newHero.gameObject.SetActiveSafe(true);
            this.newHero.Play();
            this.openLight.gameObject.SetActiveSafe(true);
            this.openLight.Play();
        }

        public void Reset() {
            //this.slowRotate.Stop();
            //this.slowRotate.gameObject.SetActiveSafe(false);
            //this.fastRotate.Stop();
            //this.fastRotate.gameObject.SetActiveSafe(false);
            //this.transform.position = this.OpenLightPosition;
            this.openLight.Stop();
            this.openLight.gameObject.SetActiveSafe(false);
            this.openLightLow.Stop();
            this.openLightLow.gameObject.SetActiveSafe(false);
            this.openLightLow.Stop();
            this.openLightLow.gameObject.SetActiveSafe(false);
            this.smoke.Stop();
            this.smoke.gameObject.SetActiveSafe(false);
            
            //this.newHero.Stop();
            //this.newHero.gameObject.SetActiveSafe(false);
        }


        public void StopOpenLight() {
            this.openLight.Stop();
            this.openLight.gameObject.SetActiveSafe(false);
            //this.newHero.Stop();
            //this.newHero.gameObject.SetActiveSafe(false);
        }
    }
}
