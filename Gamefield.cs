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


namespace System.Windows.Controls
{
    public static class MyExt
    {
         public static void PerformClick(this Button btn)
         {
             btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
         }
    }
}
namespace gomoku
{
	/// <summary>
	/// Description of Gamefield.
	/// </summary>
	public class Gamefield
	{
		private Window1 mainwindow;
		protected int sorSzam;
		protected int oszlopSzam;
		protected bool x_or_not=true;
		protected int turn_count=0;
		protected int[,] palya;
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
					gridButton.Name="CellBtn";
					gridButton.Width=40;
					gridButton.Height=40;
					gridButton.Click+=buttonClick;
					buttongrid.Children.Add(gridButton);
					Grid.SetRow(gridButton,i);
					Grid.SetColumn(gridButton,j);
				}
			}
			mainwindow.Board.Children.Add(buttongrid);
		}
		private void buttonClick(object sender,RoutedEventArgs e){
			Button gridButton=(Button)sender;
			if (x_or_not) {
				gridButton.IsEnabled=false;
				palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=1;
				gridButton.Content="X";
				x_or_not=false;
				turn_count++;
				checkForWinner();
				foreach (Grid gr in mainwindow.Board.Children) {
					foreach (Button bt in gr.Children) {
						if (bt.IsEnabled){
							
							bt.Content="O";
							palya[Grid.GetRow(bt),Grid.GetColumn(bt)]=2;
							x_or_not=true;
							turn_count++;
							//gridButton.PerformClick(); or bt.PerformClick, ha kétjátékos lenne;
							bt.IsEnabled=false;
							break;
						}
					}
				}
				checkForWinner();
			}else{
				/*
			   foreach(UIElement child in grid.Children)  
				   {  
				      if(Grid.GetRow(child) == row  
				            &&  
				         Grid.GetColumn(child) == column)  
				      {  
				         return child;  
				      }  
				   }  
 				*/
 				/*
 				 * Itt volt a második játékos lépése, megszűntettem, amíg a gép nem lép jól, ha az megvan, átszervezem az egészet
 				*/
				//gridButton.IsEnabled=false;
			}			
			//checkForWinner();
			/*
			if (x_or_not) {
				Button btnRand=new Button();
				Random vsz=new Random();
				Grid.SetColumn(mainwindow.Board,1);
				Grid.SetRow(mainwindow.Board,1);
				btnRand=gridButton;
				btnRand.PerformClick();
					Ez valami régi szemét...
			}
			*/
			//implement performclick button, ha make_a_move() visszatér egy jó lépéssel!!!
			//Button btnOK=null;
			/*
			try{
			foreach (Button c in mainwindow.Board.Children) {
				btnOK=(Button)c; //as Button;  //this tries to cast all controls as a button, if it fails, it returns as a null				
				if (btnOK != null) {
					if (btnOK.Content=="") {
						btnOK.PerformClick();
					}
				}
			}
			} catch { }
			//btnOK.PerformClick();
			*/
			//make_move(gridButton);
			//make_move();
		}
		
		private void make_move(){
			
			//1: win
			//2: ha x-nek 3 van és kirakhat ötöt, megakadályozni
			//3: ha o-nak 3 van és kirakhat ötöt, letenni a negyediket
			//4: ha o-nak 2 van és kirakhat ötöt, letenni mellé a harmadikat
			//5: ha o-nak 1 van és kirakhat ötöt, letenni mellé a másodikat
			//6: random o-nak, lehetőleg x közelébe
			//Button btnOK=gridButton;
			//Button btnOK=new Button();
			//Random vsz=new Random();
			//do{
			//	Grid.SetColumn(mainwindow.Board,vsz.Next(0,16));
			//	Grid.SetRow(mainwindow.Board,vsz.Next(0,16));	
			/*
			move=look_for_win_or_block("O"); //look for win
			if (move==null) {
				move=look_for_win_or_block("X"); //look for block
				if (move==null) {
					move=look_for_corner();
					if (move==null) {
						move=look_for_open_space();
					}
				}
			}
			*/
			/*
			Instead of PerformClick() use RaiseEvent()
			
			private void button1_Click(object sender, EventArgs e)
			{
			    MessageBox.Show("yes");
			}
			private void Form1_Load(object sender, EventArgs e)
			{
			    RoutedEventArgs newEventArgs = new RoutedEventArgs(Button.ClickEvent);
			    button1.RaiseEvent(newEventArgs);         
			}
			*/
			//}while(btnOK.Content=="");
			//btnOK.PerformClick();
			//btnOK=gridButton;
			//RoutedEventArgs newEventArgs = new RoutedEventArgs(Button.ClickEvent);
			//btnOK.RaiseEvent(newEventArgs);
			
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
			int count_left_diagonal_x=0,count_left_diagonal_o=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (i>j) {
						temp_row=i-j;
						temp_col=0;
					} else {
						temp_row=0;
						temp_col=j-i;
					}
					while (temp_row!=sorSzam && temp_col!=oszlopSzam) {
						if (palya[temp_row,temp_col]==1) {
							count_left_diagonal_x++;
							if (count_left_diagonal_x>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_left_diagonal_x=0;
						}
						if (palya[temp_row,temp_col]==2) {
							count_left_diagonal_o++;
							if (count_left_diagonal_o>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_left_diagonal_o=0;
						}
						temp_row++;
						temp_col++;
					}
					count_left_diagonal_x=0;	//ez a sarkok miatt kell
					count_left_diagonal_o=0;	
				}
			}
			
			//másik átló
			
			
			int count_right_diagonal_x=0,count_right_diagonal_o=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (i>j) {
						temp_row=i-j;
						temp_col=0;
					} else {
						temp_row=0;
						temp_col=j-i;
					}
					while (temp_row!=sorSzam && temp_col!=-1) {
						if (palya[temp_row,temp_col]==1) {
							count_right_diagonal_x++;
							if (count_right_diagonal_x>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_right_diagonal_x=0;
						}
						if (palya[temp_row,temp_col]==2) {
							count_right_diagonal_o++;
							if (count_right_diagonal_o>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_right_diagonal_o=0;
						}
						temp_row++;
						temp_col--;
					}
					count_right_diagonal_x=0;	//ez a sarkok miatt kell
					count_right_diagonal_o=0;	
				}
			}
			
			
			/*
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
						temp_col++;					
					}				
				}
			}
			*/
			//***********right diagonal**************			Hibás!!!!
			/*
			int count_right_diagonal_x=0,count_right_diagonal_o=0;
			for (int k = 0; k < sorSzam+oszlopSzam-2; k++) {
				for (int j = 0; j <= k;  j++) {
					int i = k-j;
					if (i<sorSzam && j<oszlopSzam) {
						if (palya[i,j]==1) {
							count_right_diagonal_x++;
							if (count_right_diagonal_x>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							
						}
						if (palya[i,j]==2) {
							count_right_diagonal_o++;
							if (count_right_diagonal_o>4) {
							there_is_a_winner=true;
							break;
							}
						} else {
							count_right_diagonal_o=0;
						}
					}
					count_right_diagonal_x=0;
					count_right_diagonal_o=0;
				}
			}
			*/
						
			//győztes megnevezése
			if (there_is_a_winner) {
				String winner="";
				if (x_or_not) {
					winner="O";
					//Reset_Board();
				}else{
					winner="X";
				}
				MessageBox.Show(winner+" nyert!","Hurrá!");
				Reset_Board();
			}else{
				if (turn_count==sorSzam*oszlopSzam) {
					MessageBox.Show("Döntetlen!","Nesze!");
					Reset_Board();
				}
			}
		}
		private void Reset_Board(){
			foreach (Grid gr in mainwindow.Board.Children) {
				foreach (Button bt in gr.Children) {
					bt.Content="";
					palya[Grid.GetRow(bt),Grid.GetColumn(bt)]=0;
					x_or_not=true;
					turn_count=0;
					bt.IsEnabled=true;
				}
			}
		}
	}
}
