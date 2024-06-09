using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace BlackjackProject
{
        //všechno jsem se pokusil co nejlépe popsat, snad to pochopíte
        public partial class MainWindow : Window
    {
        public List<string> suits = new List<string> { "Hearts", "Diamonds", "Clubs", "Spades" }; //dekĺarace všech použitelných karet pro metodu DrawCard()
        public List<string> values = new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        private Random random = new Random();
        public int DealerHand = 0;  //jaký součet karet drží hráč a Dealer
        public int Playerhand = 0;
        public bool Playing = false; //jestli je v tuto chvíli aktivní hra
        
        public int PlayerCards = 0; //počet karet které má v ruce dealer
        public int DealerCards = 0; //počet karet které má v ruce hráč
        public bool HiddenCard = false; //jestli má v tuto chvíli dealer skrytou kartu která se musí obrátit 

        
        public double balance = Settings.StartingBalance; //aktuální kredit hráče
        public int CurrentBet = 0; //aktuální sázka s kterou hráč hraje


        public MainWindow()
        {   
            
            InitializeComponent();
            UpdateBalance();
            PlayerName.Content = Settings.PlayerName;
            DealerName.Content = ($"Dealer \n{Settings.DealerName}");
            DefBal.Content = ($"Original - {Settings.StartingBalance} CZK");
            NewDealer();


        }

        public void UpdateBalance() { //přepsání hráčského kreditu po výhře/prohře

            BalanceBox.Content = ($"{balance} CZK");
        }
       
        //tohle jsem zde musel nechat, z nějakého důvodu mi vs hází chyby když to smažu
        //private void DealButton_Click(object sender, RoutedEventArgs e)
        //{
           
        //}
        private void HIT_click(object sender, RoutedEventArgs e) //při klidnutí si hráč vytáhne 1 kartu a zkontroluje se, zde hráč nepřesáhl hodnotu 21
        {
            
            DrawCard("player");
            CheckGameStatus();



        }

        private void STAND_click(object sender, RoutedEventArgs e) //při klidnutí hráč ukončí hru, dealer odkryje svou 2. kartu, zobrazí se tlačítka která jsou potřeba a porovnají se aktuální hodnoty karet obou stran
        {
            if (HiddenCard)
            {
                DealerDeck.Children.RemoveAt(1); //dealerovi se smaže 2. karta a tahá si novou, což vytvoří iluzi že ji dealer otočil, také se nastaví že dealer již nedisponuje skrytou kartou
                DrawCard("dealer");
                HiddenCard = false;
            }
            ResetUI();
            if (DealerHand < 21 && Playerhand < 21) 
            {
                Playing = false; //již se nehraje
            }

            
            

            CheckGameStatus();

        }
        private void double_button_Click(object sender, RoutedEventArgs e) //zdvojnásobí aktuální sázku hráče a hráč si tahá poslední kartu, dále už hráč musí standnout
        {
            CurrentBet *= 2;
            hit_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            stand_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        public void ResetUI() // skrytí tlačítek v UserControls, dealer tahá další kartu pokud má hodnoty karet pod 16, zvolení nové sázky je nyní dostupné
        {
            if (DealerHand <= 16)
                DrawCard("dealer");
            UpdateBalance();
            DealerScore.Visibility = Visibility.Visible;

            hit_button.Visibility = Visibility.Collapsed;
            stand_button.Visibility = Visibility.Collapsed;
            start_button.Visibility = Visibility.Visible;
            double_button.Visibility= Visibility.Collapsed;

            BettingBox.IsReadOnly = false;
            BettingBox.IsHitTestVisible = true;
            BettingBox.Foreground = Brushes.White;
            BettingBox.BorderBrush = Brushes.White;


        }

        private void START_click(object sender, RoutedEventArgs e) //tlačítko pro odstartování hry a uzamknutí sázek
        {
           DealerCards = 0; //dealer v tuto chvíli nemá žádné karty
           Playing = true; //nyní už hrajeme



            

            if (int.TryParse(BettingBox.Text, out CurrentBet)) //jestli je hráčova sázka použitelné číslo
            {
                if (CurrentBet <= balance) //jestli sázka nepřesahuje hráčův aktuální balance
                {
                    NewDealer_button.Visibility = Visibility.Visible;
                    DealerName.Visibility = Visibility.Visible;
                    DealerName_Copy.Visibility = Visibility.Visible;

                    EndMessage.Visibility = Visibility.Collapsed; //skrytí labelu který ukazuje výsledek předešlé hry
                    PlayerDeck.Children.Clear(); //vymazání zobrazených karet z minulé hry
                    DealerDeck.Children.Clear();

                    UpdateBalance(); //přepíše hráčův ktedit (přičte se výhra/odečta prohra)
                    DealerHand = 0; //obě strany mají hodnuty karet 0
                    Playerhand = 0;
                    MessageBox.Text = "";

                    PlayerScore.Visibility = Visibility.Visible; //hráč vidí svůj součet karet, ale dearův ne
                    DealerScore.Visibility = Visibility.Collapsed;

                    hit_button.Visibility = Visibility.Visible; //zobrazení tlačítek userControl, aby hráč mohl hrát
                    stand_button.Visibility = Visibility.Visible;
                    start_button.Visibility = Visibility.Collapsed;
                    double_button.Visibility = Visibility.Visible;


                    BettingBox.IsReadOnly = true;
                    BettingBox.IsHitTestVisible = false;
                    BettingBox.Foreground = Brushes.Yellow;
                    BettingBox.BorderBrush = Brushes.Yellow; 

                    DrawCard("player"); //obě strany si vytáhnou 2 karty, ale dealerova je skrytá (to se řeší v metodě DrawCard()
                    DrawCard("dealer");
                    DrawCard("player");
                    DrawCard("dealer");

                    
                }
                else
                    BettingBox.Text = ""; //pokud hráč vloží neplatnou sážku tak se text v bettingBoxu vymaže a hráč musí sázku vložit odznova



            }
            else
            {
                BettingBox.Text = ""; //pokud hráč vloží neplatnou sážku tak se text v bettingBoxu vymaže a hráč musí sázku vložit odznova
            }

            

           
        }

        private void DrawCard(string who) //metoda pro generování karet (ne v grafickém zobrazení), přijímá string "player" nebo "dealer". 
        {

            string CurrentSuit = suits[random.Next(suits.Count)]; //vybere náhodnou barvy a hodnotu
            string CurrentValue = values[random.Next(values.Count)];

            

            if (who == "player")
            {
                Playerhand += HandleCardValue(CurrentValue); //přičte hodnotu karty díky metodě HandleCardValue
                PlayerScore.Content = Playerhand;
                MessageBox.Text += $"\nYou got {CurrentValue} of {CurrentSuit}."; //vypíše hráči co dostal v Message boxu (vpravo nahoře)
                AddCardToDeck(CurrentSuit, CurrentValue, who); //vyvolá metodu která graficky zobrazí vygenerovanou kartu
            }

            else 
            {
                if (DealerCards == 1)
                {
                    AddCardToDeck("x", "x", "dealer"); //pokud dealer disponuje jednou kartou tak další bude skrytá (otočená)
                    DealerCards++;
                    HiddenCard = true;
                }
                else
                {
                    DealerHand += HandleCardValue(CurrentValue); //samé jako u hráče, akorát karty dostane dealer
                    DealerScore.Content = DealerHand;
                    AddCardToDeck(CurrentSuit, CurrentValue, who);
                    DealerCards++;
                }
                
            }
                
            

            
        }

        public int HandleCardValue(string v) //pokud jsou vybrané karty jako král(K) nebo vtipálek(J), přiřadí jim hodnotu 10. karta X je skrytá karta, takže hodnota ja zatím 0, v ostatních případech mají karty hodnotu stejnou
        {
            switch (v)
            {
                case "X":
                    v = "0";
                    break;
                case "J":
                    v = "10";
                    break;
                case "Q":
                    v = "10";
                    break;
                case "K":
                    v = "10";
                    break;
                case "A":
                    if (Playerhand + 11 >= 21)
                    {
                        v = "1";
                    }
                    else
                    {
                        v = "11";
                    }
                    break;
            }
            return int.Parse(v);

        }

        public void CheckGameStatus() { //při vyvolání zkontroluje stav hry
            
                switch (DealerHand)
                {
                    case > 21:  //pokud má dealer karty se součtem nad 21, vyvolává ukončení hry se stavem DLoose (dealer prohrává)
                        
                        EndGame("DLoose");
                        ShowWin(true);
                        break;

                    case 21: //pokud má dealer součet 21 tak hráč hned prohrává
                        
                        EndGame("DMax");
                        ShowWin(false);
                    break;

                    default:
                        
                        break;


                }



                switch (Playerhand)
                {
                    case > 21: //hráč má nad 21 a prohrává
                        
                        EndGame("PLoose");
                        ShowWin(false);
                        break;

                    case 21:
                        
                        EndGame("PMax"); //hráč má hodnotu 21 a vyhrává, v tomto případě získává větší vuhru definovanou v proměnné WinMultiply21
                    ShowWin(true);
                        break;
                        
                    default:
                        
                        break;
                }
            if (!Playing) { //pokud hráč Standnul tak se porovnají součty karet obou stran a vybere se výherce
                if (DealerHand > Playerhand)
                {
                    
                    EndGame("PLoose");
                    ShowWin(false);
                }
                if (DealerHand < Playerhand)
                {
                    
                    EndGame("DLoose");
                    ShowWin(true);
                }
                if (DealerHand == Playerhand) // v případě remízy hráč prohrává, pokud je ale remíza se součtem 21 tak hráč vyhrává (původně chyba ale rozhodl jsem se to zde nechat aby byla hra těžší)
                {
                    UpdateBalance();
                    ResetUI();
                    EndMessage.Content = "Its a tie!";
                    EndMessage.Foreground = Brushes.Yellow;
                    EndMessage.Visibility = Visibility.Visible;

                }


            }
           




        }

        public void NewDealer()
        {
            
            Random random = new Random();
            string Dealer = Settings.DealerNames[random.Next(Settings.DealerNames.Length)];
            Settings.DealerName = Dealer;
            DealerName.Content = ($"{Settings.DealerName}");


        }


        public void EndGame(string result) { //metoda pro ukončení hry která navazuje na metodu CheckGameStatus
            if (HiddenCard) //jelikož hra končí tak dealer odhalí svou 2. a případně další karty (pokud má součet pod 16) a vytáhne novou karty pro iluzi otočení své 2.
            {
                DealerDeck.Children.RemoveAt(1);
                DrawCard("dealer");
                HiddenCard = false;
            }
            if (result == "DLoose") //dealer prohrál, hráči se připisuje výhra stanovená v proměnné WinMultiply
            {
                balance += (CurrentBet * Settings.WinMultiply);
                UpdateBalance();
                ResetUI();
                
            }

            if (result == "DMax") //dealer maxnul a hráč prohrává, původně jsem zamýšlel že by v tomto případě hráč prohrál více než vsadil ale tak to nekonec nebude.
            {
                balance -= CurrentBet;
                UpdateBalance();
                ResetUI();
            }

            if (result == "PLoose") //hráč prohrál
            {
                balance -= CurrentBet;
                UpdateBalance();
                ResetUI();
            }

            if (result == "PMax") //hráč získal součet 21 a jednoznačně vyhrává maximální výhru stanovenou ve WinMultiply21
            {
                balance += (CurrentBet * Settings.WinMultiply21);
                UpdateBalance();
                ResetUI();
            }

        }
        private void AddCardToDeck(string suit, string value, string who) //metoda pro grafické zobrazení karet podle hodnot předaných z metody DrawCard()
        {
            Image img = new Image();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string imagePath = System.IO.Path.Combine(baseDirectory, "CardImages", $"{char.ToUpper(suit[0])}{value}.png"); //vyhledá png v herní složce s textury. v případě srdcové sedmy hledá H7 (Heart 7)

            if (File.Exists(imagePath))//pokud karta neexistuje tak nic nevykresluje (což se bez zásahu uživatele do textur karet stát nemůže)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.EndInit();

                img.Source = bitmap;

               
                    
                
                    img.Margin = new Thickness(-70, 0, 0, 0);//každou vykreslenou kartu posune o 70px doleva aby se karty překrývaly



                

                PlayerCards++; //počet karet které hráč drží narůstá

            }

            img.Width = 120; //velikost vykreslených karet
            img.Height = 200;


            if (who == "player") //vygeneruje kartu správné straně podle proměnné who
            {

                PlayerDeck.Children.Add(img);




            }

            else
            {


                DealerDeck.Children.Add(img);

            }
                
        }

        public void ShowWin(bool won) { //dá vědět hráči jestli vyhrál nebo prohrál svou sázku pomocí labelu EndMessage
            if (won)//jupí, vyhrál jsi
            {
                EndMessage.Content = "You Won!"; 
                EndMessage.Foreground = Brushes.Green;
                EndMessage.Visibility = Visibility.Visible;

            }
            if (!won)//exekuce
            {
                EndMessage.Content = "Busted!";
                EndMessage.Foreground = Brushes.Red;
                EndMessage.Visibility = Visibility.Visible;
            }
            
            
        
        
        
        }

        private void NewDealer_button_Click(object sender, RoutedEventArgs e)
        {
            NewDealer();
        }
    }
}

