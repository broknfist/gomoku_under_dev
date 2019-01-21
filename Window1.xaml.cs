﻿/*
 * Created by SharpDevelop.
 * User: zsolt
 * Date: 2018. 11. 28.
 * Time: 1:08
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
	/// Interaction logic for Window1.xaml
	/// </summary>
	
	static class MainProps{
		public static string Player1Name;
		public static string Player2Name;
		public static bool CPU;
		public static bool CPU_turn;
	}
	
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			MainProps.Player1Name="Player1";
			MainProps.Player2Name="Computer";
			MainProps.CPU=true;
			MainProps.CPU_turn=false;
			Gamefield game=new Gamefield(this,15,15);
		}
	}
}