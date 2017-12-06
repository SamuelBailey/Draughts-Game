using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormDraughts01
{
    class Board
    {
        public Board() { }
        const int BOARD_LENGTH = 8;
        const int MOVE_SCORE = 1;
        const int TAKE_SCORE = 10;

        public bool OnBoard(Coordinate coordinate)
        {
            if (coordinate.X < BOARD_LENGTH && coordinate.X >= 0 &&
                coordinate.Y < BOARD_LENGTH && coordinate.Y >= 0 &&
                (coordinate.X % 2 != coordinate.Y % 2))
                return true;
            else
                return false;
        }

        public bool IsEmpty(Coordinate coordinate, List<Coordinate> whiteList, List<Coordinate> blackList)
        {
            if (!CheckInList(coordinate, whiteList) && !CheckInList(coordinate, blackList))
                return true;
            else
                return false;
        }

        public bool CheckInList(Coordinate coordinate, List<Coordinate> coordinateList)
        {
            foreach (Coordinate position in coordinateList)
            {
                if (position.Taken == false &&          //<== added so that taken pieces no longer have an effect on the movement of pieces
                    /*coordinate.Taken == false &&*/
                    position.X == coordinate.X && position.Y == coordinate.Y)
                    return true;
            }
            return false;
        }

        bool CanMove(Coordinate toPosition, List<Coordinate> activeList, List<Coordinate> passiveList)
        {
            if (!OnBoard(toPosition))
                return false;

            //Checks that toPosition isn't in activeList or passiveList
            foreach (Coordinate coordinate in activeList)
            {
                if (coordinate.X == toPosition.X && coordinate.Y == toPosition.Y &&
                    coordinate.Taken == false)
                    return false;
            }
            foreach (Coordinate coordinate in passiveList)
            {
                if (coordinate.X == toPosition.X && coordinate.Y == toPosition.Y &&
                    coordinate.Taken == false)
                    return false;
            }
            return true;

        }

        bool CanTake(Coordinate position, int xDirection, int yDirection, List<Coordinate> activeList, List<Coordinate> passiveList)
        {
            bool canTake = false;
            Coordinate enemyPosition = new Coordinate(position.X + xDirection, position.Y + yDirection);
            Coordinate jumpPosition = new Coordinate(position.X + 2 * xDirection, position.Y + 2 * yDirection);

            if (CheckInList(enemyPosition, passiveList) &&
                !CheckInList(enemyPosition, activeList) &&
                IsEmpty(jumpPosition, activeList, passiveList) &&
                OnBoard(jumpPosition))
                canTake = true;
            return canTake;
        }

        public List<TakenList> FindTakeMoves(Coordinate piece, List<Coordinate> activeList, List<Coordinate> passiveList, List<Coordinate> takenList, int multiplier, bool taken)
        {
            List<Coordinate> localTakenList = new List<Coordinate>();

            List<TakenList> returnTakenList = new List<TakenList>();
            int takeNum = 0;

            //if promoted, first for loop goes from -1 to 1
            int yStart = multiplier;
            if (piece.Promoted)
            {
                yStart = -multiplier;
                takeNum = 0;
            }
            for (int j = yStart; j * multiplier <= 1; j += 2 * multiplier)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    if (CanTake(piece, i, j, activeList, passiveList))
                    {
                        //Place takenList back into localTakenList in case there is more than one take during the for loops
                        localTakenList.Clear();
                        foreach (Coordinate coordinate in takenList)
                        {
                            localTakenList.Add(new Coordinate(coordinate));
                        }

                        //gernerate new positions to move and take
                        takeNum++;
                        Coordinate removePosition = new Coordinate(piece.X + i, piece.Y + j);
                        Coordinate jumpPosition = new Coordinate(piece.X + (2 * i), piece.Y + (2 * j));
                        if (piece.Promoted)
                            jumpPosition.Promote();
        
                        List<Coordinate> localActiveList = new List<Coordinate>();
                        List<Coordinate> localPassiveList = new List<Coordinate>();

                        //assigning values to the local active list, to be passed onto the next instance of FindTakeMoves
                        foreach (Coordinate coordinate in activeList)
                        {
                            if (coordinate.X == piece.X &&
                                coordinate.Y == piece.Y)
                                localActiveList.Add(jumpPosition);
                            else
                                localActiveList.Add(new Coordinate(coordinate));
                        }

                        //assigning values to the local passive list, to be passed onto the next instance of FindTakeMoves
                        foreach (Coordinate coordinate in passiveList)
                        {
                            if (coordinate.X != removePosition.X ||
                                coordinate.Y != removePosition.Y)
                                localPassiveList.Add(new Coordinate(coordinate));
                        }
                        localTakenList.Add(new Coordinate(removePosition));

                        //recursive call of function with new parameters
                        foreach (TakenList takenPieces in FindTakeMoves(jumpPosition, localActiveList, localPassiveList, localTakenList, multiplier, true))
                        {
                            returnTakenList.Add(takenPieces);
                        }
                    }
                }
            }
            //if statement ensures that a move is only placed on returnTakenList if no pieces have been taken during this call of the function
            //and at least one piece was taken during a previous call of the function. Taken is only true if the last function call took 1 or more pieces.
            if (takeNum == 0 && taken == true)
                returnTakenList.Add(new TakenList(takenList, piece));
            return returnTakenList;
        }

        //returns a List of Move with one piece taken
        public List<Move> FindSingleMove(Coordinate piece, List<Coordinate> activeList, List<Coordinate> passiveList, List<Coordinate> whiteList, List<Coordinate> blackList, int multiplier)
        {
            List<Move> possibleMoves = new List<Move>();

            //looks in all directions for the piece
            int yStart = multiplier;
            if (piece.Promoted)
            {
                yStart = -multiplier;
            }
            for (int j = yStart; j * multiplier <= 1; j += 2 * multiplier)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    //adds move if it can take 1 piece
                    if (CanTake(piece, i, j, activeList, passiveList))
                    {
                        Coordinate finalPosition = new Coordinate(piece.X + 2 * i, piece.Y + 2 * j);
                        Coordinate takenPosition = new Coordinate(piece.X + i, piece.Y + j);
                        possibleMoves.Add(new Move(piece, finalPosition, new List<Coordinate> { takenPosition }, whiteList, blackList, TAKE_SCORE, 0));
                    }
                }
            }
            return possibleMoves;
        }

        //Returns a list of move with 0 pieces taken
        public List<Move> NoTakeMove(Coordinate piece, List<Coordinate> activeList, List<Coordinate> passiveList, List<Coordinate> whiteList, List<Coordinate> blackList, int multiplier)
        {
            List<Move> possibleMoves = new List<Move>();
            int yStart = multiplier;
            if (piece.Promoted)
            {
                yStart = -multiplier;
            }
            for (int j = yStart; j * multiplier <= 1; j += 2 * multiplier)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Coordinate newPosition = new Coordinate(piece.X + i, piece.Y + j);
                    if (IsEmpty(newPosition, whiteList, blackList))
                        possibleMoves.Add(new Move(piece, newPosition, new List<Coordinate>(), whiteList, blackList, MOVE_SCORE, 0));
                }
            }
            return possibleMoves;
        }

        public List<Move> FindValidMoves
            (Coordinate pieceLocation,
            List<Coordinate> whiteList,
            List<Coordinate> blackList,
            int predictNum)
        {
            const int UP = 1;
            const int DOWN = -1;
            const int LEFT = -1;
            const int RIGHT = 1;

            List<Move> possibleMoves = new List<Move>();

            //Piece moving is contained within whiteList
            if (CheckInList(pieceLocation, whiteList))
            {
                return FindMove(pieceLocation, whiteList, blackList, whiteList, blackList, UP, DOWN, LEFT, RIGHT, predictNum, 1);
            }
            //piece moving is contained wihtin blackList
            else if (CheckInList(pieceLocation, blackList))
            {
                return FindMove(pieceLocation, blackList, whiteList, whiteList, blackList, DOWN, UP, RIGHT, LEFT, predictNum, -1);
            }
            return possibleMoves;
        }

        List<Move> FindMove
            (Coordinate pieceLocation,
            List<Coordinate> activeList, 
            List<Coordinate> passiveList,
            List<Coordinate> whiteList,
            List<Coordinate> blackList,
            int up, int down, int left, int right, 
            int predictNum, int scoreMultiplier)
        {
            List<Move> possibleMoves = new List<Move>();

            List<TakenList> takenLists = new List<TakenList>();

            takenLists = FindTakeMoves(pieceLocation, activeList, passiveList, new List<Coordinate>(), up, false);

            bool pieceTaken = true;
            if (takenLists.Count == 0)
                pieceTaken = false;
            //All moves look predictNum moves into the future
            //Only adds moves which don't have pieces taken if no pieces can take
            if (pieceTaken == false)
            {
                Coordinate moveUpLeft = new Coordinate(pieceLocation.X + left, pieceLocation.Y + up);
                Coordinate moveUpRight = new Coordinate(pieceLocation.X + right, pieceLocation.Y + up);
                Coordinate moveDownLeft = new Coordinate(pieceLocation.X + left, pieceLocation.Y + down);
                Coordinate moveDownRight = new Coordinate(pieceLocation.X + right, pieceLocation.Y + down);
                if (CanMove(moveUpLeft, activeList, passiveList))
                {
                    possibleMoves.Add(new Move(pieceLocation, moveUpLeft, new List<Coordinate>(), whiteList, blackList, MOVE_SCORE * scoreMultiplier, predictNum));
                }
                if (CanMove(moveUpRight, activeList, passiveList))
                {
                    possibleMoves.Add(new Move(pieceLocation, moveUpRight, new List<Coordinate>(), whiteList, blackList, MOVE_SCORE * scoreMultiplier, predictNum));
                }
                if (pieceLocation.Promoted == true)
                {
                    if (CanMove(moveDownLeft, activeList, passiveList))
                    {
                        possibleMoves.Add(new Move(pieceLocation, moveDownLeft, new List<Coordinate>(), whiteList, blackList, MOVE_SCORE * scoreMultiplier, predictNum));
                    }
                    if (CanMove(moveDownRight, activeList, passiveList))
                    {
                        possibleMoves.Add(new Move(pieceLocation, moveDownRight, new List<Coordinate>(), whiteList, blackList, MOVE_SCORE * scoreMultiplier, predictNum));
                    }
                }
            }

            //Creates moves from the TakenLists
            foreach (TakenList takenList in takenLists)
            {
                possibleMoves.Add(new Move(pieceLocation, takenList.FinalPosition, takenList.TakenPieces, whiteList, blackList, takenList.TakenPieces.Count * TAKE_SCORE * scoreMultiplier, predictNum));
            }
            return possibleMoves;
        }
    }
}
