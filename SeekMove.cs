/*
 * Created by SharpDevelop.
 * User: zsolt
 * Date: 2018. 12. 06.
 * Time: 20:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace gomoku
{
	/// <summary>
	/// Description of SeekMove.
	/// </summary>
	public class SeekMove
	{
		private Window1 mainwindow;
		protected int rowNum;
		protected int colNum;
		protected int[,] fieldInMem;
		public SeekMove(Window1 mainwindow,int sorszam,int oszlopszam,int[,] palya)
		{
			this.mainwindow=mainwindow;
			this.rowNum=sorszam;
			this.colNum=oszlopszam;
			this.fieldInMem=palya;
		}
	}
}
