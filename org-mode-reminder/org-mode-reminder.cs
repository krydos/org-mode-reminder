/*   This file is part of Org-mode Reminder.

    Org-mode Reminder is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Org-mode Reminder is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>. */

using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Principal;
using Snarl.V44;

namespace orgmodereminder
{
	public class MainClass : Form
	{
		private NotifyIcon trayIcon;
		private ContextMenu trayMenu;

		#region Snarl 
		SnarlInterface snarl = new SnarlInterface();

		// Snarl massage types
		const String SnarlClassNormal = "Normal";
		const String SnarlClassCritical = "Critical";
		const String SnarlClassLow = "Low";

		const int NormalMsgCallbackValue = 1;
		String snarlPassword = CreateSnarlPassword(8);

		enum SnarlActions
		{
			DoSomething = 1,
			DoSomethingElse,
		}
		#endregion

		[STAThread]
		public static void Main ()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			MainClass MainForm = new MainClass();
			Application.Run();
		}

		// Constructor
		public MainClass ()
		{
			trayIcon = new NotifyIcon ();
			trayMenu = new ContextMenu ();
			
			trayMenu.MenuItems.Add ("Exit", OnExit);
			trayMenu.MenuItems.Add ("Settings", OnSettings);
			trayIcon.Text = "org-mode reminder";
			trayIcon.Icon = new Icon ("app.ico", 40, 40);
			
			trayIcon.ContextMenu = trayMenu;
			trayIcon.Visible = true;

			InitialzeSnarl ();

			if (!System.IO.File.Exists (Settings.configFile)) {
				Settings settingForm = new Settings ();
				settingForm.FormClosed += new FormClosedEventHandler (settingForm_IsClosed);
				settingForm.Show ();
			} else 
			{
				Thread mainLoop = new Thread(StartMainLoop);
				mainLoop.IsBackground = true;
				mainLoop.Start();
			}
		}

		// start main loop when setting form is closed
		private void settingForm_IsClosed (object sender, EventArgs e)
		{
			Thread mainLoop = new Thread(StartMainLoop);
			mainLoop.IsBackground = true;
			mainLoop.Start();
		}

		// If user click on "Exit" button from context menu 
		private void OnExit (object sender, EventArgs e) 
		{
			snarl.Unregister();
			Application.Exit();
		}

		// If user click on "Settings" button from context menu
		private void OnSettings (object sender, EventArgs e)
		{
			Settings SettingsForm = new Settings();
			SettingsForm.Show ();
		}
		// Initialze snarl
		private void InitialzeSnarl ()
		{
			var vers = SnarlInterface.GetVersion();

			// ReRegisterSnarl() is called when first starting, and when a launch of Snarl is detected after this program is started.
			ReRegisterSnarl();

			snarl.CallbackEvent += CallbackEventHandler;
			snarl.GlobalSnarlEvent += (snarlInstance, args) =>
			{
			 	if (args.GlobalEvent == SnarlGlobalEvent.SnarlLaunched)
				 	ReRegisterSnarl();
			};
		}

		private void ReRegisterSnarl()
		{
		 	int result = 0;
			String snarlIcon = Application.StartupPath+@"\app.ico";
		 	
		 	result = snarl.RegisterWithEvents("application/org-mode-reminders", "Emacs org-mode reminder", snarlIcon, snarlPassword);
		
			snarl.AddClass(SnarlClassNormal, "Normal messages");
			snarl.AddClass(SnarlClassCritical, "Critical messages");
			snarl.AddClass(SnarlClassLow, "Low priority messages");
		}

		private void CallbackEventHandler(object sender, SnarlCallbackEventArgs e)
		{
			
		 	switch (e.SnarlEvent)
		 	{
			 	case SnarlStatus.NotifyAction:
					HandleActionCallback(e.Parameter, e.MessageToken);
				 	break;
				
/*				case SnarlStatus.NotifyInvoked:
					case SnarlStatus.CallbackInvoked:
					 	if (e.Parameter == NormalMsgCallbackValue)
							test = "1";
					 	else
							test = "2";
				 	break;
				
					 	case SnarlStatus.NotifyExpired:
					 		case SnarlStatus.CallbackTimedOut:
								test = "3";
						 		break;
				
				 	default:
						test = "4";	
					 	break;*/
			}
	 	}
	
		private void HandleActionCallback(UInt16 actionData, int msgToken)
		{
			/*//test block
			String test;
			switch ((SnarlActions)actionData)
			{
				case SnarlActions.DoSomething:
					test = "1";
				 	break;
			 	case SnarlActions.DoSomethingElse:
					test = "2";
				 	break;
			}*/
		}

		private void StartMainLoop()
		{
			while (true)
			{
				OrgFile of;
				try
				{
					of = new OrgFile ();
				}
				catch(System.IO.IOException)
				{
					of = null;
				}
				if (of != null)
				{
					List<Task> taskList = of.getAllTasks ();
					DateTime now = DateTime.Now;
					now = now.AddTicks(- (now.Ticks % TimeSpan.TicksPerSecond)); // remove seconds

					foreach(Task task in taskList)
					{
						if(task.getScheduled().CompareTo(now) == 0 || task.getDeadline().CompareTo(now) == 0)
						{
							// play sound
							System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"app.wav");
							player.Play();

							// display notification
							String uid = Guid.NewGuid ().ToString ();
							Int32 id = snarl.Notify (uId: uid,
								                  	classId: SnarlClassNormal,
	            		                  			title: task.getTitle (),
	                		              			text: task.getDescription (),
	                    		                	priority: SnarlMessagePriority.Normal,
	                        		            	callback: "@" + NormalMsgCallbackValue,
					                         		icon: Application.StartupPath+@"\app.ico",
	                            		      		actions: GetDefaultActions ());
						}
					}

					Thread.Sleep(1000);
				}
			}
		}

		private SnarlAction[] GetDefaultActions ()
		{
			return new SnarlAction[]
		 	{
			 	new SnarlAction () { Label = "Do something", Callback = "@" + (int)SnarlActions.DoSomething },
			 	new SnarlAction () { Label = "Do something else", Callback = "@" + (int)SnarlActions.DoSomethingElse },
			};
		}
		
	 	private static string CreateSnarlPassword(int lenght)
		{
			String pass = WindowsIdentity.GetCurrent().Name.ToString() + "Snarl";
			return pass;
		}
	}
}

