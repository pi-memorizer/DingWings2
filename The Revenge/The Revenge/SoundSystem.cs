using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Media;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace GameSystem
{
    class SoundSystem
    {
        static Dictionary<string, MediaPlayer> tracks = new Dictionary<string, MediaPlayer>();

        public static void load(string filename)
        {
            MediaPlayer m = new MediaPlayer();
            m.Open(new Uri("sound/" + filename + ".wav", UriKind.Relative));
            tracks.Add(filename, m);
        }

        public static void setLoop(string filename)
        {
            MediaPlayer m = tracks[filename];
            m.MediaEnded += mediaLoop;
        }

        static void mediaLoop(object sender, EventArgs e)
        {
            MediaPlayer player = sender as MediaPlayer;
            if (player == null)
                return;

            player.Position = new TimeSpan(0);
            player.Play();
        }

        public static void play(string filename)
        {
            MediaPlayer m = tracks[filename];
            m.Position = new TimeSpan(0);
            m.Play();
        }
    }
}
