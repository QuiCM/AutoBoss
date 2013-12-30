using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Boss
{
    public class CustomColours
    {
        public static Color comColor
        {
            get
            {
                Color c = Color.Green;

                foreach (message_Obj m in Boss_Tools.boss_Config.Message_Colours)
                {
                    if (m.type.ToLower() == "command" && m.useCustomColor)
                        c.R = m.red; c.G = m.green; c.B = m.blue;
                }

                return c;
            }
        }

        public static Color msgColor
        {
            get
            {
                Color c = Color.Yellow;

                foreach (message_Obj m in Boss_Tools.boss_Config.Message_Colours)
                {
                    if (m.type.ToLower() == "message" && m.useCustomColor)
                        c.R = m.red; c.G = m.green; c.B = m.blue;
                }

                return c;
            }
        }

        public static Color dayColor
        {
            get
            {
                Color c = Color.OrangeRed;

                foreach (message_Obj m in Boss_Tools.boss_Config.Message_Colours)
                {
                    if (m.type.ToLower() == "day" && m.useCustomColor)
                        c.R = m.red; c.G = m.green; c.B = m.blue;
                }

                return c;
            }
        }

        public static Color nightColor
        {
            get
            {
                Color c = Color.RoyalBlue;

                foreach (message_Obj m in Boss_Tools.boss_Config.Message_Colours)
                {
                    if (m.type.ToLower() == "night" && m.useCustomColor)
                        c.R = m.red; c.G = m.green; c.B = m.blue;
                }

                return c;
            }
        }

        public static Color specColor
        {
            get
            {
                Color c = Color.MediumVioletRed;

                foreach (message_Obj m in Boss_Tools.boss_Config.Message_Colours)
                {
                    if (m.type.ToLower() == "special" && m.useCustomColor)
                        c.R = m.red; c.G = m.green; c.B = m.blue;
                }

                return c;
            }
        }

        public static Color minionColor
        {
            get
            {
                Color c = Color.LightGreen;

                foreach (message_Obj m in Boss_Tools.boss_Config.Message_Colours)
                {
                    if (m.type.ToLower() == "minion" && m.useCustomColor)
                        c.R = m.red; c.G = m.green; c.B = m.blue;
                }

                return c;
            }
        }

        public static Color bossColor
        {
            get
            {
                Color c = Color.Salmon;

                foreach (message_Obj m in Boss_Tools.boss_Config.Message_Colours)
                {
                    if (m.type.ToLower() == "boss" && m.useCustomColor)
                        c.R = m.red; c.G = m.green; c.B = m.blue;
                }

                return c;
            }
        }
    }
}
