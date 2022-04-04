using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Protocol;

namespace Poukoute {

    public class FteDemonAppear : MonoBehaviour {
        public Animator demon;
        public FteTrigger demonTrigger;
        public Animator hole;
        public FteTrigger holeTrigger;
        public Animator cloud;
        public FteTrigger cloudTrigger;
        public Animator dust;
        public FteTrigger dustTrigger;

        public UnityAction onEnd = null;

        void Awake() {
            this.cloudTrigger.onTriggerInvoke = this.StartHole;
            this.holeTrigger.onTriggerInvoke = this.StartDemon;
            this.demonTrigger.onTriggerInvoke = this.StartDust;
            this.dustTrigger.onTriggerInvoke = this.End;
        }

        public void StartShow() {
            this.StartCloud();
        }

        public void StartDemon() {
            this.demon.SetTrigger(GameConst.animStart);
        }

        public void StartHole() {
            this.hole.SetTrigger(GameConst.animStart);

        }

        public void StartCloud() {
            this.cloud.SetTrigger(GameConst.animStart);

        }

        public void StartDust() {
            this.dust.SetTrigger(GameConst.animStart);
        }

        public void StartMove() {
            this.demon.SetTrigger(GameConst.animMove);
        }

        public void End() {
            this.onEnd.InvokeSafe();
        }
    }
}
