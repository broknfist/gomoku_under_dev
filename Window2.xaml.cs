/*
 * Created by SharpDevelop.
 * User: zsolt
 * Date: 2019. 01. 20.
 * Time: 21:42
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
	/// Interaction logic for Window2.xaml
	/// </summary>
	public partial class Window2 : Window
	{
		public Window2()
		{
			InitializeComponent();
			this.buttonOptionSet.Click+=new RoutedEventHandler(buttonOptionSet_Click);
		}
		void buttonOptionSet_Click(object sender, RoutedEventArgs e)
		{
			if (textboxPlayer1.Text.Length>0) {
				MainProps.Player1Name=textboxPlayer1.Text;
			}
			if (textboxPlayer2.Text.Length>0) {
				MainProps.Player2Name=textboxPlayer2.Text;
			}
			if (radioComputer.IsChecked==true) {
				MainProps.CPU=true;
			}
			if (radioHuman.IsChecked==true) {
				MainProps.CPU=false;
			}


			this.Close();
		}
	}
}