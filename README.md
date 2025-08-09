<img width="780" height="342" alt="image" src="https://github.com/user-attachments/assets/573856d1-1af6-4cc6-9096-ae3b802e91fe" />
<img width="782" height="346" alt="image" src="https://github.com/user-attachments/assets/4a2a11a5-eb0a-4c96-81d0-3b743987b399" />


# 🏎️ RoFFB

**RoFFB** is a Windows **.NET** application that uses [ViGEm](https://vigem.org/) and [DirectInput](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/ee417001(v=vs.85)) to connect **Force Feedback racing wheels** and related peripherals to **Roblox** enabling realistic **force feedback**, pedal inputs, and more.

---

## 📖 Overview

RoFFB allows you to:
- Detect and connect DirectInput-compatible racing wheels, pedals, and other controllers.
- Emulate an Xbox 360 controller using ViGEm for Roblox compatibility.
- Translate real-time **Force Feedback** effects from Roblox into physical feedback on your wheel.
- Customize input mapping and tuning for your specific hardware.

---

## ⚙️ Instructions

### 1️⃣ Requirements
- Windows 10/11 (64-bit)
- [.NET Desktop Runtime 6.0+](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [ViGEmBus Driver](https://vigem.org/download/)
- You can Download ViGEmBus from either the link above or using the DriverSetup.eve included in the .zip
- A DirectInput-compatible racing wheel or pedal set

---

### 2️⃣ Installation
1. Download the latest release from the [Releases](../../releases) page.
2. Extract the `.zip` file to any folder.
3. Ensure the **ViGEmBus driver** is installed properly. 
4. Run `RoFFB.exe`.

---

### 3️⃣ Using RoFFB
1. **Select Device** – Choose your racing wheel or controller from the dropdown.
2. **Map Inputs** – Assign steering, throttle, brake, clutch, and other buttons.
3. **Enable Force Feedback** – Ensure your wheel is detected and feedback effects are active.
4. **Launch Roblox** – Start your game and enjoy realistic wheel feedback.

---

### 4️⃣ How To Bind

1. Navigate to the **Bind tab** and select your target device from the **lower dropdown**.
2. Interact with your target device's axis or button and note the corisponding axis on the display.
3. Select your target bind from the **upper dropdown**.

**Binding an Axis**
1. Select the axis you want to bind and press the bind button.

**Binding a Button**
1. The program will automatically use the most recent button pressed on the selected device.
2. Just select the Button option and press the bind button.

   ---

### 5️⃣ Setting Up Force Feedback

1. Navigate to the **Force tab** and select your target device from the dropdown.
2. Ensure the Force Enabled checkbox is marked on.
3. Press the TestForce button to ensure the connection to your selected device is correct.


