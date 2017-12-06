using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormDraughts01
{
    class Move
    {
        public Coordinate FromPosition;
        public Coordinate ToPosition;
        public List<Coordinate> RemoveList = new List<Coordinate>();
        List<Coordinate> WhiteList = new List<Coordinate>();
        List<Coordinate> BlackList = new List<Coordinate>();
        public int Score = new int();
        public int TotalScore = new int();
        public int PredictNum = new int();
        public int PieceIndex = new int();

        public Move(Move move)
        {
            this.FromPosition = move.FromPosition;
            this.ToPosition = move.ToPosition;
            this.RemoveList = AssignValues(move.RemoveList);
            this.WhiteList = AssignValues(move.WhiteList);
            this.BlackList = AssignValues(move.BlackList);
            this.Score = move.Score;
            this.TotalScore = move.TotalScore;
            this.PredictNum = move.PredictNum;
            this.PieceIndex = move.PieceIndex;
        }

        public Move(Coordinate fromPosition,
            Coordinate toPosition,
            List<Coordinate> removeList,
            List<Coordinate> whiteList,
            List<Coordinate> blackList,
            int score,
            int predictNum)
        {
            this.FromPosition = new Coordinate(fromPosition);
            this.ToPosition = new Coordinate(toPosition);
            this.RemoveList = removeList;
            this.WhiteList = AssignValues(whiteList);
            this.BlackList = AssignValues(blackList);
            this.Score = score;
            this.PredictNum = predictNum;

            if (this.ToPosition.X % 2 == this.ToPosition.Y % 2)
                throw new Exception("not on board");

            //start of recursive call
            if (this.PredictNum > 0)
            {
                //moving piece to new Locaiton and removing taken pieces from the passive list
                if (CheckInList(this.FromPosition, this.WhiteList))
                {
                    this.WhiteList = MovePiece(this.FromPosition, this.ToPosition, this.WhiteList);
                    foreach (Coordinate removePosition in this.RemoveList)
                    {
                        this.BlackList = RemoveFromList(removePosition, this.BlackList);
                    }
                }
                else if (CheckInList(this.FromPosition, this.BlackList))
                {
                    this.BlackList = MovePiece(this.FromPosition, this.ToPosition, this.BlackList);
                    foreach (Coordinate removePosition in this.RemoveList)
                    {
                        //When moving black pieces, the remove list contains pieces that are not in whiteList
                        this.WhiteList = RemoveFromList(removePosition, this.WhiteList);
                    }
                }
                else
                    throw new Exception("The FromPosition is not in either of the piece Lists");


                predictNum--;
                Board board = new Board();
                List<Move> nextMoves = new List<Move>();
                if (CheckInList(ToPosition, this.WhiteList)) //moved a piece from whitelist
                {
                    foreach (Coordinate coordinate in this.BlackList)
                    {
                        List<Move> tempNextMoves = board.FindValidMoves(coordinate, this.WhiteList, this.BlackList, predictNum);
                        foreach (Move move in tempNextMoves)
                        {
                            nextMoves.Add(move);
                        }
                    }
                }
                else //moved a piece from blacklist
                {
                    foreach (Coordinate coordinate in this.WhiteList)
                    {
                        List<Move> tempNextMoves = board.FindValidMoves(coordinate, this.WhiteList, this.BlackList, predictNum);
                        foreach (Move move in tempNextMoves)
                        {
                            nextMoves.Add(move);
                        }
                    }
                }

                if (CheckInList(ToPosition, this.WhiteList))
                {
                    this.TotalScore = score + FindLowestScore(nextMoves);
                }
                else
                {
                    this.TotalScore = score + FindLargestScore(nextMoves);
                }
            }
            else
            {
                this.TotalScore = this.Score;
            }
        }

        public void SetIndex (int index)
        {
            this.PieceIndex = index;
        }

        int FindLowestScore(List<Move> moveList)
        {
            int lowestScore = 0;
            foreach (Move move in moveList)
            {
                if (move.TotalScore < lowestScore)
                    lowestScore = move.TotalScore;
            }
            return lowestScore;
        }

        int FindLargestScore(List<Move> moveList)
        {
            int largestScore = 0;
            foreach (Move move in moveList)
            {
                if (move.TotalScore > largestScore)
                    largestScore = move.TotalScore;
            }
            return largestScore;
        }

        List<Coordinate> AssignValues(List<Coordinate> sourceList)
        {
            List<Coordinate> tempList = new List<Coordinate>();
            foreach (var dataPoint in sourceList)
                tempList.Add(dataPoint);
            return tempList;
        }

        bool CheckInList(Coordinate coordinate, List<Coordinate> coordinateList)
        {
            foreach (var position in coordinateList)
            {
                if (position.Taken == false &&          //<== added so that taken pieces no longer have an effect on the movement of pieces
                    coordinate.Taken == false &&
                    position.X == coordinate.X && position.Y == coordinate.Y)
                    return true;
            }
            return false;
        }

        List<Coordinate> MovePiece(Coordinate fromPosition, Coordinate toPosition, List<Coordinate> pieceList)
        {
            List<Coordinate> tempList = new List<Coordinate>();
            foreach (Coordinate coordinate in pieceList)
            {
                if ((coordinate.X != fromPosition.X ||
                    coordinate.Y != fromPosition.Y))
                {
                    tempList.Add(new Coordinate(coordinate));
                }
                else if (coordinate.Taken == false)
                {
                    tempList.Add(new Coordinate(toPosition));
                }
            }
            return tempList;
        }

        public List<Coordinate> RemoveFromList(Coordinate coordinate, List<Coordinate> coordinateList)
        {
            List<Coordinate> tempCoordinateList = coordinateList;
            bool removed = false;
            for (int i = 0; i < coordinateList.Count(); i++)
            {
                if (coordinate.X == tempCoordinateList[i].X && coordinate.Y == tempCoordinateList[i].Y)
                {
                    tempCoordinateList.RemoveAt(i);
                    removed = true;
                }
            }
            if (removed == false)
                throw new Exception("Attempted to remove item not in list");
            return tempCoordinateList;
        }
    }
}
