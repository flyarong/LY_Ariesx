using UnityEngine;
using System.Collections;

public class SkillConst : MonoBehaviour {
    public const string TypeAttack = "battle_effect_normalatk";
    public const string TypeBuffActing = "battle_effect_effecttriggered";
    public const string TypeBuffLose = "battle_effect_buffremove";
    public const string TypeDelaySkill = "battle_effect_spellcasting";
    public const string TypeDisableAttack = "battle_effect_disarm";
    public const string TypeDisableSkill = "battle_effect_silence";
    public const string TypeDispel = "battle_effect_dispel";
    public const string TypeGetBuff = "battle_effect_getbuff";
    public const string TypeHeal = "battle_effect_getheal";
    public const string TypeImmune = "battle_effect_immune";
    public const string TypeInjured = "battle_effect_injured";
    public const string TypeIntervene = "battle_effect_intervene";
    public const string TypeReflectAttack = "battle_effect_reflect";
    public const string TypeShieldInjured = "battle_effect_damabsorb";
    public const string TypeStuned = "battle_effect_stun";
    public const string TypeTakeEffect = "battle_effect_cause";
    public const string TypeCastSkill = "battle_effect_cast";
    public const string TypeDead = "battle_effect_herodead";

    public const string TriggerBeginning = "battlestart"; // 战斗开始释放一次，以后永不触发
    public const string TriggerDead = "dead";        // 死亡后立即释放
    public const string TriggerDefault = "default";     // 普通攻击之前，一定概率释放
    public const string TriggerHalo = "halo";        // 一直生效，直到该英雄死亡
    public const string TriggerHP = "hp";          // 血量到达一定比例后生效
    public const string TriggerHitMe = "hitme";       // 英雄被攻击后，一定概率释放
    public const string TriggerIHit = "ihit";        // 英雄攻击目标后，一定概率释放
}
