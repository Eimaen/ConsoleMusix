using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using DiscordRPC;
using DiscordRPC.Message;
using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using VideoLibrary;
using WinformsVisualization.Visualization;

namespace ConsoleVisualizer
{
    using System;
    using System.Resources;
    using System.Runtime.InteropServices;
    using static ConsoleVisualizer.NativeMethods;

    public static class ConsoleListener
    {
        public static event ConsoleMouseEvent MouseEvent;

        public static event ConsoleKeyEvent KeyEvent;

        public static event ConsoleWindowBufferSizeEvent WindowBufferSizeEvent;

        private static bool Run = false;


        public static void Start()
        {
            if (!Run)
            {
                Run = true;
                IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
                new Thread(() =>
                {
                    while (true)
                    {
                        uint numRead = 0;
                        INPUT_RECORD[] record = new INPUT_RECORD[1];
                        record[0] = new INPUT_RECORD();
                        ReadConsoleInput(handleIn, record, 1, ref numRead);
                        if (Run)
                            switch (record[0].EventType)
                            {
                                case INPUT_RECORD.MOUSE_EVENT:
                                    MouseEvent?.Invoke(record[0].MouseEvent);
                                    break;
                                case INPUT_RECORD.KEY_EVENT:
                                    KeyEvent?.Invoke(record[0].KeyEvent);
                                    break;
                                case INPUT_RECORD.WINDOW_BUFFER_SIZE_EVENT:
                                    WindowBufferSizeEvent?.Invoke(record[0].WindowBufferSizeEvent);
                                    break;
                            }
                        else
                        {
                            uint numWritten = 0;
                            WriteConsoleInput(handleIn, record, 1, ref numWritten);
                            return;
                        }
                    }
                }).Start();
            }
        }

        public static void Stop() => Run = false;


        public delegate void ConsoleMouseEvent(MOUSE_EVENT_RECORD r);

        public delegate void ConsoleKeyEvent(KEY_EVENT_RECORD r);

        public delegate void ConsoleWindowBufferSizeEvent(WINDOW_BUFFER_SIZE_RECORD r);

    }


    public static class NativeMethods
    {
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            public const ushort KEY_EVENT = 0x0001,
                MOUSE_EVENT = 0x0002,
                WINDOW_BUFFER_SIZE_EVENT = 0x0004; //more

            [FieldOffset(0)]
            public ushort EventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
            /*
            and:
             MENU_EVENT_RECORD MenuEvent;
             FOCUS_EVENT_RECORD FocusEvent;
             */
        }

        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;

            public const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
                FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
                FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
                FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
                RIGHTMOST_BUTTON_PRESSED = 0x0002;
            public uint dwButtonState;

            public const int CAPSLOCK_ON = 0x0080,
                ENHANCED_KEY = 0x0100,
                LEFT_ALT_PRESSED = 0x0002,
                LEFT_CTRL_PRESSED = 0x0008,
                NUMLOCK_ON = 0x0020,
                RIGHT_ALT_PRESSED = 0x0001,
                RIGHT_CTRL_PRESSED = 0x0004,
                SCROLLLOCK_ON = 0x0040,
                SHIFT_PRESSED = 0x0010;
            public uint dwControlKeyState;

