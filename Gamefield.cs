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
					palya[i,j]=0;				//	üres
					gridButton.Content="";		//	üres		
					gridButton.Name="CellBtn";	//	!
					gridButton.Width=40;		//e mellett akár képet is lehetne betenni
					gridButton.Height=40;
					gridButton.Click+=cellClick;//	important
					buttongrid.Children.Add(gridButton);
					Grid.SetRow(gridButton,i);	//	!
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
				make_move(gridButton);		//választható játékosnál: if(cpu_turn && cpu_on) {lépés}
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
		
		//************	Gépi Lépés	(alaposan átnézni, lehet hibaforrás)	**********
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
		
		//***********	Mankó	***************		
		private bool NotOutOfRange(int is_it_out){			//nyilván csak szimetrikusra jó
			bool notoutofrange=true;
			if (is_it_out>=sorSzam || is_it_out<0 || is_it_out>=oszlopSzam || is_it_out<0) {
				notoutofrange=false;
			}
			return notoutofrange;
		}
		
		struct moves_for_best_list{	//Hát ez tök fölösleges, de tegnap már fáradt voltam, DE: lehetne súlyozni őket fontossági sorrendben!
			public int x;
			public int y;
		}
		
		
		private void make_move(Button player_pressed){
			//groupMark best_move=new groupMark();
			//groupMark seek_move=new groupMark();
			//bool best_move_found=false;
			bool moved_alredy=false;
			//int best_move_x=0,best_move_y=0;
			Random vsz=new Random();
			
			List<groupMarkMindennel> negyesek_o=new List<groupMarkMindennel>();
			
			List<moves_for_best_list> best_moves=new List<moves_for_best_list>();
			moves_for_best_list item_for_best_moves=new moves_for_best_list();
			
			
			negyesek_o=seekAllGroups(2,4);
			
			int index_best_moves=0;
		
			foreach (var i in negyesek_o) {
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
			
			
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			//OK. Teszt:
			/*
			List<groupMarkMindennel> negyesek_x=new List<groupMarkMindennel>();
			
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
			
			//Elvileg minden jó, ha mégis gebasz van, itt keressem először!!! Mármint a seekAllGroups()-ban
			//jobb átló X mintha rossz lenne! Nem, biztos, hogy rossz, holnap kijavítani
			
			
			ButtonPressDownByKoord(1,0);
			*/
			
			//***********
			
			List<groupMarkMindennel> negyesek_x=new List<groupMarkMindennel>();
			
			negyesek_x=seekAllGroups(1,4);	
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
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			
			//***********
			
			List<groupMarkMindennel> harmasok_x=new List<groupMarkMindennel>();
			
			harmasok_x=seekAllGroups(1,3);
		
			foreach (var i in harmasok_x) {
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
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			
			//**************
			
			List<groupMarkMindennel> harmasok_o=new List<groupMarkMindennel>();
			
			harmasok_o=seekAllGroups(2,3);
			
			
			
		
			foreach (var i in harmasok_o) {
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
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			
			//**************
			
			List<groupMarkMindennel> kettesek_x=new List<groupMarkMindennel>();
			
			kettesek_x=seekAllGroups(1,2);
		
			foreach (var i in kettesek_x) {
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
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			
			
			//**************
			
			List<groupMarkMindennel> kettesek_o=new List<groupMarkMindennel>();
			
			kettesek_o=seekAllGroups(2,2);
		
			foreach (var i in kettesek_o) {
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
			
			if (!moved_alredy && best_moves.Count!=0) {
				index_best_moves=vsz.Next(0,best_moves.Count);
				ButtonPressDownByKoord(best_moves[index_best_moves].x,best_moves[index_best_moves].y);
				moved_alredy=true;
				best_moves.Clear();
			}
			

			/*
			if (best_move_found) {
				ButtonPressDownByKoord(best_move_x,best_move_y);
			}
			
			//1: win 
			
			//four_out_of_five()	lehetett volna egy jó kis metódus, de már nem. Csináltam jobbat
			
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
			
			
			//Teszthez megszüntetni a gépi lépést, csak akkor lépjen, ha van mire reagálni.
			
			if (!moved_alredy) {
				Random_Move(player_pressed);
			}
			
		}

		//Elvileg helyes, de azért majd nem árt egy alapos teszt
		private bool seekGroupsForWin(int search_what, int times){		//esetleg egy empty elemet is fel lehetne venni a paraméterek 
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
			int count_vertical_what=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[j,i]==search_what) {			//beletenni vizsgálatot, hogy talált -e már vízszintest!!!!    habár lehet listára kéne tenni a legjobb lépéseket, megfontolandó!
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
			//***********átló vizsgálat**********************
			
			//if(not_found alredy)		gyorsíthat itt a dolgok menetén
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
			//***********right diagonal*******************
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
			return found;
		}	
		
		//Minden, ami csak kellhet. Később tovább bővíthető, szükség szerint
		struct groupMarkMindennel{	
			public int start_x,start_y,end_x,end_y,next_x,next_y,before_x,before_y,length;
			public bool found,have_next,empty_before;	//found-ot mire is használtam? Ja, checkforwinre
			public string direction;
		}
		
		
		private List<groupMarkMindennel> seekAllGroups(int search_what, int times){		//esetleg egy empty elemet is fel lehetne venni a paraméterek (not yet)
			groupMarkMindennel temp_group=new groupMarkMindennel();
			groupMarkMindennel final_group=new groupMarkMindennel();
			List<groupMarkMindennel> osszes=new List<groupMarkMindennel>();
			temp_group.found=false;	//?
			temp_group.have_next=false;
			temp_group.empty_before=false;
			//temp_group.length=0;
			//*************horizontal**********Elvileg jó,tesztelve
			
			int count_horizontal_what=0;
			for (int i = 0; i < sorSzam; i++) {
				for (int j = 0; j < oszlopSzam; j++) {
					if (palya[i,j]==search_what) {
						count_horizontal_what++;
						//****start init
						if (count_horizontal_what==1) {		//fuck ez kavart be, csak három óra volt kijavítani
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
						
						//pálya szélét elvileg felesleges vizsgálni itt, mint ahogy vízszintesnél csináltam
						
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
							
							//Hát sajnos kell pálya szélét vizsgálni, csak itt azt függőlegesen
							//azaz ha a pálya alján van négyes csoport, nem tesz be elé 
							
							
							
							
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
						
						
						/*	origi a serchforwinnerből 
						if (count_vertical_what>=times) {
							temp_group.found=true;
							
						}
						*/
						
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
								//ezt itt áttenni!!!
								/*
								if (NotOutOfRange(temp_row-1) && NotOutOfRange(temp_col-1)) {
									if (palya[temp_row-1,temp_col-1]==0) {
										temp_group.have_before=true;
										temp_group.before_x=temp_row-1;
										temp_group.before_y=temp_col-1;
									}
								}
								*/
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
							
							//if (count_right_diagonal_what>=times && NotOutOfRange(i+1) && NotOutOfRange(j-1) && palya[i+1,j-1]!=search_what) {
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
								//kell még a jobb felső sarokból, és végig a felső sorból induló, aztán kész
								final_group=temp_group;
								osszes.Add(final_group);
							} else if ((!NotOutOfRange(i-1) || !NotOutOfRange(j+1)) && count_right_diagonal_what>=times){		//pálya teteje és jobb széle && helyett lehet || kell a föntiekbe is!
								temp_group.found=true;
								temp_group.end_x=i;
								temp_group.end_y=j;
								temp_group.direction="rightdiagonal";
								temp_group.have_next=false;
								
								//másik vége:
								if (NotOutOfRange(temp_group.start_x+1) && NotOutOfRange(temp_group.start_y-1)) {
									if (palya[temp_group.start_x+1,temp_group.start_y-1]==0) {
										temp_group.empty_before=true;
										temp_group.before_x=temp_group.start_x+1;
										temp_group.before_y=temp_group.start_y-1;
									} else {
										temp_group.empty_before=false;
									}
								}
								//kell még a jobb felső sarokból, és végig a felső sorból induló, aztán kész
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
			
			
			
			
			/*************origi rightdiagonal
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
			*/
			return osszes;
		}
		
		private void checkForWinner(){
			bool winning_group_found=false;
			//itt volt a win seek...
			winning_group_found=seekGroupsForWin(1,5);
			if (winning_group_found) {
				String winner="X"; 							//itt átrendezésre szorul, mert átalakítottam a keresést
				MessageBox.Show(winner+" nyert!","Hurrá!");		//ezt itt hamarabb is meg lehet vizsgálni, rögtön X lépése után. Holnapra is kell valami
				Reset_Board();
			} else {
				winning_group_found=seekGroupsForWin(2,5);
				if (winning_group_found) {
					String winner="O";
					MessageBox.Show(winner+" nyert!","Hurrá!");		//hát, elég nehéz rávenni a gépet, hogy nyerjen, holnap muszáj lesz már kicsit javítani rajta!!!
					Reset_Board();									//azóta kicsit jobb lett
				} 	
			}
			if (!winning_group_found && turn_count==sorSzam*oszlopSzam) {	//Ha a gép normálisan játszana, elvieleg erre itt nem lenne szükség (javul)
				MessageBox.Show("Döntetlen!","Nesze!");
				Reset_Board();
			}	
		}
		
		//Itt is becsúszhat valami hiba, tur_count stb.
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
