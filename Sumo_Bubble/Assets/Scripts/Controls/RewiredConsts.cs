// <auto-generated>
// Rewired Constants
// This list was generated on 1/25/2025 11:41:53 AM
// The list applies to only the Rewired Input Manager from which it was generated.
// If you use a different Rewired Input Manager, you will have to generate a new list.
// If you make changes to the exported items in the Rewired Input Manager, you will
// need to regenerate this list.
// </auto-generated>

namespace RewiredConsts
{
    using UnityEngine;
    public static partial class Action {
        // Default
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Horiz")]
        public const int Horiz = 18;
        
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Depth")]
        public const int Depth = 19;
        
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Boost")]
        public const int Boost = 20;
        
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Pump")]
        public const int Pump = 21;
        
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Start")]
        public const int Start = 17;
        
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Back")]
        public const int Back = 22;

        //!@ Constant Rewired action names
        public static readonly Vector2 deadNorm = new Vector2(RW_deadMin_norm, RW_deadMax_norm);        //Deadzone for normal stuff (+-.1f)
        //public static readonly Vector2 deadThrot = new Vector2(RW_deadMin_throt, RW_deadMax_throt);     //Deadzone for flightstick throttle axis (.25f-.75f)
        //Min/Max of normal deadzone
        public const float RW_deadMin_norm = -.1f;
        public const float RW_deadMax_norm = .1f;
        //!@ Min/Max of throttle deadzone
        //public const float RW_deadMin_throt = .25f;
        //public const float RW_deadMax_throt = .75f;

        /// <summary>Types of controllerMap actions for the game (simple)</summary>
        public enum Actions
        {
            HORIZ,
            DEPTH,
            BOOST,
            PUMP,
            START,
            BACK,
            MAX = BACK
        }

        /// <summary>Types of controllerMap actions for the game (including +/- axises)</summary>
        public enum ActionsFull
        {
            HORIZ_M,
            HORIZ_P,
            DEPTH_M,
            DEPTH_P,
            BOOST,
            PUMP,
            START,
            BACK,
            MAX = BACK
        }

    }
    public static partial class Category {
        public const int Default = 0;
        
    }
    public static partial class Layout {
        public static partial class Joystick {
            public const int Default = 0;            
            public const int Player2 = 1;

            //!@ UPDATE ME!
            /// <summary>Types of gamepads supported by SSBB</summary>
            public enum Types
            {
                UNKNOWN,        //Unknown controller
                KBD,            //Keyboard
                
                CUSTOM,         //Custom
                CTRLR,          //Controller
                GPAD,           //Other gamepad (non dual-analog)

                XINPUT,         //XInput
                X360,           //XBox 360
                XBONE,          //XBox One                

                NINTENDO,       //Nintendo Pro controllers
                WIIU_PRO,       //Wii U Pro
                NX_PRO,         //Switch Pro
                MAX = NX_PRO
            }
        }
        public static partial class Keyboard {
            public const int Default = 0;
            
            public const int Player2 = 1;
            
        }
        public static partial class Mouse {
            public const int Default = 0;
            
        }
        public static partial class CustomController {
            public const int Default = 0;
            
        }
    }
    public static partial class Player {
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "System")]
        public const int System = 9999999;
        
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player0")]
        public const int Player0 = 0;
        
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player1")]
        public const int Player1 = 1;
        
    }
    public static partial class CustomController {
    }
    public static partial class LayoutManagerRuleSet {
    }
    public static partial class MapEnablerRuleSet {
    }
}
