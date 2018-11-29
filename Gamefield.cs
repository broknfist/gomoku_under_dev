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
		private int[,] palya;
		public Gamefield(Window1 mainwindow,int sorszam,int oszlopszam)
		{
			this.mainwindow=mainwindow;
			this.sorSzam=sorszam;
			this.oszlopSzam=oszlopszam;
			palya=new int[sorSzam,oszlopSzam];
			Start();
		}
		private void Start(){
			ButtonGrid();
		}
		private void ButtonGrid(){
			Grid buttongrid=new Grid();
			for (int i = 0; i < oszlopSzam; i++) {
				ColumnDefinition aktcol=new ColumnDefinition();
				buttongrid.ColumnDefinitions.Add(aktcol);
			}
			for (int i = 0; i < sorSzam; i++) {
				RowDefinition aktrow=new RowDefinition();
				buttongrid.RowDefinitions.Add(aktrow);
			}
			for (int i = 0; i < oszlopSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {				
					Button gridButton=new Button();
					palya[i,j]=0;
					gridButton.Content="";	
					//gridButton.Content=palya[i,j].ToString();					
					gridButton.Width=40;
					gridButton.Height=40;
					//gridButton.Name="S"+sorSzam+"O"+oszlopSzam;
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
				//string[] temp1=gridButton.Name.Split('S');
				//string[] temp2=temp1[1].Split('O');
				//int sora_int=Convert.ToInt32(temp2[0]);
				//int oszlopa_int=Convert.ToInt32(temp2[1]);
				palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=1;
				//gridButton.Content=Convert.ToString(palya[sora_int,oszlopa_int]);
				gridButton.Content="X";
				//gridButton.Content=palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)];
				//gridButton.Content=Grid.GetRow(gridButton)+","+Grid.GetColumn(gridButton);
				x_or_not=false;
				turn_count++;
			}else{
				gridButton.IsEnabled=false;
				gridButton.Content="O";
				palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=2;
				//gridButton.Content=Grid.GetRow(gridButton)+","+Grid.GetColumn(gridButton);
				//gridButton.Content=palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)];
				x_or_not=true;
				turn_count++;
			}
			
			checkForWinner();
		}
		private void checkForWinner(){
			bool there_is_a_winner=false;
			
			//**************sorok vizsgálta, hogy van -e nyertes?*****************
			int count_horizontal_x=0,count_horizontal_o=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[i,j]==1) {
						count_horizontal_x++;
						if (count_horizontal_x>4) {
							there_is_a_winner=true;
							break;
						}
					} else {
						count_horizontal_x=0;
					}
					if (palya[i,j]==2) {
						count_horizontal_o++;
						if (count_horizontal_o>4) {
							there_is_a_winner=true;
							break;
						}
					} else {
						count_horizontal_o=0;
					}
					
				}
				count_horizontal_x=0;
				count_horizontal_o=0;
			}
			
			//**************oszlopok vizsgálta, hogy van -e nyertes?*****************
			
			int count_vertical_x=0,count_verticalal_o=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[j,i]==1) {			//csak szimmetrikus táblára jó!!!!!
						count_vertical_x++;
						if (count_vertical_x>4) {
							there_is_a_winner=true;
							break;
						}
					} else {
						count_vertical_x=0;
					}
					if (palya[i,j]==2) {
						count_verticalal_o++;
						if (count_verticalal_o>4) {
							there_is_a_winner=true;
							break;
						}
					} else {
						count_verticalal_o=0;
					}
					
				}
				count_vertical_x=0;
				count_verticalal_o=0;
			}
			
			/*
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[i,j]==1) {
						count_vertical++;
						if (count_vertical>4) {
							there_is_a_winner=true;
							break;
						}
					}
				}
				count_vertical=0;
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
					winner="O";
					//o_win_count.Text=(Int32.Parse(o_win_count.Text)+1).ToString();
				}else{
					winner="X";
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
