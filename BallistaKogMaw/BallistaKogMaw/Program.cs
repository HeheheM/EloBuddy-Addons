using System;
using System.Collections.Generic;
using System.Linq;
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
    /* Created by Counter */

    /* To Do:

        * 

    */
    class Program
    {
        /* Menus */
        public static Menu BallistaKogMawMenu, SettingMenu, DrawingMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu;
        /* Skills */
        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static int[] Rrange = new int[] { 1200, 1200, 1500, 1800 };
        public static Spell.Active Heal;
        public static Spell.Targeted Ignite;

        /* Slider structures */
        public struct Sliders
        {
            public static Slider
                Heathcall = new Slider("Use Health Potion if Health %", 25, 0, 100),
                Manacall = new Slider("Use Mana Potion if Mana %", 25, 0, 100),
                FlaskHcall = new Slider("Use Crystalline Flask if Health %", 25, 0, 100),
                FlaskMcall = new Slider("Use Crystalline Flask if Mana %", 25, 0, 100),
                Healcall = new Slider("Use Heal if Health %", 15, 0, 100);
        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            /* Confirming Champion */
            if (Player.Instance.ChampionName != "KogMaw")
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1280, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Circular);
            if (Player.GetSpell(SpellSlot.Summoner1).Name == "summonerdot")
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            }
            if (Player.GetSpell(SpellSlot.Summoner2).Name == "summonerdot")
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);
            }
            if (Player.GetSpell(SpellSlot.Summoner1).Name == "summonerheal")
            {
                Heal = new Spell.Active(SpellSlot.Summoner1);
            }
            if (Player.GetSpell(SpellSlot.Summoner2).Name == "summonerheal")
            {
                Heal = new Spell.Active(SpellSlot.Summoner2);
            }

            BallistaKogMawMenu = MainMenu.AddMenu("Ballista Kog'Maw", "BallistaKogMaw");
            BallistaKogMawMenu.AddGroupLabel("Ballista Kog'Maw");

            SettingMenu = BallistaKogMawMenu.AddSubMenu("Settings", "Settings");
            SettingMenu.AddGroupLabel("Settings");
            SettingMenu.AddSeparator();
            SettingMenu.Add("KSmode", new CheckBox("KS Mode"));
            SettingMenu.Add("Stackmode", new CheckBox("Stack Tear Mode"));
            if (Ignite != null)
            {
                SettingMenu.Add("Ignitemode", new CheckBox("Auto Ignite"));
            }
            SettingMenu.AddSeparator();
            SettingMenu.AddLabel("Health Potion/Mana Potion/Crystalline Flask Activator - 0 is off");
            SettingMenu.Add("Healthcall", Sliders.Heathcall);
            SettingMenu.Add("Manacall", Sliders.Manacall);
            SettingMenu.Add("FlaskHcall", Sliders.FlaskHcall);
            SettingMenu.Add("FlaskMcall", Sliders.FlaskMcall);
            if (Heal != null)
            {
                SettingMenu.AddLabel("Summoner Spell Heal Activator - 0 is off");
                SettingMenu.Add("Healcall", Sliders.Healcall);
            }

            DrawingMenu = BallistaKogMawMenu.AddSubMenu("Drawing Features", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("Drawing Features");
            DrawingMenu.AddSeparator();
            DrawingMenu.Add("Qdraw", new CheckBox("Q"));
            DrawingMenu.Add("Wdraw", new CheckBox("W"));
            DrawingMenu.Add("Edraw", new CheckBox("E"));
            DrawingMenu.Add("Rdraw", new CheckBox("R"));
            if (Ignite != null)
            {
                DrawingMenu.Add("Idraw", new CheckBox("Ignite"));
            }

            ComboMenu = BallistaKogMawMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.AddSeparator();
            ComboMenu.Add("Qcombo", new CheckBox("Q"));
            ComboMenu.Add("Wcombo", new CheckBox("W"));
            ComboMenu.Add("Ecombo", new CheckBox("E"));
            ComboMenu.Add("Rcombo", new CheckBox("R"));

            HarassMenu = BallistaKogMawMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.AddSeparator();
            HarassMenu.Add("Qharass", new CheckBox("Q"));

            JungleMenu = BallistaKogMawMenu.AddSubMenu("Jungle Features", "JungleFeatures");
            JungleMenu.AddGroupLabel("Jungle Features");
            JungleMenu.AddSeparator();
            JungleMenu.Add("Qjungle", new CheckBox("Q"));
            JungleMenu.Add("Wjungle", new CheckBox("W"));
            JungleMenu.Add("Rjungle", new CheckBox("R"));

            LaneClearMenu = BallistaKogMawMenu.AddSubMenu("Lane Clear Features", "LaneClearFeatures");
            LaneClearMenu.AddGroupLabel("Lane Clear Features");
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("Qlanec", new CheckBox("Q"));
            LaneClearMenu.Add("Wlanec", new CheckBox("W"));

            LastHitMenu = BallistaKogMawMenu.AddSubMenu("Last Hit Features", "LastHitFeatures");
            LastHitMenu.AddGroupLabel("Last Hit Features");
            LastHitMenu.AddSeparator();
            LastHitMenu.Add("Qlasthit", new CheckBox("Q"));
            LastHitMenu.Add("Wlasthit", new CheckBox("W - Out of Reach AA"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            if (BallistaHandler.KogMaw.Level == 1)
            {
                BallistaHandler.KogMaw.Spellbook.LevelSpell(SpellSlot.W);
            }
            Player.OnLevelUp += BallistaHandler.LevelerMode;
            /*Gapcloser.OnGapCloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;*/
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            BallistaHandler.DrawMode();
        }

        static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                BallistaHandler.ComboMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                BallistaHandler.HarassMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                BallistaHandler.JungleMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                BallistaHandler.LaneClearMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                BallistaHandler.LastHitMode();
            }
            if (Program.SettingMenu["KSmode"].Cast<CheckBox>().CurrentValue)
            {
                BallistaHandler.KSMode();
            }
            if (Program.SettingMenu["Stackmode"].Cast<CheckBox>().CurrentValue)
            {
                BallistaHandler.StackMode();
            }
            if (Sliders.Heathcall.CurrentValue > 0 && Sliders.Heathcall.CurrentValue < 100)
            {
                BallistaHandler.HealthPotionMode();
            }
            if (Sliders.Manacall.CurrentValue > 0 && Sliders.Manacall.CurrentValue < 100)
            {
                BallistaHandler.ManaPotionMode();
            }
            if ((Sliders.FlaskHcall.CurrentValue > 0 && Sliders.FlaskHcall.CurrentValue < 100) 
                 || (Sliders.FlaskMcall.CurrentValue > 0 && Sliders.FlaskMcall.CurrentValue < 100))
            {
                BallistaHandler.CrystallineFlaskMode();
            }
            if (Sliders.Healcall.CurrentValue > 0 && Sliders.Healcall.CurrentValue < 100)
            {
                BallistaHandler.HealMode();
            }
            if (Ignite != null)
            {
                if (Program.SettingMenu["Ignitemode"].Cast<CheckBox>().CurrentValue)
                {
                    BallistaHandler.IgniteMode();
                }
            }
            BallistaHandler.GetRrange();
            BallistaHandler.DeathFollowMode();
        }
    }
}
