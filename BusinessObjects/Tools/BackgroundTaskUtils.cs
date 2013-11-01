using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace AwfulMetro.Core.Tools
{
    public class BackgroundTaskUtils
    {
        public const string BackgroundTaskEntryPoint = "BackgroundStatus.BackgroundNotifyStatus";
        public const string BackgroundTaskName = "BackgroundNotifyStatus";
        public static string BackgroundTaskProgress = "";
        public static bool BackgroundTaskRegistered = false;

        public async static Task<BackgroundTaskRegistration> RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
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

        public static void UpdateBackgroundTaskStatus(String name, bool registered)
        {
            switch (name)
            {
                case BackgroundTaskName:
                    BackgroundTaskRegistered = registered;
                    break;
            }
        }

        public static String GetBackgroundTaskStatus(String name)
        {
            var registered = false;
            switch (name)
            {
                case BackgroundTaskName:
                    registered = BackgroundTaskRegistered;
                    break;
            }

            var status = registered ? "Registered" : "Unregistered";

            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey(name))
            {
                status += " - " + settings.Values[name].ToString();
            }

            return status;
        }
    }
}
