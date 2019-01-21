/*
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
using System.Linq;
using System.IO;
using System.Text;

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
		public static Dictionary<string,int> stat;
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
			MainProps.stat=new Dictionary<string, int>();
			/*
			 * még mondjuk jó lenne adatbázist is belenyomni, vagy fileba menteni, de arra már nincs idő
			try {
				FileStream file=new FileStream(@"stat.db",FileMode.Open);
				StreamReader sr=new StreamReader(file,Encoding.UTF8);
				while (!sr.EndOfStream) {
					string[] sor=sr.ReadLine().Split(' ');
					MainProps.stat.Add(sor[0],Convert.ToInt32(sor[1]));
				}
			} catch (Exception ex) {
				//MessageBox.Show(Convert.ToString(ex));
			}
			*/
			Gamefield game=new Gamefield(this,15,15);
		}
	}
}