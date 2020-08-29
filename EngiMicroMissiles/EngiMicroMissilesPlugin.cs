using System;
using BepInEx;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using EntityStates;
using EntityStates.Toolbot;
using EntityStates.Engi.EngiWeapon;
using UnityEngine;
using EngiMicroMissiles.MicroMissilesMod;
using RoR2.Projectile;

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
            skillDef2.activationState = new SerializableEntityStateType(typeof(ChargeMissiles));    //skillDef.activationState;
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
            skillDef2.skillDescriptionToken = "ENGI_PRIMARY_MICROMISSILES_DESCRIPTION";
            skillDef2.skillName = "ENGI_PRIMARY_MICROMISSILES_NAME";
            skillDef2.skillNameToken = "ENGI_PRIMARY_MICROMISSILES_NAME";

            
            LoadoutAPI.AddSkillDef(skillDef2);
            Array.Resize<SkillFamily.Variant>(ref skillFamily.variants, skillFamily.variants.Length + 1);
            SkillFamily.Variant[] variants = skillFamily.variants;
            int num = skillFamily.variants.Length - 1;

            SkillFamily.Variant variant = default(SkillFamily.Variant);
            variant.skillDef = skillDef2;
            variant.unlockableName = "";
            variant.viewableNode = new ViewablesCatalog.Node(skillDef2.skillNameToken, false, null);
            variants[num] = variant;

            LoadoutAPI.AddSkill(typeof(ChargeMissiles));
        }


        private void AddLanguagetokens()
        {
            Languages.AddToken("ENGI_PRIMARY_MICROMISSILES_NAME", "Micro Missiles");
            
            string text = string.Concat(new string[]
            {
                "Fire Micro Missiles for ",
                this.colorText("100% damage", "#E5C962"),
                " each."
            });
            Languages.AddToken("ENGI_PRIMARY_MICROMISSILES_DESCRIPTION", text);
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

namespace EngiMicroMissiles.MicroMissilesMod
{
    public class ChargeMissiles : BaseState
    {
        public GameObject MultMissile = Resources.Load<GameObject>("prefabs/projectiles/ToolbotGrenadeLauncherProjectile");

        public override void OnEnter()
        {
            base.OnEnter();
            this.totalDuration = ChargeMissiles.baseTotalDuration / this.attackSpeedStat;
            this.maxChargeTime = ChargeMissiles.baseMaxChargeTime / this.attackSpeedStat;
            Transform modelTransform = base.GetModelTransform();
            base.PlayAnimation("Gesture, Additive", "ChargeGrenades");
            Util.PlaySound(ChargeMissiles.chargeLoopStartSoundString, base.gameObject);
            if (modelTransform)
            {
                ChildLocator component = modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    Transform transform = component.FindChild("MuzzleLeft");
                    if (transform && ChargeMissiles.chargeEffectPrefab)
                    {
                        this.chargeLeftInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMissiles.chargeEffectPrefab, transform.position, transform.rotation);
                        this.chargeLeftInstance.transform.parent = transform;
                        ScaleParticleSystemDuration component2 = this.chargeLeftInstance.GetComponent<ScaleParticleSystemDuration>();
                        if (component2)
                        {
                            component2.newDuration = this.totalDuration;
                        }
                    }
                    Transform transform2 = component.FindChild("MuzzleRight");
                    if (transform2 && ChargeMissiles.chargeEffectPrefab)
                    {
                        this.chargeRightInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeMissiles.chargeEffectPrefab, transform2.position, transform2.rotation);
                        this.chargeRightInstance.transform.parent = transform2;
                        ScaleParticleSystemDuration component3 = this.chargeRightInstance.GetComponent<ScaleParticleSystemDuration>();
                        if (component3)
                        {
                            component3.newDuration = this.totalDuration;
                        }
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("Gesture, Additive", "Empty");
            Util.PlaySound(ChargeMissiles.chargeLoopStopSoundString, base.gameObject);
            EntityState.Destroy(this.chargeLeftInstance);
            EntityState.Destroy(this.chargeRightInstance);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.lastCharge = this.charge;
            this.charge = Mathf.Min((int)(base.fixedAge / this.maxChargeTime * (float)ChargeMissiles.maxCharges), ChargeMissiles.maxCharges);
            float t = (float)this.charge / (float)ChargeMissiles.maxCharges;
            float value = Mathf.Lerp(ChargeMissiles.minBonusBloom, ChargeMissiles.maxBonusBloom, t);
            base.characterBody.SetSpreadBloom(value, true);
            int num = Mathf.FloorToInt(Mathf.Lerp((float)ChargeMissiles.minMissileCount, (float)ChargeMissiles.maxMissileCount, t));
            if (this.lastCharge < this.charge)
            {
                Util.PlaySound(ChargeMissiles.chargeStockSoundString, base.gameObject, "engiM1_chargePercent", 100f * ((float)(num - 1) / (float)ChargeMissiles.maxMissileCount));
            }
            if ((base.fixedAge >= this.totalDuration || !base.inputBank || !base.inputBank.skill1.down) && base.isAuthority)
            {
                FireMicroMissiles fireMissiles = new FireMicroMissiles();
                fireMissiles.missileCountMax = num;
                //fireMissiles.projectilePrefab = MultMissile;
                this.outer.SetNextState(fireMissiles);
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static float baseTotalDuration = ChargeGrenades.baseTotalDuration;

        public static float baseMaxChargeTime = ChargeGrenades.baseMaxChargeTime;

        public static int maxCharges = ChargeGrenades.maxCharges;

        public static GameObject chargeEffectPrefab = ChargeGrenades.chargeEffectPrefab;

        public static string chargeStockSoundString = ChargeGrenades.chargeStockSoundString;

        public static string chargeLoopStartSoundString = ChargeGrenades.chargeLoopStartSoundString;

        public static string chargeLoopStopSoundString = ChargeGrenades.chargeLoopStopSoundString;

        public static int minMissileCount = ChargeGrenades.minGrenadeCount;

        public static int maxMissileCount = ChargeGrenades.maxGrenadeCount;

        public static float minBonusBloom = ChargeGrenades.minBonusBloom;

        public static float maxBonusBloom = ChargeGrenades.maxBonusBloom;

        private GameObject chargeLeftInstance;

        private GameObject chargeRightInstance;

        private int charge;

        private int lastCharge;

        private float totalDuration;

        private float maxChargeTime;
    }

    public class FireMicroMissiles : BaseState
    {
        private void FireMicroMissile(string targetMuzzle)
        {
            Util.PlaySound(FireMicroMissiles.attackSoundString, base.gameObject);
            this.projectileRay = base.GetAimRay();
            if (this.modelTransform)
            {
                ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    Transform transform = component.FindChild(targetMuzzle);
                    if (transform)
                    {
                        this.projectileRay.origin = transform.position;
                    }
                }
            }
            base.AddRecoil(-1f * FireMicroMissiles.recoilAmplitude, -2f * FireMicroMissiles.recoilAmplitude, -1f * FireMicroMissiles.recoilAmplitude, 1f * FireMicroMissiles.recoilAmplitude);
            if (FireMicroMissiles.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(FireMicroMissiles.effectPrefab, base.gameObject, targetMuzzle, false);
            }
            if (base.isAuthority)
            {
                float x = UnityEngine.Random.Range(0f, base.characterBody.spreadBloomAngle);
                float z = UnityEngine.Random.Range(0f, 360f);
                Vector3 up = Vector3.up;
                Vector3 axis = Vector3.Cross(up, this.projectileRay.direction);
                Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
                float y = vector.y;
                vector.y = 0f;
                float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
                float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireMicroMissiles.arcAngle;
                Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * this.projectileRay.direction);
                ProjectileManager.instance.FireProjectile(FireMicroMissiles.projectilePrefab, this.projectileRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireMicroMissiles.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
            base.characterBody.AddSpreadBloom(FireMicroMissiles.spreadBloomValue);
        }


        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FireMicroMissiles.baseDuration / this.attackSpeedStat;
            this.modelTransform = base.GetModelTransform();
            base.StartAimMode(2f, false);
        }


        public override void OnExit()
        {
            base.OnExit();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.fireTimer -= Time.fixedDeltaTime;
            float num = FireMicroMissiles.fireDuration / this.attackSpeedStat / (float)this.missileCountMax;
            if (this.fireTimer <= 0f && this.missileCount < this.missileCountMax)
            {
                this.fireTimer += num;
                if (this.missileCount % 2 == 0)
                {
                    this.FireMicroMissile("MuzzleLeft");
                    base.PlayCrossfade("Gesture Left Cannon, Additive", "FireGrenadeLeft", 0.1f);
                }
                else
                {
                    this.FireMicroMissile("MuzzleRight");
                    base.PlayCrossfade("Gesture Right Cannon, Additive", "FireGrenadeRight", 0.1f);
                }
                this.missileCount++;
            }
            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static GameObject effectPrefab = FireGrenades.effectPrefab;

        public static GameObject projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/ToolbotGrenadeLauncherProjectile");

        public int missileCountMax = 3;

        public static float damageCoefficient = FireGrenades.damageCoefficient;

        public static float fireDuration = 0.75f;

        public static float baseDuration = 1.25f;

        public static float arcAngle = 0.1f;

        public static float recoilAmplitude = 0.3f;

        public static string attackSoundString = FireGrenades.attackSoundString;

        public static float spreadBloomValue = 0.1f;

        private Ray projectileRay;

        private Transform modelTransform;

        private float duration;

        private float fireTimer;

        private int missileCount;
    }
}