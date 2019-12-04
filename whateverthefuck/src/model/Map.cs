using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    class Map
    {
        private const float GridSize = 0.1f;

        public List<GameEntity> Entities = new List<GameEntity>();

        Random random = new Random();

        public Map(int seed)
        {
            CreateRoom(0, 0, 7, 4);

            CreateRoom(10, 0, 20, 8);
        }

        private void CreateRoom(int x1, int y1, int x2, int y2)
        {
            Block e;

            for (int x = x1; x < x2; x++)
            {
                e = new Block(Color.Green);
                e.Size = new GameCoordinate(GridSize, GridSize);
                e.Location = new GameCoordinate(x * GridSize, y1 * GridSize);
                Entities.Add(e);

                e = new Block(Color.Green);
                e.Size = new GameCoordinate(GridSize, GridSize);
                e.Location = new GameCoordinate(x * GridSize, (y2-1) * GridSize);
                Entities.Add(e);
            }

            for (int y = y1 + 1; y < y2 - 1; y++)
            {
                e = new Block(Color.Blue);
                e.Size = new GameCoordinate(GridSize, GridSize);
                e.Location = new GameCoordinate(x1 * GridSize, y * GridSize);
                Entities.Add(e);

                e = new Block(Color.Blue);
                e.Size = new GameCoordinate(GridSize, GridSize);
                e.Location = new GameCoordinate((x2-1)  * GridSize, y * GridSize);
                Entities.Add(e);
            }
        }
    }
}
