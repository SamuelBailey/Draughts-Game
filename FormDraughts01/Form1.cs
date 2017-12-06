using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormDraughts01;

namespace FormDraughts01
{
    public partial class GameForm : Form
    {
        Controller Ctrl;
        const int BOARD_SCALE = 50;
        const int PREDICT_NUM = 5;
        const int BOARD_LENGTH = 8;
        Random rnd = new Random();

        public GameForm()
        {
            InitializeComponent();
        }

        void RemoveMarkers()
        {
            for (int i = 0; i < boardPanel.Controls.Count;)
            {
                if (boardPanel.Controls[i].Name[0] == 'm')
                    boardPanel.Controls.RemoveAt(i);
                else
                    i++;
            }
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

            Ctrl = new Controller(this);
        }

        bool CanTake(List<Coordinate> pieceList)
        {
            Board board = new Board();
            foreach (Coordinate counter in pieceList)
            {
                foreach (Move move in board.FindValidMoves(counter, Ctrl.WhiteList, Ctrl.BlackList, 0))
                {
                    if (move.RemoveList.Count > 0)
                        return true;
                }
            }
            return false;
        }

        public void blackPieceClicked(object sender, EventArgs e)
        {
            if ((!Ctrl.GameFinished() && !Ctrl.BlackDraw()) && !Ctrl.IsTaking && !Ctrl.GameDrawn)
            {
                //pieceNum is set to the number of the piece
                PictureBox clickedPiece = sender as PictureBox;
                Board board = new Board();
                List<Move> possibleMoves = new List<Move>();
                int pieceNum = (int)clickedPiece.Tag;

                RemoveMarkers();
                int markerNum = 0;
                if (CanTake(Ctrl.BlackList))
                    possibleMoves = board.FindSingleMove(Ctrl.BlackList[pieceNum], Ctrl.BlackList, Ctrl.WhiteList, Ctrl.WhiteList, Ctrl.BlackList, -1);
                else
                    possibleMoves = board.NoTakeMove(Ctrl.BlackList[pieceNum], Ctrl.BlackList, Ctrl.WhiteList, Ctrl.WhiteList, Ctrl.BlackList, -1);

                foreach (Move move in possibleMoves)
                {
                    Ctrl.PlaceMarker(markerNum, move.ToPosition.X, move.ToPosition.Y, pieceNum, move.RemoveList);
                    markerNum++;
                }
            }
        }

