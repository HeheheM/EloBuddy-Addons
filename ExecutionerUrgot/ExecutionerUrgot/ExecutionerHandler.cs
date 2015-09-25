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

namespace ExecutionerUrgot
{
    class ExecutionerHandler
    {
        /* Grab Spell */
        public enum AttackSpell
        {
            Q,
            W,
            E,
            I,
            S,
        }

        /* Grab Player */
        public static AIHeroClient Urgot { get { return ObjectManager.Player; } }

        /* Grab Enemies */
        public static Obj_AI_Base GetEnemy(float range, GameObjectType gametype)
        {
            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health)
                    .Where(a => a.IsEnemy
                    && a.Type == gametype && !Urgot.IsRecalling
                    && !a.IsDead && a.IsValidTarget(range) && !a.IsInvulnerable
                    && a.Distance(Urgot) <= range).FirstOrDefault();
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
                    && a.Distance(Urgot) <= Program.Q.Range).FirstOrDefault();
            }

            /* Last hit with W */
            else if (spell == AttackSpell.W)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && !a.IsDead && a.IsValidTarget(Program.W.Range) && !a.IsInvulnerable
                    && a.Health <= WDamage(a)
                    && a.Distance(Urgot) <= Program.W.Range).FirstOrDefault();
            }

            /* Last hit with E */
            else if (spell == AttackSpell.E)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && !a.IsDead && a.IsValidTarget(Program.E.Range) && !a.IsInvulnerable
                    && a.Health <= EDamage(a)
                    && a.Distance(Urgot) <= Program.E.Range).FirstOrDefault();
            }
            else if (spell == AttackSpell.I)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && !a.IsDead && a.IsValidTarget(Program.Ignite.Range) && !a.IsInvulnerable
                    && a.Health <= (Urgot.GetSummonerSpellDamage(a, DamageLibrary.SummonerSpells.Ignite) - (a.HPRegenRate / 10))
                    && a.Distance(Urgot) <= Program.Ignite.Range).FirstOrDefault();
            }
            else if (spell == AttackSpell.S)
            {
                return ObjectManager.Get<Obj_AI_Base>()
                    .Where(a => a.IsEnemy
                    && a.Type == gametype
                    && !a.IsDead && a.IsValidTarget(Program.Smite.Range) && !a.IsInvulnerable
                    && Monsters.Any(name => a.Name.StartsWith(name))
                    && a.Health <= Urgot.GetSummonerSpellDamage(a, DamageLibrary.SummonerSpells.Smite)
                    && a.Distance(Urgot) <= Program.Smite.Range).FirstOrDefault();
            }
            else
                return null;
        }

        /* Grab Monsters */
        public static string[] Monsters =
        {
            "SRU_Blue", "SRU_Red", "SRU_Krug", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak",
            "SRU_Crab", "SRU_Dragon", "SRU_Baron",
            "TTNGolem", "TTNWolf", "TTNWraith", "TT_Spiderboss"
        };

        /* Grab Items */
        public static InventorySlot[] UrgotItems { get { return Urgot.InventoryItems; } }

        /* Damage Calculator */
        static float QDamage(Obj_AI_Base target)
        {
            return Urgot.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 60, 85, 110, 135, 160 }[Program.Q.Level]
                + (0.55f * Urgot.FlatMagicDamageMod)
                + (new float[] { 0f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f }[Program.Q.Level] * Urgot.MaxMana));
        }
        static float WDamage(Obj_AI_Base target)
        {
            return Urgot.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 80, 100, 120, 140, 160 }[Program.W.Level]
                + (0.4f * Urgot.FlatMagicDamageMod)
                + (0.025f * Urgot.MaxMana));
        }
        static float EDamage(Obj_AI_Base target)
        {
            return Urgot.CalculateDamageOnUnit(target, DamageType.Magical,
                new float[] { 0, 36, 52, 68, 84, 100 }[Program.E.Level]
                + (0.2f * Urgot.FlatMagicDamageMod)
                + (0.02f * Urgot.MaxMana));
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

            if (Program.ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue == true)
            {
                AIHeroClient Rcombo = ObjectManager.Get<AIHeroClient>().Where(a => a.IsEnemy
                 && a.Distance(Urgot) <= Program.W.Range).FirstOrDefault();
                if (Rcombo != null)
                {
                    if (Program.R.IsReady())
                        Program.R.Cast();
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
        public static void IgniteMode()
        {
            Obj_AI_Base target = GetEnemyKS(AttackSpell.I, GameObjectType.AIHeroClient);
            if (target != null)
            {
                if (Program.Ignite.IsReady())
                    Program.Ignite.Cast(target);
            }
        }
        public static void SmiteMode()
        {
            Obj_AI_Base target = GetEnemyKS(AttackSpell.S, GameObjectType.obj_AI_Base);
            if (target != null)
            {
                if (Program.Smite.IsReady())
                {
                    Program.Smite.Cast(target);
                }
            }
        }

        /* Misc Modes */
        public static void DrawMode()
        {
            if (Program.DrawingMenu["Qdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Urgot.Position, Program.Q.Range, Color.LawnGreen);
                Drawing.DrawCircle(Urgot.Position, 1200, Color.OrangeRed);
            }
            if (Program.DrawingMenu["Edraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Urgot.Position, Program.E.Range, Color.SpringGreen);
            }
            if (Program.DrawingMenu["Rdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Urgot.Position, Program.R.Range, Color.DarkGreen);
            }
            if (Program.Ignite != null && Program.DrawingMenu["Idraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Urgot.Position, Program.Ignite.Range, Color.MediumVioletRed);
            }
            if (Program.Smite != null && Program.DrawingMenu["Sdraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Urgot.Position, Program.Smite.Range, Color.LightGoldenrodYellow);
            }
        }
        public static void StackMode()
        {
            foreach (InventorySlot item in UrgotItems)
            {
                if ((item.Id == ItemId.Tear_of_the_Goddess || item.Id == ItemId.Tear_of_the_Goddess_Crystal_Scar
                        || item.Id == ItemId.Archangels_Staff || item.Id == ItemId.Archangels_Staff_Crystal_Scar
                        || item.Id == ItemId.Manamune || item.Id == ItemId.Manamune_Crystal_Scar)
                    && item.Stacks < 750
                    && Urgot.IsInShopRange()
                    && Program.Q.IsReady())
                {
                    Program.Q.Cast(Urgot.Position);
                }
            }
        }
        public static void HealthPotionMode()
        {
            foreach (InventorySlot item in UrgotItems)
            {
                if (item.Id == ItemId.Health_Potion
                    && Urgot.Health <= (Urgot.MaxHealth * (0.01 * Program.SettingMenu["Healthcall"].Cast<Slider>().CurrentValue))
                    && !Urgot.IsRecalling()
                    && !Urgot.IsInShopRange()
                    && !Urgot.HasBuff("RegenerationPotion"))
                {
                    item.Cast();
                }
            }
        }
        public static void ManaPotionMode()
        {
            foreach (InventorySlot item in UrgotItems)
            {
                if (item.Id == ItemId.Mana_Potion
                    && Urgot.Mana <= (Urgot.MaxMana * (0.01 * Program.SettingMenu["FlaskHcall"].Cast<Slider>().CurrentValue))
                    && !Urgot.IsRecalling()
                    && !Urgot.IsInShopRange()
                    && !Urgot.HasBuff("FlaskOfCrystalWater"))
                {
                    item.Cast();
                }
            }
        }
        public static void CrystallineFlaskMode()
        {
            foreach (InventorySlot item in UrgotItems)
            {
                /* Flask Health Call */
                if (item.Id == ItemId.Crystalline_Flask
                    && Urgot.Health <= (Urgot.MaxHealth * (0.01 * Program.SettingMenu["FlaskHcall"].Cast<Slider>().CurrentValue))
                    && !Urgot.IsRecalling()
                    && !Urgot.IsInShopRange()
                    && !Urgot.HasBuff("ItemCrystalFlask"))
                {
                    item.Cast();
                }
                /* Flask Mana Call */
                if (item.Id == ItemId.Crystalline_Flask
                    && Urgot.Mana <= (Urgot.MaxMana * (0.01 * Program.SettingMenu["FlaskMcall"].Cast<Slider>().CurrentValue))
                    && !Urgot.IsRecalling()
                    && !Urgot.IsInShopRange()
                    && !Urgot.HasBuff("ItemCrystalFlask"))
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

            int[] array = new int[] { 1, 3, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            int skill = array[Urgot.Level];

            if (skill == 1)
                Urgot.Spellbook.LevelSpell(SpellSlot.Q);
            else if (skill == 2)
                Urgot.Spellbook.LevelSpell(SpellSlot.W);
            else if (skill == 3)
                Urgot.Spellbook.LevelSpell(SpellSlot.E);
            else if (skill == 4)
                Urgot.Spellbook.LevelSpell(SpellSlot.R);
            else
                return;
        }
    }
}
