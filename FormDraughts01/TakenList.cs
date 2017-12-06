using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormDraughts01
{
    class TakenList
    {
        public List<Coordinate> TakenPieces = new List<Coordinate>();
        public Coordinate FinalPosition;

        public TakenList(List<Coordinate> takenList, Coordinate finalPosition)
        {
            this.TakenPieces = AssignValues(takenList);
            this.FinalPosition = new Coordinate(finalPosition);
        }

        public TakenList(TakenList takenList)
        {
            this.TakenPieces = AssignValues(takenList.TakenPieces);
            this.FinalPosition = takenList.FinalPosition;
        }

        public void Add(Coordinate takenPosition)
        {
            this.TakenPieces.Add(new Coordinate(takenPosition));
        }

        List<Coordinate> AssignValues(List<Coordinate> sourceList)
        {
            List<Coordinate> tempList = new List<Coordinate>();
            foreach (var dataPoint in sourceList)
                tempList.Add(dataPoint);
            return tempList;
        }
    }
}
