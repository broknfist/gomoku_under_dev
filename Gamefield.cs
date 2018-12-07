﻿/*
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
             //hát, ez lenne a PerformClick, ha mégis szükség lenne rá
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
			//SeekMove move=new SeekMove(this???,15,15,palya);
			BoardBirth();
		}
		private void BoardBirth(){
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
					gridButton.Click+=cellClick;
					buttongrid.Children.Add(gridButton);
					Grid.SetRow(gridButton,i);
					Grid.SetColumn(gridButton,j);
				}
			}
			mainwindow.Board.Children.Add(buttongrid);
		}
		private void cellClick(object sender,RoutedEventArgs e){
			Button gridButton=(Button)sender;
			if (x_or_not) {
				gridButton.IsEnabled=false;
				palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=1;
				gridButton.Content="X";
				x_or_not=false;
				turn_count++;
				checkForWinner();
				//Random lépés a gépnek
				//Random_Move(gridButton);
				//ButtonPressDownByKoord(0,turn_count/2); //Mechanical Test
				make_move(gridButton);
				checkForWinner();
			}else{
 				//Itt volt a második játékos lépése, megszűntettem, amíg a gép nem lép jól, ha az megvan, átszervezem az egészet
			}			
			//checkForWinner();
			//implement performclick button, ha make_a_move() visszatér egy jó lépéssel!!!
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
		
		private void Random_Move(Button player_pressed){
			Random vsz=new Random();
			int rand_x,rand_y;
			int proximity_x=Grid.GetRow(player_pressed);
			int proximity_y=Grid.GetColumn(player_pressed);
			int interval_x=1;
			int interval_y=1;
			do {
				do {
					rand_x=proximity_x;
					rand_y=proximity_y;
					rand_x+=vsz.Next((-1)*interval_x,(interval_x+1));
					rand_y+=vsz.Next((-1)*interval_y,(interval_y+1));
					interval_x++;
					if (interval_x>sorSzam) {
						interval_x=1;
					}
					interval_y++;
					if (interval_y>oszlopSzam) {
						interval_y=1;
					}
				} while (rand_x>=sorSzam || rand_x<0 || rand_y>=oszlopSzam || rand_y<0);
			} while (palya[rand_x,rand_y]!=0);
			ButtonPressDownByKoord(rand_x,rand_y);
			//mechanical test: ButtonPressDownByKoord(0,0);
		}
		
		private void ButtonPressDownByKoord(int x_koord, int y_koord){
			foreach (Grid gr in mainwindow.Board.Children) {
				foreach (Button bt in gr.Children) {
					if (Grid.GetRow(bt)==x_koord && Grid.GetColumn(bt)==y_koord){
						
						bt.Content="O";
						palya[Grid.GetRow(bt),Grid.GetColumn(bt)]=2;
						x_or_not=true;
						turn_count++;
						bt.IsEnabled=false;
						break;
					}
				}
			}
		}
		
		private bool NotOutOfRange(int is_it_out){
			bool notoutofrange=true;
			if (is_it_out>=sorSzam || is_it_out<0 || is_it_out>=oszlopSzam || is_it_out<0) {
				notoutofrange=false;
			}
			return notoutofrange;
		}
		
		private void make_move(Button player_pressed){
			//groupMark best_move=new groupMark();
			groupMark seek_move=new groupMark();
			bool best_move_found=false;
			int best_move_x=0,best_move_y=0;
			
			//1: Win
			seek_move=seekGroups(2,4);
			if (seek_move.found) {
				switch (seek_move.direction) {
						case "horizontal":{
							if (NotOutOfRange(seek_move.end_y+1)) {
								if (palya[seek_move.end_x,seek_move.end_y+1]==0) {		
										best_move_x=seek_move.end_x;							 
										best_move_y=seek_move.end_y+1;
										best_move_found=true;
										break;
								}
							}
							if (NotOutOfRange(seek_move.start_y-1)) {
								if (palya[seek_move.start_x,seek_move.end_y-1]==0) {		
										best_move_x=seek_move.start_x;							 
										best_move_y=seek_move.start_y-1;
										best_move_found=true;
										break;
								}
							}
							break;
						}
						case "vertical":{
							if (NotOutOfRange(seek_move.end_x+1)) {
								if (palya[seek_move.end_x,seek_move.end_x+1]==0) {		
										best_move_x=seek_move.end_x+1;							 
										best_move_y=seek_move.end_y;
										best_move_found=true;
										break;
								}
							}
							if (NotOutOfRange(seek_move.start_x-1)) {
								if (palya[seek_move.start_x,seek_move.end_x-1]==0) {		
										best_move_x=seek_move.start_x-1;							 
										best_move_y=seek_move.start_y;
										best_move_found=true;
										break;
								}
							}
							break;
						}
						case "leftdiagonal":{
							if (NotOutOfRange(seek_move.end_x+1) && NotOutOfRange(seek_move.end_y+1)) {
								if (palya[seek_move.end_x+1,seek_move.end_y+1]==0) {		
										best_move_x=seek_move.end_x+1;							 
										best_move_y=seek_move.end_y+1;
										best_move_found=true;
										break;
								}
							}
							if (NotOutOfRange(seek_move.start_x-1) && NotOutOfRange(seek_move.start_y-1)) {
								if (palya[seek_move.start_x-1,seek_move.end_y-1]==0) {		
										best_move_x=seek_move.start_x-1;							 
										best_move_y=seek_move.start_y-1;
										best_move_found=true;
										break;
								}
							}	
							break;
						}
						case "rightdiagonal":{
							if (NotOutOfRange(seek_move.end_x+1) && NotOutOfRange(seek_move.end_y-1)) {
								if (palya[seek_move.end_x+1,seek_move.end_y+1]==0) {		
										best_move_x=seek_move.end_x+1;							 
										best_move_y=seek_move.end_y-1;
										best_move_found=true;
										break;
								}
							}
							if (NotOutOfRange(seek_move.start_x-1) && NotOutOfRange(seek_move.start_y+1)) {
								if (palya[seek_move.start_x-1,seek_move.end_y-1]==0) {		
										best_move_x=seek_move.start_x-1;							 
										best_move_y=seek_move.start_y+1;
										best_move_found=true;
										break;
								}
							}
							break;
						}
						
				}
			}
			if (best_move_found) {
				ButtonPressDownByKoord(best_move_x,best_move_y);
			}
			
			//1: win 
			
			//four_out_of_five()
			
			
			
			
			//2: block enemy win
			//3: ha ellenfélnek 3 van és helye ötöt kirakni, megakadályozni
			//4: ha a lépő félnek 3 van és van helye ötöt kirakni, letenni a negyediket
			//5:   -XX 
			//     X
			//	   X    és társai varázslatokat egyenlőre inkább hagyjuk
			//
			//8: ...ha O-nak 1 van és kirakhat ötöt, letenni mellé a másodikat
			//10: random O-nak, lehetőleg x közelébe, na ez már megvan :D
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
			 * wpf-ben nincs alapból performClick event, de igazából nincs is rá szükség, talán...
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
			//**********10: Random
			if (!best_move_found) {
				Random_Move(player_pressed);
			}
			
			
		}	
		
		struct groupMark{
			public int start_x,start_y,end_x,end_y;
			public bool found;
			public string direction;
		}

		private groupMark seekGroups(int search_what, int times){
			groupMark csopi=new groupMark();
			csopi.found=false;
			//*************horizontal**********
			int count_horizontal_what=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[i,j]==search_what) {
						count_horizontal_what++;
						if (count_horizontal_what==search_what) {
							csopi.start_x=i;
							csopi.start_y=j;
						}
						if (count_horizontal_what>=times) {
							csopi.found=true;
							csopi.end_x=i;
							csopi.end_y=j;
							csopi.direction="horizontal";
							break;
						}
					} else {
						count_horizontal_what=0;
					}
				}
				count_horizontal_what=0;
			}			
			//**************oszlopok vizsgálta, hogy van -e nyertes?*****************			
			int count_vertical_what=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[j,i]==search_what) {			//beletenni vizsgálatot, hogy talált -e már vízszintest!!!!    habár lehet listára kéne tenni a legjobb lépéseket, megfontolandó!
						count_vertical_what++;				//később lehetne random választani az egyenlő súlyú lépésekből...
						if (count_horizontal_what==search_what) {
							csopi.start_x=i;
							csopi.start_y=j;
						}
						if (count_vertical_what>=times) {
							csopi.found=true;
							csopi.end_x=i;
							csopi.end_y=j;
							csopi.direction="vertical";
							break;
						}
					} else {
						count_vertical_what=0;
					}			
				}
				count_vertical_what=0;
			}
			//***********átló vizsgálat**********************
			int temp_row,temp_col;
			int count_left_diagonal_what=0;
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
						if (palya[temp_row,temp_col]==search_what) {
							count_left_diagonal_what++;
							if (count_left_diagonal_what==search_what) {
							csopi.start_x=i;
							csopi.start_y=j;
							}
							if (count_left_diagonal_what>=times) {
							csopi.found=true;
							csopi.end_x=i;
							csopi.end_y=j;
							csopi.direction="leftdiagonal";
							break;
							}
						} else {
							count_left_diagonal_what=0;
						}
						temp_row++;
						temp_col++;
					}
					count_left_diagonal_what=0;
				}
			}
			//***********right diagonal*******************
			int count_right_diagonal_what=0;
			for (int k = 0; k < sorSzam+oszlopSzam-2; k++) {
				for (int j = 0; j <= k;  j++) {
					int i = k-j;
					if (i<sorSzam && j<oszlopSzam) {
						if (palya[i,j]==search_what) {
							count_right_diagonal_what++;
							
							if (count_right_diagonal_what>=times) {
							csopi.found=true;
							csopi.end_x=i;
							csopi.end_y=j;
							csopi.direction="rightdiagonal";
							break;
							}
						} else {
							count_right_diagonal_what=0;			
						}
					}
				}
				count_right_diagonal_what=0;
			}
			return csopi;
		}	
		
		private void checkForWinner(){
			groupMark winning_group=new groupMark();
			winning_group.found=false;
			//itt volt a win seek...
			winning_group=seekGroups(1,5);
			if (winning_group.found) {
				String winner="X"; //itt átrendezésre szorul, mert átalakítottam a keresést
				MessageBox.Show(winner+" nyert!","Hurrá!");		//ezt itt hamarabb is meg lehet vizsgálni, rögtön X lépése után. Holnapra is kell valami
				Reset_Board();
			} else {
				winning_group=seekGroups(2,5);
				if (winning_group.found) {
					String winner="O";
					MessageBox.Show(winner+" nyert!","Hurrá!");		//hát, elég nehéz rávenni a gépet, hogy nyerjen, holnap muszáj lesz már kicsit javítani rajta!!!
					Reset_Board();
				} 	
			}
			if (!winning_group.found && turn_count==sorSzam*oszlopSzam) {	//Ha a gép normálisan játszana, elvieleg erre itt nem lenne szükség
				MessageBox.Show("Döntetlen!","Nesze!");
				Reset_Board();
			}	
		}
		
		private void Reset_Board(){
			turn_count=0;
			foreach (Grid gr in mainwindow.Board.Children) {
				foreach (Button bt in gr.Children) {
					bt.Content="";
					palya[Grid.GetRow(bt),Grid.GetColumn(bt)]=0;
					bt.IsEnabled=true;
				}
			}
		}
	}
}
