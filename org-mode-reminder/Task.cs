using System;

namespace orgmodereminder
{
	public class Task
	{

		private string title;
		private string description;
		private DateTime scheduled;
		private DateTime deadline;

		public Task ()
		{
		}

		public void setTitle (string taskTitle)
		{
			title = taskTitle;
		}

		public void setDescription (string taskDesc)
		{
			description = taskDesc;
		}

		public void setScheduled(DateTime newScheduledTime)
		{
			scheduled = newScheduledTime;
		}

		public void setDeadline (DateTime newDeadLine)
		{
			deadline = newDeadLine;
		}

		public string getTitle ()
		{
			return title;
		}

		public string getDescription ()
		{
			return description;
		}

		public DateTime getScheduled()
		{
			return scheduled;
		}

		public DateTime getDeadline ()
		{
			return deadline;
		}
	}
}

