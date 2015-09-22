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
        
        * Fix LastHit
        * Ult need to work off snare
        * 
         
    */
    class Program
    {
        /* Menus */
        public static Menu MagicianRyzeMenu, SettingMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu;
        // Skills
        public static Spell.Skillshot Q;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Active R;

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

            MagicianRyzeMenu = MainMenu.AddMenu("Magician Ryze", "MagicianRyze");
            MagicianRyzeMenu.AddGroupLabel("Magician Ryze");
            MagicianRyzeMenu.AddSeparator();

            SettingMenu = MagicianRyzeMenu.AddSubMenu("Settings", "Settings");
            SettingMenu.AddGroupLabel("Settings");
            SettingMenu.AddSeparator();
            SettingMenu.Add("Drawmode", new CheckBox("Drawing Mode"));
            SettingMenu.Add("KSmode", new CheckBox("KS Mode"));
            SettingMenu.Add("Stackmode", new CheckBox("Stack Tear Mode"));
            SettingMenu.AddSeparator();
            SettingMenu.Add("Autopot_H", new Slider("Use Health Potion (%)",25,0,100));
            SettingMenu.Add("Autopot_M", new Slider("Use Mana Potion (%)",25,0,100));

            ComboMenu = MagicianRyzeMenu.AddSubMenu("Combo Features", "ComboFeatures");
            ComboMenu.AddGroupLabel("Combo Features");
            ComboMenu.AddSeparator();
            ComboMenu.Add("Qcombo", new CheckBox("Q"));
            ComboMenu.Add("Wcombo", new CheckBox("W"));
            ComboMenu.Add("Ecombo", new CheckBox("E"));
            ComboMenu.Add("Rcombo", new CheckBox("Use R with rooting"));

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
    }
}
