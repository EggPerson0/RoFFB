using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.DirectInput;
using System.Diagnostics;


namespace VirtualControllerRemapper
{
    public partial class Form1 : Form
    {
        private ViGEmClient client;
        private IXbox360Controller virtualController;

        private DirectInput directInput;
        private List<DeviceInstance> allDevices = new();
        private Joystick physicalDevice;
        List<Joystick> activeDevices = new List<Joystick>();

        private System.Windows.Forms.Timer inputTimer;
        private System.Windows.Forms.Timer reconnectTimer;

        private bool isWaitingForInput = false;
        private string bindingTarget = "";

        Dictionary<string, (int deviceIndex, int buttonIndex)> boundButtons = new();
        Dictionary<string, (int deviceIndex, Func<JoystickState, int>)> axisBindings = new();

        private static readonly Dictionary<string, Xbox360Button> ButtonNameMap =
    new(StringComparer.OrdinalIgnoreCase)
    {
        { "A", Xbox360Button.A },
        { "B", Xbox360Button.B },
        { "X", Xbox360Button.X },
        { "Y", Xbox360Button.Y },
        { "Start", Xbox360Button.Start },
        { "Back", Xbox360Button.Back },
        { "Guide", Xbox360Button.Guide },
        { "LeftShoulder", Xbox360Button.LeftShoulder },
        { "RightShoulder", Xbox360Button.RightShoulder },
        { "LeftThumb", Xbox360Button.LeftThumb },
        { "RightThumb", Xbox360Button.RightThumb },
        { "Up", Xbox360Button.Up },
        { "Down", Xbox360Button.Down },
        { "Left", Xbox360Button.Left },
        { "Right", Xbox360Button.Right }
    };



        // ForceFeedback:
        private const int WM_DEVICECHANGE = 0x0219;

        private DirectInput directForce;
        private Joystick selectedDevice;
        private Effect activeEffect;
        private DeviceInstance[] ffbDevices;
        private System.Windows.Forms.Timer updateTimer;
        private EffectParameters effectParams;


        public static double LeftRumble = 0;
        public static double RightRumble = 0;
        public static bool feedbackEnabled = false;
        public static bool feedbackTest = false;
        Stopwatch stopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            //AttachConsole(); // optional debug output window

            InitializeViGEm();
            InitializeDirectInput();
            PopulateBindTargets();
            StartInputLoop();
            StartReconnectLoop();
            LoadBindings();

            //ForceFeedback:
            LoadDevices();
            StartForceTimer();
            stopwatch.Start();
        }
        //
        // Force FeedBack::
        //



