using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Protocol;
using UnityEngine.UI;
using TMPro;

namespace Poukoute {
    public class FteStarView : BaseView {
        public UnityAction eventForceFillAmount;

        void Awake() {
           
        }
        public void FlyStar(Transform pnlStar,Transform force) {
            AnimationManager.Animate(this.gameObject, "Show", finishCallback: () => {
                AnimationManager.Animate(this.gameObject, "Move",
                   start: pnlStar.position,
                   target: force.position, finishCallback: () => {
                       AudioManager.Play("show_fte_get_crown", 
                           AudioType.Show, AudioVolumn.High, isAdditive: true);
                       AnimationManager.Animate(this.gameObject,
                           "Hide", finishCallback: () => {
                               eventForceFillAmount.Invoke();
                               AnimationManager.Animate(force.gameObject, "Jump",
                                    finishCallback: () => {
                                        
                                    }
                                   );
                               Destroy(this.gameObject);
                           });
                   }, space: PositionSpace.World
                   );
            }
            );
        }
    }
}
