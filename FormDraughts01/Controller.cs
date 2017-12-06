using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FormDraughts01
{
    class Controller
    {
        public List<Coordinate> WhiteList = new List<Coordinate>();
        public List<Coordinate> BlackList = new List<Coordinate>();
        public bool IsTaking = false;
        public bool GameDrawn = false;
        const int BOARD_SCALE = 50;
        GameForm gameForm = new GameForm();

        public Controller (GameForm gameForm)
        {
            this.gameForm = gameForm;
        }

        public List<Coordinate> SetupWhiteList()
        {
            List<Coordinate> tempList = new List<Coordinate>();
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i % 2 != j % 2)
                        tempList.Add(new Coordinate(i, j));
                }
            }
            return tempList;
        }

        public List<Coordinate> SetupBlackList()
        {
            List<Coordinate> tempList = new List<Coordinate>();
            for (int j = 5; j < 8; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i % 2 != j % 2)
                        tempList.Add(new Coordinate(i, j));
                }
            }
            return tempList;
        }

        public bool WhiteWon()
        {
            for (int i = 0; i < BlackList.Count; i++)
            {
                if (BlackList[i].Taken == false)
                    return false;
            }
            return true;
        }

        public bool BlackWon()
        {
            for (int i = 0; i < WhiteList.Count; i++)
            {
                if (WhiteList[i].Taken == false)
                    return false;
            }
            return true;
        }

        //returns true if white has no moves left
        public bool WhiteDraw()
        {
            Board board = new Board();
            for (int i = 0; i < WhiteList.Count; i++)
            {
                if (!WhiteList[i].Taken)
                {
                    foreach (Move move in board.FindValidMoves(WhiteList[i], WhiteList, BlackList, 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //returns true if black has no moves left
        public bool BlackDraw()
        {
            Board board = new Board();
            for (int i = 0; i < BlackList.Count; i++)
            {
                if (!BlackList[i].Taken)
                {
                    foreach (Move move in board.FindValidMoves(BlackList[i], WhiteList, BlackList, 0))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool GameFinished()
        {
            if (WhiteWon() || BlackWon())
                return true;
            else
                return false;
        }

        //setting up marker
        public void PlaceMarker(int pictureBoxNum, int x, int y, int counterNum, List<Coordinate> takenPieces)
        {
            string pictureBoxName = "m" + pictureBoxNum.ToString();

            MarkerBox tempMarkerBox = new MarkerBox(counterNum, takenPieces);

            tempMarkerBox.BackColor = System.Drawing.Color.Transparent;
            tempMarkerBox.BackgroundImage = global::FormDraughts01.Properties.Resources.Marker;
            tempMarkerBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            tempMarkerBox.Location = new System.Drawing.Point(BOARD_SCALE * x, BOARD_SCALE * y);
            tempMarkerBox.Name = pictureBoxName;
            tempMarkerBox.Size = new System.Drawing.Size(BOARD_SCALE, BOARD_SCALE);
            tempMarkerBox.TabIndex = 24;
            tempMarkerBox.TabStop = false;
            tempMarkerBox.Click += new System.EventHandler(gameForm.markerClicked);
            tempMarkerBox.Tag = counterNum;

            gameForm.boardPanel.Controls.Add(tempMarkerBox); //adds the piece onto the form
        }

        public void PlaceCounter (GameForm gameForm, Coordinate coordinate, string colour, int index)
        {
            this.gameForm = gameForm;
            string indexStr = index.ToString();
            if (indexStr.Length == 1)
                indexStr = "0" + indexStr;
            string name = colour.ToLower() + indexStr;

            //sets up the counter to be displayed on the board
            PictureBox tempCounter = new PictureBox();
            tempCounter.BackColor = System.Drawing.Color.Transparent;
            switch (colour)
            {
                case "b":
                    tempCounter.BackgroundImage = global::FormDraughts01.Properties.Resources.BlackPiece;
                    break;

                case "w":
                    tempCounter.BackgroundImage = global::FormDraughts01.Properties.Resources.WhitePiece;
                    break;

                case "B":
                    tempCounter.BackgroundImage = global::FormDraughts01.Properties.Resources.BlackPromotedPiece;
                    break;

                case "W":
                    tempCounter.BackgroundImage = global::FormDraughts01.Properties.Resources.WhitePromotedPiece;
                    break;
            }
            
            tempCounter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            tempCounter.Location = new System.Drawing.Point(coordinate.X * BOARD_SCALE, coordinate.Y * BOARD_SCALE);
            tempCounter.Name = name;
            tempCounter.Size = new System.Drawing.Size(BOARD_SCALE, BOARD_SCALE);
            tempCounter.TabIndex = 1;
            tempCounter.TabStop = false;
            if (colour == "b" || colour == "B")
                tempCounter.Click += new System.EventHandler(gameForm.blackPieceClicked);
            tempCounter.Tag = index;

            //Adds the counter onto the board
            gameForm.boardPanel.Controls.Add(tempCounter);
        }

        public void DrawCounters(List<Coordinate> whiteList, List<Coordinate> blackList)
        {
            //checks if promoted, then draws using PlaceCounter
            for (int i = 0; i < blackList.Count; i++)
            {
                if (blackList[i].Promoted == false)
                    PlaceCounter(this.gameForm, blackList[i], "b", i);
                else if (blackList[i].Promoted == true)
                    PlaceCounter(this.gameForm, blackList[i], "B", i);
            }
            for (int i = 0; i < whiteList.Count; i++)
            {
                if (whiteList[i].Promoted == false)
                    PlaceCounter(this.gameForm, whiteList[i], "w", i);
                else if (whiteList[i].Promoted == true)
                    PlaceCounter(this.gameForm, blackList[i], "W", i);
            }
        }
    }
}
