using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WorkStatistics.Statistics
{
	class ProcessStatWinApiWrapper: IDisposable
	{
		[DllImport("user32.dll")]
		private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);
		[DllImport("user32.dll")]
		private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

		private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

		public delegate void ProcessStatEventArgs(Process process);
		public event ProcessStatEventArgs ChangedCurrentProcess;

		private const uint WINEVENT_OUTOFCONTEXT = 0;
		private const uint EVENT_SYSTEM_FOREGROUND = 3;

		private IntPtr _winEventHook;
		private Process _currentProcess;
		private WinEventDelegate _winEventDelegate;

		public Process GetCurrentProcess { get { return _currentProcess; } }

		public ProcessStatWinApiWrapper()
		{
			_winEventDelegate = new WinEventDelegate(WinEventProc);
			_winEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventDelegate , 0, 0, WINEVENT_OUTOFCONTEXT);
		}
		private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			IntPtr handle = GetForegroundWindow();
			uint pid = 0;
			GetWindowThreadProcessId(handle, out pid);
			_currentProcess = Process.GetProcessById((int)pid);
			ChangedCurrentProcess?.Invoke(_currentProcess);
		}
		public void Dispose()
		{
			UnhookWinEvent(_winEventHook);
		}
		
	}
	public class ProcessStat
	{
		private ProcessStatWinApiWrapper _pswaw;
		public ProcessStat()
		{
			_pswaw = new ProcessStatWinApiWrapper();
			_pswaw.ChangedCurrentProcess += pswaw_ChangedCurrentProcess;
		}

		private void pswaw_ChangedCurrentProcess(Process process)
		{
			Console.WriteLine(process.ProcessName);
		}
	}

}
