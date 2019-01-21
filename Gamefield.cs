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

/*
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
*/
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
			mainwindow.NewGameBtn.Click+=new RoutedEventHandler(NewGameBtn_Click);
			mainwindow.OptionsBtn.Click+=new RoutedEventHandler(OptionBtn_Click);
			mainwindow.StatBtn.Click+=new RoutedEventHandler(StatBtn_Click);
			Start();
		}
		
		void NewGameBtn_Click(object sender, RoutedEventArgs e)
		{
			Reset_Board();
		}
		
		void OptionBtn_Click(object sender, RoutedEventArgs e)
		{
			Window2 options=new Window2();
			options.Show();
		}
		
		void buttonOptionSet_Click(object sender, RoutedEventArgs e)
		{
			Reset_Board();
		}
		
		void StatBtn_Click(object sender, RoutedEventArgs e)
		{
			Window3 options=new Window3();
			options.Show();
		}
		
		private void Start(){
			BoardBirth();	//ide még lehet sok mindent pakolni: pályaméret, játékosválasztás, etc., beengedni ide, csak ha start buttonra bökünk stb.
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
					gridButton.Click+=cellClick;			//	important
					buttongrid.Children.Add(gridButton);
					Grid.SetRow(gridButton,i);				//	!
					Grid.SetColumn(gridButton,j);
				}
			}
			mainwindow.Board.Children.Add(buttongrid);
		}
		
		//************	Main Event	!!!	*******************************************
		private void cellClick(object sender,RoutedEventArgs e){
			Button gridButton=(Button)sender;
			
			if (x_or_not) {
				gridButton.IsEnabled=false;
				palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=1;
				gridButton.Content="X";
				x_or_not=false;
				turn_count++;
				checkForWinner();
				//Random_Move(gridButton);
				//ButtonPressDownByKoord(0,turn_count/2); //Mechanical Test
				if (MainProps.CPU) {
					make_move(gridButton);		//választható játékosnál: if(cpu_turn && cpu_on) {lépés}
					checkForWinner();
				}
				
			}else{
 				if (!MainProps.CPU) {
 					gridButton.IsEnabled=false;
					palya[Grid.GetRow(gridButton),Grid.GetColumn(gridButton)]=2;
					gridButton.Content="O";
					x_or_not=true;
					turn_count++;
					checkForWinner();
 				}
			}			
			
			
			
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
		
		//************	Gépi Lépés	**********
		private void ButtonPressDownByKoord(int x_koord, int y_koord){
			foreach (Grid gr in mainwindow.Board.Children) {
				foreach (Button bt in gr.Children) {
					if (Grid.GetRow(bt)==x_koord && Grid.GetColumn(bt)==y_koord){
						
						bt.Content="O";
						palya[Grid.GetRow(bt),Grid.GetColumn(bt)]=2;
						x_or_not=true;		//Attention!!!
						turn_count++;		//!
						bt.IsEnabled=false;	//button block
						break;
					}
				}
			}
		}
		
		//***********	Mankó(olvashatóbb a kód)	***************				
		private bool NotOutOfRange(int is_it_out){			//nyilván csak szimetrikusra jó
			bool notoutofrange=true;
			if (is_it_out>=sorSzam || is_it_out<0 || is_it_out>=oszlopSzam || is_it_out<0) {
				notoutofrange=false;
			}
			//notoutofrange=(is_it_out>=sorSzam || is_it_out<0 || is_it_out>=oszlopSzam || is_it_out<0)?false:true;	//funny
			return notoutofrange;
		}
		
		struct moves_for_best_list{	//Hát ez tök fölösleges, de tegnap már fáradt voltam, DE: lehetne súlyozni őket fontossági sorrendben!
			public int x;
			public int y;
			public int weigth;
		}
		
		
		private List<moves_for_best_list> lookForBest(List<groupMarkAllProp> bemenet){
			moves_for_best_list item_for_best_moves=new moves_for_best_list();
			List<moves_for_best_list> kimenet=new List<moves_for_best_list>();
			foreach (var i in bemenet) {
				if (i.empty_before) {
					item_for_best_moves.x=i.before_x;
					item_for_best_moves.y=i.before_y;
					kimenet.Add(item_for_best_moves);
				}
				if (i.have_next) {
					item_for_best_moves.x=i.next_x;
					item_for_best_moves.y=i.next_y;
					kimenet.Add(item_for_best_moves);
				}
			}
			return kimenet;
		}
		
		private void make_move(Button player_pressed){
			bool moved_alredy=false;
			Random vsz=new Random();		
			List<moves_for_best_list> best_moves=new List<moves_for_best_list>();
			
			//win, if possible
			//Lekezelni: o-oooo és oo-oo
			
			List<groupMarkAllProp> negyesek_o=new List<groupMarkAllProp>();
			
			negyesek_o=seekAllGroups(2,4);
			best_moves=lookForBest(negyesek_o);
			int index_best_moves=0;
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
						
			//***********block four enemy blocks
			//blokkolni: x-xxxx és xx-xx
			
			List<groupMarkAllProp> negyesek_x=new List<groupMarkAllProp>();
			
			negyesek_x=seekAllGroups(1,4);	
			best_moves=lookForBest(negyesek_x);
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			//***********block threes
			//Ha mindkét vége szabad, az fontosabb, mint, ha csak az egyik!
			// -x-    xx-
			// x-x és --x 
			// -x-    --x
			
			List<groupMarkAllProp> harmasok_x=new List<groupMarkAllProp>();
			
			harmasok_x=seekAllGroups(1,3);
			best_moves=lookForBest(harmasok_x);
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}		
			
			//**************put fourth
			//a block threes támadó variációja
			
			List<groupMarkAllProp> harmasok_o=new List<groupMarkAllProp>();
			
			harmasok_o=seekAllGroups(2,3);
			best_moves=lookForBest(harmasok_o);
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			//**************block 2's
			//talán csak akkor fontos, ha mindkét vége szabad, ez talán megoldhat egy fenti problémát is
			
			List<groupMarkAllProp> kettesek_x=new List<groupMarkAllProp>();
			
			kettesek_x=seekAllGroups(1,2);
			best_moves=lookForBest(kettesek_x);
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			//***********put third
			//mindkét végén szabad kettes blokkok fontosabbak
			
			List<groupMarkAllProp> kettesek_o=new List<groupMarkAllProp>();
			
			kettesek_o=seekAllGroups(2,2);
			best_moves=lookForBest(kettesek_o);
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			//*******put second
			//mondjuk akkor a legjobb, ha minden irányból van szabad hely, lehetőleg egy ötös csoport kialakításához, hm...
			
			List<groupMarkAllProp> egyesek_o=new List<groupMarkAllProp>();
			kettesek_o=seekAllGroups(2,2);
			best_moves=lookForBest(egyesek_o);
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			//Random:
			if (!moved_alredy) {
				Random_Move(player_pressed);
			}
			
						// Teszt:
			/*
			List<groupMarkAllProp> negyesek_x=new List<groupMarkAllProp>();
			List<moves_for_best_list> best_moves=new List<moves_for_best_list>();
			moves_for_best_list item_for_best_moves=new moves_for_best_list();
			negyesek_x=seekAllGroups(1,3);	
			foreach (var i in negyesek_x) {
				if (i.empty_before) {
					item_for_best_moves.x=i.before_x;
					item_for_best_moves.y=i.before_y;
					best_moves.Add(item_for_best_moves);
				}
				if (i.have_next) {
					item_for_best_moves.x=i.next_x;
					item_for_best_moves.y=i.next_y;
					best_moves.Add(item_for_best_moves);
				}
			}
			negyesek_x.Clear();
			foreach (var i in best_moves) {
				ButtonPressDownByKoord(i.x,i.y);
			}
			best_moves.Clear();
			
			//jobb átló X mintha rossz lenne! Nem, biztos, hogy rossz, holnap kijavítani
			//valamint néha nem ad eredményt a négyes csoportokra és egyből a hármasokra megy. Miért?
			
			ButtonPressDownByKoord(3,0);
			
			*/
			
			
			/******Stratágiák:
			
			//1: win 
			
			//2: block enemy win
			//3: ha ellenfélnek 3 van és helye ötöt kirakni, megakadályozni
			//4: ha a lépő félnek 3 van és van helye ötöt kirakni, letenni a negyediket
			//5:   -XX 
			//     X
			//	   X    és társai varázslatokat egyenlőre inkább hagyjuk
			//
			//8: ...ha O-nak 1 van és kirakhat ötöt, letenni mellé a másodikat	ez fontos, hogy ott ne rugózzon, ahol nem nyerhet
			//10: random O-nak, lehetőleg x közelébe, na ez már megvan :D
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
		}

		//Elvileg helyes, de azért majd nem árt egy alapos teszt
		private bool seekGroupsForWin(int search_what, int times){		//esetleg egy empty elemet is fel lehetne venni a paraméterek (seekAllGroups() kész.)
			bool found=false;
			//*************horizontal**********
			int count_horizontal_what=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[i,j]==search_what) {
						count_horizontal_what++;
						if (count_horizontal_what>=times) {
							found=true;
							break;
						}
					} else {
						count_horizontal_what=0;
					}
				}
				count_horizontal_what=0;
			}			
			//**************oszlopok vizsgálta, hogy van -e nyertes?*****************		
			if (!found) {
				int count_vertical_what=0;
				for (int i = 0; i < sorSzam; i++) {
					for (int j = 0; j < oszlopSzam; j++) {
						if (palya[j,i]==search_what) {			//habár lehet listára kéne tenni a legjobb lépéseket, megfontolandó! Kész.
							count_vertical_what++;				//később lehetne random választani az egyenlő súlyú lépésekből...
							if (count_vertical_what>=times) {
								found=true;
								break;
							}
						} else {
							count_vertical_what=0;
						}			
					}
					count_vertical_what=0;
				}
			}			
			
			//***********átló vizsgálat**********************
			
			//if(not_found alredy)		gyorsíthat itt a dolgok menetén
			if(!found){
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
								if (count_left_diagonal_what>=times) {
								found=true;
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
			}
			
			//***********right diagonal*******************
			if(!found){
				int count_right_diagonal_what=0;
				for (int k = 0; k < sorSzam+oszlopSzam-2; k++) {
					for (int j = 0; j <= k;  j++) {
						int i = k-j;
						if (i<sorSzam && j<oszlopSzam) {
							if (palya[i,j]==search_what) {
								count_right_diagonal_what++;
								if (count_right_diagonal_what>=times) {
								found=true;
								break;
								}
							} else {
								count_right_diagonal_what=0;			
							}
						}
					}
					count_right_diagonal_what=0;
				}
			}
			//search end, return bool
			return found;
		}	
		
		struct groupMarkAllProp{	
			public int start_x,start_y,end_x,end_y,next_x,next_y,before_x,before_y,length;  //length not used, maybe a weight?
			public bool found,have_next,empty_before;
			public string direction;
		}
				
		private List<groupMarkAllProp> seekAllGroups(int search_what, int times){		//esetleg egy empty elemet is fel lehetne venni a paraméterek (not yet)
			groupMarkAllProp temp_group=new groupMarkAllProp();
			groupMarkAllProp final_group=new groupMarkAllProp();
			List<groupMarkAllProp> osszes=new List<groupMarkAllProp>();
			temp_group.found=false;	//?
			temp_group.have_next=false;
			temp_group.empty_before=false;
			//*************horizontal**********Elvileg jó,tesztelve
			
			int count_horizontal_what=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[i,j]==search_what) {
						count_horizontal_what++;
						//****start init
						if (count_horizontal_what==1) {		//itt kavart be, ha rögtön beállítottam a tem_group.empty_before=true-t, csak három óra volt kijavítani
							temp_group.start_x=i;
							temp_group.start_y=j;
						}
						//******csoport vége keresés **********
						if (count_horizontal_what>=times && !NotOutOfRange(j+1)) {  //pálya széle
							temp_group.found=true;
							temp_group.have_next=false;
								//temp_group.next_x=i;			pályán kívül van
								//temp_group.next_y=j+1;		pályán kívül van
							if (NotOutOfRange(temp_group.start_y-1) && palya[temp_group.start_x,temp_group.start_y-1]==0) {	//start pozíció előtti mező vizsgálat
								temp_group.empty_before=true;
								temp_group.before_x=temp_group.start_x;
								temp_group.before_y=temp_group.start_y-1;
							}
							temp_group.end_x=i;
							temp_group.end_y=j;
							temp_group.direction="horizontal";
							final_group=temp_group;
							osszes.Add(final_group);
						} else if (count_horizontal_what>=times && NotOutOfRange(j+1) && palya[i,j+1]!=search_what){	//még a csoport része, de a következő már nem
							temp_group.found=true;
							if (palya[i,j+1]==0) {		//következő üres
								temp_group.have_next=true;
								temp_group.next_x=i;
								temp_group.next_y=j+1;
							}
							if (NotOutOfRange(temp_group.start_y-1) && palya[temp_group.start_x,temp_group.start_y-1]==0) {	//start pozíció előtti mező vizsgálat
								temp_group.empty_before=true;
								temp_group.before_x=temp_group.start_x;
								temp_group.before_y=temp_group.start_y-1;
							}
							final_group=temp_group;
							osszes.Add(final_group);
						}
					} else {
						count_horizontal_what=0;
					}
				}
				count_horizontal_what=0;
			}
			
			//*****vertical***********elvileg jó,tesztelve
			
			int count_vertical_what=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[j,i]==search_what) {
						count_vertical_what++;
						//start init
						if (count_vertical_what==1) {
							temp_group.start_x=j;
							temp_group.start_y=i;
						}
						
						if (count_vertical_what>=times && NotOutOfRange(j+1) && palya[j+1,i]!=search_what){	//még a csoport része, de a következő már nem
							temp_group.found=true;
							if (palya[j+1,i]==0) {		//következő üres
								temp_group.have_next=true;
								temp_group.next_x=j+1;
								temp_group.next_y=i;
							}
							
							//kezdő előtti pozícióba tesz
							
							if (NotOutOfRange(temp_group.start_x-1) && palya[temp_group.start_x-1,temp_group.start_y]==0) {	//start pozíció előtti mező vizsgálat
								temp_group.empty_before=true;
								temp_group.before_x=temp_group.start_x-1;
								temp_group.before_y=temp_group.start_y;
							}
							final_group=temp_group;
							osszes.Add(final_group);
						} else if (count_vertical_what>=times && !NotOutOfRange(j+1)){	//pálya széle
							temp_group.found=true;
							//a következő már kívül lenne a pályán
							if (palya[temp_group.start_x-1,temp_group.start_y]==0) {	//start pozíció előtti mező vizsgálat
								temp_group.empty_before=true;
								temp_group.before_x=temp_group.start_x-1;
								temp_group.before_y=temp_group.start_y;
							}
							final_group=temp_group;
							osszes.Add(final_group);
						}
					} else {
						count_vertical_what=0;
					}			
				}
				count_vertical_what=0;
			}
		
			//***********átló vizsgálat********************** elvileg jó, alaposan tesztelni!
			
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
							temp_group.length++;
							if (count_left_diagonal_what==1) {
								temp_group.start_x=temp_row;
								temp_group.start_y=temp_col;
							}
							if (count_left_diagonal_what>=times) {
								temp_group.found=true;
								temp_group.end_x=temp_row;
								temp_group.end_y=temp_col;
								temp_group.direction="leftdiagonal";
								if (NotOutOfRange(temp_row+1) && NotOutOfRange(temp_col+1)) {
									if (palya[temp_row+1,temp_col+1]==0) {
										temp_group.have_next=true;
										temp_group.next_x=temp_row+1;
										temp_group.next_y=temp_col+1;
									} else {
										temp_group.have_next=false;
									}
								}
								//előző:
								if (NotOutOfRange(temp_group.start_x-1) && NotOutOfRange(temp_group.start_y-1) && palya[temp_group.start_x-1,temp_group.start_y-1]==0) {
									temp_group.empty_before=true;
									temp_group.before_x=temp_group.start_x-1;
									temp_group.before_y=temp_group.start_y-1;
								}
								final_group=temp_group;
								osszes.Add(final_group);
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
			//valami nem stimmel, 4 fölött az group_end és a have next nem jó
			int count_right_diagonal_what=0;
			for (int k = 0; k < sorSzam+oszlopSzam-2; k++) {
				for (int j = 0; j <= k;  j++) {
					int i = k-j;
					if (i<sorSzam && j<oszlopSzam) {
						if (palya[i,j]==search_what) {
							count_right_diagonal_what++;
							//temp_group.length++;
							if (count_right_diagonal_what==1) {
								temp_group.start_x=i;
								temp_group.start_y=j;
							}
							
							if (count_right_diagonal_what>=times && NotOutOfRange(i-1) && NotOutOfRange(j+1) && palya[i-1,j+1]!=search_what) {	
								temp_group.found=true;
								temp_group.end_x=i;
								temp_group.end_y=j;
								temp_group.direction="rightdiagonal";
								
									if (palya[i-1,j+1]==0) {
										temp_group.have_next=true;
										temp_group.next_x=i-1;
										temp_group.next_y=j+1;
									} else {
										temp_group.have_next=false;
									}
								
								//másik vége:
								if (NotOutOfRange(temp_group.start_x+1) && NotOutOfRange(temp_group.start_y-1)) {
									if (palya[temp_group.start_x+1,temp_group.start_y-1]==0) {
										temp_group.empty_before=true;
										temp_group.before_x=temp_group.start_x+1;
										temp_group.before_y=temp_group.start_y-1;
									} else {
										temp_group.have_next=false;
									}
								}
								final_group=temp_group;
								osszes.Add(final_group);
							} else if ((!NotOutOfRange(i-1) || !NotOutOfRange(j+1)) && count_right_diagonal_what>=times){		//pálya teteje és jobb széle && helyett lehet || kell a föntiekbe is!
								temp_group.found=true;
								temp_group.end_x=i;
								temp_group.end_y=j;
								temp_group.direction="rightdiagonal";
								temp_group.have_next=false;
								if (NotOutOfRange(temp_group.start_x+1) && NotOutOfRange(temp_group.start_y-1)) {
									if (palya[temp_group.start_x+1,temp_group.start_y-1]==0) {
										temp_group.empty_before=true;
										temp_group.before_x=temp_group.start_x+1;
										temp_group.before_y=temp_group.start_y-1;
									} else {
										temp_group.empty_before=false;
									}
								}
								final_group=temp_group;
								osszes.Add(final_group);
							}
						} else {
							count_right_diagonal_what=0;
						}
					}
				}
				count_right_diagonal_what=0;
			}
			return osszes;
		}
		
		private void checkForWinner(){
			bool winning_group_found=false;
			//itt volt a win seek...
			winning_group_found=seekGroupsForWin(1,5);
			if (winning_group_found) {
				String winner=MainProps.Player1Name; 							
				MessageBox.Show(winner+" nyert!","Hurrá!");		
				Reset_Board();
			} else {
				winning_group_found=seekGroupsForWin(2,5);
				if (winning_group_found) {
					String winner=MainProps.Player2Name;
					MessageBox.Show(winner+" nyert!","Hurrá!");		
					Reset_Board();									
				} 	
			}
			if (!winning_group_found && turn_count==sorSzam*oszlopSzam) {	
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