        public void markerClicked(object sender, EventArgs e)
        {
            MarkerBox clickedMarker = sender as MarkerBox;
            Ctrl.IsTaking = false;

            Coordinate newPosition = new Coordinate(0,0);
            int pieceReference = 0;

            for (int i = 0; i < boardPanel.Controls.Count; i++)
            {
                if ((int)boardPanel.Controls[i].Tag == (int)clickedMarker.Tag &&
                    boardPanel.Controls[i].Name[0] == 'b')
                {
                    boardPanel.Controls[i].Location = clickedMarker.Location;
                    Ctrl.BlackList[i].X = (clickedMarker.Location.X / BOARD_SCALE);
                    Ctrl.BlackList[i].Y = (clickedMarker.Location.Y / BOARD_SCALE);
                    newPosition = Ctrl.BlackList[i];
                    pieceReference = i;
                }
            }

            //removing pieces that were taken in this move
            bool pieceTaken = false;
            foreach (Coordinate coordinate in clickedMarker.TakenPieces)
            {
                pieceTaken = true;
                for (int i = 0; i < Ctrl.WhiteList.Count; i++)
                {
                    if (/*Ctrl.WhiteList[i].Taken == false &&*/
                        Ctrl.WhiteList[i].X == coordinate.X &&
                        Ctrl.WhiteList[i].Y == coordinate.Y)
                    {
                        Ctrl.WhiteList[i].Take();

                        //removes the counter from the board.
                        for (int j = 0; j < boardPanel.Controls.Count; j++)
                        {
                            if ((boardPanel.Controls[j].Name[0] == 'w' ||
                                boardPanel.Controls[j].Name[0] == 'W') &&
                                (int)boardPanel.Controls[j].Tag == i)
                            {
                                boardPanel.Controls[j].Visible = false;
                            }
                        }

                        //boardPanel.Controls[i].Visible = false;
                    }
                }
            }

            //Finds further moves that require a take if possible in the current state of the board.
            Board board = new Board();
            if (pieceTaken && board.FindSingleMove(newPosition, Ctrl.BlackList, Ctrl.WhiteList, Ctrl.WhiteList, Ctrl.BlackList, -1).Count >= 1)
            {                
                RemoveMarkers();
                Ctrl.IsTaking = true;
                List<Move> possibleMoves = board.FindSingleMove(newPosition, Ctrl.BlackList, Ctrl.WhiteList, Ctrl.WhiteList, Ctrl.BlackList, -1);
                int markerNum = 0;
                foreach (Move move in possibleMoves)
                {
                    Ctrl.PlaceMarker(markerNum, move.ToPosition.X, move.ToPosition.Y, pieceReference, move.RemoveList);
                    markerNum++;
                }
            }
            else
            {
                //Ctrl.BlackList.RemoveAt((int)clickedMarker.Tag);
                RemoveMarkers();

                if (Ctrl.BlackList[pieceReference].Y == 0)
                {
                    Ctrl.BlackList[pieceReference].Promote();
                    boardPanel.Controls[pieceReference].BackgroundImage = global::FormDraughts01.Properties.Resources.BlackPromotedPiece;
                }

                if (Ctrl.BlackWon())
                    System.Windows.Forms.MessageBox.Show("You Won!!!");
                else if (Ctrl.WhiteDraw())
                {
                    System.Windows.Forms.MessageBox.Show("Draw.");
                    Ctrl.GameDrawn = true;
                }

                //Find move for white
                if (!Ctrl.GameFinished() && !Ctrl.WhiteDraw() &&!Ctrl.GameDrawn)
                {
                    List<Move> possibleMoves = new List<Move>();
                    if (CanTake(Ctrl.WhiteList))
                    {
                        for (int i = 0; i < Ctrl.WhiteList.Count; i++)
                        {
                            if (Ctrl.WhiteList[i].Taken == false)
                            {
                                foreach (Move move in board.FindValidMoves(Ctrl.WhiteList[i], Ctrl.WhiteList, Ctrl.BlackList, PREDICT_NUM))
                                {
                                    if (move.RemoveList.Count > 0)
                                    {
                                        move.SetIndex(i);
                                        possibleMoves.Add(new Move(move));
                                    }
                                }
                                //if (board.FindValidMoves(Ctrl.WhiteList[i], Ctrl.WhiteList, Ctrl.BlackList, PREDICT_NUM).Count == 0)
                                //throw new Exception("not moving piece");
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Ctrl.WhiteList.Count; i++)
                        {
                            if (Ctrl.WhiteList[i].Taken == false)
                            {
                                foreach (Move move in board.FindValidMoves(Ctrl.WhiteList[i], Ctrl.WhiteList, Ctrl.BlackList, PREDICT_NUM))
                                {
                                    move.SetIndex(i);
                                    possibleMoves.Add(new Move(move));
                                }
                            }
                        }
                    }

                    int score = -100000;
                    List<Move> bestMoves = new List<Move>();
                    foreach (Move move in possibleMoves)
                    {
                        if (move.TotalScore > score)
                        {
                            bestMoves.Clear();
                            score = move.TotalScore;
                            bestMoves.Add(new Move(move));
                        }
                        else if (move.TotalScore == score)
                        {
                            bestMoves.Add(new Move(move));
                        }
                    }

                    int randomMoveNum = rnd.Next(bestMoves.Count);
                    Move bestMove = bestMoves[randomMoveNum];

                    //Move white piece
                    for (int i = 0; i < boardPanel.Controls.Count; i++)
                    {
                        if ((int)boardPanel.Controls[i].Tag == bestMove.PieceIndex &&
                            (boardPanel.Controls[i].Name[0] == 'w' ||
                            boardPanel.Controls[i].Name[0] == 'W'))
                        {
                            boardPanel.Controls[i].Location = new Point(bestMove.ToPosition.X * BOARD_SCALE, bestMove.ToPosition.Y * BOARD_SCALE);

                            //Ctrl.WhiteList[pieceIndex] = new Coordinate(bestMove.ToPosition);
                            Ctrl.WhiteList[bestMove.PieceIndex].X = bestMove.ToPosition.X;
                            Ctrl.WhiteList[bestMove.PieceIndex].Y = bestMove.ToPosition.Y;
                            if (Ctrl.WhiteList[bestMove.PieceIndex].Y == BOARD_LENGTH - 1)
                            {
                                Ctrl.WhiteList[bestMove.PieceIndex].Promote();
                                boardPanel.Controls[i].BackgroundImage = global::FormDraughts01.Properties.Resources.WhitePromotedPiece;
                            }
                        }
                    }

                    //Take black pieces
                    foreach (Coordinate coordinate in bestMove.RemoveList)
                    {
                        for (int i = 0; i < Ctrl.BlackList.Count; i++)
                        {
                            if (/*Ctrl.BlackList[i].Taken == false &&*/
                                Ctrl.BlackList[i].X == coordinate.X &&
                                Ctrl.BlackList[i].Y == coordinate.Y)
                            {
                                Ctrl.BlackList[i].Take();

                                //removes the counter from the board.
                                for (int j = 0; j < boardPanel.Controls.Count; j++)
                                {
                                    if ((boardPanel.Controls[j].Name[0] == 'b' ||
                                        boardPanel.Controls[j].Name[0] == 'B') &&
                                        (int)boardPanel.Controls[j].Tag == i)
                                    {
                                        boardPanel.Controls[j].Visible = false;
                                    }
                                }
                                //boardPanel.Controls[i].Visible = false;
                            }
                        }
                    }
                    if (Ctrl.WhiteWon())
                        System.Windows.Forms.MessageBox.Show("You lost.");
                    else if (Ctrl.BlackDraw())
                    {
                        System.Windows.Forms.MessageBox.Show("Draw.");
                        Ctrl.GameDrawn = true;
                    }
                }
            }            
        }

        private void newGameBtn_Click(object sender, EventArgs e)
        {
            boardPanel.Controls.Clear();
            Ctrl.IsTaking = false;
            Ctrl.GameDrawn = false;
            Ctrl.WhiteList = Ctrl.SetupWhiteList();
            Ctrl.BlackList = Ctrl.SetupBlackList();
            Ctrl.DrawCounters(Ctrl.WhiteList, Ctrl.BlackList);
        }
    }


    partial class MarkerBox : PictureBox
    {
        public int CounterReference = new int();
        public List<Coordinate> TakenPieces = new List<Coordinate>();

        public MarkerBox(int counterNum, List<Coordinate> takenPieces)
        {
            this.CounterReference = counterNum;
            foreach (Coordinate coordinate in takenPieces)
            {
                TakenPieces.Add(new Coordinate(coordinate));
            }
        }
    }
}
