using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace BallistaKogMaw
{
    class BallistaHandler
    {
        public static Obj_AI_Base Ptarget;
        /* Clockbased Events */
        public static int lastSecondUsedTear = -1;
        public static int GetSecondsSinceUses(int varlastsecused)
        {
            int curSeconds = System.DateTime.Now.Second;
            if (varlastsecused != -1)
            {
                if (curSeconds < varlastsecused)
                {
                    return (curSeconds + 60) - varlastsecused;
                }
                else
                    return curSeconds - varlastsecused;
            }
            else
                return 5;
        }
        /* Grab Spell */
        public enum AttackSpell
        {
            Q,
            E,
            R,
            I,
            A,
        }

        /* Grab Player */
        public static AIHeroClient KogMaw { get { return ObjectManager.Player; } }

        /* Grab Enemies */
        public static Obj_AI_Base GetEnemy(float range, GameObjectType gametype)
        {
            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health)
                    .Where(a => a.IsEnemy
                    && a.Type == gametype && !KogMaw.IsRecalling
                    && a.IsValidTarget(range) && !a.IsInvulnerable
                    && a.Distance(KogMaw) <= range).FirstOrDefault();
        }
        public static Obj_AI_Base GetEnemyKS(AttackSpell spell, GameObjectType gametype)
        {
            /* Last hit with Q */
            if (spell == AttackSpell.Q)
            {
                return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health)
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && a.IsValidTarget(Program.Q.Range) && !a.IsInvulnerable
                    && a.Health <= QDamage(a)
                    && a.Distance(KogMaw) <= Program.Q.Range).FirstOrDefault();
            }
            /* Last hit with R */
            else if (spell == AttackSpell.R)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && a.IsValidTarget(Program.R.Range) && !a.IsInvulnerable
                    && a.Health <= RDamage(a)
                    && a.Distance(KogMaw) <= Program.R.Range).FirstOrDefault();
            }
            else if (spell == AttackSpell.I)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && a.IsValidTarget(Program.Ignite.Range) && !a.IsInvulnerable
                    && a.Health <= (KogMaw.GetSummonerSpellDamage(a, DamageLibrary.SummonerSpells.Ignite) - (a.HPRegenRate * 0.1))
                    && a.Distance(KogMaw) <= Program.Ignite.Range).FirstOrDefault();
            }
            else if (spell == AttackSpell.A)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && a.IsValidTarget(KogMaw.GetAutoAttackRange()) && !a.IsInvulnerable
                    && a.Health <= PassiveDamage(a)
                    && a.Distance(KogMaw) <= KogMaw.GetAutoAttackRange()).FirstOrDefault();
            }
            else
                return null;
        }
        public static Obj_AI_Base GetEnemyWKS(GameObjectType gametype)
        {
            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health)
                    .Where(a => a.IsEnemy
                    && a.Type == gametype && !KogMaw.IsRecalling
                    && a.IsValidTarget(GetWRange()) && !a.IsInvulnerable
                    && a.Health <= KogMaw.GetAutoAttackDamage(a)
                    && a.Distance(KogMaw) > KogMaw.GetAutoAttackRange()
                    && a.Distance(KogMaw) <= GetWRange()).FirstOrDefault();
        }

        /* Grab Items */
        public static InventorySlot[] KogMawItems { get { return KogMaw.InventoryItems; } }

        /* Grab R Stacks */
        public static int CountStacks()
        {
            if (KogMaw.HasBuff("KogMawLivingArtillery"))
            {
                return KogMaw.GetBuffCount("KogMawLivingArtillery");
            }
            else
                return 0;
        }
        /* Get Range */
        public static void GetRrange()
        {
            Program.R = new Spell.Skillshot(SpellSlot.R, (uint)Program.Rrange[Program.R.Level], SkillShotType.Circular);
        }
        static float GetWRange()
        {
            return new float[] { 0, 630, 650, 670, 710 }[Program.W.Level];
        }
        

        /* Damage Calculator */
        static float PassiveDamage(Obj_AI_Base target)
        {
            return KogMaw.CalculateDamageOnUnit(target, DamageType.True,
                new float[] { 0, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375,
                400, 425, 450, 475, 500, 525, 550 }[KogMaw.Level]);
        }
        static float QDamage(Obj_AI_Base target)
        {
            return KogMaw.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 80, 130, 180, 230, 280 }[Program.Q.Level]
                + (0.50f * KogMaw.FlatMagicDamageMod));
        }
        /* W Buff is called KogMawBioArcaneBarrage */
        static float EDamage(Obj_AI_Base target)
        {
            return KogMaw.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 60, 110, 160, 210, 260 }[Program.E.Level]
                + (0.7f * KogMaw.FlatMagicDamageMod));
        }
        static float RDamage(Obj_AI_Base target)
        {
            if (target.Type == GameObjectType.AIHeroClient)
            {
                return KogMaw.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 160, 240, 320 }[Program.R.Level]
                + (0.3f * KogMaw.FlatMagicDamageMod)
                + (0.5f * KogMaw.FlatPhysicalDamageMod));
            }
            else
            {
                return KogMaw.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 80, 120, 160 }[Program.R.Level]
                + (0.3f * KogMaw.FlatMagicDamageMod)
                + (0.5f * KogMaw.FlatPhysicalDamageMod));
            }
        }

        /* Features Handler */
        public static void ComboMode()
        {
            /* Q in combo mode */
            if (Program.ComboMenu["Qcombo"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Qcombo = GetEnemy(Program.Q.Range, GameObjectType.AIHeroClient);
                if (Qcombo != null)
                {
                    if (Program.Q.IsReady())
                        Program.Q.Cast(Qcombo);
                }
            }

            /* W in combo mode 
            if (Program.ComboMenu["Wcombo"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wcombo = GetEnemy(GetWRange(), GameObjectType.AIHeroClient);
                if (Wcombo != null)
                {
                    if (Program.W.IsReady())
                        Program.W.Cast();
                }
            }*/
            
            /* E in combo mode */
            if (Program.ComboMenu["Ecombo"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Ecombo = GetEnemy(Program.E.Range, GameObjectType.AIHeroClient);
                if (Ecombo != null)
                {
                    if (Program.E.IsReady())
                        Program.E.Cast(Ecombo.Position);
                }
            }

            /* R in combo mode */
            if (Program.ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Rcombo = GetEnemy(Program.R.Range, GameObjectType.AIHeroClient);
                if (Rcombo != null)
                {
                    if (Program.R.IsReady() && Program.R.GetPrediction(Rcombo).HitChance >= HitChance.High)
                        Program.R.Cast(Rcombo.Position);
                }
            }
        }
        public static void HarassMode()
        {
            /* Q in harass mode */
            if (Program.HarassMenu["Qharass"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Qharass = GetEnemy(Program.Q.Range, GameObjectType.AIHeroClient);
                if (Qharass != null)
                {
                    if (Program.Q.IsReady())
                        Program.Q.Cast(Qharass);
                }
            }
        }
        public static void JungleMode()
        {
            /* Q in jungle mode */
            if (Program.JungleMenu["Qjungle"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Qcamp = GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
                if (Qcamp != null)
                {
                    if (Program.Q.IsReady())
                        Program.Q.Cast(Qcamp);
                }
            }
            /* W in jungle mode 
            if (Program.JungleMenu["Wjungle"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wcamp = GetEnemy(GetWRange(), GameObjectType.obj_AI_Minion);
                if (Wcamp != null)
                {
                    if (Program.W.IsReady())
                        Program.W.Cast();
                }
            }*/
            /* R in jungle mode */
            if (Program.JungleMenu["Rjungle"].Cast<CheckBox>().CurrentValue && CountStacks() < 2)
            {
                Obj_AI_Base Rcamp = GetEnemyKS(AttackSpell.R, GameObjectType.obj_AI_Minion);
                if (Rcamp != null)
                {
                    if (Program.R.IsReady())
                        Program.R.Cast(Rcamp.Position);
                }
            }
        }
        public static void LaneClearMode()
        {
            if (Program.LaneClearMenu["Qlanec"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Qminion = GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
                if (Qminion != null)
                {
                    if (Program.Q.IsReady())
                        Program.Q.Cast(Qminion);
                }
            }
            /*if (Program.JungleMenu["Wlanec"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wminion = GetEnemy(GetWRange(), GameObjectType.obj_AI_Minion);
                if (Wminion != null)
                {
                    if (Program.W.IsReady())
                        Program.W.Cast();
                }
            }*/
        }
        public static void LastHitMode()
        {
            if (Program.LastHitMenu["Qlasthit"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Qminion = GetEnemyKS(AttackSpell.Q, GameObjectType.obj_AI_Minion);
                if (Qminion != null)
                {
                    if (Program.Q.IsReady())
                        Program.Q.Cast(Qminion);
                }
            }
            /*if (Program.LastHitMenu["Wlasthit"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wminion = GetEnemyWKS(GameObjectType.obj_AI_Minion);
                if (Wminion != null)
                {
                    if (Program.W.IsReady())
                    {
                        Program.W.Cast();
                        Orbwalker.ForcedTarget = Wminion;
                    }
                }
            }*/
        }
        public static void KSMode()
        {
            if (Program.SettingMenu["KSmode"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Qks = GetEnemyKS(AttackSpell.Q, GameObjectType.AIHeroClient);
                if (Qks != null)
                {
                    if (Program.Q.IsReady())
                        Program.Q.Cast(Qks);
                }

                Obj_AI_Base Rks = GetEnemyKS(AttackSpell.R, GameObjectType.AIHeroClient);
                if (Rks != null)
                {
                    if (Program.R.IsReady())
                        Program.R.Cast(Rks.Position);
                }
            }
        }
        public static void IgniteMode()
        {
            Obj_AI_Base target = GetEnemyKS(AttackSpell.I, GameObjectType.AIHeroClient);
            if (target != null)
            {
                if (Program.Ignite.IsReady())
                    Program.Ignite.Cast(target);
            }
        }
        public static void DeathFollowMode()
        {
            if (KogMaw.HasBuff("kogmawicathiansurprise"))
            {
                if (Ptarget == null)
                {
                    Obj_AI_Base kstarget = GetEnemyKS(AttackSpell.A, GameObjectType.AIHeroClient);
                    Obj_AI_Base target = GetEnemy(KogMaw.GetAutoAttackRange(), GameObjectType.AIHeroClient);
                    Obj_AI_Base ksminion = GetEnemyKS(AttackSpell.A, GameObjectType.obj_AI_Minion);
                    Obj_AI_Base minion = GetEnemy(KogMaw.GetAutoAttackRange(), GameObjectType.obj_AI_Minion);
                    if (kstarget != null)
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, kstarget);
                        Ptarget = kstarget;
                    }
                    else if (target != null)
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, target);
                        Ptarget = target;
                    }
                    else if (ksminion != null)
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, ksminion);
                        Ptarget = ksminion;
                    }
                    else if (minion != null)
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, minion);
                        Ptarget = minion;
                    }
                    else
                        return;
                }
                else
                    Player.IssueOrder(GameObjectOrder.MoveTo, Ptarget);
            }
            else if (Ptarget != null)
            {
                Ptarget = null;
            }
        }

        /* Misc Modes */
        public static bool IsInShopRange()
        {
            if (ObjectManager.Get<Obj_SpawnPoint>().Where(a => a.IsAlly && a.Distance(KogMaw) <= 1000).FirstOrDefault() != null)
                return true;
            else
                return false;
        }
        public static void DrawMode()
        {
            if (Program.DrawingMenu["Qdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(KogMaw.Position, Program.Q.Range, Color.SeaGreen);
            }
            /*if (Program.DrawingMenu["Wdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(KogMaw.Position, GetWRange(), Color.SeaGreen);
            }*/
            if (Program.DrawingMenu["Edraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(KogMaw.Position, Program.E.Range, Color.SeaGreen);
            }
            if (Program.DrawingMenu["Rdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(KogMaw.Position, Program.R.Range, Color.MediumSeaGreen);
            }
            if (Program.Ignite != null && Program.DrawingMenu["Idraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(KogMaw.Position, Program.Ignite.Range, Color.MediumVioletRed);
            }
        }
        public static void StackMode()
        {
            foreach (InventorySlot item in KogMawItems)
            {
                if ((item.Id == ItemId.Tear_of_the_Goddess || item.Id == ItemId.Tear_of_the_Goddess_Crystal_Scar
                        || item.Id == ItemId.Archangels_Staff || item.Id == ItemId.Archangels_Staff_Crystal_Scar
                        || item.Id == ItemId.Manamune || item.Id == ItemId.Manamune_Crystal_Scar)
                    && GetSecondsSinceUses(lastSecondUsedTear) > 4
                    // && item.ItemData.Stacks < 750
                    && !KogMaw.IsRecalling()
                    && IsInShopRange())
                {
                    if (Program.Q.IsReady())
                    {
                        Program.Q.Cast(ObjectManager.Get<Obj_SpawnPoint>().OrderBy(a => a.Distance(KogMaw)).FirstOrDefault().Position);
                        lastSecondUsedTear = System.DateTime.Now.Second;
                    }
                    /* else if (Program.E.IsReady())
                    {
                        Program.E.Cast(ObjectManager.Get<Obj_SpawnPoint>().OrderBy(a => a.Distance(KogMaw)).FirstOrDefault().Position);
                        lastSecondUsedTear = System.DateTime.Now.Second;
                    }*/
                    else if (Program.R.IsReady() && CountStacks() < 1)
                    {
                        Program.R.Cast(ObjectManager.Get<Obj_SpawnPoint>().OrderBy(a => a.Distance(KogMaw)).FirstOrDefault().Position);
                        lastSecondUsedTear = System.DateTime.Now.Second;
                    }
                }
            }
        }
        public static void HealthPotionMode()
        {
            foreach (InventorySlot item in KogMawItems)
            {
                if (item.Id == ItemId.Health_Potion
                    && KogMaw.Health <= (KogMaw.MaxHealth * (0.01 * Program.Sliders.Heathcall.CurrentValue))
                    && !KogMaw.IsRecalling()
                    && !IsInShopRange()
                    && !KogMaw.HasBuff("RegenerationPotion"))
                {
                    item.Cast();
                }
            }
        }
        public static void ManaPotionMode()
        {
            foreach (InventorySlot item in KogMawItems)
            {
                if (item.Id == ItemId.Mana_Potion
                    && KogMaw.Mana <= (KogMaw.MaxMana * (0.01 * Program.Sliders.Manacall.CurrentValue))
                    && !KogMaw.IsRecalling()
                    && !IsInShopRange()
                    && !KogMaw.HasBuff("FlaskOfCrystalWater"))
                {
                    item.Cast();
                }
            }
        }
        public static void CrystallineFlaskMode()
        {
            foreach (InventorySlot item in KogMawItems)
            {
                /* Flask Health Call */
                if (item.Id == ItemId.Crystalline_Flask
                    && KogMaw.Health <= (KogMaw.MaxHealth * (0.01 * Program.Sliders.FlaskHcall.CurrentValue))
                    && !KogMaw.IsRecalling()
                    && !IsInShopRange()
                    && !KogMaw.HasBuff("ItemCrystalFlask"))
                {
                    item.Cast();
                }
                /* Flask Mana Call */
                if (item.Id == ItemId.Crystalline_Flask
                    && KogMaw.Mana <= (KogMaw.MaxMana * (0.01 * Program.Sliders.FlaskMcall.CurrentValue))
                    && !KogMaw.IsRecalling()
                    && !IsInShopRange()
                    && !KogMaw.HasBuff("ItemCrystalFlask"))
                {
                    item.Cast();
                }
            }
        }
        public static void HealMode()
        {
            if (Program.Heal != null
                && Program.Heal.IsReady()
                && !KogMaw.IsRecalling()
                && !IsInShopRange()
                && !KogMaw.HasBuff("summonerdot")
                && KogMaw.Health <= (KogMaw.MaxHealth * (0.01 * Program.Sliders.Healcall.CurrentValue)))
            {
                Program.Heal.Cast();
            }
        }
        public static void LevelerMode(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            int[] leveler = new int[] { 2, 3, 1, 2, 2, 4, 2, 3, 2, 3, 4, 3, 3, 1, 1, 4, 1, 1 };
            int skill = leveler[KogMaw.Level];

            if (skill == 1)
                KogMaw.Spellbook.LevelSpell(SpellSlot.Q);
            else if (skill == 2)
                KogMaw.Spellbook.LevelSpell(SpellSlot.W);
            else if (skill == 3)
                KogMaw.Spellbook.LevelSpell(SpellSlot.E);
            else if (skill == 4)
                KogMaw.Spellbook.LevelSpell(SpellSlot.R);
            else
                return;
        }
    }
}
