using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using TMPro;

namespace Poukoute {
    public enum LineDimensionType {
        ThreeD,
        TwoD,
        OneD,
        None
    }

    public class MarchLineView : MonoBehaviour, IPoolHandler,
        IPointerDownHandler, IPointerClickHandler, IPointerUpHandler {
        // Data
        //private TroopModel model;
        private HeroModel heroModel;

        private Vector2 directionNormal;
        private float direction;
        private float angle;
        private float size;
        private string id;
        private EventMarchClient eventMarch;

        //private GameObject mapUI;
        private GameObject pnlNewMarch;
        private Transform hero;
        // UI
        [Tooltip("Match.March")]
        public Transform march;
        [Tooltip("Match.March.Role")]
        public Transform role;
        [Tooltip("Match.March.Collider")]
        public Transform clickCollider;
        [Tooltip("Match.Line")]
        public Transform line;
        [Tooltip("Match.Line.Opaque")]
        public LineRenderer opaqueLineRenderer;
        [Tooltip("Match.Line.Transparent")]
        public LineRenderer transparentLineRenderer;
        [Tooltip("Match.Flag")]
        public Transform flag;
        [Tooltip("2D Sprite")]
        public SpriteRenderer twoDSprite;
        [Tooltip("Flag Animator")]
        public Animator flagAnimator;
        [Tooltip("MyArmyId")]
        public TextMeshPro MyArmyId;
        [Tooltip("PnlMyArmy")]
        public Transform PnlMyArmy;

        public class MarchEvent : UnityEvent<string> { }
        public MarchEvent onClick = new MarchEvent();
        public const float ratio = 1;

        private const string hero3DPathPrefix = "Map/Heroes/move_";
        private const string hero2DPathPrefix = "Map/Sprites/Heroes/avatar_{0}_s";

        public bool ShowMarch {
            set {
                this.march.gameObject.SetActiveSafe(value);
            }
        }

        private Dictionary<string, Hero> HeroDict {
            get {
                return this.heroModel.heroDict;
            }
        }

        public enum LineRelation {
            Self,
            Enemy
        }

        void Awake() {
            this.heroModel = ModelManager.GetModelData<HeroModel>();
            UIManager.GetUI("UIMap");
        }

        public void SetLine(EventMarchClient eventMarch,
            UnityAction<string> callback, Vector3 offset, LineDimensionType lineDimension) {
            this.eventMarch = eventMarch;

            this.ShowMarch = true;
            this.id = eventMarch.id;

            LineRelation lineRelationType = LineRelation.Enemy;

            if (eventMarch.playeId.CustomEquals(RoleManager.GetRoleId()) ||
                !eventMarch.allianceId.CustomIsEmpty() &&
                    (eventMarch.allianceId.CustomEquals(RoleManager.GetAllianceId()) ||
                    eventMarch.allianceId.CustomEquals(RoleManager.GetMasterAllianceId()))) {
                //eventMarch.
                lineRelationType = LineRelation.Self;
            }
            this.PnlMyArmy.gameObject.SetActiveSafe(
                eventMarch.playeId == RoleManager.GetRoleId()
            );
            if (eventMarch.playeId == RoleManager.GetRoleId()) {
                this.MyArmyId.text = LocalManager.GetValue(LocalHashConst.mine) + TroopModel.GetTroopName(eventMarch.troop.ArmyCamp);
            }
            //if (!eventMarch.playeId.CustomEquals(RoleManager.GetRoleId()) &&
            //    (string.IsNullOrEmpty(eventMarch.allianceId) ||
            //    !eventMarch.allianceId.CustomEquals(RoleManager.GetAllianceId()))) {
            //    lineRelationType = LineRelation.Enemy;
            //}
            this.SetLine(eventMarch.origin, eventMarch.target, offset, lineRelationType, false);
            this.march.position = MapUtils.CoordinateToPosition(eventMarch.origin);
            this.march.localPosition -= Vector3.forward;
            this.SetIcon(eventMarch.troop, lineDimension);
            this.onClick.RemoveAllListeners();
            this.onClick.AddListener(callback);
            this.Refresh(eventMarch);
        }

        public void SetLine(Vector2 start, Vector2 end, Vector3 offset = default(Vector3),
            LineRelation lineRelationType = LineRelation.Self, bool flagAnimate = true) {
            Vector2 startPos = MapUtils.CoordinateToPosition(start);
            Vector2 endPos = MapUtils.CoordinateToPosition(end);
            float length = Vector2.Distance(
                startPos,
                endPos
            );
            this.line.gameObject.SetActiveSafe(true);
            this.flag.gameObject.SetActiveSafe(true);
            if (flagAnimate) {
                this.flagAnimator.SetTrigger("Fall");
            }

            this.directionNormal = (endPos - startPos).normalized;
            this.line.localScale = new Vector3(length * ratio, 1, 0);
            this.transform.position = (Vector3)((startPos + endPos) * 0.5f) + offset;
            this.clickCollider.localPosition = -Vector3.forward;
            float sign = Mathf.Sign(endPos.x - startPos.x);
            float angle = Vector2.Angle(Vector2.up, endPos - startPos) * sign;
            this.line.transform.eulerAngles = new Vector3(0, 0, angle);
            if (angle < 0) {
                angle += 360;
            }
            this.direction = angle;
            Vector3 lineStart = (Vector3)startPos + offset;
            Vector3 lineEnd = (Vector3)endPos + offset;
            this.opaqueLineRenderer.sharedMaterial = PoolManager.GetMaterial(
                MaterialPath.matMarchOpaquePrefix +
                GameHelper.UpperFirstCase(lineRelationType.ToString())
            );
            this.opaqueLineRenderer.SetPosition(0, lineEnd);
            this.opaqueLineRenderer.SetPosition(1, lineStart);
            this.transparentLineRenderer.sharedMaterial = PoolManager.GetMaterial(
                MaterialPath.matMarchTransparentPrefix +
                GameHelper.UpperFirstCase(lineRelationType.ToString())
            );
            this.transparentLineRenderer.SetPosition(0, lineStart);
            this.transparentLineRenderer.SetPosition(1, lineStart);
            this.flag.position = endPos;
            this.flag.localPosition = new Vector3(
                this.flag.localPosition.x,
                this.flag.localPosition.y,
                -0.002f
             );
        }

        private void SetIcon(Troop troop, LineDimensionType lineDimension) {
            if (troop == null || troop.Positions.Count == 0) {
                return;
            }
            //GameHelper.ClearChildren(this.role, needPool: false);
            TroopModel.TroopPositionReSort(troop);
            HeroPosition heroPosition = troop.Positions[0];
            this.role.gameObject.SetActive(true);
            switch (lineDimension) {
                case LineDimensionType.ThreeD:
                    GameObject heroGameObject = PoolManager.GetObject(
                        hero3DPathPrefix + HeroBattleConf.GetModelPath(heroPosition.Name),
                        this.role
                    );
                    if (heroGameObject != null) {
                        this.hero = heroGameObject.transform;
                        this.SetAnimation();
                        this.twoDSprite.gameObject.SetActive(false);
                    }
                    break;
                case LineDimensionType.TwoD:
                    this.twoDSprite.sprite = PoolManager.GetSprite(
                        string.Format(hero2DPathPrefix, HeroBattleConf.GetModelPath(heroPosition.Name))
                    );
                    this.twoDSprite.gameObject.SetActive(true);
                    break;
                default:
                    this.role.gameObject.SetActive(false);
                    return;
            }
        }

        private void SetAnimation() {
            //string trigger = string.Empty;
            this.hero.eulerAngles = Vector3.right * (-45);
            this.hero.Rotate(Vector3.up, direction, Space.Self);
            //this.animator.SetBool(GameConst.animIsMove, true);
            //this.animator.SetTrigger(GameConst.animMove);
        }

        public void Refresh(EventBase eventBase) {
            float percent = (RoleManager.GetCurrentUtcTime() - eventBase.startTime) /
                (float)eventBase.duration;
            percent = Mathf.Min(Mathf.Max(percent, 0), 1);
            this.march.localPosition = (percent - 0.5f) * (line.localScale.x / ratio) * this.directionNormal;
            this.march.localPosition -= Vector3.forward * 0.003f;
            this.opaqueLineRenderer.SetPosition(1, this.march.position);
            this.transparentLineRenderer.SetPosition(1, this.march.position);
            if (this.pnlNewMarch != null) {
                QueueItemView queueItemView = this.pnlNewMarch.GetComponent<QueueItemView>();
                queueItemView.UpdateItem(eventBase);
            }
        }

        public void RemoveMarchUI() {
            if (this.pnlNewMarch != null) {
                PoolManager.RemoveObject(this.pnlNewMarch);
                this.pnlNewMarch = null;
            }
        }

        private IEnumerator RemoveMarchUIDelay() {
            yield return YieldManager.GetWaitForSeconds(Time.unscaledDeltaTime);
            this.RemoveMarchUI();
        }

        private IEnumerator MoveToQueueView() {
            yield return YieldManager.GetWaitForSeconds(2f);
            UIManager.UIUnBind(this.pnlNewMarch.transform);
            RectTransform rectTransform = this.pnlNewMarch.GetComponent<RectTransform>();
            Vector2 target = new Vector2(148.1f, -166.4f);
            AnimationManager.Animate(this.pnlNewMarch, "Move", rectTransform.anchoredPosition,
                target, () => {
                    this.eventMarch.troop.Marched = true;
                    this.StartCoroutine(this.RemoveMarchUIDelay());
                });
        }

        public void OnPointerDown(PointerEventData eventData) {
            return;
        }

        public void OnPointerUp(PointerEventData eventData) {
            return;
        }

        public void OnPointerClick(PointerEventData eventData) {
            this.onClick.Invoke(this.id);
        }

        #region POOL

        public void OnInPool() {
            if (this.hero != null) {
                PoolManager.RemoveObject(this.hero.gameObject);
                this.hero = null;
            }
            this.flagAnimator.enabled = false;
            this.opaqueLineRenderer.enabled = false;
            this.transparentLineRenderer.enabled = false;
        }

        public void OnOutPool() {
            this.flagAnimator.enabled = true;
            this.opaqueLineRenderer.enabled = true;
            this.transparentLineRenderer.enabled = true;
        }

        #endregion

        #region FTE

        public void FteSetLine() {
            this.line.gameObject.SetActiveSafe(false);
            this.flag.gameObject.SetActiveSafe(false);
            this.transform.position += Vector3.forward * TileView.FteMarchLineOffset;
        }

        #endregion
    }
}
