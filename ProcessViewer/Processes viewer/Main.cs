
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Processes_viewer {
    public partial class Main : Form {
        FileService fileServices;
        Form schedule;

        public enum Command {
            RUN,
            STOP
        }

        public Main() {
            InitializeComponent();

            fileServices=new FileService();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Thread thrProcess = new Thread(ShowProcessInfo);
            thrProcess.IsBackground=true;
            thrProcess.Start();

            Thread thrSchedule = new Thread(Schedule);
            thrSchedule.IsBackground=true;
            thrSchedule.Start();

        }

        private void Schedule() {
            while(true) {
                List<ProcessRunInfo> allSchedule= fileServices.Load();
                bool isChange = false;
                List<ProcessRunInfo> newSchedule = new List<ProcessRunInfo>();

                foreach (ProcessRunInfo processSchedule in allSchedule) {
                    if(processSchedule.date.CompareTo(DateTime.Now)<0) {
                        if(processSchedule.command==Command.RUN) {
                            try {
                                Process.Start(processSchedule.processName, processSchedule.processKey);
                            } catch (Exception e) {
                                Logger.SaveLog(processSchedule, e.Message);
                            }
                        } else {
                            try {
                                Process proc = Process.GetProcessesByName(processSchedule.processName)[0];
                                proc.Kill();
                            } catch(Exception e) {
                                Logger.SaveLog(processSchedule, e.Message);
                            }
                        }
                        isChange=true;
                    } else {
                        newSchedule.Add(processSchedule);
                    }
                }
                if(isChange) {
                    fileServices.SaveAll(newSchedule);
                }
            }
        }

        private void ShowProcessInfo() {
            while(true) {
                List<ListViewItem> processItems = GetProcessesInfo();
                BeginInvoke(new Action<List<ListViewItem>>(UpdateListView), processItems);
            }
        }

        private List<ListViewItem> GetProcessesInfo() {
            List<ListViewItem> processItems = new List<ListViewItem>();
            var doubleRow = false;
            foreach(Process process in Process.GetProcesses()) {
                var listViewItem = new ListViewItem(new string[]
                {
                    process.Id.ToString(),
                    process.ProcessName,
                    ProcessInfo.GetTimeInfo(process),
                    process.MachineName,
                    ProcessInfo.GetModuleInfo(process),
                    ProcessInfo.GetPriorityInfo(process),
                    process.Threads.Count.ToString(),
                    ProcessInfo.GetProcessorTime(process),
                    process.WorkingSet64.ToString()

                });
                listViewItem.Name=process.Id.ToString();
                if(doubleRow) {
                    listViewItem.BackColor=Color.FromArgb(200, 255, 255);
                    doubleRow=false;
                } else {
                    doubleRow=true;
                }
                processItems.Add(listViewItem);
            }

            return processItems;
        }

        private void UpdateListView(List<ListViewItem> processItems) {
            listView1.BeginUpdate();
            var top = 0;
            if (listView1.Items.Count>0) {
                top=listView1.GetItemAt(10, 10).Index;
            }

            listView1.Items.Clear();
            listView1.Items.AddRange(processItems.ToArray());
            listView1.EnsureVisible(top+22);
            listView1.Refresh();
            listView1.EndUpdate();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            StartSet(Command.RUN);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e) {
            StartSet(Command.STOP);
        }

        private void StartSet(Command command) {
            Form sett = new SetProcess(command);
            sett.Show();
        }

        private void scheduleToolStripMenuItem_Click(object sender, EventArgs e) {
            if(schedule!=null && schedule.Created) {
                schedule.BringToFront();
            } else {
                schedule=new Schedule();
                schedule.Show();
            }
        }
    }
}
