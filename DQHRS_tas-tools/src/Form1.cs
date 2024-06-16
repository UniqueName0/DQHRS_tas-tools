using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using SlimDX.Direct3D9;
using System.Linq;
using Jellyfish.Virtu;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    [ExternalTool("DQHRS_TAS_Tools")]
    public partial class Form1 : ToolFormBase, IExternalToolForm
    {
        public ApiContainer? _maybeAPIContainer { get; set; }
        private ApiContainer APIs => _maybeAPIContainer!;
        private TAStudio _TS;
        //private readonly Dictionary<byte, int> BlastDict = new Dictionary<byte, int>() { { 0, 0 }, { 1, 6784 }, { 2, 8384 }, { 3, 9280 }, { 4, 11072 }, { 5, 12736 }, { 6, 14848 }, { 7, 16000 }, { 8, 18304 }, { 9, 20096 }, { 10, 21376 }, { 11, 21600 }, { 12, 21600 }, { 13, 22656 }, { 14, 22656 }, { 15, 22880 }, { 16, 22880 }, { 17, 24000 }, { 18, 25408 }, { 19, 25632 }, { 20, 25632 }, { 21, 26752 }, { 22, 26752 }, { 23, 26752 }, { 24, 26976 }, { 25, 26976 }, { 26, 28672 }, { 27, 30208 }, { 28, 30448 }, { 29, 30448 }, { 30, 31648 }, { 31, 31648 }, { 32, 31888 }, { 33, 31888 }, { 34, 33152 }, { 35, 34816 }, { 36, 35056 }, { 37, 35056 }, { 38, 36320 }, { 39, 36320 }, { 40, 36560 }, { 41, 36560 }, { 42, 38208 } };
        private readonly Dictionary<byte, Tuple<uint, byte>> BlastDict = new Dictionary<byte, Tuple<uint, byte>>() { { 0, new(0, 0) }, { 1, new(6784, 33) }, { 2, new(8384, 34) }, { 3, new(9280, 34) }, { 4, new(11072, 35) }, { 5, new(12736, 37) }, { 6, new(14848, 38) }, { 7, new(16000, 38) }, { 8, new(18304, 39) }, { 9, new(18304, 39) }, { 10, new(18304, 39) }, { 11, new(18304, 39) }, { 12, new(18304, 39) }, { 13, new(18304, 39) }, { 14, new(18304, 39) }, { 15, new(18304, 39) }, { 16, new(18304, 39) }, { 17, new(18304, 39) }, { 18, new(18304, 39) }, { 19, new(18304, 39) }, { 20, new(18304, 39) }, { 21, new(18304, 39) }, { 22, new(18304, 39) }, { 23, new(18304, 39) }, { 24, new(18304, 39) }, { 25, new(20096, 41) }, { 26, new(21376, 43) }, { 27, new(21600, 44) }, { 28, new(21600, 45) }, { 29, new(22656, 46) }, { 30, new(22656, 47) }, { 31, new(22880, 48) }, { 32, new(22880, 49) }, { 33, new(24000, 50) }, { 34, new(25408, 52) }, { 35, new(25632, 53) }, { 36, new(25632, 54) }, { 37, new(26752, 55) }, { 38, new(26752, 56) }, { 39, new(26976, 57) }, { 40, new(26976, 58) }, { 41, new(28672, 60) }, { 42, new(30208, 62) }, { 43, new(30448, 63) }, { 44, new(30448, 64) }, { 45, new(31648, 65) }, { 46, new(31648, 66) }, { 47, new(31888, 67) }, { 48, new(31888, 68) }, { 49, new(33152, 69) }, { 50, new(34816, 71) }, { 51, new(35056, 72) }, { 52, new(35056, 73) }, { 53, new(36320, 74) }, { 54, new(36320, 75) }, { 55, new(36560, 76) }, { 56, new(36560, 77) }};
        private int waitFrames = 50;
        
        private byte GetFrameByBlastDist(double distance)
        {
            List<Tuple<uint, byte>> BlastDists = new(BlastDict.Values);
            byte closest = 0;
            for (int i = 0; i < BlastDists.Count; i++)
            {
                if (BlastDict.ContainsKey((byte)(i+1)) && BlastDists[i+1].Item1 < distance && BlastDict.ContainsKey((byte)i))
                {
                    closest = (byte)i;   
                }
            }
            return closest;
        }

        
        public Form1() => InitializeComponent();


        protected override string WindowTitleStatic => "test";


        public override void Restart()
        {
            //ran when external tool is started/restarted
            APIs.Memory.UseMemoryDomain(APIs.Memory.MainMemoryName);
            _TS = (TAStudio)APIs.Tool.GetTool("TAStudio");

            System.Windows.Forms.Timer timer1 = new()
            {
                Interval = 10
            };
            timer1.Tick += new EventHandler(UpdateTool);
            timer1.Start();

            
        }



        protected override void UpdateAfter()
        {
            
        }

        // will be changed to use GeneralUpdate when its included in a release
        protected void UpdateTool(object sender, EventArgs e)
        {
            var mouse00 = APIs.Input.GetMouse();
            if ((bool)mouse00["Middle"] == true)
            {
                //middle mouse button clicked
            }

            if (waitFrames > 0)
            {
                waitFrames--;
                return;
            }
            if (MainForm.EmulatorPaused)
            {
                Vector2 CamPos = new((int)APIs.Memory.ReadU32(0x00140f4c), (int)APIs.Memory.ReadU32(0x00140f50));
                Vector2 playerPos = new(APIs.Memory.ReadU32(0x00143B20), APIs.Memory.ReadU32(0x00143B24));

                Vector2 ScrScale = new(APIs.EmuClient.ScreenWidth() / 66046f, APIs.EmuClient.ScreenHeight() * 0.5f / 49662f);
                Vector2 ScrScale2 = new(66046f / APIs.EmuClient.ScreenWidth(), 49662f / (APIs.EmuClient.ScreenHeight() * 0.5f));
                Vector2 PlayerPos = new((float)Math.Floor(ScrScale.X * ((float)playerPos.X - CamPos.X)), (float)Math.Floor(ScrScale.Y * ((float)playerPos.Y - CamPos.Y) + APIs.EmuClient.ScreenHeight() * 0.5f));
                

                var mouse = APIs.Input.GetMouse();
                Point MousePos = APIs.EmuClient.TransformPoint(new Point((int)mouse["X"], (int)mouse["Y"] / 2));

                Vector2 MousePosInGame = new(ScrScale2.X * MousePos.X + CamPos.X, ScrScale2.Y * (MousePos.Y - APIs.EmuClient.ScreenHeight() * 0.5f) + CamPos.Y);

                double distFromPlayer = Math.Sqrt(Math.Pow(MousePosInGame.X - playerPos.X, 2) + Math.Pow(MousePosInGame.Y - playerPos.Y, 2));

                double Direction = Math.Round(Math.Atan2(MousePos.X - PlayerPos.X, MousePos.Y - PlayerPos.Y) /(Math.PI*0.25f));
                byte FramesToHold = (byte)(ManualBlastControl.Checked ? (byte)FramesHeld.Value : GetFrameByBlastDist(distFromPlayer));
                FramesHeld.Value = FramesToHold;
                uint dist = 0;
                List<String> dirs = new();
                try { dist = BlastDict[FramesToHold].Item1; } catch { }
                APIs.Gui.WithSurface(DisplaySurfaceID.Client, () => {
                    //APIs.Gui.DrawEllipse((int)MousePos.X, (int)MousePos.Y, 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                    //APIs.Gui.DrawEllipse((int)mouse["X"], (int)mouse["Y"], 10, 10, Color.Black, Color.Blue, DisplaySurfaceID.Client);
                    //APIs.Gui.DrawEllipse(mp.X, mp.Y, 10, 10, Color.Black, Color.Blue, DisplaySurfaceID.Client);
                    //APIs.Gui.DrawEllipse((int)PlayerPos.X, (int)PlayerPos.Y, 10, 10, Color.Black, Color.Blue, DisplaySurfaceID.Client);
                    switch (Direction)
                    {

                        case 4:
                        case -4:
                            APIs.Gui.DrawEllipse((int)PlayerPos.X, (int)((int)PlayerPos.Y - (dist * ScrScale.Y)), 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Up");
                            break;
                        case 0:
                            APIs.Gui.DrawEllipse((int)PlayerPos.X, (int)((int)PlayerPos.Y + (dist * ScrScale.Y)), 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Down");
                            break;
                        case 2:
                            APIs.Gui.DrawEllipse((int)((int)PlayerPos.X + (dist * ScrScale.X)), (int)PlayerPos.Y, 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Right");
                            break;
                        case -2:
                            APIs.Gui.DrawEllipse((int)((int)PlayerPos.X - (dist*ScrScale.X)), (int)PlayerPos.Y, 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Left");
                            break;
                        case 3:
                            APIs.Gui.DrawEllipse((int)((int)PlayerPos.X + (dist * (Math.Sqrt(2) / 2) * ScrScale.X)), (int)((int)PlayerPos.Y - (dist * (Math.Sqrt(2) / 2) * ScrScale.X)), 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Up");
                            dirs.Add("Right");
                            break;
                        case -1:
                            APIs.Gui.DrawEllipse((int)((int)PlayerPos.X - (dist*(Math.Sqrt(2)/2) * ScrScale.X)), (int)((int)PlayerPos.Y + (dist * (Math.Sqrt(2) / 2) * ScrScale.X)), 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Left");
                            dirs.Add("Down");
                            break;
                        case 1:
                            APIs.Gui.DrawEllipse((int)((int)PlayerPos.X + (dist * (Math.Sqrt(2) / 2) * ScrScale.X)), (int)((int)PlayerPos.Y + (dist * (Math.Sqrt(2) / 2) * ScrScale.X)), 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Down");
                            dirs.Add("Right");
                            break;
                        case -3:
                            APIs.Gui.DrawEllipse((int)((int)PlayerPos.X - (dist * (Math.Sqrt(2) / 2) * ScrScale.X)), (int)((int)PlayerPos.Y - (dist * (Math.Sqrt(2) / 2) * ScrScale.X)), 10, 10, Color.Black, Color.Red, DisplaySurfaceID.Client);
                            dirs.Add("Up");
                            dirs.Add("Left");
                            break;
                    }
                });

                if ((bool)APIs.Input.GetPressedButtons().Contains("Z"))
                {
                    blast(FramesToHold, dirs);
                    //bruteForceMeasureMents();
                    waitFrames = 50;
                }

            }
        }

        private async void blast(int FramesToHold, List<String> dirs)
        {
            int cFrame = _TS.Emulator.Frame;

            MethodInfo AdvanceFramesMethod = _TS.GetType().GetMethod("StartAtNearestFrameAndEmulate", BindingFlags.NonPublic | BindingFlags.Instance);
            ITasMovie tasMovie = _TS.CurrentTasMovie;

            _TS.Focus();
            await Task.Delay(20);
            //AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame - 4, true, false });

            

            int MIN = 23;
            int a_count = FramesToHold < MIN ? MIN : FramesToHold;

            for (int j = 0; j <= a_count; j++)
            {
                tasMovie.ToggleBoolState(_TS.Emulator.Frame + j, "A");

            }
            await Task.Delay(1);
            foreach (string dir in dirs)
            {
                for (int j = 0; j <= FramesToHold; j++)
                {
                    int frameToToggle = FramesToHold < MIN ? MIN - j : j;
                    tasMovie.ToggleBoolState(_TS.Emulator.Frame + frameToToggle, dir);
                }



            }
            AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame - 1, true, false });
            await Task.Delay(1);
            foreach (string dir in dirs)
            {

                if (FramesToHold < 14)
                {
                    tasMovie.ToggleBoolState(_TS.Emulator.Frame, dir);

                }
            }

            //for (int j = 0; j < BlastDict[(byte)FramesToHold].Item2; j++)
            AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame + BlastDict[(byte)FramesToHold].Item2, true, false });
            await Task.Delay(1);

            //check if blast actually finished
            uint playerX = 0;
            while (playerX != APIs.Memory.ReadU32(0x00143B20))
            {

                playerX = APIs.Memory.ReadU32(0x00143B20);
                MainForm.SeekFrameAdvance();
                await Task.Delay(1);
            }
        }
            private async void bruteForceMeasureMents()
        {
            MethodInfo AdvanceFramesMethod = _TS.GetType().GetMethod("StartAtNearestFrameAndEmulate", BindingFlags.NonPublic | BindingFlags.Instance);
            ITasMovie tasMovie = _TS.CurrentTasMovie;
            


            // formatted as {(A frames, Dir frames) : (distance, frames taken)}
            //Dictionary<Tuple<int, int>, Tuple<int, int>> output;
            // formatted as {frames : (distance, frames taken)}
            Dictionary<int, Tuple<uint, int>> output = new();

            int A_BUTTON_LIMIT = 101;
            int R_DIR_LIMIT = 43;

            for (int frames = 0; frames < A_BUTTON_LIMIT; frames++)
            {
                int MIN = 23;
                int DELAY = 1;
                //prep tas studio
                int a_count = frames < MIN ? MIN : frames;
                for (int j = 0; j <= a_count; j++)
                {
                    tasMovie.ToggleBoolState(_TS.Emulator.Frame + j, "A");
                }

                for (int j = 0; j <= frames; j++)
                {
                    int frameToToggle = frames < MIN ? MIN - j : j;
                    tasMovie.ToggleBoolState(_TS.Emulator.Frame + frameToToggle, "Right");
                }
                AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame - 1, true, false });
                await Task.Delay(DELAY);
                if (frames < 14)
                {
                    tasMovie.ToggleBoolState(_TS.Emulator.Frame, "Right");
                
                }
                AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame + 5, true, false });
                await Task.Delay(DELAY);

                // run frames and take measurements

                int framesTaken = 0;
                //wait for start of blast
                uint playerX = APIs.Memory.ReadU32(0x00143B20);
                uint playerStartX = playerX;
                while (playerX == APIs.Memory.ReadU32(0x00143B20))
                {
                    playerX = APIs.Memory.ReadU32(0x00143B20);
                    MainForm.SeekFrameAdvance();
                    await Task.Delay(DELAY);
                    framesTaken++;

                    //debug_text_box.Text = "a: " + (playerX == APIs.Memory.ReadU32(0x00143B20)).ToString();
                }
                

                // during blast
                while (playerX != APIs.Memory.ReadU32(0x00143B20))
                {

                    playerX = APIs.Memory.ReadU32(0x00143B20);
                    MainForm.SeekFrameAdvance();
                    await Task.Delay(DELAY);
                    framesTaken++; 
                    //debug_text_box.Text = "b: " + (playerX == APIs.Memory.ReadU32(0x00143B20)).ToString();

                }
                //debug_text_box.Text = "END";


                //blast ended
                uint distance = playerX - playerStartX;
                output.Add(frames, new Tuple<uint, int>(distance,framesTaken));

                //reset
                AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame - framesTaken-5, true, false });
                await Task.Delay(DELAY);
                _TS.ClearFrames(_TS.Emulator.Frame, framesTaken+5);
                await Task.Delay(DELAY);
                MainForm.SeekFrameAdvance();
                await Task.Delay(DELAY);

            }
            //format and display output
            String outputStr = "";
            foreach (int Key in output.Keys)
            {
                Tuple<uint, int> Val = output[Key];
                outputStr += Key.ToString() + ": (" + Val.Item1.ToString() + ", " + Val.Item2.ToString() + "), ";
            }
            //debug_text_box.Text = outputStr;
        }

        

        private async void SkipDialogButton_Click(object sender, EventArgs e)
        {
            MethodInfo AdvanceFramesMethod = _TS.GetType().GetMethod("StartAtNearestFrameAndEmulate", BindingFlags.NonPublic | BindingFlags.Instance);
            ITasMovie tasMovie = _TS.CurrentTasMovie;


            tasMovie.ToggleBoolState(_TS.Emulator.Frame, "B");
            AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame + 5, true, false });
            await Task.Delay(20); //needs short delay for tastudio to catch up with this tool
            AdvanceFramesMethod.Invoke(_TS, new object[] { _TS.Emulator.Frame - 4, true, false });
            await Task.Delay(20);

            while (tasMovie[_TS.Emulator.Frame].Lagged!.Value) {
                MainForm.SeekFrameAdvance();
                await Task.Delay(20);
            }
            tasMovie.ToggleBoolState(_TS.Emulator.Frame, "A");
            MainForm.SeekFrameAdvance();
            await Task.Delay(20);

            
        }

      
    }
}



/*
input names for tas studio 
 
Up
Down
Left
Right
Start
Select
B
A
X
Y
L
R
LidOpen
LidClose
Touch
Power



-----Axes

Touch X
Touch Y
Mic Volume
GBA Light Sensor
*/