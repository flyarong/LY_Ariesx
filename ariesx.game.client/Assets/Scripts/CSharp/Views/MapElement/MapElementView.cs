using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Poukoute {
    public class MapElementView : MonoBehaviour, IPoolHandler {
        private Animator anim;
        private SpriteRenderer spriteRenderer;
        public UnityAction<GameObject> endAction;

        public static Dictionary<string, int> directionScale = new Dictionary<string, int> {
            {"WE", -1 },
            {"EW", 1 },
            {"SN", -1 },
            {"NS", 1 },
            {"SENW", 1 },
            {"SWNE", -1 },
            {"NESW", 1 },
            {"NWSE", -1 }
        };

        private int idleCount = 0;
        public int idleMinCount = 3;
        public int idleMaxCount = 5;

        private int moveCount = 0;
        public int moveMinCount = 4;
        public int moveMaxCount = 6;

        public int maxTime = 15;

        Vector2 target = Vector2.zero;

        void Awake() {
            this.anim = this.GetComponent<Animator>();
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        public void Idle() {
            if ((Random.Range(0, 2) == 1 && idleCount > idleMinCount) ||
                idleCount >= idleMaxCount) {
                AnimationManager.Animate(this.gameObject, "Show", 
                    this.transform.position, this.target, this.End, space: PositionSpace.World);
                this.anim.SetTrigger("Move");
                idleCount = 0;
            } else {
                idleCount++;
            }
        }

        public void Move() {
            if ((Random.Range(0, 2) == 1 && moveCount > moveMinCount) ||
                moveCount >= moveMaxCount) {
                AnimationManager.Stop(this.gameObject);
                this.anim.SetTrigger("Idle");
                moveCount = 0;
            } else {
                moveCount++;
            }
        }

        public void Begin(string direction, Vector3 start, Vector3 end) {
            this.target = end;
            this.anim.SetTrigger(direction);
            this.transform.position = start;
            this.transform.localScale = new Vector3(directionScale[direction], 1, 1);
            AnimationManager.Animate(this.gameObject, "Show", start, end, this.End, space: PositionSpace.World);
            base.StartCoroutine(this.MaxEnd());
        }

        private void End() {
            AnimationManager.Animate(this.gameObject, "Hide", () => {
                this.endAction.InvokeSafe(this.gameObject);
            });
        }

        private IEnumerator MaxEnd() {
            yield return YieldManager.GetWaitForSeconds(this.maxTime);
            if (this.gameObject != null) {
                AnimationManager.Stop(this.gameObject);
                AnimationManager.Animate(this.gameObject, "Hide", () => {
                    this.endAction.InvokeSafe(this.gameObject);
                });
            }
        }

        public void OnInPool() {
            this.anim.enabled = false;
        }

        public void OnOutPool() {
            this.anim.enabled = true;
            this.spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
}
