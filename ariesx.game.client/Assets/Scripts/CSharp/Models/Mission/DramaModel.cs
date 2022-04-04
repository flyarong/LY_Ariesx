using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using System.Text;

namespace Poukoute {
    public class DramaModel : BaseModel {
        /* Add data member in this */

        public List<ChapterTask> dramaList;

        /***************************/

        public void Refresh(List<ChapterTask> chapterTaskList) {
            dramaList = chapterTaskList;
            dramaList.Sort((a, b) => { return a.Id.CompareTo(b.Id); });
            DramaConf conf;
            foreach (ChapterTask task in this.dramaList) {
                conf = DramaConf.GetConf(task.Id.ToString());
                task.unlocked = FteManager.CheckDrama(conf.unlockId);
            }
            /* Refresh your data in this function */
        }

        public ChapterTask GetTaskById(int id) {
            foreach (ChapterTask task in this.dramaList) {
                if (task.Id == id) {
                    return task;
                }
            }
            return null;
        }

        public ChapterTask SetValue(int id, int value) {
            foreach (ChapterTask task in this.dramaList) {
                if (task.Id == id) {
                    task.Value = value;
                    DramaConf conf = DramaConf.GetConf(task.Id.ToString());
                    task.unlocked = FteManager.CheckDrama(conf.unlockId);
                    return task;
                }
            }
            return null;
        }

        public int GetChapterUnDoneTaskID() {
            foreach (ChapterTask task in dramaList) {
                if (!task.IsDone) {
                    return task.Id;
                }
            }
            return 0;
        }

        public bool CheckTaskDone(int taskId) {
            foreach (ChapterTask task in dramaList) {
                if (task.Id == taskId) {
                    return task.IsDone;
                }
            }
            return false;
        }

        public bool CheckChapterReceive(int index) {
            foreach (ChapterTask task in dramaList) {
                if (task.Id == index) {
                    return task.IsCollect;
                }
            }
            return false;
        }

        public int GetCollectableTasksCount() {
            int count = 0;
            foreach (ChapterTask task in this.dramaList) {
                if (task.IsDone && !task.IsCollect && task.unlocked) {
                    ++count;
                }
            }
            if (this.IsChapterRewardReceived()) {
                ++count;
            }
            return count;
        }

        public bool HasUnreceivedTask() {
            foreach (ChapterTask task in this.dramaList) {
                if (task.IsDone && !task.IsCollect && task.unlocked) {
                    return true;
                }
            }
            return false;
        }

        public string GetDramaDetailInMap(out int taskId) {
            taskId = -1;
            if (this.dramaList.Count == 0) {
                return string.Empty;
            }

            foreach (ChapterTask task in this.dramaList) {
                if (task.IsDone && !task.IsCollect && task.unlocked) {
                    taskId = task.Id;
                    break;
                }
            }

            if (taskId == -1) { // Get next drmamission
                foreach (ChapterTask task in this.dramaList) {
                    if (!task.IsDone && task.unlocked) {
                        taskId = task.Id;
                        break;
                    }
                }
            }

            return (taskId == -1) ?
                string.Empty : DramaConf.GetConf(taskId.ToString()).GetName();
        }

        private int GetReceiveableChapterTaskId() {
            foreach (ChapterTask task in this.dramaList) {
                if (task.IsDone && !task.IsCollect && task.unlocked) {
                    return task.Id;
                }
            }
            return -1;
        }

        public bool IsChapterRewardReceived() {
            bool finishAll = (this.GetChapterUnDoneTaskID() == 0);
            bool hasUnreceivedTask = this.HasUnreceivedTask();
            bool hasDramaContent = (this.dramaList.Count > 0);
            if (finishAll && !hasUnreceivedTask && hasDramaContent) {
                return true;
            }
            return false;
        }

        public int GetLastIndex() {
            int taskId = 0;
            if (this.dramaList.Count == 0) {
                return 100;
            }
            foreach (ChapterTask task in this.dramaList) {
                if (task.IsDone && task.Id > taskId) {
                    taskId = task.Id;
                }
            }
            return taskId;
        }
    }
}
