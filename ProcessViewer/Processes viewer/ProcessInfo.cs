using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processes_viewer {
    
    static class ProcessInfo {
        public static string GetTimeInfo(Process process) {
            try {
                return process.StartTime.ToString();
            } catch {
                return "";
            }
        }

        public static string GetModuleInfo(Process process) {
            try {
                return process.MainModule.ToString();
            } catch {
                return "";
            }
        }

        public static string GetPriorityInfo(Process process) {
            try {
                return process.PriorityClass.ToString();
            } catch {
                return "";
            }
        }

        public static string GetProcessorTime(Process process) {
            try {
                return process.TotalProcessorTime.ToString();
            } catch {
                return "";
            }
        }

    }
}
