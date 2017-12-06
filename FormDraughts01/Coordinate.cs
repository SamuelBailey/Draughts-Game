using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormDraughts01
{
    class Coordinate
    {
        public int X = new int();
        public int Y = new int();
        public bool Taken = new bool();
        public bool Promoted = new bool();

        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Taken = false;
            this.Promoted = false;
        }

        public Coordinate(Coordinate coordinate)
        {
            this.X = coordinate.X;
            this.Y = coordinate.Y;
            this.Taken = false;
            this.Promoted = false;
        }

        public void Take()
        {
            this.Taken = true;
            this.X = -1;
            this.Y = -1;
        }

        public void Promote()
        {
            this.Promoted = true;
        }
    }
}
