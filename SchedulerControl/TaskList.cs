using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerControl
{
    public class TaskList : List<Task> 
    {
        public event EventHandler OnAdd;
        public event EventHandler OnRemove;
        public event EventHandler OnEdit;
        public event EventHandler OnColision;

        public SchedulerControl owner;

        public new void Add(Task item)
        {
            if (isColision(item)) 
            {
                if(OnColision != null)
                {
                    OnColision(this, null);
                }
                return;
            }
            if (null != OnAdd)
            {
                OnAdd(this, null);
            }
            item.Owner = owner;
            base.Add(item);
            owner.Calendar.Controls.Add(item.TaskBox);

            owner.resetTaskTimer();
        }

        public Task? findById(int index)
        {
            return base.Find(x => x.Id() == index);
        }

        public void Edit(Task editedTask)
        {
            if (null != OnEdit)
            {
                OnEdit(this, null);
            }

            int index =  base.FindIndex(x => x.Id() == editedTask.Id());
            base[index] = editedTask;

            owner.resetTaskTimer();
        }

        public void Remove(int id)
        {
            if (null != OnRemove)
            {
                OnRemove(this, null);
            }

            int indexInList = base.FindIndex(x => x.Id() == id);
            base[indexInList].TaskBox.Dispose();
            base.RemoveAt(indexInList);

            owner.resetTaskTimer();
        }

        public bool isColisionBetweenTwoTasks(Task t1, Task t2)
        {
            if(t1.Day != t2.Day || t1.Enabled == false || t2.Enabled == false) { return false; }

            return (t1.From > t2.From && t2.From < t1.To) || (t2.From > t1.From && t1.From < t2.To);
        }

        public bool isColision(Task newTask)
        {
            foreach(Task el in base.ToArray())
            {
                if(isColisionBetweenTwoTasks(el, newTask))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
