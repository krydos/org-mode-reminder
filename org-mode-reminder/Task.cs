/*  
	Written and maintained by KryDos (furyinbox@gmail.com)

	This file is part of Org-mode Reminder.

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

