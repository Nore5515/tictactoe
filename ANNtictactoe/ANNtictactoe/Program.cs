using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

////Tic Tac Toe!
//- = None
//O = symbol for O's
//X = symbol for X's
//those should be pretty obvious...



namespace ANNtictactoe
{
    class Program
    {

        public string reply;
        public bool playing = true;
        public bool inGame = false;
        public bool isTurn = false;
        public bool err = false;
        public string icon; //what symbol u are, aka X or O
        public ANN enemy;

        static void Main(string[] args)
        {
            Program p = new Program();
            TicTacToe t;
            int x = -1;
            int y = -1;
            int turn;
            p.enemy = new ANN();
            string s = "";
            bool temp = false;
            bool fullBoard = true;

            Console.WriteLine("Welcome to Tic Tac Toe! Your opponent will be an ANN, which is an Artifical Neural Network!");
            Console.WriteLine("It will learn more and more as you play against it.");
            //Console.WriteLine("How many generations old do you want it to be?") then prompt here
            Console.Read();
            Console.WriteLine("Ready? Let's go!");

            while (p.playing)
            {
                t = new TicTacToe();
                turn = 0;
                Console.WriteLine("First or Second? (1 or 2)");
                p.reply = Console.ReadLine();
                if (p.reply.Equals("1"))
                {
                    p.isTurn = true;
                    p.inGame = true;
                    p.icon = "X";
                    p.enemy.icon = "O";
                }
                else if (p.reply.Equals("2"))
                {
                    p.isTurn = false;
                    p.inGame = true;
                    p.icon = "O";
                    p.enemy.icon = "X";
                }
                else
                {
                    Console.WriteLine("Sorry, I didn't understand that.");
                }

                turn = 1;
                while (p.inGame)
                {
                    if (p.isTurn)
                    {
                        temp = true;
                        while (temp)
                        {
                            Console.WriteLine(t.ToString());
                            //if even a SINGLE tile is empty, you can play
                            fullBoard = false;
                            for (int x1 = 0; x1 < 3; x1++)
                            {
                                for (int y1 = 0; y1 < 3; y1++)
                                {
                                    if (t.tilesEmpty[x1, y1]) { fullBoard = true; }
                                }
                            }
                            if (!fullBoard)
                            {
                                Console.WriteLine("Board full! Breaking!");
                                p.inGame = false;
                                p.isTurn = false;
                                break;
                            }

                            Console.WriteLine("Where would you like to go? (ex 1,3 or 2,2)");
                            p.reply = Console.ReadLine();
                            try
                            {
                                x = int.Parse(p.reply.Substring(0, 1));
                                y = int.Parse(p.reply.Substring(2, 1));
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Console.WriteLine("Out of bounds!");
                                p.err = true;
                            }

                            if (!p.err)
                            {
                                if (t.setTile(x - 1, y - 1, p.icon))
                                {
                                    temp = false;
                                }
                                else
                                {
                                    Console.WriteLine("Sorry, that didn't work!");
                                }
                            }
                        }
                        p.err = false;
                        p.isTurn = false;
                    }
                    //ANN plays their turn
                    else if (!p.isTurn)
                    {
                        Console.WriteLine("Turn: {0}", turn);   //test
                        Console.WriteLine("ANN is thinking...");
                        s = p.enemy.getBestLocation(turn-1, t.tilesEmpty);
                        if (s.Equals("-1,-1"))
                        {
                            Console.WriteLine("Breaking!"); //add proper leave here
                            break;
                        }
                        x = int.Parse(s.Substring(0, 1));
                        y = int.Parse(s.Substring(2, 1));
                        Console.WriteLine("ANN has decided!");
                        t.setTile(x - 1, y - 1, p.enemy.icon);
                        turn++;
                        p.isTurn = true;
                    }
                }
            }



        }
    }

    
    class TicTacToe
    {
        Tile[,] tiles = new Tile[3,3];
        public bool[,] tilesEmpty = new bool[3,3];

        public TicTacToe()
        {
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 3; y++)
                {
                    tiles[x, y] = new Tile("-");
                    tilesEmpty[x, y] = true;
                }
            }
        }

        public void update()
        {
            int count = 0;
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (!tiles[x, y].value.Equals("-"))
                    {
                        tilesEmpty[x,y] = false;
                    }
                    count++;
                }
            }
        }

        //have this return true if successful, false if not
        public bool setTile(int x, int y, string v)
        {
            if (tilesEmpty[x, y])
            {
                tiles[x, y].value = v;
                tilesEmpty[x, y] = false;
                return true;
            }
            return false;
        }

        public string getTile(int x, int y)
        {
            return tiles[x, y].value;
        }

        public override string ToString()
        {
            string s = "";
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    s += tiles[x, y].value;
                }
                s += "\n";
            }
            return s;
        }

    }


    class ANN
    {
        Tile[,] turns = new Tile[9,5];
        Random rand = new Random();
        double mutation;   //to clarify, 0 means 0 mutation while 1 means very random!
        public string icon;     //don't sweat this too much

        //brand new ANN, with values between 0 and 1 for each tile
        public ANN()
        {
            for (int x = 0; x < 5; x++){
                for (int y = 0; y < 9; y++){
                    turns[y, x] = new Tile(rand.NextDouble());
                }
            }
        }

        //child ANN, with values of the parent, but modified by the mutation
        public ANN(ANN annie)
        {
            for (int x = 0; x < 5; x++){
                for (int y = 0; y < 9; y++){
                    turns[y, x].fitness = annie.turns[y,x].fitness + mutation * rand.NextDouble();  //broken!!!!
                }
            }
        }

        public string getBestLocation(int turn,bool[,] a)
        {
            string s = "";
            int pos = 0;
            bool[] isEmpty = new bool[9];
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    isEmpty[pos] = a[x, y];
                    pos++;
                }
            }

            Tile t = new Tile(-100.0);
            pos = -1;
            for (int x = 0; x < 9; x++)
            {
                //if the tile in the database has a better fitness than the current tile AND isn't empty, take it
                if (turns[x,turn].fitness > t.fitness && isEmpty[x])      //should break if it takes it's 6th turn, cus it shouldn't be able to!!
                {
                    t = turns[x, turn];
                    pos = x;
                }
            }
            if (t.fitness == -100.0)
            {
                Console.WriteLine("ANN cannot play.");
                s = "-1,-1";
            }
            else
            {
                int row = getRow(pos + 1);
                int col = getCol(pos + 1);

                //gotta return the position in a string like 33 or 13 something like that 
                s += $"{row},{col}";
            }
            return s;
        }

        public int getRow(int num)
        {
            int temp = num;
            int graphLength = (int)Math.Sqrt(turns.GetLength(0));
            int ito = 0;
            while (temp > 0)
            {
                temp -= graphLength;
                ito++;
            }
            return ito;
        }

        public int getCol(int num)
        {
            int temp = num;
            int graphLength = (int)Math.Sqrt(turns.GetLength(0));
            int ito = graphLength;
            while (temp % graphLength != 0)
            {
                temp++;
                ito--;
            }
            return ito;
        }


    }

    class Tile
    {
        //value is the literal value of the tile, while fitness is for ANN's to evaluate the Tile's worth vs others
        public string value;
        public double fitness;
        
        public Tile()
        {
            value =  "-";
            fitness = 0.0f;
        }

        public Tile(string v)
        {
            value = v;
        }

        public Tile(double f)
        {
            fitness = f;
        }

        public Tile(string v, double f)
        {
            value = v;
            fitness = f;
        }


    }
}
