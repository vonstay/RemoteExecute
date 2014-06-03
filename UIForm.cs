/*
 * Created by SharpDevelop.
 * User: delmore
 * Date: 6/1/2014
 * Time: 2:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using InstallationWrapper.Engine;

namespace InstallationWrapper
{
	/// <summary>
	/// Description of UIForm.
	/// </summary>
	public partial class UIForm : Form
	{
		#region Variables
		WrapperEngine engine;
		#endregion
		private const bool DEBUG_APPLICATION = true;
		private SynchronizationContext synchronizationContext ;
		
		public UIForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Init();
		}
		
		#region Private
		void Init()
		{
			//Set window title if property field else default
			var title = ConfigurationManager.AppSettings["WindowTitle"];
			if(!string.IsNullOrEmpty(title))
				this.Text = title;
			
			synchronizationContext = SynchronizationContext.Current;
			engine = WrapperEngine.Instance;
			engine.Update += (obj,evt) =>
			{
				switch(evt.EventType)
				{
					case Status.Completed:
						ProcedureCompleted(evt.Message);
						break;
					case Status.Failed:
						ProcedureFailed(evt.Message);
						break;
					case Status.Starting:
						EngineStarting();
						break;
					case Status.Running:
						ProcedureRunning();
						break;
					default:
						UpdateUI();
						break;
				}
			};
			engine.Start();
		}
		
		void ProcedureCompleted(string message)
		{
			WriteLogMessage(message, "Completed");
			Application.Exit();
		}
		
		void ProcedureFailed(string message)
		{
			WriteLogMessage(message, "Failure");
		}
		
		void ProcedureRunning()
		{
			WriteLogMessage(engine.FileName + " " + engine.Arguments, "Starting");
		}
		
		void EngineStarting()
		{
			synchronizationContext.Send(
				(d) =>
				{
					progressBar.Maximum = progressBar.Value = engine.RandomStartTimeInSeconds;
					remainingTimeLabel.Text = string.Format("Time Until Execution: {0}", GetTimeLeft());
				},null);
		}
		
		void UpdateUI()
		{
			synchronizationContext.Send((d) => UpdateUISafe(),null);
			WriteLogMessage(engine.TimeRemainingInSeconds.ToString(), "Remaining Time");
		}
		
		void UpdateUISafe()
		{
			remainingTimeLabel.Text = string.Format("Time Until Execution: {0}", GetTimeLeft());
			progressBar.Value = engine.TimeRemainingInSeconds;
			progressBar.Update();
		}
		
		string GetTimeLeft()
		{
			return TimeSpan.FromSeconds(engine.TimeRemainingInSeconds).ToString();
		}
		
		void WriteLogMessage(string message, string category)
		{
			System.Diagnostics.Debug.WriteLineIf(DEBUG_APPLICATION, message, category);
		}
		#endregion
	}
}
