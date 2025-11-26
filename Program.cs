using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

namespace AdamLaurinGameF23
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Int32 menuchoice;
            const Int32 defaultmovespeed = 1500;
            Int32 movespeed = defaultmovespeed;

            do
            {
                Console.Clear();

                Console.SetCursorPosition(10, 3);
                Console.Write("Welcome to \"Labyrinth Explorer!\"");
                Console.SetCursorPosition(10, 5);
                Console.Write("Your objective is simple: collect all of the coins as fast as possible");
                Console.SetCursorPosition(10, 7);
                Console.Write("+  <-  This is you! Use the ARROW KEYS to move around");
                Console.SetCursorPosition(10, 9);
                Console.Write("*  <-  This is a coin. Walk over it to pick it up. Rooms containing coins will have randomized");
                Console.SetCursorPosition(17, 10);
                Console.Write("placement, so they won't be in the same spot every game!");
                Console.SetCursorPosition(10, 12);
                Console.Write("@  <-  This is a portal. Enter it to travel between rooms");
                Console.SetCursorPosition(10, 14);
                Console.Write("#  <-  This is a Guardian. Watch out! They patrol the Labyrinth and if they touch you, you lose!");
                Console.SetCursorPosition(10, 16);
                Console.Write("Press ESC at any time to end the game");
                Console.SetCursorPosition(10, 19);
                Console.Write("1. Start Game");
                Console.SetCursorPosition(10, 20);
                Console.Write("2. Adjust Game Speed");
                Console.SetCursorPosition(10, 21);
                Console.Write("3. Exit");
                Console.SetCursorPosition(10, 23);
                Console.Write("Enter a number for the option you would like to select: ");
                menuchoice = ValidMenuChoice();
                
                if (menuchoice == 1)
                {
                    StartGame(movespeed);
                }
                if (menuchoice == 2)
                {
                    SetGameSpeed(ref movespeed, defaultmovespeed);  
                }
            } while (menuchoice != 3);
        } //end of Main

        static void SetGameSpeed(ref Int32 rmovespeed, Int32 ddefaultmovespeed)
        {
            Int32 userinput;
            bool validchoice;
            
            Console.Clear();

            Console.SetCursorPosition(10, 3);
            Console.Write("If you find the game too slow or too fast, you can adjust the speed here");
            Console.SetCursorPosition(10, 4);
            Console.Write("A SMALLER number makes the game FASTER, a LARGER number makes it SLOWER");
            Console.SetCursorPosition(10, 5);
            Console.Write("Current game speed is " + rmovespeed + ", the default speed is " + ddefaultmovespeed);
            Console.SetCursorPosition(10, 7);
            Console.Write("Set game speed: ");
            do
            {
                validchoice = Int32.TryParse(Console.ReadLine(), out userinput);
                if (!validchoice || userinput < 0)
                {
                    validchoice = false;
                    Console.Write("          Invalid input, please enter a whole number ");
                }
            } while (!validchoice || userinput < 0);

            rmovespeed = userinput;
        } //SetGameSpeed end

        static Int32 ValidMenuChoice()
        {
            Int32 userinput;
            bool validchoice;
            do
            {
                validchoice = Int32.TryParse(Console.ReadLine(), out userinput);
                if (!validchoice || userinput < 1 || userinput > 3)
                {
                    Console.Write("          Invalid input, please enter 1, 2, or 3 ");
                }
            } while (!validchoice || userinput < 1 || userinput > 3);
            return userinput;
        } //ValidMenuChoice end

        static void StartGame(Int32 movespeed)
        {
            Console.Clear();

            Random random = new Random();
            bool gameover = false;
            bool win;
            bool move = false, guardianmove = false;
            bool newroom = true;
            bool[] coinscollected = { false, false, false, false }; //rooms 3, 4, 5, 6
            ConsoleKey k = ConsoleKey.NoName;
            string guardiandirection = "D";
            Int32 meX = 15, meY = 13, oldmeX = meX, oldmeY = meY;
            Int32 movecounter = 0, guardiancounter = 0;
            Int32 guardianX = 13, guardianY = 3, oldguardianX = guardianX, oldguardianY = guardianY;
            Int32 guardianspeed = movespeed;
            Int32 room = 1;
            Int32 roomlength = 30;
            Int32 gametimer = 0, timercounter = 0;
            Int32[] coinpositions = new Int32[4]; //rooms 3, 4, 5, 6

            for (Int32 i = 0; i < coinpositions.Length; i++)
            {
                coinpositions[i] = random.Next(0, 4);
            }

            Console.CursorVisible = false;

            do
            {
                GetKey(ref k);
                if (k == ConsoleKey.Escape)
                {
                    gameover = true;
                }

                win = CheckWin(coinscollected, ref gameover);

                //ManualSetRoom(k, ref room, ref newroom); // DEV TOOL, Uncomment to use. Allows user to manually set the room by using number keys

                movecounter++;
                if (movecounter >= movespeed)
                {
                    oldmeX = meX;
                    oldmeY = meY;
                    MoveMe(k, ref meX, ref meY, ref room, ref move, ref newroom, coinpositions, coinscollected);
                    k = ConsoleKey.NoName;
                    movecounter = 0;                    
                }

                guardiancounter++;
                if (guardiancounter >= guardianspeed)
                {
                    oldguardianX = guardianX;
                    oldguardianY = guardianY;
                    MoveGuardian(room, ref guardianX, ref guardianY, meX, meY, ref gameover, ref guardiandirection);
                    guardianmove = true;
                    guardiancounter = 0;
                }

                timercounter++;
                if (timercounter >= movespeed * 6)
                {
                    gametimer++;
                    timercounter = 0;
                    if (gametimer < 0) //on the off chance that your time exceeds 2,147,483,647
                    {
                        gameover = true;
                    }
                }

                if (newroom)
                {
                    SetSpawn(ref meX, ref meY, ref guardianX, ref guardianY, ref guardiandirection, room, ref guardianspeed, movespeed);
                    oldmeX = meX;
                    oldmeY = meY;
                    oldguardianX = guardianX;
                    oldguardianY = guardianY;
                }

                Draw(meX, meY, oldmeX, oldmeY, guardianX, guardianY, oldguardianX, oldguardianY, room, ref roomlength, ref move, ref guardianmove, ref newroom, coinpositions, coinscollected, gametimer);

            } while (!gameover);

            Console.Clear();
            
            if (win)
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - 21, (Console.WindowHeight / 2) - 4);
                Console.WriteLine("Congratulations! You collected all " + coinscollected.Length + " coins!");
                Console.SetCursorPosition((Console.WindowWidth / 2) - 25, (Console.WindowHeight / 2) - 2);
                Console.WriteLine("Your time was " + gametimer + ". See if you can beat it next time!");
            }
            else
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - 5, (Console.WindowHeight / 2) - 4);
                Console.Write("Game Over!");
                Console.SetCursorPosition((Console.WindowWidth / 2) - 10, (Console.WindowHeight / 2) - 2);
                Console.Write("Better luck next time");
            }

            Console.SetCursorPosition((Console.WindowWidth / 2) - 15, Console.WindowHeight / 2);
            Console.Write("Press \"Enter\" to return to menu");
            Console.ReadLine();
        } //end of StartGame

        //************************** Dev tool. Uncomment line 138 above to access it.
        static void ManualSetRoom(ConsoleKey dk, ref Int32 rroom, ref bool rnewroom) 
        {
            switch (dk)
            {
                case ConsoleKey.D1:
                    rroom = 1; 
                    rnewroom = true;
                    break;
                case ConsoleKey.D2:
                    rroom = 2;
                    rnewroom = true;
                    break;
                case ConsoleKey.D3:
                    rroom = 3;
                    rnewroom = true;
                    break;
                case ConsoleKey.D4:
                    rroom = 4;
                    rnewroom = true;
                    break;
                case ConsoleKey.D5:
                    rroom = 5;
                    rnewroom = true;
                    break;
                case ConsoleKey.D6:
                    rroom = 6;
                    rnewroom = true;
                    break;
            }
        }
        //**************************


        /*
        *  #### Game Mechanics Functions ####
        */

        static void GetKey(ref ConsoleKey rk)
        {

            if (Console.KeyAvailable)
            {
                rk = Console.ReadKey(true).Key;
            }

        } //getKey end

        static void MoveMe(ConsoleKey dk, ref Int32 rmeX, ref Int32 rmeY, ref Int32 rroom, ref bool rmove, ref bool rnewroom, Int32[] coinpositions, bool[] coinscollected)
        {
            bool check;

            switch (dk)
            {
                case ConsoleKey.UpArrow:
                    rmeY--;                    
                    check = WhichRoomCheck(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    if (!check)                            
                    {                                      
                        rmeY++;
                    }
                    rmove = true;
                    break;
                case ConsoleKey.DownArrow:
                    rmeY++;
                    check = WhichRoomCheck(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    if (!check)
                    {
                        rmeY--;
                    }
                    rmove = true;
                    break;
                case ConsoleKey.LeftArrow:
                    rmeX--;
                    check = WhichRoomCheck(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    if (!check)
                    {
                        rmeX++;
                    }
                    rmove = true;
                    break;
                case ConsoleKey.RightArrow:
                    rmeX++;
                    check = WhichRoomCheck(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    if (!check)
                    {
                        rmeX--;
                    }
                    rmove = true;
                    break;
                default:
                    break;
            }

            if (rmeX >= Console.WindowWidth)  //keep you on the screen
            {
                rmeX = 0;
            }
            if (rmeX < 0)
            {
                rmeX = Console.WindowWidth - 1;
            }
            if (rmeY >= Console.WindowHeight)
            {
                rmeY = 0;
            }
            if (rmeY < 0)
            {
                rmeY = Console.WindowHeight - 1;
            }
        } //MoveMe end

        static void MoveGuardian(Int32 droom, ref Int32 rguardianX, ref Int32 rguardianY, Int32 dmeX, Int32 dmeY, ref bool rgameover, ref string rguardiandirection)
        {           
            
            switch (rguardiandirection)
            {
                case "D":
                    rguardianY++;
                    break;
                case "U":
                    rguardianY--;
                    break;
                case "L":
                    rguardianX--;
                    break;
                case "R":
                    rguardianX++;
                    break;
            }

            switch (droom)
            {
                case 1:
                    Room1GuardianTurns(ref rguardiandirection, rguardianX, rguardianY);
                    break;
                case 2:
                    Room2GuardianTurns(ref rguardiandirection, rguardianX, rguardianY);
                    break;
                case 3:
                    Room3GuardianTurns(ref rguardiandirection, rguardianX, rguardianY);
                    break;
                case 4:
                    Room4GuardianTurns(ref rguardiandirection, rguardianX, rguardianY);
                    break;
                case 5:
                    Room5GuardianTurns(ref rguardiandirection, rguardianX, rguardianY);
                    break;
                case 6:
                    Room6GuardianTurns(ref rguardiandirection, rguardianX, rguardianY);
                    break;
            }

            if ((rguardianX == dmeX) && (rguardianY == dmeY))
            {
                rgameover = true;
            }
        }

        static void SetSpawn (ref Int32 rmeX, ref Int32 rmeY, ref Int32 rguardianX, ref Int32 rguardianY, ref string rguardiandirection, Int32 droom, ref Int32 rguardianspeed, Int32 dmovespeed)
        {
            switch (droom)
            {
                case 1:
                    rmeX = 15;
                    rmeY = 13;
                    rguardianX = 13;
                    rguardianY = 3;
                    rguardiandirection = "D";
                    rguardianspeed = dmovespeed;
                    break;
                case 2:
                    rmeX = 2;
                    rmeY = 3;
                    rguardianX = 14;
                    rguardianY = 4;
                    rguardiandirection = "L";
                    rguardianspeed = dmovespeed / 2;
                    break;
                case 3:
                    rmeX = 14;
                    rmeY = 8;
                    rguardianX = 9;
                    rguardianY = 8;
                    rguardiandirection = "R";
                    rguardianspeed = (Int32)(dmovespeed / 1.5);
                    break;
                case 4:
                    rmeX = 5;
                    rmeY = 1;
                    rguardianX = 1;
                    rguardianY = 19;
                    rguardiandirection = "R";
                    rguardianspeed = dmovespeed / 4;
                    break;
                case 5:
                    rmeX = 1;
                    rmeY = 18;
                    rguardianX = 10;
                    rguardianY = 8;
                    rguardiandirection = "L";
                    rguardianspeed = dmovespeed * 2;
                    break;
                case 6:
                    rmeX = 18;
                    rmeY = 8;
                    rguardianX = 1;
                    rguardianY = 7;
                    rguardiandirection = "R";
                    rguardianspeed = dmovespeed;
                    break;
            }
        } //SetSpawn end

        static void Draw(Int32 dmeX, Int32 dmeY, Int32 doldmeX, Int32 doldmeY, Int32 dguardianX, Int32 dguardianY, Int32 doldguardianX, Int32 doldguardianY, Int32 droom, ref Int32 rroomlength, ref bool rmove, ref bool rguardianmove, ref bool rnewroom, Int32[] coinpositions, bool[] coinscollected, Int32 dgametimer)
        {
            
            if (rnewroom)
            {
                Console.Clear();
                rroomlength = WhichRoomDraw(droom, coinpositions, coinscollected);
                rnewroom = false;
            }

            if (rmove)
            {
                Console.SetCursorPosition(doldmeX, doldmeY);
                Console.Write(" ");
                rmove = false;
            }

            Console.SetCursorPosition(dmeX, dmeY);
            Console.Write("+");

            if (rguardianmove)
            {
                Console.SetCursorPosition(doldguardianX, doldguardianY);
                Console.Write(" ");
                rguardianmove = false;
            }

            Console.SetCursorPosition(dguardianX, dguardianY);
            Console.Write("#");

            RoomUI(rroomlength, droom, coinscollected, dgametimer);
        } //Draw end

        static void RoomUI(Int32 droomlength, Int32 droom, bool[] coinscollected, Int32 dgametimer)
        {
            Int32 coins = 0;

            for (int i = 0; i < coinscollected.Length; i++)
            {
                if (coinscollected[i])
                {
                    coins++;
                }
            }

            Console.SetCursorPosition(droomlength + 4, 1);
            Console.Write("Room " + droom);
            Console.SetCursorPosition(droomlength + 4, 3);
            Console.Write("Coins collected: " + coins + "/" + coinscollected.Length);
            Console.SetCursorPosition(droomlength + 4, 5);
            Console.Write("Time: " + dgametimer);
        } //RoomUI end

        static bool CheckWin(bool[] coinscollected, ref bool rgameover)
        {
            bool win = true;

            for (int i = 0; i < coinscollected.Length; i++)
            {
                if (!coinscollected[i])
                {
                    win = false;
                }
            }

            if (win)
            {
                rgameover = true;
            }
            return win;
        }



        /*
         *  ##### Room Functions #####
         */

        static bool WhichRoomCheck(Int32 rmeX, Int32 rmeY, ref Int32 rroom, ref bool rnewroom, Int32[] coinpositions, bool[] coinscollected)
        {
            bool check = true;
            switch (rroom)
            {
                case 1:
                    check = Room1Check(rmeX, rmeY, ref rroom, ref rnewroom);
                    break;
                case 2:
                    check = Room2Check(rmeX, rmeY, ref rroom, ref rnewroom);
                    break;
                case 3:
                    check = Room3Check(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    break;
                case 4:
                    check = Room4Check(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    break;
                case 5:
                    check = Room5Check(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    break;
                case 6:
                    check = Room6Check(rmeX, rmeY, ref rroom, ref rnewroom, coinpositions, coinscollected);
                    break;
            }
            return check;
        } //end of WhichRoomCheck

        static Int32 WhichRoomDraw(Int32 droom, Int32[] coinpositions, bool[] coinscollected)
        {
            Int32 roomlength;
            
            switch (droom)
            {
                case 1:
                    roomlength = Room1Draw();
                    break;
                case 2:
                    roomlength = Room2Draw();
                    break;
                case 3:
                    roomlength = Room3Draw(coinpositions, coinscollected);
                    break;
                case 4:
                    roomlength = Room4Draw(coinpositions, coinscollected);
                    break;
                case 5:
                    roomlength = Room5Draw(coinpositions, coinscollected);
                    break;
                case 6:
                    roomlength = Room6Draw(coinpositions, coinscollected);
                    break;
                default:
                    roomlength = 30;
                    break;
            }
            return roomlength;
        }

        //expand these if you want to take a look. I keep them folded for easier navigation.
        static bool Room1Check(Int32 rmeX, Int32 rmeY, ref Int32 rroom, ref bool rnewroom)
        {
            bool check = true;

            Int32[] wY;

            Int32[] wall1Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 }; // X position 0
            Int32[] wall2Y = { 0, 1, 3, 4, 5, 6, 7, 11, 21, 29 };
            Int32[] wall3Y = { 0, 1, 3, 4, 5, 6, 7, 11, 21, 22, 23, 24, 25, 26, 29 };
            Int32[] wall4Y = { 0, 1, 3, 9, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 26, 29 };
            Int32[] wall5Y = { 0, 1, 3, 9, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 26, 29 };
            Int32[] wall6Y = { 0, 1, 3, 5, 6, 7, 9, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 24, 26, 29 };
            Int32[] wall7Y = { 0, 1, 3, 5, 6, 7, 8, 9, 11, 13, 24, 25, 26, 29 };
            Int32[] wall8Y = { 0, 1, 3, 5, 6, 11, 13, 24, 29 };
            Int32[] wall9Y = { 0, 1, 3, 5, 6, 11, 13, 15, 16, 17, 18, 19, 21, 22, 23, 24, 29 };
            Int32[] wall10Y = { 0, 3, 6, 8, 9, 10, 11, 13, 15, 19, 29 };
            Int32[] wall11Y = { 0, 3, 6, 13, 15, 19, 26, 29 };
            Int32[] wall12Y = { 0, 2, 3, 5, 6, 13, 15, 17, 19, 20, 21, 22, 23, 24, 25, 26, 29 };
            Int32[] wall13Y = { 0, 2, 6, 7, 8, 9, 10, 11, 13, 15, 17, 21, 29 };
            Int32[] wall14Y = { 0, 2, 17, 21, 29 };
            Int32[] wall15Y = { 0, 2, 3, 4, 17, 19, 21, 24, 25, 26, 29 };
            Int32[] wall16Y = { 0, 6, 7, 8, 9, 10, 11, 15, 16, 17, 19, 21, 29 };
            Int32[] wall17Y = { 0, 9, 19, 21, 29 };
            Int32[] wall18Y = { 0, 1, 2, 4, 5, 9, 13, 19, 21, 22, 23, 24, 25, 26, 27, 29 };
            Int32[] wall19Y = { 0, 2, 4, 5, 7, 9, 11, 13, 14, 15, 16, 17, 18, 19, 29 };
            Int32[] wall20Y = { 0, 2, 5, 11, 19, 29 };
            Int32[] wall21Y = { 0, 11, 19, 20, 21, 22, 23, 25, 26, 27, 28, 29 };
            Int32[] wall22Y = { 0, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 23, 25, 29 };
            Int32[] wall23Y = { 0, 1, 2, 3, 4, 7, 15, 17, 23, 25, 29 };
            Int32[] wall24Y = { 0, 4, 5, 7, 15, 29 };
            Int32[] wall25Y = { 0, 4, 7, 8, 9, 11, 12, 13, 15, 27, 29 };
            Int32[] wall26Y = { 0, 2, 4, 7, 9, 11, 15, 17, 18, 19, 20, 21, 27, 29 };
            Int32[] wall27Y = { 0, 2, 4, 5, 6, 7, 9, 11, 13, 14, 15, 17, 21, 22, 23, 24, 25, 26, 27, 29 };
            Int32[] wall28Y = { 0, 2, 11, 17, 29 };
            Int32[] wall29Y = { 0, 1, 2, 11, 17, 29 };
            Int32[] wall30Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };


            Int32 roomlength = 30;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++)
                {

                    if (rmeX == i && rmeY == wY[k])
                    {
                        check = false;
                        break;
                    }
                }

                switch (rmeX) //portal checks
                {
                    case 1: //room2 and 5 portals
                        if (rmeY == 2) 
                        {
                            rroom = 2;
                            rnewroom = true;
                        }
                        if (rmeY == 23)
                        {
                            rroom = 5;
                            rnewroom = true;
                        }
                        break;
                    case 27: //room4 and ? portals
                        if (rmeY == 1)
                        {
                            rroom = 4;
                            rnewroom = true;
                        }
                        if (rmeY == 19)
                        {
                            rroom = 6; //subject to change
                            rnewroom = true;
                        }
                        break;
                }
            }
            return check;
        } //end of Room1Check

        static Int32 Room1Draw()
        {
            Int32[] wY;

            Int32[] wall1Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
            Int32[] wall2Y = { 0, 1, 3, 4, 5, 6, 7, 11, 21, 29 };
            Int32[] wall3Y = { 0, 1, 3, 4, 5, 6, 7, 11, 21, 22, 23, 24, 25, 26, 29 };
            Int32[] wall4Y = { 0, 1, 3, 9, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 26, 29 };
            Int32[] wall5Y = { 0, 1, 3, 9, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 26, 29 };
            Int32[] wall6Y = { 0, 1, 3, 5, 6, 7, 9, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 24, 26, 29 };
            Int32[] wall7Y = { 0, 1, 3, 5, 6, 7, 8, 9, 11, 13, 24, 25, 26, 29 };
            Int32[] wall8Y = { 0, 1, 3, 5, 6, 11, 13, 24, 29 };
            Int32[] wall9Y = { 0, 1, 3, 5, 6, 11, 13, 15, 16, 17, 18, 19, 21, 22, 23, 24, 29 };
            Int32[] wall10Y = { 0, 3, 6, 8, 9, 10, 11, 13, 15, 19, 29 };
            Int32[] wall11Y = { 0, 3, 6, 13, 15, 19, 26, 29 };
            Int32[] wall12Y = { 0, 2, 3, 5, 6, 13, 15, 17, 19, 20, 21, 22, 23, 24, 25, 26, 29 };
            Int32[] wall13Y = { 0, 2, 6, 7, 8, 9, 10, 11, 13, 15, 17, 21, 29 };
            Int32[] wall14Y = { 0, 2, 17, 21, 29 };
            Int32[] wall15Y = { 0, 2, 3, 4, 17, 19, 21, 24, 25, 26, 29 };
            Int32[] wall16Y = { 0, 6, 7, 8, 9, 10, 11, 15, 16, 17, 19, 21, 29 };
            Int32[] wall17Y = { 0, 9, 19, 21, 29 };
            Int32[] wall18Y = { 0, 1, 2, 4, 5, 9, 13, 19, 21, 22, 23, 24, 25, 26, 27, 29 };
            Int32[] wall19Y = { 0, 2, 4, 5, 7, 9, 11, 13, 14, 15, 16, 17, 18, 19, 29 };
            Int32[] wall20Y = { 0, 2, 5, 11, 19, 29 };
            Int32[] wall21Y = { 0, 11, 19, 20, 21, 22, 23, 25, 26, 27, 28, 29 };
            Int32[] wall22Y = { 0, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 23, 25, 29 };
            Int32[] wall23Y = { 0, 1, 2, 3, 4, 7, 15, 17, 23, 25, 29 };
            Int32[] wall24Y = { 0, 4, 5, 7, 15, 29 };
            Int32[] wall25Y = { 0, 4, 7, 8, 9, 11, 12, 13, 15, 27, 29 };
            Int32[] wall26Y = { 0, 2, 4, 7, 9, 11, 15, 17, 18, 19, 20, 21, 27, 29 };
            Int32[] wall27Y = { 0, 2, 4, 5, 6, 7, 9, 11, 13, 14, 15, 17, 21, 22, 23, 24, 25, 26, 27, 29 };
            Int32[] wall28Y = { 0, 2, 11, 17, 29 };
            Int32[] wall29Y = { 0, 1, 2, 11, 17, 29 };
            Int32[] wall30Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };


            Int32 roomlength = 30;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++) //drawing walls
                {
                    Console.SetCursorPosition(i, wY[k]);
                    Console.Write("|");
                }
            }

            Console.SetCursorPosition(1, 2); //room2 portal
            Console.Write("@");

            Console.SetCursorPosition(27, 1); //room4 portal
            Console.Write("@");

            Console.SetCursorPosition(1, 23); //room5 portal
            Console.Write("@");

            Console.SetCursorPosition(27, 19); //room6 (?) portal
            Console.Write("@");

            return roomlength;
        }//Room1Draw end

        static void Room1GuardianTurns(ref string rguardiandirection, Int32 dguardianX, Int32 dguardianY)
        {
            switch (dguardianX)
            {
                case 7:
                    if (dguardianY == 14)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 20)
                    {
                        rguardiandirection = "R";
                    }
                    break;
                case 9:
                    if (dguardianY == 20)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 28)
                    {
                        rguardiandirection = "R";
                    }
                    break;
                case 13:
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 14)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 15:
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 17:
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 12)
                    {
                        rguardiandirection="U";
                    }
                    break;
                case 19:
                    if (dguardianY == 24)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 28)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 20:
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "U";
                    }
                    if (dguardianY == 12)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 18)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 24:
                    if (dguardianY == 18)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 24)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                default:
                    break;
            }
        } //Room1GuardianTurns end

        static bool Room2Check(Int32 rmeX, Int32 rmeY, ref Int32 rroom, ref bool rnewroom)
        {
            bool check = true;

            Int32[] wY;

            Int32[] wall1Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // X position 0
            Int32[] wall2Y = { 0, 8 };
            Int32[] wall3Y = { 0, 8 };
            Int32[] wall4Y = { 0, 4, 5, 6, 8 };
            Int32[] wall5Y = { 0, 1, 2, 3, 4, 6, 8, 9, 10 };
            Int32[] wall6Y = { 6, 10 };
            Int32[] wall7Y = { 6, 8, 10 };
            Int32[] wall8Y = { 6, 8, 10 };
            Int32[] wall9Y = { 3, 4, 5, 6, 8, 10 };
            Int32[] wall10Y = { 3, 8, 10 };
            Int32[] wall11Y = { 1, 2, 3, 8, 10, 11, 12 };
            Int32[] wall12Y = { 1, 3, 5, 6, 7, 8, 12 };
            Int32[] wall13Y = { 1, 3, 5, 12 };
            Int32[] wall14Y = { 1, 7, 8, 9, 12 };
            Int32[] wall15Y = { 1, 2, 3, 7, 12 };
            Int32[] wall16Y = { 3, 4, 5, 6, 7, 12 };
            Int32[] wall17Y = { 7, 9, 10, 12, 13, 14 };
            Int32[] wall18Y = { 7, 9, 10, 14 };
            Int32[] wall19Y = { 7, 9, 10, 12, 14 };
            Int32[] wall20Y = { 4, 5, 6, 7, 9, 12, 14 };
            Int32[] wall21Y = { 4, 12, 13, 14 };
            Int32[] wall22Y = { 4, 11, 12 };
            Int32[] wall23Y = { 4, 6, 7, 8, 9, 11 };
            Int32[] wall24Y = { 4, 6, 7, 11 };
            Int32[] wall25Y = { 4, 11 };
            Int32[] wall26Y = { 4, 6, 7, 9, 10, 11 };
            Int32[] wall27Y = { 4, 6, 7, 9 };
            Int32[] wall28Y = { 4, 6, 7, 9, 10, 11, 12, 13, 14 };
            Int32[] wall29Y = { 4, 6, 7, 14 };
            Int32[] wall30Y = { 4, 6, 7, 14 };
            Int32[] wall31Y = { 4, 6, 7, 9, 10, 11, 12, 14 };
            Int32[] wall32Y = { 4, 9, 14 };
            Int32[] wall33Y = { 4, 9, 14 };
            Int32[] wall34Y = { 4, 5, 6, 7, 8, 9, 14 };
            Int32[] wall35Y = { 9, 10, 11, 12, 13, 14 };

            Int32 roomlength = 35;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    case 30:
                        wY = wall31Y;
                        break;
                    case 31:
                        wY = wall32Y;
                        break;
                    case 32:
                        wY = wall33Y;
                        break;
                    case 33:
                        wY = wall34Y;
                        break;
                    case 34:
                        wY = wall35Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++)
                {

                    if (rmeX == i && rmeY == wY[k])
                    {
                        check = false;
                        break;
                    }
                }

                switch (rmeX) //portal checks
                {
                    case 2: //room1 portal
                        if (rmeY == 2)
                        {
                            rroom = 1;
                            rnewroom = true;
                        }
                        break;
                    case 32: //room3 portal
                        if (rmeY == 11)
                        {
                            rroom = 3;
                            rnewroom = true;
                        }
                        break;
                }
            }
            return check;
        } //end of Room2Check

        static Int32 Room2Draw()
        {
            Int32[] wY;

            Int32[] wall1Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // X position 0
            Int32[] wall2Y = { 0, 8 };
            Int32[] wall3Y = { 0, 8 };
            Int32[] wall4Y = { 0, 4, 5, 6, 8 };
            Int32[] wall5Y = { 0, 1, 2, 3, 4, 6, 8, 9, 10 };
            Int32[] wall6Y = { 6, 10 };
            Int32[] wall7Y = { 6, 8, 10 };
            Int32[] wall8Y = { 6, 8, 10 };
            Int32[] wall9Y = { 3, 4, 5, 6, 8, 10 };
            Int32[] wall10Y = { 3, 8, 10 };
            Int32[] wall11Y = { 1, 2, 3, 8, 10, 11, 12 };
            Int32[] wall12Y = { 1, 3, 5, 6, 7, 8, 12 };
            Int32[] wall13Y = { 1, 3, 5, 12 };
            Int32[] wall14Y = { 1, 7, 8, 9, 12 };
            Int32[] wall15Y = { 1, 2, 3, 7, 12 };
            Int32[] wall16Y = { 3, 4, 5, 6, 7, 12 };
            Int32[] wall17Y = { 7, 9, 10, 12, 13, 14 };
            Int32[] wall18Y = { 7, 9, 10, 14 };
            Int32[] wall19Y = { 7, 9, 10, 12, 14 };
            Int32[] wall20Y = { 4, 5, 6, 7, 9, 12, 14 };
            Int32[] wall21Y = { 4, 12, 13, 14 };
            Int32[] wall22Y = { 4, 11, 12 };
            Int32[] wall23Y = { 4, 6, 7, 8, 9, 11 };
            Int32[] wall24Y = { 4, 6, 7, 11 };
            Int32[] wall25Y = { 4, 11 };
            Int32[] wall26Y = { 4, 6, 7, 9, 10, 11 };
            Int32[] wall27Y = { 4, 6, 7, 9 };
            Int32[] wall28Y = { 4, 6, 7, 9, 10, 11, 12, 13, 14 };
            Int32[] wall29Y = { 4, 6, 7, 14 };
            Int32[] wall30Y = { 4, 6, 7, 14 };
            Int32[] wall31Y = { 4, 6, 7, 9, 10, 11, 12, 14 };
            Int32[] wall32Y = { 4, 9, 14 };
            Int32[] wall33Y = { 4, 9, 14 };
            Int32[] wall34Y = { 4, 5, 6, 7, 8, 9, 14 };
            Int32[] wall35Y = { 9, 10, 11, 12, 13, 14 };

            Int32 roomlength = 35;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    case 30:
                        wY = wall31Y;
                        break;
                    case 31:
                        wY = wall32Y;
                        break;
                    case 32:
                        wY = wall33Y;
                        break;
                    case 33:
                        wY = wall34Y;
                        break;
                    case 34:
                        wY = wall35Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++) //drawing walls
                {
                    Console.SetCursorPosition(i, wY[k]);
                    Console.Write("|");
                }
            }

            Console.SetCursorPosition(2, 2); //room1 portal
            Console.Write("@");

            Console.SetCursorPosition(32, 11); //room3 portal
            Console.Write("@");

            return roomlength;
        }//Room2Draw end

        static void Room2GuardianTurns(ref string rguardiandirection, Int32 dguardianX, Int32 dguardianY)
        {
            switch (dguardianX)
            {
                case 5:
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "R";
                    }
                    break;
                case 10:
                    if (dguardianY == 4)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 11:
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "R";
                    }
                    break;
                case 12:
                    if (dguardianY == 6)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 11)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 13:
                    if (dguardianY == 4)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 6)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 14:
                    if (dguardianY == 8)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 20:
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 8)
                    {
                        rguardiandirection = "U";
                    }
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 11)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 23:
                    if (dguardianY == 8)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 31:
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 8)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                default:
                    break;
            }
        } //Room2GuardianTurns end

        static bool Room3Check(Int32 rmeX, Int32 rmeY, ref Int32 rroom, ref bool rnewroom, Int32[] coinpositions, bool[] coinscollected)
        {
            bool check = true;

            Int32[] wY;

            Int32[] wall1Y = { 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            Int32[] wall2Y = { 5, 9, 10, 13 };
            Int32[] wall3Y = { 5, 9, 10, 13 };
            Int32[] wall4Y = { 5, 9, 10, 11, 13 };
            Int32[] wall5Y = { 5, 6, 7, 13 };
            Int32[] wall6Y = { 7, 13 };
            Int32[] wall7Y = { 4, 5, 6, 7, 8, 9, 10, 11, 13 };
            Int32[] wall8Y = { 4, 13 };
            Int32[] wall9Y = { 3, 4, 6, 7, 9, 10, 11, 12, 13 };
            Int32[] wall10Y = { 3, 7, 9 };
            Int32[] wall11Y = { 3, 7, 9 };
            Int32[] wall12Y = { 3, 7, 9, 10 };
            Int32[] wall13Y = { 0, 1, 2, 3, 4, 5, 6, 7, 10 };
            Int32[] wall14Y = { 0, 3, 4, 5, 6, 10 };
            Int32[] wall15Y = { 0, 10 };
            Int32[] wall16Y = { 0, 10, 11, 12, 13, 14 };
            Int32[] wall17Y = { 0, 1, 2, 3, 4, 5, 6, 10, 14 };
            Int32[] wall18Y = { 6, 7, 9, 10, 14 };
            Int32[] wall19Y = { 7, 9, 14 };
            Int32[] wall20Y = { 7, 9, 11, 12, 13, 14 };
            Int32[] wall21Y = { 2, 3, 4, 5, 6, 7, 9, 11 };
            Int32[] wall22Y = { 2, 9, 11 };
            Int32[] wall23Y = { 2, 4, 6, 7, 8, 9, 11 };
            Int32[] wall24Y = { 2, 3, 4, 11 };
            Int32[] wall25Y = { 4, 5, 6, 7, 8, 9, 11 };
            Int32[] wall26Y = { 7, 11 };
            Int32[] wall27Y = { 7, 11 };
            Int32[] wall28Y = { 7, 11 };
            Int32[] wall29Y = { 7, 8, 9, 10, 11 };

            Int32 roomlength = 28;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++)
                {

                    if (rmeX == i && rmeY == wY[k])
                    {
                        check = false;
                        break;
                    }
                }

                switch (rmeX) //portal checks
                {
                    case 14: //room2 portal
                        if (rmeY == 1)
                        {
                            rroom = 2;
                            rnewroom = true;
                        }
                        break;
                }

                if (coinscollected[0] == false) //coin check
                {
                    switch (coinpositions[0])
                    {
                        case 0:
                            if ((rmeX == 2) && (rmeY == 7))
                            {
                                coinscollected[0] = true;
                            }
                            break;
                        case 1:
                            if ((rmeX == 10) && (rmeY == 5))
                            {
                                coinscollected[0] = true;
                            }
                            break;
                        case 2:
                            if ((rmeX == 17) && (rmeY == 12))
                            {
                                coinscollected[0] = true;
                            }
                            break;
                        case 3:
                            if ((rmeX == 26) && (rmeY == 9))
                            {
                                coinscollected[0] = true;
                            }
                            break;
                    }
                }
            }
            return check;
        } //end of Room3Check

        static Int32 Room3Draw(Int32[] coinpositions, bool[] coinscollected)
        {
            Int32[] wY;

            Int32[] wall1Y = { 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            Int32[] wall2Y = { 5, 9, 10, 13 };
            Int32[] wall3Y = { 5, 9, 10, 13 };
            Int32[] wall4Y = { 5, 9, 10, 11, 13 };
            Int32[] wall5Y = { 5, 6, 7, 13 };
            Int32[] wall6Y = { 7, 13 };
            Int32[] wall7Y = { 4, 5, 6, 7, 8, 9, 10, 11, 13 };
            Int32[] wall8Y = { 4, 13 };
            Int32[] wall9Y = { 3, 4, 6, 7, 9, 10, 11, 12, 13 };
            Int32[] wall10Y = { 3, 7, 9 };
            Int32[] wall11Y = { 3, 7, 9 };
            Int32[] wall12Y = { 3, 7, 9, 10 };
            Int32[] wall13Y = { 0, 1, 2, 3, 4, 5, 6, 7, 10 };
            Int32[] wall14Y = { 0, 3, 4, 5, 6, 10 };
            Int32[] wall15Y = { 0, 10 };
            Int32[] wall16Y = { 0, 10, 11, 12, 13, 14 };
            Int32[] wall17Y = { 0, 1, 2, 3, 4, 5, 6, 10, 14 };
            Int32[] wall18Y = { 6, 7, 9, 10, 14 };
            Int32[] wall19Y = { 7, 9, 14 };
            Int32[] wall20Y = { 7, 9, 11, 12, 13, 14 };
            Int32[] wall21Y = { 2, 3, 4, 5, 6, 7, 9, 11 };
            Int32[] wall22Y = { 2, 9, 11 };
            Int32[] wall23Y = { 2, 4, 6, 7, 8, 9, 11 };
            Int32[] wall24Y = { 2, 3, 4, 11 };
            Int32[] wall25Y = { 4, 5, 6, 7, 8, 9, 11 };
            Int32[] wall26Y = { 7, 11 };
            Int32[] wall27Y = { 7, 11 };
            Int32[] wall28Y = { 7, 11 };
            Int32[] wall29Y = { 7, 8, 9, 10, 11 };

            Int32 roomlength = 29;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++) //drawing walls
                {
                    Console.SetCursorPosition(i, wY[k]);
                    Console.Write("|");
                }
            }

            Console.SetCursorPosition(14, 1); //room2 portal
            Console.Write("@");

            if (coinscollected[0] == false) //coin draw
            {
                switch (coinpositions[0])
                {
                    case 0:
                        Console.SetCursorPosition(2, 7);
                        break;
                    case 1:
                        Console.SetCursorPosition(10, 5);
                        break;
                    case 2:
                        Console.SetCursorPosition(17, 12);
                        break;
                    case 3:
                        Console.SetCursorPosition(26, 9);
                        break;
                }
                Console.Write("*");
            }

            return roomlength;
        }//Room3Draw end

        static void Room3GuardianTurns(ref string rguardiandirection, Int32 dguardianX, Int32 dguardianY)
        {
            switch (dguardianX)
            {
                case 4:
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 12)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    break;
                case 7:
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 12)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                case 12:
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "R";
                    }
                    break;
                case 13:
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 8)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 16:
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "U")
                        {
                            rguardiandirection = "R";
                        }
                        else
                        {
                            rguardiandirection = "U";
                        }
                    }
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 18:
                    if (dguardianY == 10)
                    {
                        rguardiandirection = "R";
                    }
                    break;
                case 21:
                    if (dguardianY == 5)
                    {
                        if (rguardiandirection == "U")
                        {
                            rguardiandirection = "R";
                        }
                        else
                        {
                            rguardiandirection = "D";
                        }
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                case 23:
                    if (dguardianY == 5)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "D")
                        {
                            rguardiandirection = "L";
                        }
                        else
                        {
                            rguardiandirection = "U";
                        }
                    }
                    break;
                default:
                    break;
            }
        } //Room3GuardianTurns end

        static bool Room4Check(Int32 rmeX, Int32 rmeY, ref Int32 rroom, ref bool rnewroom, Int32[] coinpositions, bool[] coinscollected)
        {
            bool check = true;

            Int32[] wY;

            Int32[] wall1Y = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall2Y = { 2, 6, 11, 13, 17, 18, 20, 21, 22, 23, 24 };
            Int32[] wall3Y = { 0, 1, 2, 4, 6, 8, 9, 11, 12, 13, 15, 17, 18, 20, 24 };
            Int32[] wall4Y = { 0, 24 };
            Int32[] wall5Y = { 0, 24 };
            Int32[] wall6Y = { 0, 24 };
            Int32[] wall7Y = { 0, 1, 2, 4, 6, 8, 9, 11, 12, 13, 15, 17, 18, 20, 21, 22, 23, 24 };
            Int32[] wall8Y = { 2, 4, 8, 9, 15, 20 };
            Int32[] wall9Y = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall10Y = { 9, 10, 11, 13 };
            Int32[] wall11Y = { 9, 13 };
            Int32[] wall12Y = { 9, 13 };
            Int32[] wall13Y = { 9, 13 };
            Int32[] wall14Y = { 9, 10, 11, 12, 13 };

            Int32 roomlength = 14;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++)
                {

                    if (rmeX == i && rmeY == wY[k])
                    {
                        check = false;
                        break;
                    }
                }

                switch (rmeX) //portal checks
                {
                    case 3: //room1 portal
                        if (rmeY == 1)
                        {
                            rroom = 1;
                            rnewroom = true;
                        }
                        break;
                }

                if (coinscollected[1] == false) //coin check
                {
                    switch (coinpositions[1])
                    {
                        case 0:
                            if ((rmeX == 3) && (rmeY == 22))
                            {
                                coinscollected[1] = true;
                            }
                            break;
                        case 1:
                            if ((rmeX == 11) && (rmeY == 11))
                            {
                                coinscollected[1] = true;
                            }
                            break;
                        case 2:
                            if ((rmeX == 3) && (rmeY == 22))
                            {
                                coinscollected[1] = true;
                            }
                            break;
                        case 3:
                            if ((rmeX == 11) && (rmeY == 11))
                            {
                                coinscollected[1] = true;
                            }
                            break;
                    }
                }
            }
            return check;
        } //end of Room4Check

        static Int32 Room4Draw(Int32[] coinpositions, bool[] coinscollected)
        {
            Int32[] wY;

            Int32[] wall1Y = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall2Y = { 2, 6, 11, 13, 17, 18, 20, 21, 22, 23, 24 };
            Int32[] wall3Y = { 0, 1, 2, 4, 6, 8, 9, 11, 12, 13, 15, 17, 18, 20, 24 };
            Int32[] wall4Y = { 0, 24 };
            Int32[] wall5Y = { 0, 24 };
            Int32[] wall6Y = { 0, 24 };
            Int32[] wall7Y = { 0, 1, 2, 4, 6, 8, 9, 11, 12, 13, 15, 17, 18, 20, 21, 22, 23, 24 };
            Int32[] wall8Y = { 2, 4, 8, 9, 15, 20 };
            Int32[] wall9Y = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall10Y = { 9, 10, 11, 13 };
            Int32[] wall11Y = { 9, 13 };
            Int32[] wall12Y = { 9, 13 };
            Int32[] wall13Y = { 9, 13 };
            Int32[] wall14Y = { 9, 10, 11, 12, 13 };

            Int32 roomlength = 14;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++) //drawing walls
                {
                    Console.SetCursorPosition(i, wY[k]);
                    Console.Write("|");
                }
            }

            Console.SetCursorPosition(3, 1); //room1 portal
            Console.Write("@");

            if (coinscollected[1] == false) //coin draw
            {
                switch (coinpositions[1])
                {
                    case 0:
                        Console.SetCursorPosition(3, 22);
                        break;
                    case 1:
                        Console.SetCursorPosition(11, 11);
                        break;
                    case 2:
                        Console.SetCursorPosition(3, 22);
                        break;
                    case 3:
                        Console.SetCursorPosition(11, 11);
                        break;
                }
                Console.Write("*");
            }

            return roomlength;
        }//Room4Draw end

        static void Room4GuardianTurns(ref string rguardiandirection, Int32 dguardianX, Int32 dguardianY)
        {
            switch (dguardianX)
            {
                case 1:
                    if (dguardianY == 3)
                    {
                        if (rguardiandirection == "U")
                        {
                            rguardiandirection = "R";
                        }
                        else
                        {
                            rguardiandirection = "D";
                        }
                    }
                    if (dguardianY == 5)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 7)
                    {
                        if (rguardiandirection == "U")
                        {
                            rguardiandirection = "R";
                        }
                        else
                        {
                            rguardiandirection = "D";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 14)
                    {
                        if (rguardiandirection == "U")
                        {
                            rguardiandirection = "R";
                        }
                        else
                        {
                            rguardiandirection = "D";
                        }
                    }
                    if (dguardianY == 16)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 19)
                    {
                        rguardiandirection = "R";
                    }
                    break;

                case 7:
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 5)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 7)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 14)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 16)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 19)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                default:
                    break;
            }
        } //Room4GuardianTurns end

        static bool Room5Check(Int32 rmeX, Int32 rmeY, ref Int32 rroom, ref bool rnewroom, Int32[] coinpositions, bool[] coinscollected)
        {
            bool check = true;

            Int32[] wY;

            Int32[] wall1Y = { 17, 18, 19, 20 };
            Int32[] wall2Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 20 };
            Int32[] wall3Y = { 0, 9, 20 };
            Int32[] wall4Y = { 0, 2, 3, 4, 5, 6, 7, 9, 11, 13, 15, 16, 17, 18, 19, 20 };
            Int32[] wall5Y = { 0, 7, 11, 13, 15, 20 };
            Int32[] wall6Y = { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 13, 14, 15, 17, 18, 20 };
            Int32[] wall7Y = { 0, 5, 9, 13, 17, 20 };
            Int32[] wall8Y = { 0, 2, 3, 4, 5, 6, 7, 9, 11, 12, 13, 15, 16, 17, 19, 20 };
            Int32[] wall9Y = { 0, 9, 15, 17, 20 };
            Int32[] wall10Y = { 0, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 17, 18, 20 };
            Int32[] wall11Y = { 0, 2, 7, 18, 20 };
            Int32[] wall12Y = { 0, 2, 3, 4, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall13Y = { 0, 4, 7, 13, 20 };
            Int32[] wall14Y = { 0, 1, 2, 4, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 20 };
            Int32[] wall15Y = { 0, 4, 11, 15, 17, 20 };
            Int32[] wall16Y = { 0, 2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 14, 15, 17, 20 };
            Int32[] wall17Y = { 0, 9, 13, 17, 20 };
            Int32[] wall18Y = { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20 };
            Int32[] wall19Y = { 0, 7, 9, 20 };
            Int32[] wall20Y = { 0, 2, 3, 4, 5, 7, 9, 11, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall21Y = { 0, 2, 7, 9, 11, 17, 20 };
            Int32[] wall22Y = { 0, 2, 4, 5, 6, 7, 9, 11, 17, 18, 20 };
            Int32[] wall23Y = { 0, 2, 9, 13, 14, 17, 18, 20 };
            Int32[] wall24Y = { 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 16, 17, 18, 20 };
            Int32[] wall25Y = { 0, 11, 13, 20 };
            Int32[] wall26Y = { 0, 1, 2, 3, 4, 6, 7, 8, 9, 11, 12, 13, 17, 18, 19, 20 };
            Int32[] wall27Y = { 0, 6, 9, 17, 20 };
            Int32[] wall28Y = { 0, 2, 3, 4, 5, 6, 9, 10, 11, 12, 13, 14, 15, 16, 17, 20 };
            Int32[] wall29Y = { 0, 20 };
            Int32[] wall30Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            Int32 roomlength = 30;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++)
                {

                    if (rmeX == i && rmeY == wY[k])
                    {
                        check = false;
                        break;
                    }
                }

                switch (rmeX) //portal checks
                {
                    case 1: //room1 portal
                        if (rmeY == 19)
                        {
                            rroom = 1;
                            rnewroom = true;
                        }
                        break;
                }

                if (coinscollected[2] == false) //coin check
                {
                    switch (coinpositions[2])
                    {
                        case 0:
                            if ((rmeX == 10) && (rmeY == 5))
                            {
                                coinscollected[2] = true;
                            }
                            break;
                        case 1:
                            if ((rmeX == 16) && (rmeY == 18))
                            {
                                coinscollected[2] = true;
                            }
                            break;
                        case 2:
                            if ((rmeX == 20) && (rmeY == 15))
                            {
                                coinscollected[2] = true;
                            }
                            break;
                        case 3:
                            if ((rmeX == 27) && (rmeY == 19))
                            {
                                coinscollected[2] = true;
                            }
                            break;
                    }
                }
            }
            return check;
        } //end of Room5Check

        static Int32 Room5Draw(Int32[] coinpositions, bool[] coinscollected)
        {
            Int32[] wY;

            Int32[] wall1Y = { 17, 18, 19, 20 };
            Int32[] wall2Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 20 };
            Int32[] wall3Y = { 0, 9, 20 };
            Int32[] wall4Y = { 0, 2, 3, 4, 5, 6, 7, 9, 11, 13, 15, 16, 17, 18, 19, 20 };
            Int32[] wall5Y = { 0, 7, 11, 13, 15, 20 };
            Int32[] wall6Y = { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 13, 14, 15, 17, 18, 20 };
            Int32[] wall7Y = { 0, 5, 9, 13, 17, 20 };
            Int32[] wall8Y = { 0, 2, 3, 4, 5, 6, 7, 9, 11, 12, 13, 15, 16, 17, 19, 20 };
            Int32[] wall9Y = { 0, 9, 15, 17, 20 };
            Int32[] wall10Y = { 0, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 17, 18, 20 };
            Int32[] wall11Y = { 0, 2, 7, 18, 20 };
            Int32[] wall12Y = { 0, 2, 3, 4, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall13Y = { 0, 4, 7, 13, 20 };
            Int32[] wall14Y = { 0, 1, 2, 4, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 20 };
            Int32[] wall15Y = { 0, 4, 11, 15, 17, 20 };
            Int32[] wall16Y = { 0, 2, 3, 4, 5, 6, 7, 8, 9, 11, 13, 14, 15, 17, 20 };
            Int32[] wall17Y = { 0, 9, 13, 17, 20 };
            Int32[] wall18Y = { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20 };
            Int32[] wall19Y = { 0, 7, 9, 20 };
            Int32[] wall20Y = { 0, 2, 3, 4, 5, 7, 9, 11, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall21Y = { 0, 2, 7, 9, 11, 17, 20 };
            Int32[] wall22Y = { 0, 2, 4, 5, 6, 7, 9, 11, 17, 18, 20 };
            Int32[] wall23Y = { 0, 2, 9, 13, 14, 17, 18, 20 };
            Int32[] wall24Y = { 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 16, 17, 18, 20 };
            Int32[] wall25Y = { 0, 11, 13, 20 };
            Int32[] wall26Y = { 0, 1, 2, 3, 4, 6, 7, 8, 9, 11, 12, 13, 17, 18, 19, 20 };
            Int32[] wall27Y = { 0, 6, 9, 17, 20 };
            Int32[] wall28Y = { 0, 2, 3, 4, 5, 6, 9, 10, 11, 12, 13, 14, 15, 16, 17, 20 };
            Int32[] wall29Y = { 0, 20 };
            Int32[] wall30Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            Int32 roomlength = 30;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++) //drawing walls
                {
                    Console.SetCursorPosition(i, wY[k]);
                    Console.Write("|");
                }
            }

            Console.SetCursorPosition(1, 19); //room1 portal
            Console.Write("@");

            if (coinscollected[2] == false) //coin draw
            {
                switch (coinpositions[2])
                {
                    case 0:
                        Console.SetCursorPosition(10, 5);
                        break;
                    case 1:
                        Console.SetCursorPosition(16, 18);
                        break;
                    case 2:
                        Console.SetCursorPosition(20, 15);
                        break;
                    case 3:
                        Console.SetCursorPosition(27, 19);
                        break;
                }
                Console.Write("*");
            }

            return roomlength;
        }//Room5Draw end

        static void Room5GuardianTurns(ref string rguardiandirection, Int32 dguardianX, Int32 dguardianY)
        {
            switch (dguardianX)
            {
                case 2:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 12)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    break;
                case 4:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 6)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 16)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 19)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    break;
                case 6:
                    if (dguardianY == 6)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 12)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 14)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 16)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 18)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 19)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                case 8:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 14)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 18)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 19)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    break;
                case 12:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 3)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 14)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 19)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                case 14:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 3)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                case 16:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    break;
                case 18:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 6)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 19)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    break;
                case 20:
                    if (dguardianY == 3)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 6)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                case 22:
                    if (dguardianY == 3)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 8)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 12)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    break;
                case 24:
                    if (dguardianY == 1)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 12)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 15)
                    {
                        if (rguardiandirection == "L")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "R";
                        }
                    }
                    if (dguardianY == 19)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;                   
                case 26:
                    if (dguardianY == 10)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "D";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    if (dguardianY == 15)
                    {
                        if (rguardiandirection == "R")
                        {
                            rguardiandirection = "U";
                        }
                        else
                        {
                            rguardiandirection = "L";
                        }
                    }
                    break;
                default:
                    break;
            }
        } //Room5GuardianTurns end

        static bool Room6Check(Int32 rmeX, Int32 rmeY, ref Int32 rroom, ref bool rnewroom, Int32[] coinpositions, bool[] coinscollected)
        {
            bool check = true;

            Int32[] wY;

            Int32[] wall1Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall2Y = { 0, 2, 20 };
            Int32[] wall3Y = { 0, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 15, 16, 20 };
            Int32[] wall4Y = { 0, 6, 8, 12, 16, 20 };
            Int32[] wall5Y = { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 14, 16, 17, 18, 19, 20 };
            Int32[] wall6Y = { 0, 6, 14, 20 };
            Int32[] wall7Y = { 0, 2, 3, 4, 6, 8, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall8Y = { 0, 2, 6, 8, 10, 18, 20 };
            Int32[] wall9Y = { 0, 2, 4, 5, 6, 8, 10, 12, 13, 14, 15, 16, 18, 20 };
            Int32[] wall10Y = { 0, 2, 4, 8, 12, 16, 18, 20 };
            Int32[] wall11Y = { 0, 2, 4, 6, 7, 8, 9, 10, 12, 14, 16, 17, 18, 20 };
            Int32[] wall12Y = { 0, 2, 6, 12, 14, 20 };
            Int32[] wall13Y = { 0, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 14, 15, 16, 17, 18, 20 };
            Int32[] wall14Y = { 0, 2, 8, 14, 20 };
            Int32[] wall15Y = { 0, 2, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20 };
            Int32[] wall16Y = { 0, 2, 6, 8, 20 };
            Int32[] wall17Y = { 0, 2, 3, 4, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall18Y = { 0, 6, 10, 20 };
            Int32[] wall19Y = { 0, 2, 3, 4, 5, 6, 10, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall20Y = { 0, 6, 10, 20 };
            Int32[] wall21Y = { 0, 1, 2, 3, 4, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20 };
            Int32[] wall22Y = { 0, 4, 6, 16, 20 };
            Int32[] wall23Y = { 0, 2, 4, 6, 8, 10, 11, 12, 13, 14, 16, 18, 20 };
            Int32[] wall24Y = { 0, 2, 6, 8, 10, 14, 16, 18, 20 };
            Int32[] wall25Y = { 0, 2, 3, 4, 5, 6, 8, 10, 12, 13, 14, 16, 18, 20 };
            Int32[] wall26Y = { 0, 2, 8, 10, 16, 18, 20 };
            Int32[] wall27Y = { 0, 2, 4, 5, 6, 7, 8, 10, 12, 14, 15, 16, 17, 18, 20 };
            Int32[] wall28Y = { 0, 2, 6, 10, 12, 14, 20 };
            Int32[] wall29Y = { 0, 2, 3, 4, 5, 6, 8, 9, 10, 12, 14, 15, 16, 17, 18, 20 };
            Int32[] wall30Y = { 0, 4, 8, 12, 18, 20 };
            Int32[] wall31Y = { 0, 1, 2, 4, 6, 7, 8, 10, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall32Y = { 0, 4, 8, 10, 14, 18, 20 };
            Int32[] wall33Y = { 0, 2, 3, 4, 5, 6, 8, 10, 11, 12, 13, 14, 16, 18, 20 };
            Int32[] wall34Y = { 0, 2, 8, 16, 20 };
            Int32[] wall35Y = { 0, 2, 4, 8, 10, 11, 12, 13, 14, 15, 16, 20};
            Int32[] wall36Y = { 0, 4, 8, 16, 20 };
            Int32[] wall37Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            Int32 roomlength = 37;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    case 30:
                        wY = wall31Y;
                        break;
                    case 31:
                        wY = wall32Y;
                        break;
                    case 32:
                        wY = wall33Y;
                        break;
                    case 33:
                        wY = wall34Y;
                        break;
                    case 34:
                        wY = wall35Y;
                        break;
                    case 35:
                        wY = wall36Y;
                        break;
                    case 36:
                        wY = wall37Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++)
                {

                    if (rmeX == i && rmeY == wY[k])
                    {
                        check = false;
                        break;
                    }
                }

                switch (rmeX) //portal checks
                {
                    case 15: //room1 portal
                        if (rmeY == 7)
                        {
                            rroom = 1;
                            rnewroom = true;
                        }
                        break;
                }

                if (coinscollected[3] == false) //coin check
                {
                    switch (coinpositions[3])
                    {
                        case 0:
                            if ((rmeX == 2) && (rmeY == 18))
                            {
                                coinscollected[3] = true;
                            }
                            break;
                        case 1:
                            if ((rmeX == 18) && (rmeY == 12))
                            {
                                coinscollected[3] = true;
                            }
                            break;
                        case 2:
                            if ((rmeX == 34) && (rmeY == 6))
                            {
                                coinscollected[3] = true;
                            }
                            break;
                        case 3:
                            if ((rmeX == 34) && (rmeY == 18))
                            {
                                coinscollected[3] = true;
                            }
                            break;
                    }
                }
            }
            return check;
        } //end of Room6Check

        static Int32 Room6Draw(Int32[] coinpositions, bool[] coinscollected)
        {
            Int32[] wY;

            Int32[] wall1Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall2Y = { 0, 2, 20 };
            Int32[] wall3Y = { 0, 2, 4, 5, 6, 8, 9, 10, 12, 13, 14, 15, 16, 20 };
            Int32[] wall4Y = { 0, 6, 8, 12, 16, 20 };
            Int32[] wall5Y = { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 14, 16, 17, 18, 19, 20 };
            Int32[] wall6Y = { 0, 6, 14, 20 };
            Int32[] wall7Y = { 0, 2, 3, 4, 6, 8, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall8Y = { 0, 2, 6, 8, 10, 18, 20 };
            Int32[] wall9Y = { 0, 2, 4, 5, 6, 8, 10, 12, 13, 14, 15, 16, 18, 20 };
            Int32[] wall10Y = { 0, 2, 4, 8, 12, 16, 18, 20 };
            Int32[] wall11Y = { 0, 2, 4, 6, 7, 8, 9, 10, 12, 14, 16, 17, 18, 20 };
            Int32[] wall12Y = { 0, 2, 6, 12, 14, 20 };
            Int32[] wall13Y = { 0, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 14, 15, 16, 17, 18, 20 };
            Int32[] wall14Y = { 0, 2, 8, 14, 20 };
            Int32[] wall15Y = { 0, 2, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20 };
            Int32[] wall16Y = { 0, 2, 6, 8, 20 };
            Int32[] wall17Y = { 0, 2, 3, 4, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall18Y = { 0, 6, 10, 20 };
            Int32[] wall19Y = { 0, 2, 3, 4, 5, 6, 10, 14, 15, 16, 17, 18, 19, 20 };
            Int32[] wall20Y = { 0, 6, 10, 20 };
            Int32[] wall21Y = { 0, 1, 2, 3, 4, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20 };
            Int32[] wall22Y = { 0, 4, 6, 16, 20 };
            Int32[] wall23Y = { 0, 2, 4, 6, 8, 10, 11, 12, 13, 14, 16, 18, 20 };
            Int32[] wall24Y = { 0, 2, 6, 8, 10, 14, 16, 18, 20 };
            Int32[] wall25Y = { 0, 2, 3, 4, 5, 6, 8, 10, 12, 13, 14, 16, 18, 20 };
            Int32[] wall26Y = { 0, 2, 8, 10, 16, 18, 20 };
            Int32[] wall27Y = { 0, 2, 4, 5, 6, 7, 8, 10, 12, 14, 15, 16, 17, 18, 20 };
            Int32[] wall28Y = { 0, 2, 6, 10, 12, 14, 20 };
            Int32[] wall29Y = { 0, 2, 3, 4, 5, 6, 8, 9, 10, 12, 14, 15, 16, 17, 18, 20 };
            Int32[] wall30Y = { 0, 4, 8, 12, 18, 20 };
            Int32[] wall31Y = { 0, 1, 2, 4, 6, 7, 8, 10, 12, 13, 14, 15, 16, 17, 18, 20 };
            Int32[] wall32Y = { 0, 4, 8, 10, 14, 18, 20 };
            Int32[] wall33Y = { 0, 2, 3, 4, 5, 6, 8, 10, 11, 12, 13, 14, 16, 18, 20 };
            Int32[] wall34Y = { 0, 2, 8, 16, 20 };
            Int32[] wall35Y = { 0, 2, 4, 8, 10, 11, 12, 13, 14, 15, 16, 20 };
            Int32[] wall36Y = { 0, 4, 8, 16, 20 };
            Int32[] wall37Y = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            Int32 roomlength = 37;

            for (Int32 i = 0; i < roomlength; i++)
            {
                switch (i)
                {
                    case 0:
                        wY = wall1Y;
                        break;
                    case 1:
                        wY = wall2Y;
                        break;
                    case 2:
                        wY = wall3Y;
                        break;
                    case 3:
                        wY = wall4Y;
                        break;
                    case 4:
                        wY = wall5Y;
                        break;
                    case 5:
                        wY = wall6Y;
                        break;
                    case 6:
                        wY = wall7Y;
                        break;
                    case 7:
                        wY = wall8Y;
                        break;
                    case 8:
                        wY = wall9Y;
                        break;
                    case 9:
                        wY = wall10Y;
                        break;
                    case 10:
                        wY = wall11Y;
                        break;
                    case 11:
                        wY = wall12Y;
                        break;
                    case 12:
                        wY = wall13Y;
                        break;
                    case 13:
                        wY = wall14Y;
                        break;
                    case 14:
                        wY = wall15Y;
                        break;
                    case 15:
                        wY = wall16Y;
                        break;
                    case 16:
                        wY = wall17Y;
                        break;
                    case 17:
                        wY = wall18Y;
                        break;
                    case 18:
                        wY = wall19Y;
                        break;
                    case 19:
                        wY = wall20Y;
                        break;
                    case 20:
                        wY = wall21Y;
                        break;
                    case 21:
                        wY = wall22Y;
                        break;
                    case 22:
                        wY = wall23Y;
                        break;
                    case 23:
                        wY = wall24Y;
                        break;
                    case 24:
                        wY = wall25Y;
                        break;
                    case 25:
                        wY = wall26Y;
                        break;
                    case 26:
                        wY = wall27Y;
                        break;
                    case 27:
                        wY = wall28Y;
                        break;
                    case 28:
                        wY = wall29Y;
                        break;
                    case 29:
                        wY = wall30Y;
                        break;
                    case 30:
                        wY = wall31Y;
                        break;
                    case 31:
                        wY = wall32Y;
                        break;
                    case 32:
                        wY = wall33Y;
                        break;
                    case 33:
                        wY = wall34Y;
                        break;
                    case 34:
                        wY = wall35Y;
                        break;
                    case 35:
                        wY = wall36Y;
                        break;
                    case 36:
                        wY = wall37Y;
                        break;
                    default:  //to appease the IDE
                        wY = wall1Y;
                        break;
                }

                for (int k = 0; k < wY.Length; k++) //drawing walls
                {
                    Console.SetCursorPosition(i, wY[k]);
                    Console.Write("|");
                }
            }

            Console.SetCursorPosition(15, 7); //room1 portal
            Console.Write("@");

            if (coinscollected[3] == false) //coin draw
            {
                switch (coinpositions[3])
                {
                    case 0:
                        Console.SetCursorPosition(2, 18);
                        break;
                    case 1:
                        Console.SetCursorPosition(18, 12);
                        break;
                    case 2:
                        Console.SetCursorPosition(34, 6);
                        break;
                    case 3:
                        Console.SetCursorPosition(34, 18);
                        break;
                }
                Console.Write("*");
            }

            return roomlength;
        }//Room6Draw end

        static void Room6GuardianTurns(ref string rguardiandirection, Int32 dguardianX, Int32 dguardianY)
        {
            switch (dguardianX)
            {
                case 3:
                    if (dguardianY == 13)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 5:
                    if (dguardianY == 1)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "U";
                    }
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 13)
                    {
                        rguardiandirection = "U";
                    }
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 19)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 7:
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 9:
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 11:
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 13:
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 19)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 15:
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 19)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 17:
                    if (dguardianY == 13)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 19)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 19:
                    if (dguardianY == 1)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 13)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 17)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 21:
                    if (dguardianY == 1)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "U";
                    }
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 17)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 19)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 23:
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 25:
                    if (dguardianY == 11)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 27:
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 29:
                    if (dguardianY == 1)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 11)
                    {
                        rguardiandirection = "U";
                    }
                    break;
                case 31:
                    if (dguardianY == 1)
                    {
                        rguardiandirection = "R";
                    }
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "U";
                    }
                    if (dguardianY == 5)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "U";
                    }
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 17)
                    {
                        rguardiandirection = "R";
                    }
                    break;
                case 33:
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 7)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 9)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 15)
                    {
                        rguardiandirection = "L";
                    }
                    if (dguardianY == 17)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 19)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                case 35:
                    if (dguardianY == 1)
                    {
                        rguardiandirection = "D";
                    }
                    if (dguardianY == 3)
                    {
                        rguardiandirection = "L";
                    }
                    break;
                default:
                    break;
            }
        } //Room6GuardianTurns end
    }
}