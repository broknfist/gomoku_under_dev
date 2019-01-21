/*
 * Created by SharpDevelop.
 * User: zsolt
 * Date: 01/20/2019
 * Time: 21:51
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
	/// Interaction logic for Window3.xaml
	/// </summary>
	public partial class Window3 : Window
	{
		public Window3()
		{
			InitializeComponent();
			this.adatok.ItemsSource=MainProps.stat;
		}
	}
}