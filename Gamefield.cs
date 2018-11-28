/*
 * Created by SharpDevelop.
 * User: zsolt
 * Date: 11/28/2018
 * Time: 01:28
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
	/// Description of Gamefield.
	/// </summary>
	public class Gamefield
	{
		private Window1 mainwindow;
		private int sorSzam;
		private int oszlopSzam;
		private bool x_or_not=true;
		private int turn_count=0;
		public Gamefield(Window1 mainwindow,int sorszam,int oszlopszam)
		{
			this.mainwindow=mainwindow;
			this.sorSzam=sorszam;
			this.oszlopSzam=oszlopszam;
			Start();
		}
		private void Start(){
			ButtonGrid();
		}
		private void ButtonGrid(){
			Grid buttongrid=new Grid();
			for (int i = 0; i < 15; i++) {
				ColumnDefinition aktcol=new ColumnDefinition();
				buttongrid.ColumnDefinitions.Add(aktcol);
			}
			for (int i = 0; i < 15; i++) {
				RowDefinition aktrow=new RowDefinition();
				buttongrid.RowDefinitions.Add(aktrow);
			}
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {				
					Button gridButton=new Button();
					gridButton.Content="";					
					gridButton.Width=40;
					gridButton.Height=40;
					gridButton.Name="s"+sorSzam.ToString()+"o"+oszlopSzam.ToString();
					gridButton.Click+=buttonClick;
					buttongrid.Children.Add(gridButton);
					Grid.SetRow(gridButton,i);
					Grid.SetColumn(gridButton,j);
				}
			}
			mainwindow.GameField.Children.Add(buttongrid);
		}
		private void buttonClick(object sender,RoutedEventArgs e){
			Button gridButton=(Button)sender;
			if (x_or_not) {
				gridButton.IsEnabled=false;
				gridButton.Content="X";
				x_or_not=false;
				turn_count++;
			}else{
				gridButton.IsEnabled=false;
				gridButton.Content="O";
				x_or_not=true;
				turn_count++;
			}
			
			checkForWinner();
		}
		private void checkForWinner(){
			bool there_is_a_winner=false;
			
			
			//horizontal checks
			/*
			if ((A1.Text==A2.Text)&&(A2.Text==A3.Text)&&(!A1.Enabled)) {
				there_is_a_winner=true;
			}
			else if((B1.Text==B2.Text)&&(B2.Text==B3.Text)&&(!B1.Enabled)) {
				there_is_a_winner=true;
			}
			else if((C1.Text==C2.Text)&&(C2.Text==C3.Text)&&(!C1.Enabled)) {
				there_is_a_winner=true;
			}
			
			//vertical checks
			else if ((A1.Text==B1.Text)&&(B1.Text==C1.Text)&&(!A1.Enabled)) {
				there_is_a_winner=true;
			}
			else if((A2.Text==B2.Text)&&(B2.Text==C2.Text)&&(!A2.Enabled)) {
				there_is_a_winner=true;
			}
			else if((A3.Text==B3.Text)&&(B3.Text==C3.Text)&&(!A3.Enabled)) {
				there_is_a_winner=true;
			}
			
			//diagonal checks
			if ((A1.Text==B2.Text)&&(B2.Text==C3.Text)&&(!A1.Enabled)) {
				there_is_a_winner=true;
			}
			else if((A3.Text==B2.Text)&&(B2.Text==C1.Text)&&(!A3.Enabled)) {
				there_is_a_winner=true;
			}
			*/
			
			if (there_is_a_winner) {
				//disableButtons();
				String winner="";
				if (x_or_not) {
					winner="X";
					//o_win_count.Text=(Int32.Parse(o_win_count.Text)+1).ToString();
				}else{
					winner="O";
					//x_win_count.Text=(Int32.Parse(x_win_count.Text)+1).ToString();
				}
				MessageBox.Show(winner+" nyert!","Hurrá!");
			}else{
				if (turn_count==sorSzam*oszlopSzam) {
					//draw_count.Text=(Int32.Parse(draw_count.Text)+1).ToString();
					MessageBox.Show("Döntetlen!","Nesze!");
				}
			}
			
		}
	}
}
