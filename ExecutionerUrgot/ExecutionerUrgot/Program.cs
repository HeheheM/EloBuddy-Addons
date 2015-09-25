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
    /* Created by Counter */

    /* To Do:
        
        * Ignite + Smite input
        * don't Q Wind Wall/Unbreakable/BlackShield
         
    */
    class Program
    {
        /* Menus */
        public static Menu ExecutionerUrgotMenu, SettingMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu;
        // Skills
        public static Spell.Skillshot Q;
        public static Spell.Active Q2;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        public static Spell.Targeted Ignite;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            /* Confirming Champion */
            if (Player.Instance.ChampionName != "Urgot")
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear);
            Q2 = new Spell.Active(SpellSlot.Q);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Circular);
            R = new Spell.Targeted(SpellSlot.R, 550);
            if (Player.GetSpell(SpellSlot.Summoner1).Name == "Summonerdot")
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            }
            if (Player.GetSpell(SpellSlot.Summoner2).Name == "Summonerdot")
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);
            }

            ExecutionerUrgotMenu = MainMenu.AddMenu("Executioner Urgot", "ExecutionerUrgot");
            ExecutionerUrgotMenu.AddGroupLabel("Executioner Urgot");

            SettingMenu = ExecutionerUrgotMenu.AddSubMenu("Settings", "Settings");
            SettingMenu.AddGroupLabel("Settings");
            SettingMenu.AddSeparator();
            SettingMenu.Add("Drawmode", new CheckBox("Drawing Mode"));
            SettingMenu.Add("KSmode", new CheckBox("KS Mode"));
            SettingMenu.Add("Stackmode", new CheckBox("Stack Tear Mode"));
            SettingMenu.Add("Ignitemode", new CheckBox("Auto Ignite"));
            SettingMenu.AddSeparator();
            SettingMenu.AddLabel("Health Potion/Mana Potion/Crystalline Flask Activator - 0 is off");
            SettingMenu.Add("Healthcall", new Slider("Use Health Potion if Health %", 25, 0, 100));
            SettingMenu.Add("Manacall", new Slider("Use Mana Potion if Mana %", 25, 0, 100));
            SettingMenu.Add("FlaskHcall", new Slider("Use Crystalline Flask if Health %", 25, 0, 100));
            SettingMenu.Add("FlaskMcall", new Slider("Use Crystalline Flask if Mana %", 25, 0, 100));

            ComboMenu = ExecutionerUrgotMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.AddSeparator();
            ComboMenu.Add("Qcombo", new CheckBox("Q"));
            ComboMenu.Add("Wcombo", new CheckBox("W"));
            ComboMenu.Add("Ecombo", new CheckBox("E"));
            ComboMenu.Add("Rcombo", new CheckBox("Use R with rooting"));

            HarassMenu = ExecutionerUrgotMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.AddSeparator();
            HarassMenu.Add("Qharass", new CheckBox("Q"));

            JungleMenu = ExecutionerUrgotMenu.AddSubMenu("Jungle Features", "JungleFeatures");
            JungleMenu.AddGroupLabel("Jungle Features");
            JungleMenu.AddSeparator();
            JungleMenu.Add("Qjungle", new CheckBox("Q"));
            JungleMenu.Add("Ejungle", new CheckBox("E"));

            LaneClearMenu = ExecutionerUrgotMenu.AddSubMenu("Lane Clear Features", "LaneClearFeatures");
            LaneClearMenu.AddGroupLabel("Lane Clear Features");
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("Qlanec", new CheckBox("Q"));

            LastHitMenu = ExecutionerUrgotMenu.AddSubMenu("Last Hit Features", "LastHitFeatures");
            LastHitMenu.AddGroupLabel("Last Hit Features");
            LastHitMenu.AddSeparator();
            LastHitMenu.Add("Qlasthit", new CheckBox("Q"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Player.OnLevelUp += ExecutionerHandler.LevelerMode;
            /*Gapcloser.OnGapCloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;*/

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Program.SettingMenu["Drawmode"].Cast<CheckBox>().CurrentValue)
            {
                ExecutionerHandler.DrawMode();
            }
        }

        static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ExecutionerHandler.ComboMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                ExecutionerHandler.HarassMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                ExecutionerHandler.JungleMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                ExecutionerHandler.LaneClearMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                ExecutionerHandler.LastHitMode();
            }
            if (Program.SettingMenu["KSmode"].Cast<CheckBox>().CurrentValue)
            {
                ExecutionerHandler.KSMode();
            }
            if (Program.SettingMenu["Stackmode"].Cast<CheckBox>().CurrentValue)
            {
                ExecutionerHandler.StackMode();
            }
            if (Program.SettingMenu["Healthcall"].Cast<Slider>().CurrentValue > 0)
            {
                ExecutionerHandler.HealthPotionMode();
            }
            if (Program.SettingMenu["Manacall"].Cast<Slider>().CurrentValue > 0)
            {
                ExecutionerHandler.ManaPotionMode();
            }
            if (Program.SettingMenu["FlaskHcall"].Cast<Slider>().CurrentValue > 0 || Program.SettingMenu["FlaskMcall"].Cast<Slider>().CurrentValue > 0)
            {
                ExecutionerHandler.CrystallineFlaskMode();
            }
            if (Program.SettingMenu["Ignitemode"].Cast<CheckBox>().CurrentValue)
            {
                ExecutionerHandler.IgniteMode();
            }

            /* Menu Information */
            if (ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue == false)
            {
                ComboMenu["Rcombo"].Cast<CheckBox>().DisplayName = "Use R without rooting";
            }
            if (ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue == true)
            {
                ComboMenu["Rcombo"].Cast<CheckBox>().DisplayName = "Use R with rooting";
            }
        }

        /*static void Gapcloser_OnGapcloser(AIHeroClient target, GapCloserEventArgs args)
        {
            if (Program.W.IsReady())
                Program.W.Cast(target);
        }
        static void Interrupter_OnInterruptableSpell(AIHeroClient target, Interrupter.OnInterruptableSpellDelegate args)
        {
            if (Program.W.IsReady())
                Program.W.Cast(target);
        }*/
    }
}
