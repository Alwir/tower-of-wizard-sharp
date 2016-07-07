using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Processes_viewer {
    public partial class Schedule : Form {
        FileService fileServices;

        public Schedule() {
            InitializeComponent();
            fileServices=new FileService();
        }

        private void Schedule_Load(object sender, EventArgs e) {
            Thread schProcess = new Thread(ShowScheduleInfo);
            schProcess.IsBackground=true;
            schProcess.Start();
        }

        private void ShowScheduleInfo() {
            while(true) {
                List<ProcessRunInfo> allSchedule = fileServices.Load();
                List<ListViewItem> scheduleItems = new List<ListViewItem>();
                bool doubleRow = true;
                foreach(ProcessRunInfo procSchedule in allSchedule) {
                    var listViewItem = new ListViewItem(new string[]
                    {
                    procSchedule.processName,
                    procSchedule.processKey,
                    procSchedule.date.ToString(),
                    procSchedule.command.ToString()
                    });
                    listViewItem.Name=procSchedule.processName;
                    if(doubleRow) {
                        listViewItem.BackColor=Color.FromArgb(200, 255, 255);
                        doubleRow=false;
                    } else {
                        doubleRow=true;
                    }
                    scheduleItems.Add(listViewItem);
                }

                try {
                    BeginInvoke(new Action<List<ListViewItem>>(UpdateListView), scheduleItems);
                } catch(Exception e) {
                    Logger.SaveLog(e.Message);
                    break;
                }

                Thread.Sleep(100);
            }
        }

        private void UpdateListView(List<ListViewItem> allSchedule) {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            listView1.Items.AddRange(allSchedule.ToArray());
            listView1.EndUpdate();
        }
    }
}
