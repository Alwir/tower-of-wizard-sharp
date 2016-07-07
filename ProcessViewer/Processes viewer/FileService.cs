using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processes_viewer {
    public struct ProcessRunInfo {
        public Main.Command command;
        public string processName;
        public string processKey;
        public DateTime date;
    }

    class FileService {
        const string SCHEDULEFILENAME = "sav.sav";

        internal void Save(object obj) {
            ProcessRunInfo processRunInfo = (ProcessRunInfo)obj;
            while(true) {
                try {
                    using(StreamWriter sw = new StreamWriter(SCHEDULEFILENAME, true)) {
                        WriteProcessRunInfo(processRunInfo, sw);
                    }
                    break;
                } catch {
                    //do nothing
                }
            }
        }

        internal void SaveAll(List<ProcessRunInfo> allSchedule) {
            while(true) {
                try {
                    using(StreamWriter sw = new StreamWriter(SCHEDULEFILENAME)) {
                        foreach(ProcessRunInfo processRunInfo in allSchedule) {
                            WriteProcessRunInfo(processRunInfo, sw);
                        }
                    }
                    break;
                } catch {
                    //do nothing
                }
            }
        }

        internal List<ProcessRunInfo> Load() {
            List<ProcessRunInfo> allSchedule = new List<ProcessRunInfo>();
            while (true) {
                try {
                    using(StreamReader sr = new StreamReader(SCHEDULEFILENAME, true)) {
                        while(!sr.EndOfStream) {
                            ProcessRunInfo temp = new ProcessRunInfo();
                            temp.processName=sr.ReadLine();
                            temp.processKey=sr.ReadLine();
                            temp.date=DateTime.Parse(sr.ReadLine());
                            temp.command=(Main.Command)Enum.Parse(typeof(Main.Command), sr.ReadLine());
                            allSchedule.Add(temp);
                        }
                    }
                    break;
                } catch {
                    //do nothing
                }
            }
            return allSchedule;
        }

        private static void WriteProcessRunInfo(ProcessRunInfo processRunInfo, StreamWriter sw) {
            sw.WriteLine(processRunInfo.processName);
            sw.WriteLine(processRunInfo.processKey);
            sw.WriteLine(processRunInfo.date);
            sw.WriteLine(processRunInfo.command);
        }
    }
}
