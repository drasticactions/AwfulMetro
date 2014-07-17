#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace AwfulMetro.Pcl.Core.Tools
{
    public class BackgroundTaskUtils
    {
        public const string BackgroundTaskEntryPoint = "AwfulMetro.BackgroundStatus.BackgroundNotifyStatus";
        public const string BackgroundTaskName = "BackgroundNotifyStatus";
        public static string BackgroundTaskProgress = string.Empty;
        public static bool BackgroundTaskRegistered = false;

        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(string taskEntryPoint, string name,
            IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            var builder = new BackgroundTaskBuilder {Name = name, TaskEntryPoint = taskEntryPoint};

            builder.SetTrigger(trigger);


            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
            BackgroundTaskRegistration task = builder.Register();
            return task;
        }

        public static void UnregisterBackgroundTasks(string name)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(true);
                }
            }

            UpdateBackgroundTaskStatus(name, false);
        }

        public static void UpdateBackgroundTaskStatus(string name, bool registered)
        {
            switch (name)
            {
                case BackgroundTaskName:
                    BackgroundTaskRegistered = registered;
                    break;
            }
        }

        public static string GetBackgroundTaskStatus(string name)
        {
            bool registered = false;
            switch (name)
            {
                case BackgroundTaskName:
                    registered = BackgroundTaskRegistered;
                    break;
            }

            string status = registered ? "Registered" : "Unregistered";

            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(name))
            {
                status += " - " + settings.Values[name];
            }

            return status;
        }
    }
}