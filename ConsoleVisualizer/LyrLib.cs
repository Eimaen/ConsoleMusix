using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleVisualizer
{
    class LyrLibMusicLyrics
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public Dictionary<TimeSpan, string> LyricsTimecodes { get; set; }

        public void AddLyricEntry(TimeSpan span, string text)
        {
            if (LyricsTimecodes == null)
                LyricsTimecodes = new Dictionary<TimeSpan, string>();

            LyricsTimecodes.Add(span, text);
        }

        public bool TryLoadFromFile(string file)
        {
            try
            {
                var fileContains = File.ReadAllText(file);
                LyrLibMusicLyrics lyrics = JsonConvert.DeserializeObject<LyrLibMusicLyrics>(fileContains);
                Name = lyrics.Name;
                Artist = lyrics.Artist;
                Album = lyrics.Album;
                LyricsTimecodes = lyrics.LyricsTimecodes;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TrySaveToFile(string file)
        {
            try
            {
                string content = JsonConvert.SerializeObject(this);
                File.WriteAllText(file, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Dictionary<TimeSpan, string> GetAllTimecodes()
        {
            return LyricsTimecodes;
        }

        public TimeSpan GetNextTimecodeByTime(TimeSpan time)
        {
            foreach (var lyricTime in LyricsTimecodes.Keys)
            {
                if (lyricTime > time)
                    return lyricTime;
            }
            return new TimeSpan();
        }
    }
}
