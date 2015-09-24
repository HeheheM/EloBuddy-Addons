using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace MagicianRyze
{
    /* Handler */
    class MagicianHandler
    {
        /* Grab Spell */
        public enum AttackSpell
        {
            Q,
            W,
            E,
        }
        
        /* Grab Player */
        public static AIHeroClient Ryze { get { return ObjectManager.Player; } }

        /* Grab Enemies */
        public static Obj_AI_Base GetEnemy(float range, GameObjectType gametype)
        {
            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health)
                    .Where(a => a.IsEnemy
                    && a.Type == gametype && !Ryze.IsRecalling
                    && !a.IsDead && a.IsValidTarget(range) && !a.IsInvulnerable
                    && a.Distance(Ryze) <= range).FirstOrDefault();
        }
        public static Obj_AI_Base GetEnemyKS(AttackSpell spell, GameObjectType gametype)
        {
            /* Last hit with Q */
            if (spell == AttackSpell.Q)
            {
                return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health)
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && !a.IsDead && a.IsValidTarget(Program.Q.Range) && !a.IsInvulnerable
                    && a.Health <= QDamage(a)
                    && a.Distance(Ryze) <= Program.Q.Range).FirstOrDefault();
            }

            /* Last hit with W */
            else if (spell == AttackSpell.W)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && !a.IsDead && a.IsValidTarget(Program.W.Range) && !a.IsInvulnerable
                    && a.Health <= WDamage(a)
                    && a.Distance(Ryze) <= Program.W.Range).FirstOrDefault();
            }

            /* Last hit with E */
            else if (spell == AttackSpell.E)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && !a.IsDead && a.IsValidTarget(Program.E.Range) && !a.IsInvulnerable
                    && a.Health <= EDamage(a)
                    && a.Distance(Ryze) <= Program.E.Range).FirstOrDefault();
            }

            else
                return null;
        }

        /* Grab Items */
        public static InventorySlot[] RyzeItems { get { return Ryze.InventoryItems; } }

        /* Damage Calculator */
        static float QDamage(Obj_AI_Base target)
        {
            return Ryze.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 60, 85, 110, 135, 160 }[Program.Q.Level] 
                + (0.55f * Ryze.FlatMagicDamageMod)
                + (new float[] { 0f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f }[Program.Q.Level] * Ryze.MaxMana));
        }
        static float WDamage(Obj_AI_Base target)
        {
            return Ryze.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 80, 100, 120, 140, 160 }[Program.W.Level]
                + (0.4f * Ryze.FlatMagicDamageMod)
                + (0.025f * Ryze.MaxMana));
        }
        static float EDamage(Obj_AI_Base target)
        {
            return Ryze.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 36, 52, 68, 84, 100}[Program.E.Level]
                + (0.2f * Ryze.FlatMagicDamageMod)
                + (0.02f * Ryze.MaxMana));
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

            /* W in combo mode */
            if (Program.ComboMenu["Wcombo"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wcombo = GetEnemy(Program.W.Range, GameObjectType.AIHeroClient);
                if (Wcombo != null)
                {
                    if (Program.W.IsReady())
                    {
                        Program.W.Cast(Wcombo);
                    }
                }
            }

            /* E in combo mode */
            if (Program.ComboMenu["Ecombo"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Ecombo = GetEnemy(Program.E.Range, GameObjectType.AIHeroClient);
                if (Ecombo != null)
                {
                    if (Program.E.IsReady())
                        Program.E.Cast(Ecombo);
                }
            }

            /* R in combo mode with root */
            if (Program.ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue == true)
            {
                AIHeroClient Rcombo = ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy
                 && a.Distance(Ryze) <= Program.W.Range
                && a.HasBuff("RyzeW")).FirstOrDefault();
                if (Rcombo != null)
                {
                    if (Program.R.IsReady())
                        Program.R.Cast();
                }
            }

            /* R in combo mode without root */
            if (Program.ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue == false)
            {
                AIHeroClient Rcombo = ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy && a.Distance(Ryze) <= Program.Q.Range).FirstOrDefault();
                if (Rcombo != null)
                {
                    if (Program.R.IsReady())
                        Program.R.Cast();
                }
            }

            /* Seraph's Embrace Shield */
            if (Program.ComboMenu["Seraphscall"].Cast<Slider>().CurrentValue > 0)
            {
                SeraphShieldMode();
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

            /* W in jungle mode */
            if (Program.JungleMenu["Wjungle"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wcamp = GetEnemy(Program.W.Range, GameObjectType.obj_AI_Minion);
                if (Wcamp != null)
                {
                    if (Program.W.IsReady())
                        Program.W.Cast(Wcamp);
                }
            }

            /* E in jungle mode */
            if (Program.JungleMenu["Ejungle"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Ecamp = GetEnemy(Program.E.Range, GameObjectType.obj_AI_Minion);
                if (Ecamp != null)
                {
                    if (Program.E.IsReady())
                        Program.E.Cast(Ecamp);
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

            if (Program.LaneClearMenu["Wlanec"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wminion = GetEnemy(Program.W.Range, GameObjectType.obj_AI_Minion);
                if (Wminion != null)
                {
                    if (Program.W.IsReady())
                        Program.W.Cast(Wminion);
                }
            }

            if (Program.LaneClearMenu["Wlanec"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Eminion = GetEnemy(Program.E.Range, GameObjectType.obj_AI_Minion);
                if (Eminion != null)
                {
                    if (Program.E.IsReady())
                        Program.E.Cast(Eminion);
                }
            }
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

            if (Program.LastHitMenu["Wlasthit"].Cast<CheckBox>().CurrentValue)
            {
                Obj_AI_Base Wminion = GetEnemyKS(AttackSpell.W, GameObjectType.obj_AI_Minion);
                if (Wminion != null)
                {
                    if (Program.W.IsReady())
                        Program.W.Cast(Wminion);
                }
            }
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

                Obj_AI_Base Wks = GetEnemyKS(AttackSpell.W, GameObjectType.AIHeroClient);
                if (Wks != null)
                {
                    if (Program.W.IsReady())
                        Program.W.Cast(Wks);
                }

                Obj_AI_Base Eks = GetEnemyKS(AttackSpell.E, GameObjectType.AIHeroClient);
                if (Eks != null)
                {
                    if (Program.E.IsReady())
                        Program.E.Cast(Eks);
                }
            }
        }

        /* Misc Modes */
        public static void DrawMode()
        {
            Drawing.DrawCircle(Ryze.Position, Program.Q.Range, Color.DeepSkyBlue);
            Drawing.DrawCircle(Ryze.Position, Program.W.Range, Color.SkyBlue);
        }
        public static void StackMode()
        {
            foreach (InventorySlot item in RyzeItems)
            {
                if ((item.Id == ItemId.Tear_of_the_Goddess || item.Id == ItemId.Tear_of_the_Goddess_Crystal_Scar
                        || item.Id == ItemId.Archangels_Staff || item.Id == ItemId.Archangels_Staff_Crystal_Scar
                        || item.Id == ItemId.Manamune || item.Id == ItemId.Manamune_Crystal_Scar)
                    && item.Stacks < 750
                    && Ryze.IsInShopRange()
                    && Program.Q.IsReady())
                {
                    Program.Q.Cast(Ryze.Position);
                }
            }
        }
        public static void SeraphShieldMode()
        {
            foreach (InventorySlot item in RyzeItems)
            {
                if (((int)item.Id == 3040 || (int)item.Id == 3048)
                    && Ryze.Health <= (Ryze.MaxHealth * (0.01 * Program.ComboMenu["Seraphscall"].Cast<Slider>().CurrentValue))
                    && item.CanUseItem())
                {
                    item.Cast();
                }
            }
        }
        public static void HealthPotionMode()
        {
            foreach(InventorySlot item in RyzeItems)
            {
                if (item.Id == ItemId.Health_Potion
                    && Ryze.Health <= (Ryze.MaxHealth * (0.01 * Program.SettingMenu["Healthcall"].Cast<Slider>().CurrentValue))
                    && !Ryze.IsRecalling()
                    && !Ryze.IsInShopRange()
                    && !Ryze.HasBuff("RegenerationPotion"))
                {
                    item.Cast();
                }
            }
        }
        public static void ManaPotionMode()
        {
            foreach (InventorySlot item in RyzeItems)
            {
                if (item.Id == ItemId.Mana_Potion
                    && Ryze.Mana <= (Ryze.MaxMana * (0.01 * Program.SettingMenu["FlaskHcall"].Cast<Slider>().CurrentValue))
                    && !Ryze.IsRecalling()
                    && !Ryze.IsInShopRange()
                    && !Ryze.HasBuff("FlaskOfCrystalWater"))
                {
                    item.Cast();
                }
            }
        }
        public static void CrystallineFlaskMode()
        {
            foreach (InventorySlot item in RyzeItems)
            {
                /* Flask Health Call */
                if (item.Id == ItemId.Crystalline_Flask
                    && Ryze.Health <= (Ryze.MaxHealth * (0.01 * Program.SettingMenu["FlaskHcall"].Cast<Slider>().CurrentValue))
                    && !Ryze.IsRecalling()
                    && !Ryze.IsInShopRange()
                    && !Ryze.HasBuff("ItemCrystalFlask"))
                {
                    item.Cast();
                }
                /* Flask Mana Call */
                if (item.Id == ItemId.Crystalline_Flask
                    && Ryze.Mana <= (Ryze.MaxMana * (0.01 * Program.SettingMenu["FlaskMcall"].Cast<Slider>().CurrentValue))
                    && !Ryze.IsRecalling()
                    && !Ryze.IsInShopRange()
                    && !Ryze.HasBuff("ItemCrystalFlask"))
                {
                    item.Cast();
                }
            }
        }
        public static void LevelerMode(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            int[] array = new int[] { 1, 2, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            int skill = array[args.Level - 1];

            if (skill == 1)
                Ryze.Spellbook.LevelSpell(SpellSlot.Q);
            else if (skill == 2)
                Ryze.Spellbook.LevelSpell(SpellSlot.W);
            else if (skill == 3)
                Ryze.Spellbook.LevelSpell(SpellSlot.E);
            else if (skill == 4)
                Ryze.Spellbook.LevelSpell(SpellSlot.R);
        }
    }
}
