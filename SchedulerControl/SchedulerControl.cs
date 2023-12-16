namespace SchedulerControl
{
    public partial class SchedulerControl : UserControl
    {
        private Panel calendar;
        public Panel Calendar
        { 
            get { return calendar; } 
        }


        private ComboBox dayComboBox;
        private ComboBox timeFromComboBox;
        private ComboBox timeToComboBox;

        private RichTextBox task;

        private Label[] hoursLabels;
        private Label[] daysLabels;

        private TaskList tasks = new TaskList();

        public TaskList Tasks
        { 
            get { return tasks; }
            set { tasks = (TaskList)value; }
        }


        private Button addTaskButton;
        private Button chooseColorButton;
        private Button cancelEditingButton;
        private Button deleteTaskButton;

        private System.Windows.Forms.Timer timer;
        public System.Windows.Forms.Timer TaskTimer
        { get { return timer; } set { timer = value; } }

        private string[] timestamps;
        public string[] Timestamps
        {
            get { return timestamps; }
        }

        private int dayLabelLength = 110;
        private int dayLabelHeight = 20;
        private int calendarLength;
        private int addTaskPanelLength = 160;
        private int taskLength;
        private int taskHeight;

        private int editingTaskId;
        public int EditingTaskId
        {
            get { return editingTaskId; }
            set
            {
                if (value > -1) 
                {
                    deleteTaskButton.Visible = true;
                    cancelEditingButton.Visible = true;
                    addTaskButton.Text = "Edytuj"; 
                }
                else 
                { 
                    deleteTaskButton.Visible = false;
                    cancelEditingButton.Visible = false;
                    resetTaskTextBox();
                    addTaskButton.Text = "Dodaj"; 
                }
                editingTaskId = value;
            }
        }

        public event EventHandler NearestTaskIsNear;
        public event EventHandler ChangedNearestTask;

        public static readonly int DAYS_IN_WEEK = 7;
        public static readonly int MINUTES_PER_BLOCK = 30;
        public static readonly int BLOCKS_PER_DAY = (60 / MINUTES_PER_BLOCK) * 24;

        public SchedulerControl()
        {
            InitializeComponent();

            tasks.owner = this;

            initTimestamps();

            this.AutoScroll = true;

            calendar = new Panel();

            this.calendar.AutoScroll = true;
            this.calendar.Location = new System.Drawing.Point(0, 0);
            this.calendar.Name = "calendar";
            this.calendar.TabIndex = 0;
            this.calendar.TabStop = false;

            initHoursLabels();

            calendarLength = hoursLabels[0].Size.Width + DAYS_IN_WEEK * dayLabelLength;

            taskHeight = hoursLabels[0].Size.Height + 20;
            taskLength = dayLabelLength;

            this.calendar.Size = new System.Drawing.Size(calendarLength, this.Size.Height);

            initDaysLabels();

            this.Size = new System.Drawing.Size(calendarLength + addTaskPanelLength + 5, this.Size.Height + 10);
            this.Controls.Add(this.calendar);

            initAddTaskPanel();

            initTimer();

            EditingTaskId = -1;
        }

        private void initTimestamps()
        {
            timestamps = new string[BLOCKS_PER_DAY];

            for (int i = 0; i < timestamps.Length; i++)
            {
                timestamps[i] = minutesToTimeString(i * MINUTES_PER_BLOCK);
            }
        }

        private static string minutesToTimeString(int totalMinutes)
        {
            int hours = totalMinutes / 60, minutes = totalMinutes % 60;
            string hoursStr, minutesStr;
            if (hours < 10)
            {
                hoursStr = "0" + hours.ToString();
            }
            else { hoursStr = hours.ToString(); }

            if (minutes < 10)
            {
                minutesStr = "0" + minutes.ToString();
            }
            else { minutesStr = minutes.ToString(); }

            return hoursStr + ":" + minutesStr;
        }

        private void initTimer()
        {
            this.timer = new System.Windows.Forms.Timer();

            this.timer.Enabled = true;
            this.timer.Interval = 20;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if(NearestTaskIsNear != null)
            {
                NearestTaskIsNear(this, null);
            }
            resetTaskTimer();
        }

        private bool canAddTask()
        {
            return timeFromComboBox.SelectedIndex != -1 && timeToComboBox.SelectedIndex != -1 && dayComboBox.SelectedIndex != -1
                   && timeToComboBox.SelectedIndex > timeFromComboBox.SelectedIndex;
        }

        private void initHoursLabels()
        {
            hoursLabels = new Label[BLOCKS_PER_DAY];

            for (int i = 0; i < hoursLabels.Length; i++)
            {
                hoursLabels[i] = new Label();

                hoursLabels[i].AutoSize = true;
                hoursLabels[i].Text = timestamps[i];
                hoursLabels[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                hoursLabels[i].Location = new System.Drawing.Point(0, dayLabelHeight + i * (hoursLabels[i].Height + 20));

                this.calendar.Controls.Add(this.hoursLabels[i]);
            }
        }

        private void initDaysLabels()
        {
            int hourLabelLength = hoursLabels[0].Size.Width;

            daysLabels = new Label[DAYS_IN_WEEK];

            for (int i = 0; i < daysLabels.Length; i++)
            {
                daysLabels[i] = new Label();

                daysLabels[i].AutoSize = false;
                daysLabels[i].Location = new System.Drawing.Point(hourLabelLength + (i * dayLabelLength), 0);
                daysLabels[i].Size = new System.Drawing.Size(dayLabelLength, dayLabelHeight);
                daysLabels[i].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

                this.calendar.Controls.Add(this.daysLabels[i]);
            }

            daysLabels[0].Text = "Poniedziałek";
            daysLabels[1].Text = "Wtorek";
            daysLabels[2].Text = "Środa";
            daysLabels[3].Text = "Czwartek";
            daysLabels[4].Text = "Piątek";
            daysLabels[5].Text = "Sobota";
            daysLabels[6].Text = "Niedziela";
        }

        private void initAddTaskPanel()
        {
            task = new RichTextBox();

            this.task.Location = new System.Drawing.Point(calendarLength + 5, dayLabelHeight);
            this.task.Name = "richTextBox1";
            this.task.Size = new System.Drawing.Size(taskLength, taskHeight);
            this.task.TabIndex = 0;
            this.task.Text = "";

            this.Controls.Add(task);

            dayComboBox = new ComboBox();

            this.dayComboBox.FormattingEnabled = true;
            this.dayComboBox.Location = new System.Drawing.Point(calendarLength + 5, task.Location.Y + task.Size.Height + 10);
            this.dayComboBox.Name = "comboBox1";
            this.dayComboBox.Size = new System.Drawing.Size(addTaskPanelLength - 10, 30);
            this.dayComboBox.Items.AddRange(new object[] {
                                            "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela"});
            this.dayComboBox.Text = "dzień tygodnia...";
            this.Controls.Add(this.dayComboBox);

            timeFromComboBox = new ComboBox();

            this.timeFromComboBox.FormattingEnabled = true;
            this.timeFromComboBox.Location = new System.Drawing.Point(calendarLength + 5, dayComboBox.Location.Y + dayComboBox.Size.Height + 10);
            this.timeFromComboBox.Name = "comboBox2";
            this.timeFromComboBox.Size = new System.Drawing.Size(addTaskPanelLength - 10, 30);
            this.timeFromComboBox.Items.AddRange(timestamps);
            this.timeFromComboBox.Text = "godzina od...";
            this.Controls.Add(this.timeFromComboBox);

            timeToComboBox = new ComboBox();

            this.timeToComboBox.FormattingEnabled = true;
            this.timeToComboBox.Location = new System.Drawing.Point(calendarLength + 5, 
                                                                    timeFromComboBox.Location.Y + timeFromComboBox.Size.Height + 10);
            this.timeToComboBox.Name = "comboBox3";
            this.timeToComboBox.Size = new System.Drawing.Size(addTaskPanelLength - 10, 30);
            this.timeToComboBox.Items.AddRange(timestamps);
            this.timeToComboBox.Text = "godzina do...";
            this.Controls.Add(this.timeToComboBox);

            chooseColorButton = new System.Windows.Forms.Button();

            this.chooseColorButton.Location = new System.Drawing.Point(calendarLength + 5, 
                                                                       timeToComboBox.Location.Y + timeToComboBox.Size.Height + 10);
            this.chooseColorButton.Name = "chooseColorButton";
            this.chooseColorButton.Size = new System.Drawing.Size(taskLength, 25);
            this.chooseColorButton.Text = "wybierz kolor...";
            this.chooseColorButton.Click += new System.EventHandler(this.ClickColorButton);

            this.Controls.Add(chooseColorButton);

            addTaskButton = new System.Windows.Forms.Button();

            this.addTaskButton.Location = new System.Drawing.Point(calendarLength + 5,
                                                                   chooseColorButton.Location.Y + chooseColorButton.Size.Height + 10);
            this.addTaskButton.Name = "addTaskButton1";
            this.addTaskButton.Size = new System.Drawing.Size(taskLength, 25);
            this.addTaskButton.Text = "Dodaj/Edytuj";
            this.addTaskButton.Click += new System.EventHandler(this.clickAddTaskButton);

            this.Controls.Add(addTaskButton);

            cancelEditingButton = new System.Windows.Forms.Button();

            this.cancelEditingButton.Location = new System.Drawing.Point(calendarLength + 5,
                                                                         addTaskButton.Location.Y + addTaskButton.Size.Height + 10);
            this.cancelEditingButton.Name = "cancelEditingButton";
            this.cancelEditingButton.Size = new System.Drawing.Size(addTaskPanelLength - 10, 25);
            this.cancelEditingButton.Text = "Anuluj edytowanie";
            this.cancelEditingButton.Click += new System.EventHandler(this.clickCancelEditingButton);

            this.Controls.Add(cancelEditingButton);

            deleteTaskButton = new System.Windows.Forms.Button();

            this.deleteTaskButton.Location = new System.Drawing.Point(calendarLength + 5,
                                                                      cancelEditingButton.Location.Y + cancelEditingButton.Size.Height + 10);
            this.deleteTaskButton.Name = "deleteTaskButton";
            this.deleteTaskButton.Size = new System.Drawing.Size(addTaskPanelLength - 10, 25);
            this.deleteTaskButton.Text = "Usuń";
            this.deleteTaskButton.Click += new System.EventHandler(this.deleteTask);

            this.Controls.Add(deleteTaskButton);
        }
        private void deleteTask(object sender, EventArgs e)
        {
            tasks.Remove(EditingTaskId);

            EditingTaskId = -1;
            
            resetTaskTextBox();
        }

        private void clickCancelEditingButton(object sender, EventArgs e)
        {
            EditingTaskId = -1;
        }

        private void clickAddTaskButton(object sender, EventArgs e)
        {
            if (!canAddTask()) 
            {
                MessageBox.Show("Aby dodać zadanie wybierz wszystkie właściwości i godzina do powinna być późniejsza niż godzina od");
                return;
            }

            if(EditingTaskId > -1)
            {
                Task? task = tasks.findById(EditingTaskId);
                if (task is not null)
                {
                    editTask((Task)task);
                }
                else
                {
                    MessageBox.Show("Nie można edytować zadania. Nie ma zadania o takim id!");
                }
            }
            else
            {
                tasks.Add(new Task(timeFromComboBox.SelectedIndex, timeToComboBox.SelectedIndex, dayComboBox.SelectedIndex,
                                    task.Text, task.BackColor));
            }

            resetTaskTextBox();
        }

        private void editTask(Task editedTask)
        {
            editedTask.TaskColor = task.BackColor;
            editedTask.setTaskDuration(timeFromComboBox.SelectedIndex, timeToComboBox.SelectedIndex);
            editedTask.Day = dayComboBox.SelectedIndex;
            editedTask.Text = task.Text;

            tasks.Edit(editedTask);

            EditingTaskId = -1;
        }

        public void clickTask(int id)
        {
            Task task = tasks.findById(id);
            if(task is not null)
            {
                copyTaskToAddTaskPanel(task);
            }
            
            EditingTaskId = id;
        }

        public void copyTaskToAddTaskPanel(Task copyingTask)
        {
            task.Text = copyingTask.Text;
            timeFromComboBox.SelectedIndex = copyingTask.From;
            timeToComboBox.SelectedIndex = copyingTask.To;
            dayComboBox.SelectedIndex = copyingTask.Day;
            task.BackColor = copyingTask.TaskColor;
        }

        private void ClickColorButton(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                task.BackColor = colorDialog1.Color;
        }

        private void resetTaskTextBox()
        {
            task.Text = "";
            timeFromComboBox.SelectedIndex = -1;
            timeFromComboBox.Text = "godzina od...";
            timeToComboBox.SelectedIndex = -1;
            timeToComboBox.Text = "godzina do...";
            dayComboBox.SelectedIndex = -1;
            dayComboBox.Text = "dzień tygodnia...";
            task.BackColor = Color.White;
        }

        public TimeSpan? getNearestTask(TimeSpan time)
        {
            int day = (int)DateTime.Now.DayOfWeek;

            if (day == 0) { day = 6; }
            else { day--; }

            TimeSpan? result = null;

            foreach (Task task in tasks.ToArray())
            {
                if (task.Day != day) { continue; }
                if (result is null) 
                { 
                    result = task.BeginTask; 
                }
                else if(result >= task.BeginTask)
                {
                    result = task.BeginTask;
                }
            }
            return result;
        }

        public Point getLocation(int from, int day)
        {
            return new System.Drawing.Point(daysLabels[day].Location.X,
                                            hoursLabels[from].Location.Y);
        }

        public Size getSize(int from, int to)
        {
            return new System.Drawing.Size(taskLength, hoursLabels[to].Location.Y - hoursLabels[from].Location.Y);
        }

        public void resetTaskTimer()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan? nearestTask = getNearestTask(now);

            if (nearestTask is null)
            {
                timer.Enabled = false;
            }
            else
            {
                TimeSpan difference = (TimeSpan)nearestTask - now;
                if(difference.Milliseconds > 0)
                {
                    timer.Interval = difference.Milliseconds;
                    timer.Enabled = true;
                }
                else
                {
                    timer.Enabled = false;
                }
            }

            if(ChangedNearestTask is not null)
            {
                ChangedNearestTask(this, null);
            }
        }
    }
}