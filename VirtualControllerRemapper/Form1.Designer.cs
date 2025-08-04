using System;
using System.Windows.Forms;

namespace VirtualControllerRemapper
{
    partial class Form1
    {
        private System.Windows.Forms.TabControl tabControl;
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox bindTargetSelector;
        private System.Windows.Forms.Button startBindButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label bindStatus;
        private System.Windows.Forms.ComboBox deviceSelector;
        private System.Windows.Forms.Button btnBindingsTab;
        private System.Windows.Forms.Button btnSettingsTab;

        private ComboBox comboBoxDevices;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            bindTargetSelector = new ComboBox();
            startBindButton = new Button();
            resetButton = new Button();
            bindStatus = new Label();
            deviceSelector = new ComboBox();
            tabControl1 = new TabControl();
            Binding = new TabPage();
            label1 = new Label();
            Force = new TabPage();
            label2 = new Label();
            comboBoxDevices = new ComboBox();
            rightLabel = new Label();
            forceCheck = new CheckBox();
            button1 = new Button();
            forceAmount = new Label();
            btnReset = new Button();
            leftLabel = new Label();
            labelStatus = new Label();
            testTimer = new System.Windows.Forms.Timer(components);
            tabControl1.SuspendLayout();
            Binding.SuspendLayout();
            Force.SuspendLayout();
            SuspendLayout();
            // 
            // bindTargetSelector
            // 
            bindTargetSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            bindTargetSelector.FormattingEnabled = true;
            bindTargetSelector.Location = new Point(52, 37);
            bindTargetSelector.Name = "bindTargetSelector";
            bindTargetSelector.Size = new Size(200, 33);
            bindTargetSelector.TabIndex = 0;
            // 
            // startBindButton
            // 
            startBindButton.Location = new Point(272, 37);
            startBindButton.Name = "startBindButton";
            startBindButton.Size = new Size(100, 34);
            startBindButton.TabIndex = 1;
            startBindButton.Text = "Bind Input";
            startBindButton.UseVisualStyleBackColor = true;
            startBindButton.Click += startBindButton_Click;
            // 
            // resetButton
            // 
            resetButton.Location = new Point(272, 77);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(100, 30);
            resetButton.TabIndex = 2;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            // 
            // bindStatus
            // 
            bindStatus.AutoSize = true;
            bindStatus.Location = new Point(44, 186);
            bindStatus.Name = "bindStatus";
            bindStatus.Size = new Size(135, 25);
            bindStatus.TabIndex = 3;
            bindStatus.Text = "Ready to bind...";
            // 
            // deviceSelector
            // 
            deviceSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            deviceSelector.FormattingEnabled = true;
            deviceSelector.Location = new Point(44, 134);
            deviceSelector.Name = "deviceSelector";
            deviceSelector.Size = new Size(320, 33);
            deviceSelector.TabIndex = 4;
            deviceSelector.SelectedIndexChanged += deviceSelector_SelectedIndexChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(Binding);
            tabControl1.Controls.Add(Force);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(465, 270);
            tabControl1.TabIndex = 5;
            // 
            // Binding
            // 
            Binding.Controls.Add(label1);
            Binding.Controls.Add(bindTargetSelector);
            Binding.Controls.Add(deviceSelector);
            Binding.Controls.Add(startBindButton);
            Binding.Controls.Add(bindStatus);
            Binding.Controls.Add(resetButton);
            Binding.Location = new Point(4, 34);
            Binding.Name = "Binding";
            Binding.Padding = new Padding(3);
            Binding.Size = new Size(457, 232);
            Binding.TabIndex = 0;
            Binding.Text = "Binding";
            Binding.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(313, 202);
            label1.Name = "label1";
            label1.Size = new Size(141, 25);
            label1.TabIndex = 5;
            label1.Text = "By: Egg_Person0";
            // 
            // Force
            // 
            Force.Controls.Add(label2);
            Force.Controls.Add(comboBoxDevices);
            Force.Controls.Add(rightLabel);
            Force.Controls.Add(forceCheck);
            Force.Controls.Add(button1);
            Force.Controls.Add(forceAmount);
            Force.Controls.Add(btnReset);
            Force.Controls.Add(leftLabel);
            Force.Controls.Add(labelStatus);
            Force.Location = new Point(4, 34);
            Force.Name = "Force";
            Force.Padding = new Padding(3);
            Force.Size = new Size(457, 232);
            Force.TabIndex = 1;
            Force.Text = "Force";
            Force.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(313, 202);
            label2.Name = "label2";
            label2.Size = new Size(141, 25);
            label2.TabIndex = 14;
            label2.Text = "By: Egg_Person0";
            // 
            // comboBoxDevices
            // 
            comboBoxDevices.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDevices.Location = new Point(38, 24);
            comboBoxDevices.Name = "comboBoxDevices";
            comboBoxDevices.Size = new Size(256, 33);
            comboBoxDevices.TabIndex = 0;
            comboBoxDevices.SelectedIndexChanged += comboBoxDevices_SelectedIndexChanged;
            // 
            // rightLabel
            // 
            rightLabel.AutoSize = true;
            rightLabel.Location = new Point(192, 129);
            rightLabel.Name = "rightLabel";
            rightLabel.Size = new Size(59, 25);
            rightLabel.TabIndex = 13;
            rightLabel.Text = "label1";
            // 
            // forceCheck
            // 
            forceCheck.AutoSize = true;
            forceCheck.Location = new Point(38, 99);
            forceCheck.Name = "forceCheck";
            forceCheck.Size = new Size(149, 29);
            forceCheck.TabIndex = 7;
            forceCheck.Text = "Force Enabled";
            forceCheck.UseVisualStyleBackColor = true;
            forceCheck.CheckedChanged += forceCheck_CheckedChanged;
            // 
            // button1
            // 
            button1.Location = new Point(38, 129);
            button1.Name = "button1";
            button1.Size = new Size(106, 29);
            button1.TabIndex = 12;
            button1.Text = "Test Force";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // forceAmount
            // 
            forceAmount.AutoSize = true;
            forceAmount.Location = new Point(192, 161);
            forceAmount.Name = "forceAmount";
            forceAmount.Size = new Size(59, 25);
            forceAmount.TabIndex = 8;
            forceAmount.Text = "label1";
            // 
            // btnReset
            // 
            btnReset.Location = new Point(314, 24);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(75, 29);
            btnReset.TabIndex = 11;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            // 
            // leftLabel
            // 
            leftLabel.AutoSize = true;
            leftLabel.Location = new Point(192, 99);
            leftLabel.Name = "leftLabel";
            leftLabel.Size = new Size(59, 25);
            leftLabel.TabIndex = 9;
            leftLabel.Text = "label1";
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(38, 65);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(59, 25);
            labelStatus.TabIndex = 10;
            labelStatus.Text = "label1";
            // 
            // testTimer
            // 
            testTimer.Interval = 500;
            testTimer.Tick += testTimer_Tick;
            // 
            // Form1
            // 
            ClientSize = new Size(465, 270);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Roblox FFB";
            FormClosing += Form1_FormClosing;
            tabControl1.ResumeLayout(false);
            Binding.ResumeLayout(false);
            Binding.PerformLayout();
            Force.ResumeLayout(false);
            Force.PerformLayout();
            ResumeLayout(false);

        }

        private CheckBox forceCheck;
        private Label forceAmount;
        private Label leftLabel;
        private Label labelStatus;
        private Button btnReset;
        private Button button1;
        private Label rightLabel;
        private TabControl tabControl1;
        private TabPage Binding;
        private TabPage Force;
        private System.Windows.Forms.Timer testTimer;
        private Label label1;
        private Label label2;
    }
}
