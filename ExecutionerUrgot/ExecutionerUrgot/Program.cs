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
        
        * W shield logic needed
        * Fix Smite Modes
        * don't Q/E Wind Wall/BlackShield
        * don't E Unbreakable
         
    */
    class Program
    {
        /* Menus */
        public static Menu ExecutionerUrgotMenu, SettingMenu, DrawingMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu;
        // Skills
        public static Spell.Skillshot Q;
        public static Spell.Active Q2;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        public static Spell.Targeted Ignite;
        public static Spell.Targeted Smite;
        private static string[] Smites = new[] { "summonersmite", "itemsmiteaoe", "s5_summonersmiteplayerganker", "s5_summonersmitequick", "s5_summonersmiteduel" };

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
            if (Player.GetSpell(SpellSlot.Summoner1).Name == "summonerdot")
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            }
            if (Player.GetSpell(SpellSlot.Summoner2).Name == "summonerdot")
            {
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);
            }
            if (Smites.Contains(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name))
            {
                Smite = new Spell.Targeted(SpellSlot.Summoner1, 500);
            }
            if (Smites.Contains(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name))
            {
                Smite = new Spell.Targeted(SpellSlot.Summoner2, 500);
            }

            ExecutionerUrgotMenu = MainMenu.AddMenu("Executioner Urgot", "ExecutionerUrgot");
            ExecutionerUrgotMenu.AddGroupLabel("Executioner Urgot");

            SettingMenu = ExecutionerUrgotMenu.AddSubMenu("Settings", "Settings");
            SettingMenu.AddGroupLabel("Settings");
            SettingMenu.AddSeparator();
            SettingMenu.Add("Drawmode", new CheckBox("Drawing Mode"));
            SettingMenu.Add("KSmode", new CheckBox("KS Mode"));
            SettingMenu.Add("Stackmode", new CheckBox("Stack Tear Mode"));
            if (Ignite != null)
            {
                SettingMenu.Add("Ignitemode", new CheckBox("Auto Ignite"));
            }
            if (Smite != null)
            {
                SettingMenu.Add("Smitemode", new CheckBox("Auto Smite"));
                SettingMenu.Add("KSsmite", new CheckBox("Smite KS"));
            }
            SettingMenu.AddLabel("Auto R - While under turret, use R to grab enemy.");
            SettingMenu.Add("Grabmode", new CheckBox("Auto R Mode"));
            SettingMenu.AddSeparator();
            SettingMenu.AddLabel("Health Potion/Mana Potion/Crystalline Flask Activator - 0 is off");
            SettingMenu.Add("Healthcall", new Slider("Use Health Potion if Health %", 25, 0, 100));
            SettingMenu.Add("Manacall", new Slider("Use Mana Potion if Mana %", 25, 0, 100));
            SettingMenu.Add("FlaskHcall", new Slider("Use Crystalline Flask if Health %", 25, 0, 100));
            SettingMenu.Add("FlaskMcall", new Slider("Use Crystalline Flask if Mana %", 25, 0, 100));

            DrawingMenu = ExecutionerUrgotMenu.AddSubMenu("Drawing Features", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("Drawing Features");
            DrawingMenu.AddSeparator();
            DrawingMenu.Add("Qdraw", new CheckBox("Q"));
            DrawingMenu.Add("Edraw", new CheckBox("E"));
            DrawingMenu.Add("Rdraw", new CheckBox("R"));
            if (Ignite != null)
            {
                DrawingMenu.Add("Idraw", new CheckBox("Ignite"));
            }
            if (Smite != null)
            {
                DrawingMenu.Add("Sdraw", new CheckBox("Smite"));
            }

            ComboMenu = ExecutionerUrgotMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.AddSeparator();
            ComboMenu.Add("Qcombo", new CheckBox("Q"));
            /* ComboMenu.Add("Wcombo", new CheckBox("W")); */
            ComboMenu.Add("Ecombo", new CheckBox("E"));
            ComboMenu.Add("Rcombo", new CheckBox("R"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Muramana Activation - 0 is Off");
            ComboMenu.Add("Muracall", new Slider("Muramana - Current Mana Limiter", 50, 0, 100));

            HarassMenu = ExecutionerUrgotMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.AddSeparator();
            HarassMenu.Add("Qharass", new CheckBox("Q"));

            JungleMenu = ExecutionerUrgotMenu.AddSubMenu("Jungle Features", "JungleFeatures");
            JungleMenu.AddGroupLabel("Jungle Features");
            JungleMenu.AddSeparator();
            JungleMenu.Add("Qjungle", new CheckBox("Q"));
            JungleMenu.Add("Ejungle", new CheckBox("E"));
            if (Smite != null)
            {
                JungleMenu.AddGroupLabel("Smite Features");
                JungleMenu.AddLabel("Summoner's Rift Camps");
                JungleMenu.Add("Bluesmite", new CheckBox("Blue Sentinel"));
                JungleMenu.Add("Redsmite", new CheckBox("Red Brambleback"));
                JungleMenu.Add("Krugsmite", new CheckBox("Ancient Krug"));
                JungleMenu.Add("Grompsmite", new CheckBox("Gromp"));
                JungleMenu.Add("Murksmite", new CheckBox("Greater Murk Wolf"));
                JungleMenu.Add("Birdsmite", new CheckBox("Crimson Raptor"));
                JungleMenu.Add("Crabsmite", new CheckBox("Rift Scuttler"));
                JungleMenu.Add("Dragonsmite", new CheckBox("Dragon"));
                JungleMenu.Add("Baronsmite", new CheckBox("Baron Nashor"));
                JungleMenu.AddLabel("Twisted Treeline Camps");
                JungleMenu.Add("Golemsmite", new CheckBox("Big Golem"));
                JungleMenu.Add("Wolfsmite", new CheckBox("Giant Wolf"));
                JungleMenu.Add("Wraithsmite", new CheckBox("Wraith"));
                JungleMenu.Add("Spidersmite", new CheckBox("Vilemaw"));
            }

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
            if (ExecutionerHandler.Urgot.Level == 1)
            {
                ExecutionerHandler.Urgot.Spellbook.LevelSpell(SpellSlot.Q);
            }
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
            if (Program.SettingMenu["Grabmode"].Cast<CheckBox>().CurrentValue)
            {
                ExecutionerHandler.GrabMode();
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
            if (Ignite != null)
            {
                if (Program.SettingMenu["Ignitemode"].Cast<CheckBox>().CurrentValue)
                {
                    ExecutionerHandler.IgniteMode();
                }
            }
            if (Smite != null)
            {
                if (Program.SettingMenu["Smitemode"].Cast<CheckBox>().CurrentValue)
                {
                    ExecutionerHandler.SmiteMode();
                }
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