        private void Controller_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                Invoke(new Action(() =>
            {

                LeftRumble = (double)e.LargeMotor / 260;
                RightRumble = (double)e.SmallMotor / -260;
                leftLabel.Text = $"Rumble → Left: {LeftRumble}";
                rightLabel.Text = $"Rumble → Right: {RightRumble}";
                //labelRumble.Text = $"Rumble → Left: {e.LargeMotor}, Right: {e.SmallMotor}";
            }));
            }
        }


        private void LoadDevices()
        {
            directForce = new DirectInput();
            var devices = directForce.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            ffbDevices = devices.Where(d => d.ForceFeedbackDriverGuid != Guid.Empty).ToArray();

            comboBoxDevices.Items.Clear();
            foreach (var device in ffbDevices)
                comboBoxDevices.Items.Add(device.ProductName);

            if (comboBoxDevices.Items.Count > 0)
                comboBoxDevices.SelectedIndex = 0;

            InitializeFeedbackDevice();
        }

        private void comboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeFeedbackDevice();
        }

        private void InitializeFeedbackDevice()
        {
            if (comboBoxDevices.SelectedIndex < 0) return;

            var deviceInstance = ffbDevices[comboBoxDevices.SelectedIndex];
            selectedDevice = new Joystick(directForce, deviceInstance.InstanceGuid);
            selectedDevice.SetCooperativeLevel(Handle, CooperativeLevel.Exclusive | CooperativeLevel.Background);
            selectedDevice.Acquire();

            var effects = selectedDevice.GetEffects();
            var constantEffect = effects.FirstOrDefault(e => e.Guid == EffectGuid.ConstantForce);
            if (constantEffect == null)
            {
                MessageBox.Show("Device doesn't support Constant Force.");
                return;
            }

            effectParams = new EffectParameters
            {
                Flags = EffectFlags.ObjectOffsets | EffectFlags.Cartesian,
                Duration = int.MaxValue,
                Gain = 10000,
                TriggerButton = -1,
                SamplePeriod = 0,
                StartDelay = 0,
                Axes = new int[] { 0 },
                Directions = new int[] { 0 },
                Parameters = new ConstantForce
                {
                    Magnitude = 0
                }
            };

            activeEffect = new Effect(selectedDevice, constantEffect.Guid, effectParams);
            activeEffect.Start(1);
        }


        private void StartForceTimer()
        {
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 5; // 100 Hz
            updateTimer.Tick += UpdateForceFeedback;
            updateTimer.Start();
        }

        private void UpdateForceFeedback(object sender, EventArgs e)
        {
            if (activeEffect == null || selectedDevice == null || effectParams == null) return;
            int finalMagnitude = (int)((LeftRumble + RightRumble) * 10000);
            if (feedbackTest == true)
            {
                finalMagnitude = (int)(Math.Sin(stopwatch.ElapsedMilliseconds) * 1000);
            }
            if (feedbackEnabled == false)
            {
                finalMagnitude = 0;
            }
            forceAmount.Text = $"Torque: {finalMagnitude}";
            if (!feedbackEnabled) return;
            ((ConstantForce)effectParams.Parameters).Magnitude = finalMagnitude;

            try
            {
                activeEffect.SetParameters(effectParams, EffectParameterFlags.TypeSpecificParameters | EffectParameterFlags.NoRestart);
            }
            catch
            {
                // Ignore FFB glitches
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//
        {
            SaveBindings();
            client.Dispose();
        }

        private void forceCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (forceCheck.Checked == true)
            {
                feedbackEnabled = true;
            }
            else
            {
                feedbackEnabled = false;
            }
        }


        private void testTimer_Tick(object sender, EventArgs e)
        {
            feedbackTest = false;
            testTimer.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            feedbackTest = true;
            testTimer.Start();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_DEVICECHANGE)
            {
                LoadDevices();
            }
        }


        //
        // Binding ::
        //


        private string bindingsPath = "bindings.txt";

        private void SaveBindings()
        {
            using (StreamWriter writer = new StreamWriter(bindingsPath))
            {
                foreach (var pair in boundButtons)
                {
                    writer.WriteLine($"BUTTON|{pair.Key}|{pair.Value.deviceIndex}|{pair.Value.buttonIndex}");
                }

                foreach (var pair in axisBindings)
                {
                    writer.WriteLine($"AXIS|{pair.Key}|{pair.Value.deviceIndex}|{pair.Value.Item2.Method.Name}");
                }
            }
        }

        private void LoadBindings()
        {
            if (!File.Exists(bindingsPath)) return;

            foreach (var line in File.ReadAllLines(bindingsPath))
            {
                var parts = line.Split('|');
                if (parts.Length < 4) continue;

                string type = parts[0];
                string name = parts[1];
                int deviceIndex = int.Parse(parts[2]);

                if (type == "BUTTON")
                {
                    int buttonIndex = int.Parse(parts[3]);
                    boundButtons[name] = (deviceIndex, buttonIndex);
                }
                else if (type == "AXIS")
                {
                    string axisName = parts[3];
                    Func<JoystickState, int> extractor = parts[3] switch
                    {
                        "GetX" => GetX,
                        "GetY" => GetY,
                        "GetZ" => GetZ,
                        "GetRX" => GetRX,
                        "GetRY" => GetRY,
                        "GetRZ" => GetRZ,
                        "GetSlider0" => GetSlider0,
                        "GetSlider1" => GetSlider1,
                        "GetSlider2" => GetSlider2,
                        _ => null
                    };
                    Console.WriteLine(axisName);
                    if (extractor != null)
                    {
                        axisBindings[name] = (deviceIndex, extractor);
                        Console.WriteLine(name);
                    }
                }
            }

            bindStatus.Text = "Bindings loaded.";
        }

        private void AttachConsole()
        {
            NativeMethods.AllocConsole();
        }

        private void InitializeViGEm()
        {
            client = new ViGEmClient();
            virtualController = client.CreateXbox360Controller();
            virtualController.FeedbackReceived += Controller_FeedbackReceived;
            virtualController.Connect();
        }

        private void InitializeDirectInput()
        {
            directInput = new DirectInput();
            allDevices.Clear();
            activeDevices.Clear();

            allDevices = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).ToList();

            for (int i = 0; i < allDevices.Count; i++)
            {
                var devInfo = allDevices[i];
                var stick = new Joystick(directInput, devInfo.InstanceGuid);

                try
                {
                    stick.Acquire();
                    activeDevices.Add(stick);
                    deviceSelector.Items.Add($"{i}: {devInfo.InstanceName}");
                }
                catch
                {
                    // Ignore devices that can't be acquired
                    deviceSelector.Items.Add($"{i}: (Unavailable)");
                    activeDevices.Add(null); // Maintain index alignment
                }
            }

            deviceSelector.SelectedIndex = 0;
            InitializeSelectedDevice(0);
        }

        private void InitializeSelectedDevice(int index)
        {
            if (physicalDevice != null)
            {
                try { physicalDevice.Unacquire(); physicalDevice.Dispose(); }
                catch { }
            }

            var deviceInfo = allDevices[index];
            physicalDevice = new Joystick(directInput, deviceInfo.InstanceGuid);
            physicalDevice.Acquire();

            Console.WriteLine("Switched to: " + deviceInfo.InstanceName);
            Console.WriteLine("Buttons: " + physicalDevice.Capabilities.ButtonCount);
            Console.WriteLine("Axes: " + physicalDevice.Capabilities.AxeCount);

        }

        private void PopulateBindTargets()
        {
            string[] xboxButtons = new[]
            {
                "A", "B", "X", "Y",
                "Start", "Back", "Guide",
                "LeftShoulder", "RightShoulder",
                "LeftThumb", "RightThumb",
                "Up", "Down", "Left", "Right"
            };


            foreach (var name in xboxButtons)
                bindTargetSelector.Items.Add(name);

            bindTargetSelector.Items.AddRange(new[]
             {
                "LeftThumbX", "LeftThumbY", "RightThumbX", "RightThumbY",
                "LeftTrigger", "RightTrigger"
            });
        }

        private void startBindButton_Click(object sender, EventArgs e)
        {
            if (bindTargetSelector.SelectedItem == null)
                return;

            isWaitingForInput = true;
            bindingTarget = bindTargetSelector.SelectedItem.ToString();
            bindStatus.Text = $"Waiting for input for: {bindingTarget}";
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            boundButtons.Clear();
            axisBindings.Clear();
            bindStatus.Text = "Bindings cleared.";
        }

        private void deviceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeSelectedDevice(deviceSelector.SelectedIndex);
        }

        private void StartInputLoop()
        {
            inputTimer = new System.Windows.Forms.Timer();
            inputTimer.Interval = 10; // 100 Hz
            inputTimer.Tick += UpdateLoop;
            inputTimer.Start();
        }

        private static bool DeviceSupportsAxis(Joystick device, string axisName)
        {
            if (device == null) return false;

            foreach (var obj in device.GetObjects())
            {
                string name = obj.Name?.ToLowerInvariant() ?? "";

                if (axisName == "X" && name.Contains("x axis")) return true;
                if (axisName == "Y" && name.Contains("y axis")) return true;
                if (axisName == "Z" && name.Contains("z axis")) return true;

                if (axisName == "RotationX" && name.Contains("x rotation")) return true;
                if (axisName == "RotationY" && name.Contains("y rotation")) return true;
                if (axisName == "RotationZ" && name.Contains("z rotation")) return true;

                if (axisName == "Slider0" && name.Contains("slider 0")) return true;
                if (axisName == "Slider1" && name.Contains("slider 1")) return true;
            }

            return false;
        }

        private void StartReconnectLoop()
        {
            reconnectTimer = new System.Windows.Forms.Timer();
            reconnectTimer.Interval = 2000; // Check every 2 seconds
            reconnectTimer.Tick += (s, e) => RefreshDevices();
            reconnectTimer.Start();
        }

        private void RefreshDevices()
        {
            for (int i = 0; i < allDevices.Count; i++)
            {
                if (activeDevices[i] == null)
                {
                    try
                    {
                        var stick = new Joystick(directInput, allDevices[i].InstanceGuid);
                        stick.Acquire();
                        activeDevices[i] = stick;
                        Console.WriteLine($"🔄 Reconnected device {i}: {allDevices[i].InstanceName}");
                    }
                    catch
                    {
                        // Still unavailable
                    }
                }
            }
        }


        private static int GetX(JoystickState s) => s.X;
        private static int GetY(JoystickState s) => -s.Y;
        private static int GetZ(JoystickState s) => s.Z;
        private static int GetRX(JoystickState s) => s.RotationX;
        private static int GetRY(JoystickState s) => s.RotationY;
        private static int GetRZ(JoystickState s) => s.RotationZ;
        private static int GetSlider0(JoystickState s) => s.Sliders.Length > 0 ? s.Sliders[0] : 0;
        private static int GetSlider1(JoystickState s) => s.Sliders.Length > 1 ? s.Sliders[1] : 0;
        private static int GetSlider2(JoystickState s) => s.Sliders.Length > 2 ? s.Sliders[2] : 0;

        private void UpdateLoop(object? sender, EventArgs e)
        {
            if (activeDevices.Count == 0) return;

            // Check for input binding
            if (isWaitingForInput)
            {
                // for (int deviceIndex = 0; deviceIndex < activeDevices.Count; deviceIndex++)
                //{

                int deviceIndex = deviceSelector.SelectedIndex;

                if (deviceIndex < 0 || deviceIndex >= activeDevices.Count) return;
                var device = activeDevices[deviceIndex];
                if (device == null) return;

                JoystickState state;
                try
                {
                    state = device.GetCurrentState();
                }
                catch
                {
                    // Mark device as disconnected
                    activeDevices[deviceIndex] = null;
                    Console.WriteLine($"⚠️ Device {deviceIndex} disconnected.");
                    return; // Or continue
                }

                // Button binding
                for (int i = 0; i < state.Buttons.Length; i++)
                {
                    if (state.Buttons[i])
                    {
                        boundButtons[bindingTarget] = (deviceIndex, i);
                        bindStatus.Text = $"Bound {bindingTarget} to Button {i} on Device {deviceIndex}";
                        isWaitingForInput = false;
                        return;
                    }
                }

                // Axis binding
                int threshold = 10000;
                if (Math.Abs(state.X - 32768) > threshold && DeviceSupportsAxis(device, "X"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetX);
                    bindStatus.Text = $"Bound {bindingTarget} to X Axis on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }
                if (Math.Abs(state.Y - 32768) > threshold && DeviceSupportsAxis(device, "Y"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetY);
                    bindStatus.Text = $"Bound {bindingTarget} to Y Axis on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }
                if (Math.Abs(state.Z - 0) > threshold && DeviceSupportsAxis(device, "Z"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetZ);
                    bindStatus.Text = $"Bound {bindingTarget} to Z Axis on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }
                if (Math.Abs(state.RotationZ - 0) > threshold && DeviceSupportsAxis(device, "RotationZ"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetRZ);
                    bindStatus.Text = $"Bound {bindingTarget} to RZ Axis on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }
                if (Math.Abs(state.RotationX - 0) > threshold && DeviceSupportsAxis(device, "RotationX"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetRX);
                    bindStatus.Text = $"Bound {bindingTarget} to RX Axis on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }
                if (Math.Abs(state.RotationY - 0) > threshold && DeviceSupportsAxis(device, "RotationY"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetRY);
                    bindStatus.Text = $"Bound {bindingTarget} to RZ Axis on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }

                if (Math.Abs(state.Sliders[0] - 0) > threshold && DeviceSupportsAxis(device, "Slider0"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetSlider0);
                    bindStatus.Text = $"Bound {bindingTarget} to Slider0 on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }
                if (Math.Abs(state.Sliders[1] - 0) > threshold && DeviceSupportsAxis(device, "Slider1"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetSlider1);
                    bindStatus.Text = $"Bound {bindingTarget} to Slider1 on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }
                if (Math.Abs(state.Sliders[2] - 0) > threshold && DeviceSupportsAxis(device, "Slider2"))
                {
                    axisBindings[bindingTarget] = (deviceIndex, GetSlider2);
                    bindStatus.Text = $"Bound {bindingTarget} to Slider2 on Device {deviceIndex}";
                    isWaitingForInput = false;
                    return;
                }


                // }

                return; // Still waiting for input, nothing found
            }



            // Apply button bindings
            foreach (var pair in boundButtons)
            {
                string name = pair.Key;
                var (deviceIndex, buttonIndex) = pair.Value;

                if (deviceIndex >= activeDevices.Count) continue;
                JoystickState state;
                try
                {
                    state = activeDevices[deviceIndex].GetCurrentState();
                }
                catch
                {
                    // Mark device as disconnected
                    activeDevices[deviceIndex] = null;
                    Console.WriteLine($"⚠️ Device {deviceIndex} disconnected.");
                    return; // Or continue
                }
                bool pressed = buttonIndex < state.Buttons.Length && state.Buttons[buttonIndex];

                if (ButtonNameMap.TryGetValue(name, out var button))
                {
                    virtualController.SetButtonState(button, pressed);
                }
            }

            // Apply axis bindings
            foreach (var pair in axisBindings)
            {
                string name = pair.Key;
                var (deviceIndex, extractor) = pair.Value;

                if (deviceIndex >= activeDevices.Count || activeDevices[deviceIndex] == null)
                    continue;
                JoystickState state;
                try
                {
                    state = activeDevices[deviceIndex].GetCurrentState();
                }
                catch
                {
                    Console.WriteLine($"⚠️ Device {deviceIndex} disconnected.");
                    // Null out the device safely
                    if (deviceIndex < activeDevices.Count)
                        activeDevices[deviceIndex] = null;
                    continue;
                }
                int raw = extractor(state);

                if (name == "LeftTrigger" || name == "RightTrigger")
                {
                    byte value = (byte)Math.Clamp(raw / 257, 0, 255);
                    if (name == "LeftTrigger")
                        virtualController.SetSliderValue(Xbox360Slider.LeftTrigger, value);
                    else if (name == "RightTrigger")
                        virtualController.SetSliderValue(Xbox360Slider.RightTrigger, value);
                }
                else
                {
                    int centered = raw - 32768;
                    int clamped = Math.Clamp(centered, -32768, 32767);
                    short final = (short)clamped;

                    switch (name)
                    {
                        case "LeftThumbX": virtualController.SetAxisValue(Xbox360Axis.LeftThumbX, final); break;
                        case "LeftThumbY": virtualController.SetAxisValue(Xbox360Axis.LeftThumbY, final); break;
                        case "RightThumbX": virtualController.SetAxisValue(Xbox360Axis.RightThumbX, final); break;
                        case "RightThumbY": virtualController.SetAxisValue(Xbox360Axis.RightThumbY, final); break;
                    }
                }
            }

            virtualController.SubmitReport();
        }


        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            public static extern bool AllocConsole();
        }

    }
}
