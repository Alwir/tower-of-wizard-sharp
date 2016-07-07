using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Processes_viewer {
    static class Logger {

        const string FILENAME = "log.txt";

        internal static void SaveLog(ProcessRunInfo processSchedule, string message) {
            string temp = $"{DateTime.Now.ToString()} {processSchedule.processName}[{processSchedule.processKey}] "+
               $"({processSchedule.date.ToString()}) {processSchedule.command} : {message}";

            Save(temp);
        }

        internal static void SaveLog(string message) {
            string temp = $"{DateTime.Now.ToString()} {message}";
            Save(temp);
        }

        private static void Save(string str) {
            Thread thr = new Thread(()=>{
                while(true) {
                    try {
                        using(StreamWriter sw = new StreamWriter(FILENAME, true)) {
                            sw.WriteLine(str);
                        }
                        break;
                    } catch {
                        //do nothing
                    }
                }

            } );
            thr.IsBackground=true;
            thr.Start();
        }
    }
}
