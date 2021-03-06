﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoItX3Lib;
using System.Windows.Forms;

namespace Bot5PokeMMO.Framework
{
    public class BotLogic
    {
        // Console variables
        public int type = 1;
        string message = "Test message";

        // Keeping track of encounters and attempts
        public double encounters = 0;
        public int attempt = 0;

        // Global variables
        public string walkPattern;
        public bool catchUnspecifiedPokemon = true;

        #region random val
        // sleep values for methods Run, Attack
        public int randomSleepWalkB = 100;
        public int randomSleepWalkE = 300;

        // Not used yet
        public int randomSleepAtkB = 100;
        public int randomSleepAtkE = 200;

        public int randomSleepRunB = 300;
        public int randomSleepRunE = 400;
        #endregion

        // Hotkeys
        string _hotkeyUp = App.hotkeyUp;
        string _hotkeyDown = App.hotkeyDown;
        string _hotkeyLeft = App.hotkeyLeft;
        string _hotkeyRight = App.hotkeyRight;

        // mouse clicks
        public int runMouseX;
        public int runMouseY;

        public int catchPokemonX1;
        public int catchPokemonY1;
        public int catchPokemonX2;
        public int catchPokemonY2;
        public int catchPokemonX3;
        public int catchPokemonY3;
        public int catchPokemonX4;
        public int catchPokemonY4;

        public int atkMove1X;
        public int atkMove1Y;


        // Creating an instance of AutoItX3
        AutoItX3 autoit = new AutoItX3();

        public static int totalPokemon; // should be set by App.cs

        // VarHandler variables
        List<Int32> colpix = new List<Int32>();
        List<int> cordx = new List<int>();
        List<int> cordy = new List<int>();
        List<string> state = new List<string>();
        List<string> pokemon = new List<string>();

        // In battle coordinates
        public int battleX;
        public int battleY;
        public Int32 battleCol;

        // In horde coordinates
        public int hordeX;
        public int hordeY;
        public Int32 hordeCol;

        #region initialize
        public void AssignVars(Int32 col, int x, int y, string _pokemon, string _state = "run")
        {
            if(totalPokemon >= 1)
            {
                // Storing the variables so they can be used later.
                colpix.Add(col);
                cordx.Add(x);
                cordy.Add(y);
                state.Add(_state);
                pokemon.Add(_pokemon);

            }
            else
            {
                MessageBox.Show("Error: totalPokemon should be set and the method AssignVars should not be used more than what totalPokemon is equal to!");
            }

        }

        public void SetTotalPokemon(int x)
        {
            totalPokemon = x;

            // setting array size to totalPokemon
            Int32[] colPix = new Int32[totalPokemon];
            int[] cordX = new int[totalPokemon];
            int[] cordY = new int[totalPokemon];

            //MessageBox.Show("Total pokemon set to: " + totalPokemon);

        }

        public void GetDefVal(int x1, int y1, Int32 col1, int x2, int y2, Int32 col2)
        {
            // this method should set the default values from a config file in App.cs
            // for now we are just setting it manually via code in App.cs
            battleX = x1;
            battleY = y1;
            battleCol = col1;

            hordeX = x2;
            hordeY = y2;
            hordeCol = col2;



        }
        #endregion

        public void DoRun()
        {
            if (autoit.PixelGetColor(battleX, battleY) == battleCol && autoit.PixelGetColor(hordeX, hordeY) != hordeCol)
            {
                encounters++;
                for (int i = 0; i < totalPokemon; i++)
                {
                    if (autoit.PixelGetColor(cordx[i], cordy[i]) == colpix[i])
                    {
                        if (state[i] == "run")
                        {
                            runPokemon();
                            message = "Ran from = " + pokemon[i] + " col: " + colpix[i]; // message
                            type = 1; // used for message 
                            break;
                        }
                        else if (state[i] == "catch")
                        {
                            catchPokemon();
                            message = "Catching  = " + pokemon[i] + " col: " + colpix[i] + " - attempt #" + attempt;
                            type = 1;
                            break;
                        }
                        else if (state[i] == "attack")
                        {
                            attackPokemon(1);
                            message = "Attacked (atk1) = " + pokemon[i] + " col: " + colpix[i];
                            type = 1;
                            // and putting encounters back to real encounter
                            encounters--;
                            break; // breaking here sets i back to 0 so we wont catch it 
                        }
                        else
                        {
                            message = "Error: undefined state";
                            type = 1;
                            // and putting encounters back to real encounter
                            encounters--;
                            break; // breaking here sets i back to 0 so we wont catch it 
                        }
                    }
                    else if (i == totalPokemon - 1 && catchUnspecifiedPokemon == true) //when for loop is finished
                    {
                        // Catch function - presumrably shiny
                        attempt++;
                        message = "Catching = unspecifed pokemon - attempt #" + attempt;
                        type = 1;
                        catchPokemon();

                        break;
                    }
                   

                }
            }
            else if (autoit.PixelGetColor(hordeX, hordeY) == hordeCol)
            {
                message = "Error: Horde Battle - there are no defined methods given for horde battle yet!";
                type = 1;
                // We have a horde battle - do something
            }
            else
            {
                WalkingPattern(walkPattern);
            }

        }

