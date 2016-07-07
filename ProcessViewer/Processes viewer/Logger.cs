using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Processes_viewer {
    class Logger {

        internal static void SaveLog(ProcessRunInfo processSchedule, string message) {
            string temp = String.Format("{0} {1}[{2}] ({3}) {4} : {5}", DateTime.Now.ToString(),processSchedule.processName,
                processSchedule.processKey,processSchedule.date.ToString(),processSchedule.command,message);

            
            Save(temp);
        }

        internal static void SaveLog(string message) {
            string temp = String.Format("{0} {1}", DateTime.Now.ToString(),message);
            Save(temp);
        }

        private static void Save(string str) {
            Thread thr = new Thread(()=>{
                while(true) {
                    try {
                        using(StreamWriter sw = new StreamWriter("log.txt", true)) {
                            sw.WriteLine(str);
                        }
                        break;
                    } catch { }
                }

            } );
            thr.IsBackground=true;
            thr.Start();
        }
    }
}
