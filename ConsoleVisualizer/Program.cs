using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using DiscordRPC;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WinformsVisualization.Visualization;

namespace ConsoleVisualizer
{
    class Program
    {
        // Heared you in the rain
        // Will you always be there?
        // The last thing you said
        // Sorry if it`s too late

        private static bool LyricsExist = false;
        private static bool TagsExist = false;
        private static ISoundOut _soundOut;
        private static IWaveSource _source;
        private static LineSpectrum _lineSpectrum;
        private static string Filename = "";
        private static TagLib.File TaglibFile;
        private static Stopwatch stopwatch = new Stopwatch();
        private static Random random = new Random();
        private static DiscordRpcClient discord = new DiscordRpcClient("637644971716902933");

        private static void SetupSampleSource(ISampleSource aSampleSource)
        {
            const FftSize fftSize = FftSize.Fft4096;
            var spectrumProvider = new BasicSpectrumProvider(aSampleSource.WaveFormat.Channels,
                aSampleSource.WaveFormat.SampleRate, fftSize);
            _lineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = Console.WindowWidth,
                BarSpacing = 1,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt
            };

            var notificationSource = new SingleBlockNotificationStream(aSampleSource);
            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

            _source = notificationSource.ToWaveSource(16);
        }

        private static LyrLibMusicLyrics Lyrics = new LyrLibMusicLyrics();

        private static bool IsOutputBusy = false;

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private static void OpenFileToPlay(string filename)
        {
            ISampleSource source = CodecFactory.Instance.GetCodec(filename)
                    .ToSampleSource();

            SetupSampleSource(source);

            _soundOut = new WasapiOut();
            _soundOut.Initialize(_source);
            _soundOut.Play();

            if (File.Exists(Path.Combine(new FileInfo(filename).DirectoryName, new FileInfo(filename).Name.Replace(".mp3", "") + ".lyrlib")))
                LyricsExist = Lyrics.TryLoadFromFile(Path.Combine(new FileInfo(filename).DirectoryName, new FileInfo(filename).Name.Replace(".mp3", "") + ".lyrlib"));
            Filename = filename;
            TaglibFile = TagLib.File.Create(Filename);
            if (TaglibFile.Tag.Title != null && TaglibFile.Tag.Title != "")
                TagsExist = true;
            stopwatch.Start();
            discord.SetPresence(new RichPresence()
            {
                Details = Lyrics.Artist + " - " + Lyrics.Name,
                State = "Listening",
                Assets = new Assets()
                {
                    LargeImageKey = "cat"
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.Now
                }
            });
        }

        private static void OpenFileToPlay()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = CodecFactory.SupportedFilesFilterEn,
                Title = "Выбери любой файл с музыкой..."
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ISampleSource source = CodecFactory.Instance.GetCodec(openFileDialog.FileName)
                    .ToSampleSource();

                SetupSampleSource(source);

                _soundOut = new WasapiOut();
                _soundOut.Initialize(_source);
                _soundOut.Play();

