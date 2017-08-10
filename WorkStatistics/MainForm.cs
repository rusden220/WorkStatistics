using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WorkStatistics.Statistics;

namespace WorkStatistics
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			//KeyboardStat ks = new KeyboardStat();
			ProcessStat ps = new ProcessStat();
		}

		
	}
}
