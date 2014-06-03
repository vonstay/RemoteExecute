/*
 * Created by SharpDevelop.
 * User: delmore
 * Date: 6/1/2014
 * Time: 2:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace InstallationWrapper
{
	partial class UIForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.remainingTimeLabel = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// reminaingTimeLabel
			// 
			this.remainingTimeLabel.Location = new System.Drawing.Point(13, 38);
			this.remainingTimeLabel.Name = "reminaingTimeLabel";
			this.remainingTimeLabel.Size = new System.Drawing.Size(304, 18);
			this.remainingTimeLabel.TabIndex = 0;
			this.remainingTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// progressBar
			// 
			this.progressBar.Enabled = false;
			this.progressBar.Location = new System.Drawing.Point(13, 12);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(305, 23);
			this.progressBar.TabIndex = 1;
			// 
			// UIForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(330, 70);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.remainingTimeLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "UIForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Installation Wrapper";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label remainingTimeLabel;
	}
}
