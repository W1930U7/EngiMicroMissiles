using System;
using BepInEx;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using EntityStates;
using UnityEngine;

namespace EngiMicroMissiles
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(nameof(LoadoutAPI))]
    [BepInPlugin(GUID, MODNAME, VERSION)]

    public sealed class EngiMicroMissilesPlugin : BaseUnityPlugin
    {
        public const string
            MODNAME = "EngiMicroMissiles",
            AUTHOR = "Wipeout",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.0.1";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Awake is automatically called by Unity")]
        private void Awake() //Called when loaded by BepInEx.
        {
            SetupMicroMissilesSkillDef();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Start is automatically called by Unity")]
        private void Start() //Called at the first frame of the game.
        {

        }

        public void SetupMicroMissilesSkillDef()
        {
            GameObject gameObject = Resources.Load<GameObject>("prefabs/characterbodies/EngiBody");
            AddLanguagetokens();
            SkillLocator component = gameObject.GetComponent<SkillLocator>();
            SkillFamily skillFamily = component.primary.skillFamily;
            SkillDef skillDef = skillFamily.variants[0].skillDef;


            SkillDef skillDef2 = ScriptableObject.CreateInstance<SkillDef>();
            skillDef2.activationState = skillDef.activationState;
            skillDef2.activationStateMachineName = skillDef.activationStateMachineName;
            skillDef2.baseMaxStock = skillDef.baseMaxStock;
            skillDef2.baseRechargeInterval = skillDef.baseRechargeInterval;
            skillDef2.beginSkillCooldownOnSkillEnd = skillDef.beginSkillCooldownOnSkillEnd;
            skillDef2.canceledFromSprinting = skillDef.canceledFromSprinting;
            skillDef2.fullRestockOnAssign = skillDef.fullRestockOnAssign;
            skillDef2.interruptPriority = skillDef.interruptPriority;
            skillDef2.isBullets = skillDef.isBullets;
            skillDef2.isCombatSkill = skillDef.isCombatSkill;
            skillDef2.mustKeyPress = skillDef.mustKeyPress;
            skillDef2.noSprint = skillDef.noSprint;
            skillDef2.rechargeStock = skillDef.rechargeStock;
            skillDef2.requiredStock = skillDef.requiredStock;
            skillDef2.shootDelay = skillDef.shootDelay;
            skillDef2.stockToConsume = skillDef.stockToConsume;
            skillDef2.icon = skillDef.icon;
            skillDef2.skillDescriptionToken = "ARTI_PRIMARY_FASTBOLTS_DESCRIPTION";
            skillDef2.skillName = "Micro Missiles";
            skillDef2.skillNameToken = "ARTI_PRIMARY_FASTBOLTS_NAME";


            LoadoutAPI.AddSkillDef(skillDef);
            Array.Resize<SkillFamily.Variant>(ref skillFamily.variants, skillFamily.variants.Length + 1);
            SkillFamily.Variant[] variants = skillFamily.variants;
            int num = skillFamily.variants.Length - 1;

            SkillFamily.Variant variant = default(SkillFamily.Variant);
            variant.skillDef = skillDef2;
            variant.unlockableName = "";
            variant.viewableNode = new ViewablesCatalog.Node(skillDef2.skillNameToken, false, null);
            variants[num] = variant;
        }


        private void AddLanguagetokens()
        {
            Languages.AddToken("ARTI_PRIMARY_FASTBOLTS_NAME", "Micro Missiles");
            string text = string.Concat(new string[]
            {
                "Fire Flame Bolts for ",
                this.colorText("110% damage", "#E5C962"),
                " each. Shoots ",
                this.colorText("more bolts", "#95CDE5"),
                " when you have higher ",
                this.colorText("attack speed", "#95CDE5")
            });
            Languages.AddToken("ARTI_PRIMARY_FASTBOLTS_DESCRIPTION", text);
        }

        private string colorText(string text, string hex)
        {
            return string.Concat(new string[]
            {
                "<color=",
                hex,
                ">",
                text,
                "</color>"
            });
        }
    }
}
