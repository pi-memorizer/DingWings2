using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameSystem;

namespace The_Revenge
{
    static class Program
    {
        public static Game game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
            Player.getKeyCallback = getKey;
            game = new Game();
            if(Game.args==null) Game.args = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static Dictionary<string, bool> keys = new Dictionary<string, bool>();

        //names of keys in this list are tracked
        private static string[] keyKeys =
        {
            "p1Up","p1Down","p1Left","p1Right","p1A","p1B","p1Start","p1Select",
            "p2Up","p2Down","p2Left","p2Right","p2A","p2B","p2Start","p2Select"
        };

        public static void setKey(int key, bool value)
        {
            for (int i = 0; i < keyKeys.Length; i++)
            {
                string s = keyKeys[i];
                int sKey = Form1.config[s];
                if (key == sKey)
                {
                    if (keys.ContainsKey(s))
                        keys[s] = value;
                    else
                        keys.Add(s, value);
                    if (Game.getPlayer(s[1] - '0') == null)
                    {
                        Game.addPlayer(s[1] - '0');
                    }
                }
            }
        }

        public static bool getKey(string key)
        {
            bool a;
            if (keys.TryGetValue(key, out a))
                return a;
            else
                return false;
        }
    }
}
