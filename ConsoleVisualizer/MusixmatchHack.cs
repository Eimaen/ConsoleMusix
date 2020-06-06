using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleVisualizer
{
    // Get music lyrics using Musixmatch desktop app protocol
    // I guess it will never be fixed.
    // It`s much better to say that it is a lifeHack, not a hack :)
    static class MusixmatchHack
    {
        public static string Get(string uri, CookieCollection cookie = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            request.AllowAutoRedirect = false;
            if (cookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookie);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == (HttpStatusCode)301)
                    return Get(uri, response.Cookies);
                else
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
            }
        }

        public static List<MusixmatchSubtitleDataJson> SearchLyrics(string song, string artist, string token)
        {
            string unparsedSongDataJson = Get("https://apic-desktop.musixmatch.com/ws/1.1/" +
                "macro.subtitles.get?format=json" +
                "&namespace=lyrics_synched" +
                "&q_album=" + song + " - Single" +
                "&q_artist=" + artist +
                "&q_artists=" + artist +
                "&q_track=" + song +
                "&tags=nowplaying" +
                "&subtitle_format=mxm" +
                "&user_language=en" +
                "&f_subtitle_length_max_deviation=1" +
                "&app_id=web-desktop-app-v1.0" +
                "&usertoken=" + token);
            try
            {
                var jsonSongDataParsed = JsonConvert.DeserializeObject<MusixmatchMacroSubtitlesGetRequest>(unparsedSongDataJson);
                var subtitleJson = jsonSongDataParsed.SongData.Body.MacroCalls.Message.Body.Body.SubtitleList[0].Subtitle.SubtitleBodyJson.Replace("\\", "");
                return JsonConvert.DeserializeObject<List<MusixmatchSubtitleDataJson>>(subtitleJson);
            }
            catch
            {
                return null;
            }
        }
    }
}
