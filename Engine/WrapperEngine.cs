/*
 * Created by SharpDevelop.
 * User: delmore
 * Date: 5/28/2014
 * Time: 12:50 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Configuration;

namespace InstallationWrapper.Engine
{
	/// <summary>
	/// Controls all the mechanics and messaging of the Installation Wrapper
	/// application
	/// </summary>
	public class WrapperEngine
	{
		#region Events
		/// <summary>
		/// Single notifier for all messages from Wrapper Engine
		/// </summary>
		public event EventHandler<WrapperArgs> Update;
		
		/// <summary>
		/// Method for invoking Update event
		/// </summary>
		/// <param name="e">Wrapper Arguments</param>
		protected virtual void OnUpdate(WrapperArgs e)
		{
			if(Update != null)
				Update(this,e);
		}
		#endregion
		
		#region Constructor -> Singleton Pattern
		/// <summary>
		/// Variable to hold unique instance of the Wrapper Engine
		/// </summary>
		private static WrapperEngine instance;
		
		/// <summary>
		/// Private constructor for the Wrapper Engine
		/// </summary>
		private WrapperEngine(){	}
		
		/// <summary>
		/// Property for aquiring a single instance of the Wrapper Engine
		/// </summary>
		public static WrapperEngine Instance
		{
			get{
				if(instance == null)
				{
					instance = new WrapperEngine();
				}
				return instance;
			}
		}
		#endregion
		
		#region Constants
		private const int    MINIMUM_TICK_IDENTIFIER      = 0;
		private const string CURRENT_DIRECTORY_IDENTIFIER = @".\";
		#endregion
		
		#region Properties
		public int    MinimumStartTimeInSeconds { get; private set; }
		public int    MaximumStartTimeInSeconds { get; private set; }
		/// <summary>
		/// Drive engine updates and granularity of the timer ticks in seconds. 30 seconds may be optimal.
		/// </summary>
		public int    UpdateIntervalInSeconds   { get; private set; }
		public int    RandomStartTimeInSeconds  { get; private set; }
		public int    TimeRemainingInSeconds    { get; private set; }
		public string FileName                  { get; private set; }
		public string Arguments                 { get; private set; }
		public string Redirect                  { get; private set; }
		#endregion
		
		#region Variables
		private DateTime StartTime;
		private System.Timers.Timer timer;
		private System.Diagnostics.Process proc;
		#endregion
		
		/// <summary>
		/// Starts the Wrapper Engine Proccess
		/// </summary>
		public void Start()
		{
			ThreadStart ths = new ThreadStart(() => StartAsync());
			Thread th = new Thread(ths);
			th.Start();
		}
		
		#region Private
		/// <summary>
		/// Starts the Wrapper Engine without blocking thread
		/// </summary>
		private void StartAsync()
		{
			InitializeFromSettingsFile();
			RandomStartTimeInSeconds = new Random().Next(MinimumStartTimeInSeconds,MaximumStartTimeInSeconds);
			OnUpdate(new WrapperArgs(Status.Starting,null));
			StartCountDown();
		}
		
		/// <summary>
		/// Executes procedure using information from INI file
		/// </summary>
		private void Execute()
		{
			OnUpdate(new WrapperArgs(Status.Running,
			                         string.Format("Running {0} at {1}",FileName,DateTime.Now)));
			ExecuteAsync();
		}
		
		/// <summary>
		/// Populate engine variables with information from settings file
		/// </summary>
		private void InitializeFromSettingsFile()
		{
			MinimumStartTimeInSeconds =
				Convert.ToInt32(ConfigurationManager.AppSettings["MinimumStartTimeInSeconds"]);
			MaximumStartTimeInSeconds =
				Convert.ToInt32(ConfigurationManager.AppSettings["MaximumStartTimeInSeconds"]);
			UpdateIntervalInSeconds =
				Convert.ToInt32(ConfigurationManager.AppSettings["UpdateIntervalInSeconds"]);
			FileName = ConfigurationManager.AppSettings["FileName"];
			Arguments = ConfigurationManager.AppSettings["Arguments"];
		}
		
		/// <summary>
		/// Executes procedure without blocking thread
		/// </summary>
		private void ExecuteAsync()
		{
			ThreadStart ths = new ThreadStart(()=>ExecuteSync());
			Thread th = new Thread(ths);
			th.Start();
		}
		
		/// <summary>
		/// Executes procedure
		/// </summary>
		private void ExecuteSync()
		{
			try
			{
				proc = new System.Diagnostics.Process();
				proc.EnableRaisingEvents = true;
				proc.StartInfo.Arguments = Arguments;
				proc.StartInfo.FileName = FileName;
				proc.Exited += (obj, evt) => ProcedureExited();
				proc.Start();
			}catch(Exception ex)
			{
				OnUpdate(new WrapperArgs(Status.Failed,ex.Message));
			}
			
		}
		
		/// <summary>
		/// Begin countdown to execution of procedure
		/// </summary>
		private void StartCountDown()
		{
			timer = new System.Timers.Timer();
			timer.Interval = UpdateIntervalInSeconds * 1000;
			timer.Elapsed += (obj, evt) => Tick();
			StartTime = DateTime.Now;
			timer.Start();
		}
		
		/// <summary>
		/// Call on on each timer interval.
		/// </summary>
		private void Tick()
		{
			TimeSpan delta = DateTime.Now.Subtract(StartTime);
			TimeRemainingInSeconds =  RandomStartTimeInSeconds - Convert.ToInt32(delta.TotalSeconds);
			if(TimeRemainingInSeconds <=  MINIMUM_TICK_IDENTIFIER)
			{
				timer.Stop();
				TimeRemainingInSeconds = 0; //just to make sure a negative number is not returned to UI
				Execute(); //Execute procedure
			}
			OnUpdate(new WrapperArgs(Status.Update,null));
		}
		
		private void ProcedureExited()
		{
			OnUpdate(new WrapperArgs(Status.Completed,proc.ExitCode.ToString()));
		}
		#endregion
	}
	
	/// <summary>
	/// Argument class for Wrapper events
	/// </summary>
	public class WrapperArgs : EventArgs
	{
		#region Properties
		/// <summary>
		/// Type of event using current status
		/// </summary>
		public Status EventType { get; private set;}
		
		/// <summary>
		/// Message associated with this notification may be null
		/// </summary>
		public string Message {get; private set;}
		#endregion
		
		/// <summary>
		/// Constructor for a Wrapper Engine event arguments
		/// </summary>
		/// <param name="eventType">Type of event using current status</param>
		/// <param name="message">Message associated with this notification may be null</param>
		public WrapperArgs(Status eventType, string message)
		{
			EventType = eventType;
			Message = message;
		}
	}
	
	/// <summary>
	/// Enumerated class that contains the different status of an Wrapper Engine event
	/// </summary>
	public enum Status
	{
		Starting, Running, Update, Ending, Completed, Failed
	}
}