                if (File.Exists(Path.Combine(new FileInfo(openFileDialog.FileName).DirectoryName, new FileInfo(openFileDialog.FileName).Name.Replace(".mp3", "") + ".lyrlib")))
                {
                    LyricsExist = Lyrics.TryLoadFromFile(Path.Combine(new FileInfo(openFileDialog.FileName).DirectoryName, new FileInfo(openFileDialog.FileName).Name.Replace(".mp3", "") + ".lyrlib"));
                }
                Filename = openFileDialog.FileName;
                TaglibFile = TagLib.File.Create(Filename);
                if (TaglibFile.Tag.Title != null && TaglibFile.Tag.Title != "")
                    TagsExist = true;
                stopwatch.Start();
                discord.SetPresence(new RichPresence()
                {
                    Details = Lyrics.Artist + " - " + Lyrics.Name,
                    State = "Listening",
                    Assets = new Assets()
                    {
                        LargeImageKey = "cat"
                    }
                });
            }
        }

        private static TimeSpan lastLyric = new TimeSpan();
        private static Size size = new Size(Console.WindowWidth - 2, 20);
        private static int leftPos = 0;

        private static void DrawPlayerLayer()
        {
            if (LyricsExist)
            {
                Thread.Sleep(1);
                TimeSpan span = stopwatch.Elapsed;
                foreach (var key in Lyrics.LyricsTimecodes.Keys)
                {
                    if (Math.Abs(span.TotalMilliseconds - key.TotalMilliseconds) < 50 && lastLyric != key)
                    {
                        IsOutputBusy = true;
                        if (lastLyric != new TimeSpan())
                        {
                            Console.SetCursorPosition(2, 2);
                            ClearCurrentConsoleLine();
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(" " + Lyrics.LyricsTimecodes[lastLyric]);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                        }
                        {
                            Console.SetCursorPosition(2, 3);
                            ClearCurrentConsoleLine();
                            Console.Write(" " + Lyrics.LyricsTimecodes[key]);
                            Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                        }
                        {
                            Console.SetCursorPosition(2, 4);
                            ClearCurrentConsoleLine();
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            try
                            {
                                Console.Write(" " + Lyrics.LyricsTimecodes[Lyrics.GetNextTimecodeByTime(span.Add(TimeSpan.FromMilliseconds(50)))]);
                            }
                            catch (Exception ex) { }
                            leftPos = Console.CursorLeft;
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                        }
                        {
                            Console.SetCursorPosition(2, 6);
                            ClearCurrentConsoleLine();
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(" > Song: ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write(Lyrics.Name);
                            Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                        }
                        {
                            Console.SetCursorPosition(2, 7);
                            ClearCurrentConsoleLine();
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(" > Artist: ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write(Lyrics.Artist + " ");
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write("(Album: " + Lyrics.Album + ")");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                        }
                        IsOutputBusy = false;
                        lastLyric = key;
                    }
                }

                if (leftPos != 0)
                {
                    IsOutputBusy = true;
                    Console.SetCursorPosition(leftPos, 4);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("          ");
                    Console.SetCursorPosition(leftPos, 4);
                    Console.Write(" (" + Math.Truncate(Math.Abs(Lyrics.GetNextTimecodeByTime(span.Add(TimeSpan.FromMilliseconds(50))).TotalMilliseconds - span.TotalMilliseconds)) + "ms)");
                    IsOutputBusy = false;
                }
            }
            else if (TagsExist)
            {
                {
                    Console.SetCursorPosition(2, 2);
                    ClearCurrentConsoleLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" > Song: ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(TaglibFile.Tag.Title);
                    Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                }
                {
                    Console.SetCursorPosition(2, 3);
                    ClearCurrentConsoleLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" > Artist: ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(TaglibFile.Tag.FirstPerformer + " ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("(Album: " + TaglibFile.Tag.Album + ")");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                }
            }
            if (LyricsExist)
                size = new Size(Console.WindowWidth - 2, Console.WindowHeight - 10);
            else if (TagsExist)
                size = new Size(Console.WindowWidth - 2, Console.WindowHeight - 6);
            else
                size = new Size(Console.WindowWidth - 2, Console.WindowHeight - 3);
            var spectrumDataConsole = _lineSpectrum.CreateSpectrumLine(size);
            if (spectrumDataConsole == null)
                return;
            float fftSum = 0;
            foreach (int line in spectrumDataConsole)
                fftSum += line;
            Console.SetCursorPosition(0, Console.WindowHeight - size.Height);
            for (int i = 0; i < size.Height; i++)
            {
                Console.Write(' ');
                for (int j = 0; j < size.Width; j++)
                {
                    if (spectrumDataConsole[j] > i)
                    {
                        Console.Write(' ');
                    }
                    else if (spectrumDataConsole[j] == i)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('▌');
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.Write('▌');
                    }

                }
                if (!IsOutputBusy)
                    Console.WriteLine();
            }
            
        }

        static bool IsKeyPressed(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }

        private static bool EnableLastHighlight = false;

        static void DrawHighlight(bool enable)
        {
            if (enable)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                if (!EnableLastHighlight)
                    Console.Clear();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                if (EnableLastHighlight)
                    Console.Clear();
            }
            EnableLastHighlight = enable;
        }

        static void OpenFileAndPlay(string file = "", string joinSecret = "")
        {
            Console.Clear();
            if (file != "")
                OpenFileToPlay(file);
            else
                OpenFileToPlay();
            if (!LyricsExist)
            {
                RichPresence presence = new RichPresence()
                {
                    Details = new FileInfo(Filename).Name,
                    State = "Listening",
                    Assets = new Assets()
                    {
                        LargeImageKey = "cat"
                    }
                };
                if (joinSecret != "")
                {
                    presence.Secrets = new Secrets()
                    {
                        JoinSecret = joinSecret
                    };
                    presence.Party = new Party()
                    {
                        ID = Secrets.CreateFriendlySecret(new Random()),
                        Size = 1,
                        Max = 1338
                    };
                    discord.SetSubscription(EventType.Join);
                }
                presence.Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow.AddMilliseconds(_source.GetMilliseconds(_source.Length))
                };
                discord.SetPresence(presence);
            }
            Console.CursorVisible = false;
            while (_soundOut.PlaybackState != PlaybackState.Stopped)
            {
                DrawPlayerLayer();
                if (IsKeyPressed(Key.P) && IsKeyPressed(Key.LeftCtrl) && IsKeyPressed(Key.LeftShift))
                {
                    if (_soundOut.PlaybackState == PlaybackState.Playing)
                    {
                        _soundOut.Pause();
                        stopwatch.Stop();
                        var presence = discord.CurrentPresence;
                        presence.State = "Paused";
                        discord.SetPresence(presence);
                    }
                    else if (_soundOut.PlaybackState == PlaybackState.Paused)
                    {
                        _soundOut.Play();
                        stopwatch.Start();
                        var presence = discord.CurrentPresence;
                        presence.State = "Listening";
                        discord.SetPresence(presence);
                    }
                    Thread.Sleep(100);
                }
                if (IsKeyPressed(Key.S) && IsKeyPressed(Key.LeftCtrl) && IsKeyPressed(Key.LeftShift))
                {
                    _soundOut.Stop();
                }
                DrawHighlight(IsKeyPressed(Key.RightShift));
            }
            Console.Clear();
        }

        static string[] Menu =
        {
            "Open File",
            "Download from SoundCloud (works in 90% cases)",
            "Settings"
        };
        static int SelectedMenuItem = 0;

        static void DrawSoundCloud()
        {
            Console.Clear();
            Console.Write("Paste SoundCloud song link (right-click): ");
            var link = Console.ReadLine();
            Directory.CreateDirectory("soundcloud");
            MessageBox.Show("Join Secret Debug", link.Replace("https://soundcloud.com/", "[SOUNDCLOUD]#"));
            OpenFileAndPlay(SCDownload.Downloader.DownloadSong(link, "soundcloud"), link.Replace("https://soundcloud.com/", "[SOUNDCLOUD]#"));
        }

        static void DrawSettings()
        {
            while (true)
            {
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            RichPresence presence = new RichPresence()
            {
                Details = "Idle",
                State = ""
            };
            discord.OnJoin += OnJoin;
            discord.OnJoinRequested += OnJoinRequested;
            discord.Initialize();
            discord.RegisterUriScheme();
            discord.Logger = new DiscordRPC.Logging.FileLogger("discord-rpc.log", DiscordRPC.Logging.LogLevel.Trace);
            discord.SetPresence(presence);
            Console.OutputEncoding = Encoding.UTF8;
            if (args.Length > 0)
                OpenFileAndPlay(args[0]);
            else
                while (true)
                {
                    SelectedMenuItem %= Menu.Length;
                    for (int itemNum = 0; itemNum < Menu.Length; itemNum++)
                    {
                        Console.SetCursorPosition(1, itemNum + 1);
                        if (itemNum == SelectedMenuItem)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(Menu[itemNum]);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.WriteLine(Menu[itemNum]);
                        }
                    }
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (SelectedMenuItem == 0)
                            OpenFileAndPlay();
                        if (SelectedMenuItem == 1)
                            DrawSoundCloud();
                        if (SelectedMenuItem == 2)
                            DrawSettings();
                    }
                    if (key.Key == ConsoleKey.DownArrow)
                        SelectedMenuItem++;
                    if (key.Key == ConsoleKey.UpArrow)
                        SelectedMenuItem--;
                }
        }
        private static void OnJoin(object sender, JoinMessage args)
        {
            MessageBox.Show(args.Secret, "Ready to play!");
            Console.Clear();
            Console.WriteLine("Starting...");
            var link = args.Secret.Replace("[SOUNDCLOUD]#", "https://soundcloud.com/");
            Directory.CreateDirectory("soundcloud");
            OpenFileAndPlay(SCDownload.Downloader.DownloadSong(link, "soundcloud"), link.Replace("https://soundcloud.com/", "[SOUNDCLOUD]#"));
        }

        private static void OnJoinRequested(object sender, JoinRequestMessage args)
        {
            DiscordRpcClient client = (DiscordRpcClient)sender;
            client.Respond(args, true);
        }
    }
}
