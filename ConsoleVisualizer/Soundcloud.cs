using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCDownload
{
    public class Requests
    {
        public string MakeRequest(string args)
        {
            var client = new RestClient("https://soundcloud.com/");
            client.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Safari/605.1.15";
            client.FollowRedirects = true;
            var request = new RestRequest(args)
                .AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9")
                .AddHeader("Accept-Encoding", "gzip, deflate, br")
                .AddHeader("Accept-Language", "en-US,en;q=0.9");
            string response = client.Get(request).Content;
            return response;
        }
    }
    public class Product
    {
        public string id { get; set; }
    }

    public class CreatorSubscription
    {
        public Product product { get; set; }
    }

    public class Product2
    {
        public string id { get; set; }
    }

    public class CreatorSubscription2
    {
        public Product2 product { get; set; }
    }

    public class Visual
    {
        public string urn { get; set; }
        public int entry_time { get; set; }
        public string visual_url { get; set; }
    }

    public class Visuals
    {
        public string urn { get; set; }
        public bool enabled { get; set; }
        public List<Visual> visuals { get; set; }
        public object tracking { get; set; }
    }

    public class Format
    {
        public string protocol { get; set; }
        public string mime_type { get; set; }
    }

    public class Transcoding
    {
        public string url { get; set; }
        public string preset { get; set; }
        public int duration { get; set; }
        public bool snipped { get; set; }
        public Format format { get; set; }
        public string quality { get; set; }
    }

    public class Media
    {
        public List<Transcoding> transcodings { get; set; }
    }

    public class PublisherMetadata
    {
        public string p_line_for_display { get; set; }
        public string artist { get; set; }
        public string isrc { get; set; }
        public string c_line { get; set; }
        public string upc_or_ean { get; set; }
        public string p_line { get; set; }
        public string urn { get; set; }
        public bool @explicit { get; set; }
        public string c_line_for_display { get; set; }
        public bool contains_music { get; set; }
        public int id { get; set; }
        public string album_title { get; set; }
        public string release_title { get; set; }
    }
    public class User
    {
        public string avatar_url { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public int id { get; set; }
        public string kind { get; set; }
        public DateTime last_modified { get; set; }
        public string last_name { get; set; }
        public string permalink { get; set; }
        public string permalink_url { get; set; }
        public string uri { get; set; }
        public string urn { get; set; }
        public string username { get; set; }
        public bool verified { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
    }
    public class Track
    {
        public int comment_count { get; set; }
        public int full_duration { get; set; }
        public bool downloadable { get; set; }
        public DateTime created_at { get; set; }
        public object description { get; set; }
        public Media media { get; set; }
        public string title { get; set; }
        public PublisherMetadata publisher_metadata { get; set; }
        public int duration { get; set; }
        public bool has_downloads_left { get; set; }
        public string artwork_url { get; set; }
        public string stream_url { get; set; }
        public bool @public { get; set; }
        public bool streamable { get; set; }
        public string tag_list { get; set; }
        public string genre { get; set; }
        public int id { get; set; }
        public int reposts_count { get; set; }
        public string state { get; set; }
        public string label_name { get; set; }
        public DateTime last_modified { get; set; }
        public bool commentable { get; set; }
        public string policy { get; set; }
        public object visuals { get; set; }
        public string kind { get; set; }
        public object purchase_url { get; set; }

        public string sharing { get; set; }
        public string uri { get; set; }
        public object secret_token { get; set; }
        public int download_count { get; set; }
        public int likes_count { get; set; }
        public string urn { get; set; }
        public string license { get; set; }
        public object purchase_title { get; set; }
        public DateTime display_date { get; set; }
        public string embeddable_by { get; set; }
        public DateTime? release_date { get; set; }
        public int user_id { get; set; }
        public string monetization_model { get; set; }
        public string waveform_url { get; set; }
        public string permalink { get; set; }
        public string permalink_url { get; set; }
        public User user { get; set; }
        public int playback_count { get; set; }
    }
    public class Product3
    {
        public string id { get; set; }
    }
    public class CreatorSubscription3
    {
        public Product3 product { get; set; }
    }
    public class Product4
    {
        public string id { get; set; }
    }
    public class CreatorSubscription4
    {
        public Product4 product { get; set; }
    }
    public class Visual2
    {
        public string urn { get; set; }
        public int entry_time { get; set; }
        public string visual_url { get; set; }
    }
    public class Visuals2
    {
        public string urn { get; set; }
        public bool enabled { get; set; }
        public List<Visual2> visuals { get; set; }
        public object tracking { get; set; }
    }
    public class User2
    {
        public string avatar_url { get; set; }
        public string city { get; set; }
        public int comments_count { get; set; }
        public string country_code { get; set; }
        public DateTime created_at { get; set; }
        public List<CreatorSubscription3> creator_subscriptions { get; set; }
        public CreatorSubscription4 creator_subscription { get; set; }
        public string description { get; set; }
        public int followers_count { get; set; }
        public int followings_count { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public int groups_count { get; set; }
        public int id { get; set; }
        public string kind { get; set; }
        public DateTime last_modified { get; set; }
        public string last_name { get; set; }
        public int likes_count { get; set; }
        public int playlist_likes_count { get; set; }
        public string permalink { get; set; }
        public string permalink_url { get; set; }
        public int playlist_count { get; set; }
        public object reposts_count { get; set; }
        public int track_count { get; set; }
        public string uri { get; set; }
        public string urn { get; set; }
        public string username { get; set; }
        public bool verified { get; set; }
        public Visuals2 visuals { get; set; }
    }
    public class Datum
    {
        public bool allows_messages_from_unfollowed_users { get; set; }
        public bool analytics_opt_in { get; set; }
        public bool communications_opt_in { get; set; }
        public bool targeted_advertising_opt_in { get; set; }
        public List<object> legislation { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public bool? v2_oscp_german_tax_fields_support { get; set; }
        public bool? v2_use_new_connect { get; set; }
        public bool? v2_hq_file_storage_release { get; set; }
        public bool? at_mentions { get; set; }
        public bool? v2_direct_support_link { get; set; }
        public bool? v2_distro_2_phase_1_announcement { get; set; }
        public bool? v2_monetization_fi_dk_se_no { get; set; }
        public string avatar_url { get; set; }
        public int? comments_count { get; set; }
        public DateTime? created_at { get; set; }
        public List<CreatorSubscription> creator_subscriptions { get; set; }
        public CreatorSubscription2 creator_subscription { get; set; }
        public string description { get; set; }
        public int? followers_count { get; set; }
        public int? followings_count { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public int? groups_count { get; set; }
        public int? id { get; set; }
        public string kind { get; set; }
        public DateTime? last_modified { get; set; }
        public string last_name { get; set; }
        public int? likes_count { get; set; }
        public int? playlist_likes_count { get; set; }
        public string permalink { get; set; }
        public string permalink_url { get; set; }
        public int? playlist_count { get; set; }
        public int? reposts_count { get; set; }
        public int? track_count { get; set; }
        public string uri { get; set; }
        public string urn { get; set; }
        public string username { get; set; }
        public bool? verified { get; set; }
        public Visuals visuals { get; set; }
        public string url { get; set; }
        public int? duration { get; set; }
        public string genre { get; set; }
        public object purchase_url { get; set; }
        public object label_name { get; set; }
        public string tag_list { get; set; }
        public string set_type { get; set; }
        public bool? @public { get; set; }
        public int? user_id { get; set; }
        public string license { get; set; }
        public List<Track> tracks { get; set; }
        public DateTime? release_date { get; set; }
        public DateTime? display_date { get; set; }
        public string sharing { get; set; }
        public object secret_token { get; set; }
        public string title { get; set; }
        public object purchase_title { get; set; }
        public bool? managed_by_feeds { get; set; }
        public string artwork_url { get; set; }
        public bool? is_album { get; set; }
        public User2 user { get; set; }
        public DateTime? published_at { get; set; }
        public string embeddable_by { get; set; }
    }



    public class SetObject
    {
        public int id { get; set; }
        public List<int> chunks { get; set; }
        public List<Datum> data { get; set; }
    }
    public class SCJson
    {
        public int comment_count { get; set; }
        public bool downloadable { get; set; }
        public object release { get; set; }
        public string created_at { get; set; }
        public object description { get; set; }
        public int original_content_size { get; set; }
        public string title { get; set; }
        public object track_type { get; set; }
        public int duration { get; set; }
        public object video_url { get; set; }
        public string original_format { get; set; }
        public string artwork_url { get; set; }
        public bool streamable { get; set; }
        public string tag_list { get; set; }
        public int? release_month { get; set; }
        public string genre { get; set; }
        public int? release_day { get; set; }
        public string download_url { get; set; }
        public int id { get; set; }
        public string state { get; set; }
        public int reposts_count { get; set; }
        public string last_modified { get; set; }
        public string label_name { get; set; }
        public bool commentable { get; set; }
        public object bpm { get; set; }
        public int favoritings_count { get; set; }
        public string kind { get; set; }
        public object purchase_url { get; set; }
        public int? release_year { get; set; }
        public object key_signature { get; set; }
        public string isrc { get; set; }
        public string sharing { get; set; }
        public string uri { get; set; }
        public int download_count { get; set; }
        public string license { get; set; }
        public object purchase_title { get; set; }
        public int user_id { get; set; }
        public string embeddable_by { get; set; }
        public string waveform_url { get; set; }
        public string permalink { get; set; }
        public string permalink_url { get; set; }
        public User user { get; set; }
        public object label_id { get; set; }
        public string stream_url { get; set; }
        public int playback_count { get; set; }
    }
    public class TrackRippingObject
    {
        public int duration { get; set; }
        public int? release_day { get; set; }
        public string permalink_url { get; set; }
        public string genre { get; set; }
        public string permalink { get; set; }
        public object purchase_url { get; set; }
        public int? release_month { get; set; }
        public object description { get; set; }
        public string uri { get; set; }
        public object label_name { get; set; }
        public string tag_list { get; set; }
        public int? release_year { get; set; }
        public int track_count { get; set; }
        public int user_id { get; set; }
        public string last_modified { get; set; }
        public string license { get; set; }
        public List<Track> tracks { get; set; }
        public string playlist_type { get; set; }
        public int id { get; set; }
        public bool? downloadable { get; set; }
        public string sharing { get; set; }
        public string created_at { get; set; }
        public object release { get; set; }
        public string kind { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public object purchase_title { get; set; }
        public string artwork_url { get; set; }
        public object ean { get; set; }
        public bool streamable { get; set; }
        public User2 user { get; set; }
        public string embeddable_by { get; set; }
        public object label_id { get; set; }
    }

    public static class Downloader
    {
        public static string clientID = "4dd97a35cf647de595b918944aa6915d";
        public static string dir = "";
        public static double len = 0;

        public static string DownloadSong(string url, string path)
        {
            try
            {
                string t = new Requests().MakeRequest(url.Replace("https://soundcloud.com/", ""));
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(t);
                string id = doc.DocumentNode.SelectSingleNode("//meta[@property='twitter:app:url:googleplay']").Attributes["content"].Value.Replace("soundcloud://sounds:", "");
                SCJson json = JsonConvert.DeserializeObject<SCJson>(new WebClient().DownloadString("https://api.soundcloud.com/tracks/" + id + ".json?client_id=" + clientID));
                new WebClient().DownloadFile($"{json.stream_url}?client_id={clientID}", Path.Combine(path, $"{json.user.username} - {json.title}.mp3"));
                dir = Path.Combine(path, $"{Guid.NewGuid().ToString()}.jpg");
                TagLib.File file = TagLib.File.Create(Path.Combine(path, $"{json.user.username} - {json.title}.mp3"));
                file.Tag.Title = json.title.Substring(json.title.IndexOf(" - ") + 3);
                file.Tag.Performers = new[] { json.title.Substring(0, json.title.IndexOf(" - ")) };
                file.Tag.Album = file.Tag.Title + " - Single";
                file.Save();
                return Path.Combine(path, $"{json.user.username} - {json.title}.mp3");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Maybe, this song is undownloadable...");
                return "";
            }
        }

        public static string getFileSize(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            string result = string.Format("{0:0.##} {1}", len, sizes[order]);

            return result;
        }
    }
}