            public const int DOUBLE_CLICK = 0x0002,
                MOUSE_HWHEELED = 0x0008,
                MOUSE_MOVED = 0x0001,
                MOUSE_WHEELED = 0x0004;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0)]
            public bool bKeyDown;
            [FieldOffset(4)]
            public ushort wRepeatCount;
            [FieldOffset(6)]
            public ushort wVirtualKeyCode;
            [FieldOffset(8)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(10)]
            public byte AsciiChar;

            public const int CAPSLOCK_ON = 0x0080,
                ENHANCED_KEY = 0x0100,
                LEFT_ALT_PRESSED = 0x0002,
                LEFT_CTRL_PRESSED = 0x0008,
                NUMLOCK_ON = 0x0020,
                RIGHT_ALT_PRESSED = 0x0001,
                RIGHT_CTRL_PRESSED = 0x0004,
                SCROLLLOCK_ON = 0x0040,
                SHIFT_PRESSED = 0x0010;
            [FieldOffset(12)]
            public uint dwControlKeyState;
        }

        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;
        }

        public const uint STD_INPUT_HANDLE = unchecked((uint)-10),
            STD_OUTPUT_HANDLE = unchecked((uint)-11),
            STD_ERROR_HANDLE = unchecked((uint)-12);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);


        public const uint ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008; //more

        [DllImportAttribute("kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

        [DllImportAttribute("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleInput, uint dwMode);


        [DllImportAttribute("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

        [DllImportAttribute("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsWritten);

    }

    class Program
    {
        // Heared you in the rain
        // Will you always be there?
        // The last thing you said
        // Sorry if it`s too late

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;
        public static IntPtr ConsoleHandle;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static bool LyricsExist = false;
        private static bool TagsExist = false;
        private static bool Musixmatch = false;
        private static bool EnableMusixmatchHack = true;
        private static bool SaveLyrlibFiles = false;
        private static ISoundOut _soundOut;
        private static IWaveSource _source;
        private static LineSpectrum _lineSpectrum;
        private static string Filename = "";
        private static TagLib.File TaglibFile;
        private static Random random = new Random();
        private static DiscordRpcClient discord = new DiscordRpcClient("637644971716902933");
        private static RightForm RightForm = new RightForm();

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

        private static void OpenFileToPlay(string filename = "")
        {
            LastLyric = new TimeSpan();
            ConsoleListener.Start();
            var openFileDialog = new OpenFileDialog()
            {
                Filter = CodecFactory.SupportedFilesFilterEn,
                Title = "Выбери любой файл с музыкой..."
            };
            if (filename == "")
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filename = openFileDialog.FileName;
                }
                else
                    return;
            Lyrics = new LyrLibMusicLyrics();
            LyricsExist = false;
            TagsExist = false;
            Musixmatch = false;
            ISampleSource source = CodecFactory.Instance.GetCodec(filename)
                    .ToSampleSource();

            SetupSampleSource(source);

            _soundOut = new WasapiOut();
            _soundOut.Initialize(_source);

            if (File.Exists(Path.Combine(new FileInfo(filename).DirectoryName, new FileInfo(filename).Name.Replace(".mp3", "") + ".lyrlib")))
                LyricsExist = Lyrics.TryLoadFromFile(Path.Combine(new FileInfo(filename).DirectoryName, new FileInfo(filename).Name.Replace(".mp3", "") + ".lyrlib"));
            Filename = filename;
            TaglibFile = TagLib.File.Create(Filename);
            if (TaglibFile.Tag.Title != null && TaglibFile.Tag.Title != "")
                TagsExist = true;
            if (EnableMusixmatchHack && !LyricsExist)
            {
                Console.WriteLine("\n Searching Musixmatch for lyrics, stay patient!");
                var lyricsMH = MusixmatchHack.SearchLyrics(TaglibFile.Tag.Title, TaglibFile.Tag.FirstPerformer, "200415fd8f532190e7bdca0aaa418b3771eb24ba79b0a99a7e5ac5");
                if (lyricsMH != null)
                {
                    Musixmatch = true;
                    Lyrics = new LyrLibMusicLyrics();
                    foreach (var lyricsLine in lyricsMH)
                    {
                        if (lyricsLine.Text == "")
                            continue;
                        Lyrics.AddLyricEntry(TimeSpan.FromSeconds(lyricsLine.Time.Total), lyricsLine.Text);
                    }
                    Lyrics.Name = TaglibFile.Tag.Title;
                    Lyrics.Artist = TaglibFile.Tag.FirstPerformer;
                    Lyrics.Album = TaglibFile.Tag.Album;
                    LyricsExist = true;
                    if (SaveLyrlibFiles)
                    {
                        Lyrics.TrySaveToFile(Path.Combine(new FileInfo(filename).DirectoryName, new FileInfo(filename).Name.Replace(".mp3", "") + ".lyrlib"));
                    }
                }
            }
            _soundOut.Play();
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

        private static TimeSpan LastLyric = new TimeSpan();
        private static Size size = new Size(Console.WindowWidth - 2, 20);
        private static int leftPos = 0;
        private static int TrackLineBegin, TrackLineEnd, TrackLineMax;
        private static bool LyricsUpdateNeeded = false;

        private static void DrawControls(int line)
        {
            Console.SetCursorPosition(1, line);
            if (_soundOut.PlaybackState == PlaybackState.Playing)
            {
                if (IsMouseOnPlayPauseButton)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Pause");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                    Console.Write("Pause");
                Console.Write(" ");
                if (IsMouseOnStopButton)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Stop");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                    Console.Write("Stop");
                Console.Write(" [");
            }
            if (_soundOut.PlaybackState == PlaybackState.Paused)
            {
                if (IsMouseOnPlayPauseButton)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Play ");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                    Console.Write("Play ");
                Console.Write(" ");
                if (IsMouseOnStopButton)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Stop");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                    Console.Write("Stop");
                Console.Write(" [");
            }
            TrackLineMax = Console.WindowWidth - 4 - "Pause Stop [".Length;
            TrackLineBegin = "Pause Stop [".Length + 1;
            TrackLineEnd = Console.WindowWidth - 2;
            int size = (int)(_source.GetPosition().TotalMilliseconds * TrackLineMax / _source.GetLength().TotalMilliseconds);
            for (int i = 0; i < size; i++)
                Console.Write('-');
            Console.Write('^');
            for (int i = 0; i < TrackLineMax - size; i++)
                Console.Write('-');
            Console.Write("]");
        }

        private static void DrawPlayerLayer()
        {
            if (_soundOut.PlaybackState != PlaybackState.Paused)
                if (LyricsExist)
                {
                    TimeSpan span = _source.GetPosition();
                    foreach (var key in Lyrics.LyricsTimecodes.Keys)
                    {
                        if ((Math.Abs(span.TotalMilliseconds - key.TotalMilliseconds) < 50 && LastLyric != key) || LyricsUpdateNeeded)
                        {
                            Console.SetCursorPosition(2, 1);
                            ClearCurrentConsoleLine();

                            if (LastLyric != new TimeSpan())
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(" " + Lyrics.LyricsTimecodes[LastLyric]);
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 2);
                            }
                            Console.SetCursorPosition(2, 2);
                            ClearCurrentConsoleLine();
                            if (!LyricsUpdateNeeded)
                            {
                                Console.Write(" " + Lyrics.LyricsTimecodes[key]);
                                Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 2);
                            }
                            LyricsUpdateNeeded = false;
                            {
                                Console.SetCursorPosition(2, 3);
                                ClearCurrentConsoleLine();
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                try
                                {
                                    Console.Write(" " + Lyrics.LyricsTimecodes[Lyrics.GetNextTimecodeByTime(span.Add(TimeSpan.FromMilliseconds(50)))]);
                                }
                                catch { }
                                leftPos = Console.CursorLeft;
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 2);
                            }
                            {
                                Console.SetCursorPosition(2, 5);
                                ClearCurrentConsoleLine();
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write(" > Song: ");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write(Lyrics.Name);
                                Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                            }
                            {
                                Console.SetCursorPosition(2, 6);
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
                            LastLyric = key;
                        }
                    }

                    if (leftPos != 0)
                    {
                        Console.SetCursorPosition(leftPos, 3);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("          "); // Ahahahah lol wtf maaan nice clear :P
                        Console.SetCursorPosition(leftPos, 3);
                        Console.Write(" (" + Math.Truncate(Math.Abs(Lyrics.GetNextTimecodeByTime(span.Add(TimeSpan.FromMilliseconds(50))).TotalMilliseconds - span.TotalMilliseconds)) + "ms)");
                    }
                }
                else if (TagsExist)
                {
                    {
                        Console.SetCursorPosition(2, 1);
                        ClearCurrentConsoleLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(" > Song: ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(TaglibFile.Tag.Title);
                        Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
                    }
                    {
                        Console.SetCursorPosition(2, 2);
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
                DrawControls(8);
            else if (TagsExist)
                DrawControls(4);
            else
                DrawControls(1);
            if (LyricsExist)
                size = new Size(Console.WindowWidth - 2, Console.WindowHeight - 12);
            else if (TagsExist)
                size = new Size(Console.WindowWidth - 2, Console.WindowHeight - 8);
            else
                size = new Size(Console.WindowWidth - 2, Console.WindowHeight - 5);
            var spectrumDataConsole = _lineSpectrum.CreateSpectrumLine(size);
            if (spectrumDataConsole == null)
                return;
            float fftSum = 0;
            foreach (int line in spectrumDataConsole)
                fftSum += line;
            Console.SetCursorPosition(0, Console.WindowHeight - size.Height - 1);
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
            Bitmap bitmap = _lineSpectrum.Bitmap;
            Graphics graphics = Graphics.FromImage(bitmap);
            
            if (LastLyric != new TimeSpan())
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.DrawString(Lyrics.LyricsTimecodes[LastLyric], new Font("Arial", 32, FontStyle.Italic), Brushes.Gray, new Rectangle(0, bitmap.Height / 2, bitmap.Width, bitmap.Height / 2), sf);
            }
            if (TagsExist && TaglibFile.Tag.Pictures.Length > 0)
            {
                var bin = (byte[])(TaglibFile.Tag.Pictures[0].Data.Data);
                graphics.DrawImage(Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(bitmap.Height / 2 - 5, bitmap.Height / 2 - 5, null, IntPtr.Zero), new Rectangle(5, 5, bitmap.Height / 2, bitmap.Height / 2));
            }
            else
            {
                Bitmap myImage = (Bitmap)Resources.ResourceManager.GetObject("MissingArt");
                graphics.DrawImage(myImage, new Rectangle(5, 5, bitmap.Height / 2, bitmap.Height / 2));
            }
            if (TagsExist || LyricsExist)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Near;
                sf.Alignment = StringAlignment.Near;
                graphics.DrawString(Lyrics.Name, new Font("Arial", 32, FontStyle.Regular), Brushes.White, new Rectangle(bitmap.Height / 2, 0, bitmap.Width / 2, 56), sf);
                graphics.DrawString(Lyrics.Artist, new Font("Arial", 16, FontStyle.Regular), Brushes.White, new Rectangle(bitmap.Height / 2 + 4, 48, bitmap.Width, 32), sf);
            }
            RightForm.RedrawImage(bitmap);
            Application.DoEvents();
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

        public static void OpenFileAndPlay(string file = "", string joinSecret = "")
        {
            Application.EnableVisualStyles();
            RightForm.Show();
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
                        var presence = discord.CurrentPresence;
                        presence.State = "Paused";
                        discord.SetPresence(presence);
                    }
                    else if (_soundOut.PlaybackState == PlaybackState.Paused)
                    {
                        _soundOut.Play();
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
            "Download from SoundCloud",
            "Download from YouTube",
            "Musixmatch Unofficial API: ON",
            "Save Lyrlib Files: OFF"
        };
        static int SelectedMenuItem = 0;

        static void DrawSoundCloud()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Downloading from SoundCloud is possible thanks to him: https://github.com/InsaneSlay");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Paste SoundCloud song link: ");
            var link = Console.ReadLine();
            Directory.CreateDirectory("soundcloud");
            MessageBox.Show("Join Secret Debug", link.Replace("https://soundcloud.com/", "_SC__"));
            OpenFileAndPlay(SCDownload.Downloader.DownloadSong(link, "soundcloud"), link.Replace("https://soundcloud.com/", "_SC__"));
            Directory.Delete("soundcloud", true);
        }

        static string DownloadYTSong(string link, string directory)
        {
            Directory.CreateDirectory(directory);
            var youtube = YouTube.Default;
            var vid = youtube.GetVideo(link);
            File.WriteAllBytes(Path.Combine(directory, vid.FullName), vid.GetBytes());

            var inputFile = new MediaFile { Filename = Path.Combine(directory, vid.FullName) };
            var outputFile = new MediaFile { Filename = Path.Combine(directory, vid.FullName + ".mp3") };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }
            TagLib.File file = TagLib.File.Create(Path.Combine(directory, vid.FullName + ".mp3"));
            file.Tag.Title = vid.Title.Substring(vid.Title.IndexOf(" - ") + 3);
            file.Tag.Performers = new[] { vid.Title.Substring(0, vid.Title.IndexOf(" - ")) };
            file.Tag.Album = file.Tag.Title + " - Single";
            file.Save();
            return Path.Combine(directory, vid.FullName + ".mp3");
        }

        static void DrawYouTube()
        {
            Console.Clear();
            Console.Write("Paste YouTube link: ");
            var link = Console.ReadLine();
            OpenFileAndPlay(DownloadYTSong(link, "youtube"));
        }

        static void DoShit()
        {
            Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;
            ConsoleHandle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(ConsoleHandle, false);
            if (ConsoleHandle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            uint mode = 0;
            GetConsoleMode(inHandle, ref mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            mode |= ENABLE_WINDOW_INPUT;
            mode |= ENABLE_MOUSE_INPUT;
            SetConsoleMode(inHandle, mode);
            ConsoleListener.MouseEvent += ConsoleListener_MouseEvent;
            Console.Clear();
            Console.WindowWidth = 100;
            Console.BufferWidth = 100;
            Console.WindowHeight = 25;
            Console.BufferHeight = 25;
            Console.Clear();
            Console.Title = "ConsoleMusix v1.59";
            Console.OutputEncoding = Encoding.UTF8;
        }

        [STAThread]
        static void Main(string[] args)
        {
            RichPresence presence = new RichPresence()
            {
                Details = "Idle",
                State = "Main Menu"
            };
            discord.OnJoin += OnJoin;
            discord.SetPresence(presence);
            discord.Initialize();
            discord.RegisterUriScheme();
            DoShit();
            if (args.Length > 0)
                OpenFileAndPlay(args[0]);
            else
                while (true)
                {
                    try
                    {
                        ConsoleListener.Stop();
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
                                DrawYouTube();
                            if (SelectedMenuItem == 3)
                            {
                                if (EnableMusixmatchHack)
                                {
                                    EnableMusixmatchHack = false;
                                    Menu[3] = "Musixmatch Unofficial API: OFF";
                                }
                                else
                                {
                                    EnableMusixmatchHack = true;
                                    Menu[3] = "Musixmatch Unofficial API: ON ";
                                }
                            }
                            if (SelectedMenuItem == 4)
                            {
                                if (SaveLyrlibFiles)
                                {
                                    SaveLyrlibFiles = false;
                                    Menu[4] = "Save Lyrlib Files: OFF";
                                }
                                else
                                {
                                    SaveLyrlibFiles = true;
                                    Menu[4] = "Save Lyrlib Files: ON ";
                                }
                            }
                            if (SelectedMenuItem == 5)
                            {
                                
                            }
                        }
                        if (key.Key == ConsoleKey.DownArrow)
                            SelectedMenuItem++;
                        if (key.Key == ConsoleKey.UpArrow)
                            SelectedMenuItem--;
                    }
                    catch { }
                }
        }

        private static bool IsMouseOnPlayPauseButton = false;
        private static bool IsMouseOnStopButton = false;

        private static void ConsoleListener_MouseEvent(MOUSE_EVENT_RECORD r)
        {
            if ((r.dwMousePosition.Y == 8 && LyricsExist) || (r.dwMousePosition.Y == 4 && TagsExist && !LyricsExist) || (r.dwMousePosition.Y == 1 && !TagsExist && !LyricsExist))
            {
                IsMouseOnPlayPauseButton = (r.dwMousePosition.X >= 1 && r.dwMousePosition.X <= 5 && _soundOut.PlaybackState == PlaybackState.Playing) || (r.dwMousePosition.X >= 1 && r.dwMousePosition.X <= 4 && _soundOut.PlaybackState == PlaybackState.Paused);
                IsMouseOnStopButton = (r.dwMousePosition.X >= 7 && r.dwMousePosition.X <= 10);
                if (IsMouseOnPlayPauseButton && r.dwButtonState == 1)
                    if (_soundOut.PlaybackState == PlaybackState.Paused)
                        _soundOut.Play();
                    else if (_soundOut.PlaybackState == PlaybackState.Playing)
                        _soundOut.Pause();
                if (IsMouseOnStopButton && r.dwButtonState == 1)
                    _soundOut.Stop();
                if (r.dwMousePosition.X >= TrackLineBegin && r.dwMousePosition.X < TrackLineEnd && r.dwButtonState == 1)
                {
                    int selected = r.dwMousePosition.X - TrackLineBegin;
                    //Debug.WriteLine(selected + " / " + TrackLineMax);
                    _source.SetPosition(TimeSpan.FromMilliseconds((_source.GetLength().TotalMilliseconds * (selected)) / TrackLineMax));
                    LastLyric = new TimeSpan();
                    LyricsUpdateNeeded = true;
                    //Debug.WriteLine(TimeSpan.FromMilliseconds(_source.GetLength().TotalMilliseconds * selected / TrackLineMax));
                }
                Thread.Sleep(100);
            }
            else
            {
                IsMouseOnPlayPauseButton = false;
                IsMouseOnStopButton = false;
            }
        }

        #region Events

        #region State Events
        private static void OnReady(object sender, ReadyMessage args)
        {
            //It can be a good idea to send a inital presence update on this event too, just to setup the inital game state.
            MessageBox.Show("On Ready. RPC Version: " + args.Version);

        }
        private static void OnClose(object sender, CloseMessage args)
        {
            //This is called when our client has closed. The client can no longer send or receive events after this message.
            // Connection will automatically try to re-establish and another OnReady will be called (unless it was disposed).
            MessageBox.Show("Lost Connection with client because of " + args.Reason);
        }
        private static void OnError(object sender, ErrorMessage args)
        {
            //Some error has occured from one of our messages. Could be a malformed presence for example.
            // Discord will give us one of these events and its upto us to handle it
            MessageBox.Show("Error occured within discord. " + args.Message + " " + args.Code);
        }
        #endregion

        #region Pipe Connection Events
        private static void OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
        {
            //This is called when a pipe connection is established. The connection is not ready yet, but we have at least found a valid pipe.
            MessageBox.Show("Pipe Connection Established. Valid on pipe " + args.ConnectedPipe);
        }
        private static void OnConnectionFailed(object sender, ConnectionFailedMessage args)
        {
            //This is called when the client fails to establish a connection to discord. 
            // It can be assumed that Discord is unavailable on the supplied pipe.
            MessageBox.Show("Pipe Connection Failed. Could not connect to pipe " + args.FailedPipe);
        }
        #endregion

        private static void OnPresenceUpdate(object sender, PresenceMessage args)
        {
            //This is called when the Rich Presence has been updated in the discord client.
            // Use this to keep track of the rich presence and validate that it has been sent correctly.
            MessageBox.Show("Rich Presence Updated.");
        }

        #region Subscription Events
        private static void OnSubscribe(object sender, SubscribeMessage args)
        {
            //This is called when the subscription has been made succesfully. It will return the event you subscribed too.
            MessageBox.Show("Subscribed: " + args.Event);
        }
        private static void OnUnsubscribe(object sender, UnsubscribeMessage args)
        {
            //This is called when the unsubscription has been made succesfully. It will return the event you unsubscribed from.
            MessageBox.Show("Unsubscribed: " + args.Event);
        }
        #endregion

        #region Join / Spectate feature
        
        private static void OnSpectate(object sender, SpectateMessage args)
        {   /*
			 * This is called when the Discord Client wants to join a online game to watch and spectate.
			 * It can be triggered from a invite that your user has clicked on within discord.
			 * 
			 * The secret should be some sort of encrypted data that will give your game the nessary information to connect.
			 * For example, it could be the Game ID and the Game Password which will allow you to look up from the Master Server.
			 * Please avoid using IP addresses within these fields, its not secure and defeats the Discord security measures.
			 * 
			 * This feature requires the RegisterURI to be true on the client.
			*/
            MessageBox.Show("Spectating Game '{0}'", args.Secret);
        }
        #endregion

        #endregion

        private static void OnJoin(object sender, JoinMessage args)
        {
            DoShit();
            MessageBox.Show(args.Secret, "Ready to play!");
            Console.Clear();
            Console.WriteLine("Starting...");
            var link = args.Secret.Replace("_SC__", "https://soundcloud.com/");
            Directory.CreateDirectory("soundcloud");
            OpenFileAndPlay(SCDownload.Downloader.DownloadSong(link, "soundcloud"), link.Replace("https://soundcloud.com/", "_SC__"));
        }

        // U know absolutely same function
        public static void OnJoinDebug(string secret)
        {
            DoShit();
            MessageBox.Show(secret, "Ready to play!");
            Console.Clear();
            Console.WriteLine("Starting...");
            var link = secret.Replace("_SC__", "https://soundcloud.com/");
            Directory.CreateDirectory("soundcloud");
            OpenFileAndPlay(SCDownload.Downloader.DownloadSong(link, "soundcloud"), link.Replace("https://soundcloud.com/", "_SC__"));
        }

        private static void OnJoinRequested(object sender, JoinRequestMessage args)
        {
            DiscordRpcClient client = (DiscordRpcClient)sender;
            client.Respond(args, true);
        }
    }
}
