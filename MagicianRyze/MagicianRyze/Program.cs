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

namespace MagicianRyze
{
    /* Created by Counter */

    /* To Do:
        
        * Ignite + Smite input
        * don't Q Wind Wall/Unbreakable/BlackShield
         
    */
    class Program
    {
        /* Menus */
        public static Menu MagicianRyzeMenu, SettingMenu, DrawingMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu;
        // Skills
        public static Spell.Skillshot Q;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Active R;
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
            if (Player.Instance.ChampionName != "Ryze")
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear,250,1400,55);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);
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

            MagicianRyzeMenu = MainMenu.AddMenu("Magician Ryze", "MagicianRyze");
            MagicianRyzeMenu.AddGroupLabel("Magician Ryze");

            SettingMenu = MagicianRyzeMenu.AddSubMenu("Settings", "Settings");
            SettingMenu.AddGroupLabel("Settings");
            SettingMenu.AddSeparator();
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
            SettingMenu.AddSeparator();
            SettingMenu.AddLabel("Health Potion/Mana Potion/Crystalline Flask Activator - 0 is off");
            SettingMenu.Add("Healthcall", new Slider("Use Health Potion if Health %",25,0,100));
            SettingMenu.Add("Manacall", new Slider("Use Mana Potion if Mana %",25,0,100));
            SettingMenu.Add("FlaskHcall", new Slider("Use Crystalline Flask if Health %", 25, 0, 100));
            SettingMenu.Add("FlaskMcall", new Slider("Use Crystalline Flask if Mana %", 25, 0, 100));

            DrawingMenu = MagicianRyzeMenu.AddSubMenu("Drawing Features", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("Drawing Features");
            DrawingMenu.AddSeparator();
            DrawingMenu.Add("Qdraw", new CheckBox("Q"));
            DrawingMenu.Add("WEdraw", new CheckBox("W/E"));
            if (Ignite != null)
            {
                DrawingMenu.Add("Idraw", new CheckBox("Ignite"));
            }
            if (Smite != null)
            {
                DrawingMenu.Add("Sdraw", new CheckBox("Smite"));
            }

            ComboMenu = MagicianRyzeMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.AddSeparator();
            ComboMenu.Add("Qcombo", new CheckBox("Q"));
            ComboMenu.Add("Wcombo", new CheckBox("W"));
            ComboMenu.Add("Ecombo", new CheckBox("E"));
            ComboMenu.Add("Rcombo", new CheckBox("Use R with rooting"));
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Seraph's Embrace Activation - 0 is Off");
            ComboMenu.Add("Seraphscall", new Slider("Seraph's Embrace if Health %", 25, 0, 100));

            HarassMenu = MagicianRyzeMenu.AddSubMenu("Harass Features", "HarassFeatures");
            HarassMenu.AddGroupLabel("Harass Features");
            HarassMenu.AddSeparator();
            HarassMenu.Add("Qharass", new CheckBox("Q"));

            JungleMenu = MagicianRyzeMenu.AddSubMenu("Jungle Features", "JungleFeatures");
            JungleMenu.AddGroupLabel("Jungle Features");
            JungleMenu.AddSeparator();
            JungleMenu.Add("Qjungle", new CheckBox("Q"));
            JungleMenu.Add("Wjungle", new CheckBox("W"));
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

            LaneClearMenu = MagicianRyzeMenu.AddSubMenu("Lane Clear Features", "LaneClearFeatures");
            LaneClearMenu.AddGroupLabel("Lane Clear Features");
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("Qlanec", new CheckBox("Q"));
            LaneClearMenu.Add("Wlanec", new CheckBox("W"));
            LaneClearMenu.Add("Elanec", new CheckBox("E"));
            

            LastHitMenu = MagicianRyzeMenu.AddSubMenu("Last Hit Features", "LastHitFeatures");
            LastHitMenu.AddGroupLabel("Last Hit Features");
            LastHitMenu.AddSeparator();
            LastHitMenu.Add("Qlasthit", new CheckBox("Q"));
            LastHitMenu.Add("Wlasthit", new CheckBox("W"));
            
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            if (MagicianHandler.Ryze.Level == 1)
            {
                MagicianHandler.Ryze.Spellbook.LevelSpell(SpellSlot.Q);
            }
            Player.OnLevelUp += MagicianHandler.LevelerMode;
            /*Gapcloser.OnGapCloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;*/

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            MagicianHandler.DrawMode();
        }

        static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                MagicianHandler.ComboMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                MagicianHandler.HarassMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                MagicianHandler.JungleMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                MagicianHandler.LaneClearMode();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                MagicianHandler.LastHitMode();
            }
            if (Program.SettingMenu["KSmode"].Cast<CheckBox>().CurrentValue)
            {
                MagicianHandler.KSMode();
            }
            if (Program.SettingMenu["Stackmode"].Cast<CheckBox>().CurrentValue)
            {
                MagicianHandler.StackMode();
            }
            if (Program.SettingMenu["Healthcall"].Cast<Slider>().CurrentValue > 0)
            {
                MagicianHandler.HealthPotionMode();
            }
            if (Program.SettingMenu["Manacall"].Cast<Slider>().CurrentValue > 0)
            {
                MagicianHandler.ManaPotionMode();
            }
            if (Program.SettingMenu["FlaskHcall"].Cast<Slider>().CurrentValue > 0 || Program.SettingMenu["FlaskMcall"].Cast<Slider>().CurrentValue > 0)
            {
                MagicianHandler.CrystallineFlaskMode();
            }
            if (Ignite != null)
            {
                if (Program.SettingMenu["Ignitemode"].Cast<CheckBox>().CurrentValue)
                {
                    MagicianHandler.IgniteMode();
                }
            }
            if (Smite != null)
            {
                if (Program.SettingMenu["Smitemode"].Cast<CheckBox>().CurrentValue)
                {
                    MagicianHandler.SmiteMode();
                }
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
