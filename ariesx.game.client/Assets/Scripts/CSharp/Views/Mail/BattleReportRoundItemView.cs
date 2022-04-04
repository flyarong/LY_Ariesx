using UnityEngine;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using TMPro;
using Protocol;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

namespace Poukoute {
    [DisallowMultipleComponent]
    public class BattleReportRoundItemView : BaseItemViewsHolder {
        #region ui serialized
        public TextMeshProUGUI txtTitle;
        public Transform pnlRoundList;
        [SerializeField]
        private ContentSizeFitter contentSizeFitter;
        #endregion

        public Dictionary<string, Battle.Hero> attackerHeroes;
        public Dictionary<string, Battle.Hero> defenderHeroes;

        private Transform pnlRoundAction;
        private readonly string relationSelf = "self";
        private readonly string relationEnemy = "enemy";

        private readonly string attack = "<color=#D53C3BFF>";
        private readonly string health = "<color=#019F53FF>";
        private readonly string shield = "<color=#A5A5A5FF>";
        /*******************************/

        public override void MarkForRebuild() {
            base.MarkForRebuild();
            contentSizeFitter.enabled = true;
        }

        public void PrintAction(int offset,
            Battle.Action action, Battle.Action parent, bool isOneAction) {
            string description = string.Empty;
            if (isOneAction) {
                this.pnlRoundAction =
                    PoolManager.GetObject(PrefabPath.pnlRoundAction, this.pnlRoundList).transform;
                this.pnlRoundAction.GetComponent<Image>().color = this.GetHeroRelation(action.HeroId);
                GameHelper.ClearChildren(this.pnlRoundAction.transform);
            }
            switch (action.Type) {
                case SkillConst.TypeAttack:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId));
                    if (action.Percent != 0) {
                        description = string.Concat(description,
                            string.Format(
                                LocalManager.GetValue(LocalHashConst.battle_effect_normalatk_dominant),
                                action.Percent * 100));
                    }
                    break;
                case SkillConst.TypeBuffActing:
                case SkillConst.TypeDelaySkill:
                case SkillConst.TypeImmune:
                case SkillConst.TypeCastSkill:
                    //Debug.LogError("trigger_" + action.Trigger);
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId),
                        LocalManager.GetValue("trigger_", action.Trigger),
                        SkillConf.GetName(action.Name));
                    break;
                case SkillConst.TypeBuffLose:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId),
                        SkillConf.GetName("lose_" + action.Name));
                    break;
                case SkillConst.TypeGetBuff:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId),
                        this.GetBuffFormat(action));
                    break;
                case SkillConst.TypeDisableAttack:
                case SkillConst.TypeDisableSkill:
                case SkillConst.TypeStuned:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId));
                    break;
                case SkillConst.TypeDispel:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        SkillConf.GetName(action.Name));
                    break;
                case SkillConst.TypeHeal:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId),
                        this.GetNumber(action.Amount, health),
                        action.RemainAmount.Abs());
                    break;
                case SkillConst.TypeInjured:
                case SkillConst.TypeReflectAttack:
                    //Debug.LogError(action.Amount);
                    if (action.Amount == 0) {
                        break;
                    }
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId),
                        this.GetNumber(action.Amount, attack),
                        action.RemainAmount.Abs());
                    break;
                case SkillConst.TypeIntervene:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId),
                        this.GetHeroName(action.Actions[0].HeroId));
                    break;
                case SkillConst.TypeShieldInjured:
                    description = string.Format(
                        LocalManager.GetValue(action.Type),
                        this.GetHeroName(action.HeroId),
                        this.GetNumber(action.Amount, shield));
                    break;
                case SkillConst.TypeTakeEffect:
                case SkillConst.TypeDead:
                    break;
                default:
                    Debug.LogErrorf("No such type {0} in battle.", action.Type);
                    break;
            }

            if (!description.CustomIsEmpty()) {
                GameObject pnlRoundLine = PoolManager.GetObject(PrefabPath.pnlRoundLine, this.pnlRoundAction);
                pnlRoundLine.GetComponent<HorizontalLayoutGroup>().padding.left = (offset - 1) * 18 + 5;
                GameObject pnlNewLineImage = pnlRoundLine.transform.Find("PnlImage").gameObject;
                pnlNewLineImage.SetActiveSafe(offset == 0);
                description = string.Concat(description, "\n");
                pnlRoundLine.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = description;
                offset++;
            }

            foreach (Battle.Action child in action.Actions) {
                this.PrintAction(offset, child, action, false);
            }
        }


        private Color GetHeroRelation(string id) {
            if (this.attackerHeroes.ContainsKey(id)) {
                return ArtConst.battleReportColor[this.relationSelf];
            } else if (defenderHeroes.ContainsKey(id)) {
                return ArtConst.battleReportColor[this.relationEnemy];
            } else {
                return ArtConst.battleReportColor[this.relationSelf];
            }
        }

        private string GetHeroName(string id) {
            Battle.Hero attackHero;
            Battle.Hero defenceHero;
            if (this.attackerHeroes.TryGetValue(id, out attackHero)) {
                return "<color=#019F53FF>" +
                    HeroAttributeConf.GetLocalName(attackHero.GetId()) + "</color>";
            } else if (defenderHeroes.TryGetValue(id, out defenceHero)) {
                return "<color=#D53C3BFF>" +
                    HeroAttributeConf.GetLocalName(defenceHero.GetId()) + "</color>";
            } else {
                Debug.LogError("No such hero");
                return string.Empty;
            }
        }

        private string GetBuffFormat(Battle.Action action) {
            string prefix = string.Empty;
            StringBuilder s = new StringBuilder();
            if (action.IsHalo) {
                prefix = "halo_";
            }
            if (action.Name.CustomEquals("dot") ||
                action.Name.CustomEquals("hot") ||
                action.Name.CustomEquals("shield") ||
                action.Name.CustomEquals("damup") ||
                action.Name.CustomEquals("damdown") ||
                action.Name.CustomEquals("defup") ||
                action.Name.CustomEquals("defdown") ||
                action.Name.CustomEquals("healup") ||
                action.Name.CustomEquals("healdown")) {
                if (action.Rounds == -1) {
                    return string.Concat(
                        string.Format(LocalManager.GetValue("halo_", action.Name),
                        this.GetNumber(action.Amount, action.Amount > 0 ? health : attack),
                        LocalManager.GetValue(LocalHashConst.skill_last_ends))
                    );
                } else {
                    return s.AppendFormat(
                        LocalManager.GetValue(prefix, action.Name),
                        this.GetNumber(action.Amount, action.Amount > 0 ? health : attack),
                        action.Rounds
                    ).ToString();
                }
            } else if (action.Name.CustomEquals("speedup") ||
                       action.Name.CustomEquals("speeddown")) {
                //Debug.LogError(LocalManager.GetValue(prefix, action.Name));
                return string.Format(
                    LocalManager.GetValue(prefix, action.Name),
                    action.Speed,
                    action.Rounds
                );
            } else if (action.Name.CustomEquals("focusfire")) {
                return s.AppendFormat(
                    LocalManager.GetValue(prefix, action.Name),
                    this.GetHeroName(action.HeroId),
                    action.Rounds
                ).ToString();
            } else {
                if (action.IsHalo) {
                    return LocalManager.GetValue(prefix + action.Name);
                } else {
                    return s.AppendFormat(
                        LocalManager.GetValue(prefix + action.Name),
                        action.Rounds
                    ).ToString();
                }
            }
        }

        private string GetNumber(int number, string color) {
            return string.Concat(color, (number > 0) ? number : -number, "</color>");
        }
    }
}