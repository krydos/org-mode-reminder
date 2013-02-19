using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace orgmodereminder
{
	public class OrgFile
	{
		private List<string> FilePath;

		public OrgFile ()
		{
			Settings s = new Settings();
			FilePath = s.getOrgFilesPath();
		}

		// get all lines from OrgFile this mean that we get all lines from ALL org files
		public string[] getAllLineOfFile ()
		{
			List<string> FileLines = new List<string>();
			foreach (string path in FilePath) 
			{
				if(System.IO.File.Exists(path))
				{
					string[] temp = System.IO.File.ReadAllLines(path);
					FileLines.AddRange(temp);
				}
			}
			
			return FileLines.ToArray();
		}

		// method which return the all task from Org file(s)
		public List<Task> getAllTasks ()
		{
			// task array
			List<Task> taskArray = new List<Task>();

			string thisIsTaskPattern = @"^\*{1,}";

			// all lines from org file(s)
			string[] fileLines = this.getAllLineOfFile ();

			int index = 0;

			// get information about all task from org file(s)
			foreach (string line in fileLines) 
			{
				Task task = new Task();
				Regex thisIsTask = new Regex(thisIsTaskPattern);

				if (thisIsTask.IsMatch(line))
			    {
					string title = getCleanTitle(line);
					string description = getCleanDescription(line);
					
					string scheduled = getScheduledTime(index+1);
					if(scheduled != "")
					{
						task.setScheduled(DateTime.Parse(scheduled));
					}

					string deadline = getDeadlineTime(index+1);
					if(deadline != "")
					{
						task.setDeadline(DateTime.Parse(deadline));
					}

					task.setTitle(title);
					task.setDescription(description);
					
					taskArray.Add (task);
				}

				index++;
			}

			return taskArray;
		}

		private string getScheduledTime (int index)
		{
			string scheduled;
			string[] fileLines = this.getAllLineOfFile ();
			string scheduledFinderPattern = @"SCHEDULED:\s+<";
			string getScheduledPattern = @"<(.+?)>";
			string thisIsNewTaskPattern = @"^\*{1,}";
			Regex scheduledFinder = new Regex(scheduledFinderPattern);
			Regex getScheduled = new Regex(getScheduledPattern);
			Regex thisIsNewTask = new Regex(thisIsNewTaskPattern);

			while (true)
			{
				if(fileLines.Length == index)
				{
					return "";
				}

				if(thisIsNewTask.IsMatch(fileLines[index]))
				{
					return "";
				}

				if(scheduledFinder.IsMatch(fileLines[index]))
				{
					// remove "<" and ">" from scheduled time. 
					scheduled = getScheduled.Match(fileLines[index]).Value;
					scheduled = scheduled.Replace("<", ""); // TODO not pretty. Change this.
					scheduled = scheduled.Replace(">", ""); // TODO not pretty. Change this.
					return scheduled;
				}

				index++;
			}
		}

		private string getDeadlineTime (int index)
		{
			string deadline;
			string[] fileLines = this.getAllLineOfFile ();
			string deadlineFinderPattern = @"DEADLINE:\s+<";
			string getDeadlinePattern = @"<(.+?)>";
			string thisIsNewTaskPattern = @"^\*{1,}";
			Regex deadlineFinder = new Regex(deadlineFinderPattern);
			Regex getDeadline = new Regex(getDeadlinePattern);
			Regex thisIsNewTask = new Regex(thisIsNewTaskPattern);
			
			while (true)
			{
				if(fileLines.Length == index)
				{
					return "";
				}

				if(thisIsNewTask.IsMatch(fileLines[index]))
				{
					return "";
				}
				
				if(deadlineFinder.IsMatch(fileLines[index]))
				{
					//remove "<" and ">" from deadline time.
					deadline = getDeadline.Match(fileLines[index]).Value;
					deadline = deadline.Replace("<", ""); // TODO not pretty. Change this.
					deadline = deadline.Replace(">", ""); // TODO not pretty. Change this.
					return deadline;
				}
				
				index++;
			}
		}

		// get line with task and return only task name without tags or stars or "todo" | "done" words
		private string getCleanTitle(string line)
		{
			string getTagPattern = @":(.+):";
			Regex getTag = new Regex(getTagPattern);

			string title = getTag.Match(line).Value;
			title = title.Replace(":", "");

			return title;
		}

		// get line with task and return only tag of task
		private string getCleanDescription(string line)
		{
			string removeStarsPattern = @"^(\*)+";
			string removeTODOorDONEPattern = @"(DONE)|(TODO)";
			string removeTagPattern = @"(:.+:)";

			Regex removeStars = new Regex(removeStarsPattern);
			Regex removeTODOorDONE = new Regex(removeTODOorDONEPattern);
			Regex removeTag = new Regex(removeTagPattern);
			
			string description = removeStars.Replace(line, "");
			description = removeTODOorDONE.Replace(description, "");
			description = removeTag.Replace(description, "");
			description = description.Trim();

			return description;
		}
	}
}