        #region ALL methods
        #region inBattle methods
        public void catchPokemon(string pokemon = "not defined") // use different attack - 1 more argument
        {
            // setting string Pokemon equals pokemonName
            string pokemonName;
            pokemonName = pokemon;

            // Sleeping before battle has begun
            autoit.Sleep(2000);

            // A MouseClick is now simulated to kill the pokemon
            autoit.MouseClick("LEFT", catchPokemonX1, catchPokemonY1);

            // navigate bag 1/2
            autoit.Sleep(2500);

            autoit.MouseClick("LEFT", catchPokemonX2, catchPokemonY2);

            // navigate bag 2/2
            autoit.Sleep(1500);

            autoit.MouseClick("LEFT", catchPokemonX3, catchPokemonY3);

            // Use pokeball
            autoit.Sleep(1500);

            autoit.MouseClick("LEFT", catchPokemonX4, catchPokemonY4);

            // Sleeping here for a long time so we are sure that we are through catching sequence.
            autoit.Sleep(10000);

        }

        public void runPokemon(string pokemon = "not defined")
        {
            // setting string Pokemon equals pokemonName
            string pokemonName;
            pokemonName = pokemon;

            // Making the bot sleep at a random interval after randomSleep has been set
            autoit.Sleep(RandomSleep(randomSleepRunB, randomSleepRunE));

            // A MouseClick is now simulated to kill the pokemon
            autoit.MouseClick("LEFT", runMouseX, runMouseY);
        }

        public void attackPokemon(int move = 1)
        {
            // Not implemented yet
            switch (move)
            {
                case 1:
                    autoit.MouseClick("LEFT", atkMove1X, atkMove1Y);
                    autoit.Sleep(30);
                    autoit.MouseClick("LEFT", atkMove1X, atkMove1Y);
                    break;

                case 2:

                    break;

                case 3:

                    break;

                case 4:

                    break;
            }
        }

        #endregion

        #region Walking methods: walk, WalkingPattern & combineString
        public void walk(string direction)
        {
            // setting string direction equals dir
            string dir = direction;

            switch (dir)
            {
                case "left":
                    autoit.Send(combineString(_hotkeyLeft, "d"));

                    // Making the bot sleep at a random interval after randomSleep has been set
                    autoit.Sleep(RandomSleep(randomSleepWalkB, randomSleepWalkE));

                    autoit.Send(combineString(_hotkeyLeft, "u"));

                    //autoit.ToolTip("Walking: " + dir, 571, 222);

                    break;

                case "right":
                    autoit.Send(combineString(_hotkeyRight, "d"));

                    // Making the bot sleep at a random interval after randomSleep has been set
                    autoit.Sleep(RandomSleep(randomSleepWalkB, randomSleepWalkE));

                    autoit.Send(combineString(_hotkeyRight, "u"));

                    //autoit.ToolTip("Walking: " + dir, 571, 222);
                    break;

                case "up":
                    autoit.Send(combineString(_hotkeyUp, "d"));

                    // Making the bot sleep at a random interval after randomSleep has been set
                    autoit.Sleep(RandomSleep(randomSleepWalkB, randomSleepWalkE));

                    autoit.Send(combineString(_hotkeyUp, "u"));

                    //autoit.ToolTip("Walking: " + dir, 571, 222);
                    break;

                case "down":
                    autoit.Send(combineString(_hotkeyDown, "d"));

                    // Making the bot sleep at a random interval after randomSleep has been set
                    autoit.Sleep(RandomSleep(randomSleepWalkB, randomSleepWalkE));

                    autoit.Send(combineString(_hotkeyDown, "u"));

                    // autoit.ToolTip("Walking: " + dir, 571, 222);
                    break;

                default:


                    break;

            }

        }

        public void WalkingPattern(string pattern)
        {
            string _walkPattern = pattern;
            
            if (_walkPattern == "leftright")
            {
                walk("left");
                walk("right");
            }
            else if (_walkPattern == "updown")
            {
                walk("up");
                walk("down");
            }
            else if (_walkPattern == "square")
            {
                walk("up");
                walk("right");
                walk("down");
                walk("left");
            }
            else if (_walkPattern == "perfectsquare")
            {
                randomSleepWalkB = 300;
                randomSleepWalkE = 300;
                walk("up");
                walk("right");
                walk("down");
                walk("left");
            }
            else if (_walkPattern == "")
            {
                MessageBox.Show("Error: A walking pattern has not been defined");
            }
            else
            {
                MessageBox.Show("Error: unknown walkpattern");
            }
        }

        public string combineString(string x, string state)
        {
            // d = down
            // u = up

            string combineKey = x;

            string result;

            if (state == "u")
            {
                result = "{" + combineKey + " UP}";
            }
            else if (state == "d")
            {
                result = "{" + combineKey + " DOWN}";
            }
            else
            {
                result = "No state was signed";
            }

            return result;

        }

        public int RandomSleep(int begin, int end)
        {
            int result;

            int _begin = begin;
            int _end = end;

            // Creating a new instance of Random
            Random rnd = new Random();

            // randomSleep is set to a random number
            result = rnd.Next(_begin, _end);

            return result;

        }

        public void setHotkeys()
        {
            _hotkeyUp = App.hotkeyUp;
            _hotkeyDown = App.hotkeyDown;
            _hotkeyLeft = App.hotkeyLeft;
            _hotkeyRight = App.hotkeyRight;


        }

        public string WriteToConsole()
        {
            int msgType = type;
            string msgToWrite = message;

            if(type == 1)
            {
                msgToWrite = DateTime.Now.ToString("HH:mm:ss") + " - " + msgToWrite + "\n";
                type = 0;

            }
            else
            {

            }

            return msgToWrite;

        }
        #endregion




    }
}
#endregion ALL methods