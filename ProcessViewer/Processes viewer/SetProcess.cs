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
    public partial class SetProcess : Form {
        private ProcessRunInfo processRunInfo;
        private FileService fileService;


        public SetProcess(Main.Command command) {
            InitializeComponent();

            if (command==Main.Command.RUN) {
                button1.Text="Run";
            } else {
                button1.Text="Stop";
                label2.Visible=false;
                textBox2.Visible=false;
            }

            processRunInfo=new ProcessRunInfo();
            processRunInfo.command=command;

            fileService=new FileService();
        }

        private void button1_Click(object sender, EventArgs e) {
            processRunInfo.processKey=textBox2.Text;
            processRunInfo.processName=textBox1.Text;
            DateTime temp= new DateTime();
            temp=temp.AddYears(dateTimePicker1.Value.Year-1);
            temp=temp.AddMonths(dateTimePicker1.Value.Month-1);
            temp=temp.AddDays(dateTimePicker1.Value.Day-1);

            temp=temp.AddHours(dateTimePicker2.Value.Hour);
            temp=temp.AddMinutes(dateTimePicker2.Value.Minute);
            temp=temp.AddSeconds(dateTimePicker2.Value.Second);

            processRunInfo.date=temp;

            Thread thrSave = new Thread(fileService.Save);
            thrSave.IsBackground=true;
            thrSave.Start(processRunInfo);

            this.Close();
        }
    }
}
