namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SkipDialogButton = new System.Windows.Forms.Button();
            this.FramesHeld = new System.Windows.Forms.NumericUpDown();
            this.ManualBlastControl = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.FramesHeld)).BeginInit();
            this.SuspendLayout();
            // 
            // SkipDialogButton
            // 
            this.SkipDialogButton.Location = new System.Drawing.Point(12, 12);
            this.SkipDialogButton.Name = "SkipDialogButton";
            this.SkipDialogButton.Size = new System.Drawing.Size(114, 31);
            this.SkipDialogButton.TabIndex = 0;
            this.SkipDialogButton.Text = "skip dialog";
            this.SkipDialogButton.UseVisualStyleBackColor = true;
            this.SkipDialogButton.Click += new System.EventHandler(this.SkipDialogButton_Click);
            // 
            // FramesHeld
            // 
            this.FramesHeld.Location = new System.Drawing.Point(14, 51);
            this.FramesHeld.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.FramesHeld.Name = "FramesHeld";
            this.FramesHeld.Size = new System.Drawing.Size(36, 22);
            this.FramesHeld.TabIndex = 1;
            this.FramesHeld.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ManualBlastControl
            // 
            this.ManualBlastControl.AutoSize = true;
            this.ManualBlastControl.Location = new System.Drawing.Point(56, 52);
            this.ManualBlastControl.Name = "ManualBlastControl";
            this.ManualBlastControl.Size = new System.Drawing.Size(127, 20);
            this.ManualBlastControl.TabIndex = 2;
            this.ManualBlastControl.Text = "lock frames held";
            this.ManualBlastControl.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 176);
            this.Controls.Add(this.ManualBlastControl);
            this.Controls.Add(this.FramesHeld);
            this.Controls.Add(this.SkipDialogButton);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.FramesHeld)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SkipDialogButton;
        private System.Windows.Forms.NumericUpDown FramesHeld;
        private System.Windows.Forms.CheckBox ManualBlastControl;
    }
}

