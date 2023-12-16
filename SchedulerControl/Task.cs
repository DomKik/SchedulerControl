using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerControl
{
    public class Task
    {
        private int id;
        private int from;
        public int From
        {
            get { return from; }
        }
        private int to;
        public int To
        {
            get { return to; }
        }
        private int day;
        public int Day
        {
            get { return day; }
            set
            {
                if (value >= 0 && value < SchedulerControl.DAYS_IN_WEEK)
                {
                    day = value;
                    TaskBox.Location = owner.getLocation(this.From, value);
                }
            }
        }
        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set 
            { 
                enabled = value;
                if (value) { TaskBox.Visible = true; }
                else { TaskBox.Visible = false; }
            }
        }
        private bool defaultClickTaskEvent;
        public bool DefaultClickTaskEvent
        {
            get { return enabled; }
            set
            {
                enabled = value;
            }
        }
        public string Text
        {
            get { return TaskBox.Text; }
            set { TaskBox.Text = value; }
        }

        private TimeSpan beginTask;
        public TimeSpan BeginTask
        {
            get { return beginTask; }
        }

        private TextBox textBox;
        public TextBox TaskBox
        {
            get { return textBox; } 
        }

        public Color TaskColor
        {
            get { return TaskBox.BackColor; }
            set { TaskBox.BackColor = value; }
        }

        private SchedulerControl? owner = null;
        public SchedulerControl? Owner
        {
            get { return owner; }
            set 
            { 
                owner = value;
                if(value is not null)
                {
                    TaskBox.Location = owner.getLocation(from, day);
                    TaskBox.Size = owner.getSize(this.From, this.To);
                }
            }
        }

        public event EventHandler TaskClick;

        private static int freeId = 0;

        public Task(int From, int To, int Day, string Text, Color color)
        {
            TextBox textBox = new TextBox();
            textBox.Text = Text;
            textBox.BackColor = color;
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox.AutoSize = false;
            textBox.Click += new System.EventHandler(this.clickTask);
            textBox.Multiline = true;

            this.textBox = textBox;
            day = Day;
            from = From;
            to = To;

            this.id = freeId++;
            DefaultClickTaskEvent = true;
            Enabled = true;
            textBox.TabIndex = this.id;
        }

        public Task()
        {
            this.id = freeId++;
            DefaultClickTaskEvent = true;
            Enabled = true;

            textBox = new TextBox();
            textBox.Multiline = true;
            textBox.AutoSize = false;
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        public int Id()
        {
            return id;
        }

        public void setTaskDuration(int begin, int end)
        {
            if (begin >= end) { return; }

            from = begin;
            to = end;

            TaskBox.Location = owner.getLocation(begin, this.day);
            TaskBox.Size = owner.getSize(begin, end);

            beginTask = new TimeSpan((int)(From / 2), (int)(From % 2), 0);
        }

        private void clickTask(object sender, EventArgs e)
        {
            if (null != TaskClick)
            {
                TaskClick(this, null);
            }
            if(this.DefaultClickTaskEvent && owner is not null)
            {
                owner.clickTask(((TextBox)sender).TabIndex);
            }
        }

    }
}
