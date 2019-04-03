using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashMemCore.Gui.Objects
{
    public class CProcess
    {
        public Process Process { get; private set; }

        public CProcess(Process process)
        {
            Process = process;
        }

        public override string ToString()
        {
            return $"[{Process.Id}] {(Process.MainWindowTitle != "" ? Process.MainWindowTitle : "n/a")} - {Process.ProcessName}";
        }
    }
}
