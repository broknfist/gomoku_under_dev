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
					gridButton.Width=40;
					gridButton.Height=40;
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
				palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=1;
				gridButton.Content="X";
				x_or_not=false;
				turn_count++;
			}else{
				gridButton.IsEnabled=false;
				gridButton.Content="O";
				palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=2;
				x_or_not=true;
				turn_count++;
			}			
			checkForWinner();
			//implement 
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
			int count_vertical_x=0,count_vertical_o=0;
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
					if (palya[j,i]==2) {
						count_vertical_o++;
						if (count_vertical_o>4) {
							there_is_a_winner=true;
							break;
						}
					} else {
						count_vertical_o=0;
					}					
				}
				count_vertical_x=0;
				count_vertical_o=0;
			}
			//***********átló vizsgálat**********************		
			int temp_row,temp_col;
			int count_diagonal_x=0,count_diagonal_o=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					//left diagonal
					if (i>=j) {
						temp_row=i-j;
						temp_col=0;
					} else {
						temp_row=0;
						temp_col=j-i;
					}
					while (temp_row!=sorSzam && temp_col!=oszlopSzam) {
						if (palya[temp_row,temp_col]==1) {
							count_diagonal_x++;
							if (count_diagonal_x>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_diagonal_x=0;
						}
						temp_row++;
						temp_col++;				
					}				
					//***********right diagonal**************
					count_diagonal_x=0;
					if (i+j>=sorSzam) {
						temp_row=sorSzam;
						temp_col=i+j-sorSzam;
					} else {
						temp_row=0;
						temp_col=i+j;
					}
					while (temp_row!=sorSzam && temp_col!=0) {
						if (palya[temp_row,temp_col]==1) {
							count_diagonal_x++;
							if (count_diagonal_x>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_diagonal_x=0;
						}				
						if (palya[temp_row,temp_col]==2) {
							count_diagonal_o++;
							if (count_diagonal_o>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_diagonal_o=0;
						}			
						temp_row++;
						temp_col--;				
					}				
				}
			}
			if (there_is_a_winner) {
				String winner="";
				if (x_or_not) {
					winner="O";
				}else{
					winner="X";
				}
				MessageBox.Show(winner+" nyert!","Hurrá!");
			}else{	//Kell ez?, jó ez?, később átgondolni
				if (turn_count==sorSzam*oszlopSzam) {
					MessageBox.Show("Döntetlen!","Nesze!");
				}
			}
		}
	}
}
