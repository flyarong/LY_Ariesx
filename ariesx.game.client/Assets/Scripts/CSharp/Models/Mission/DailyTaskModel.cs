using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace Poukoute {  
	public class DailyTaskModel : BaseModel {
        public List<Task> taskList = new List<Task>(30);
        public List<int> vitalityCanGetReward = 
            new List<int>(GameConst.DAILY_TASK_STAGE);
        public int AllVitality = 0;
        public int TownhallRequireLevel = 3;
        public int CurrentTaskTownHallLevel = 0;

        public int countShow = 10;
        public int tailIndex = 0;

        public long dailyExpireAt;

        public void HowManyIsDone(out int isDone, out int max) {
            int count = 0;
            foreach (var task in taskList) {
                if (task.IsDone) {
                    count++;
                }
            }
            max = taskList.Count;
            isDone = count;
        }

        public bool SetTaskCollected(int taskId) {
            foreach(Task task in this.taskList) {
                if (task.Id == taskId && task.IsDone) {
                    task.IsCollect = true;
                    return true;
                }
            }
            return false;
        }

        public int GetCollectableTasksCount() {
            int count = 0;
            foreach (Task task in this.taskList) {
                if (task.IsDone && !task.IsCollect) {
                    count++;
                }
            }
            return count;
        }
    }
}
