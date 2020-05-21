using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleVisualizer
{
    class MusixmatchHeader
    {
        [JsonProperty("status_code")]
        public int StatusCode;
        [JsonProperty("execute_time")]
        public double ExecuteTime;
    }

    class MusixmatchTrackSubtitle
    {
        [JsonProperty("subtitle_id")]
        public int SubtitleId;
        [JsonProperty("restricted")]
        public int Restricted;
        [JsonProperty("subtitle_body")]
        public string SubtitleBodyJson;
        [JsonProperty("subtitle_avg_count")]
        public int SubtitleAverageCount;
        [JsonProperty("lyrics_copyright")]
        public string LyricsCopyright;
        [JsonProperty("subtitle_length")]
        public string SubtitleLength;
        [JsonProperty("subtitle_language")]
        public string SubtitleLanguage;
        [JsonProperty("subtitle_language_description")]
        public string SubtitleLanguageFull;
    }

    class MusixmatchTrackSubtitleData
    {
        [JsonProperty("subtitle")]
        public MusixmatchTrackSubtitle Subtitle;
    }

    class MusixmatchTrackSubtitlesBody
    {
        [JsonProperty("subtitle_list")]
        public List<MusixmatchTrackSubtitleData> SubtitleList;
    }

    class MusixmatchTrackSubtitlesMessage
    {
        [JsonProperty("message")]
        public MusixmatchTrackSubtitles Body;
    }

    class MusixmatchTrackSubtitles
    {
        [JsonProperty("header")]
        public MusixmatchHeader Header;
        [JsonProperty("body")]
        public MusixmatchTrackSubtitlesBody Body;
    }

    class MusixmatchMacroCalls
    {
        [JsonProperty("track.subtitles.get")]
        public MusixmatchTrackSubtitlesMessage Message;
    }

    class MusixmatchMessageBody
    {
        [JsonProperty("macro_calls")]
        public MusixmatchMacroCalls MacroCalls;
    }

    class MusixmatchMessageSongData
    {
        [JsonProperty("header")]
        public MusixmatchHeader Header;
        [JsonProperty("body")]
        public MusixmatchMessageBody Body;
    }

    class MusixmatchMacroSubtitlesGetRequest
    {
        [JsonProperty("message")]
        public MusixmatchMessageSongData SongData;
    }

    class MusixmatchSubtitleTimeJson
    {
        [JsonProperty("total")]
        public double Total;
        [JsonProperty("minutes")]
        public int Minutes;
        [JsonProperty("seconds")]
        public int Seconds;
        [JsonProperty("hundredths")]
        public int Hundredths;
    }

    class MusixmatchSubtitleDataJson
    {
        [JsonProperty("text")]
        public string Text;
        [JsonProperty("time")]
        public MusixmatchSubtitleTimeJson Time;
    }
}
