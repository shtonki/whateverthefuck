using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;
using static whateverthefuck.src.model.EntityGenerator;

namespace whateverthefuck.src.model
{
    static class Map
    {
        private const float GridSize = 0.1f;

        public static List<GameEntity> CreateRoom(EntityGenerator generator, int x1, int y1, int x2, int y2)
        {
            List<GameEntity> blocks = new List<GameEntity>();

            for (int x = x1; x < x2; x++)
            {
                blocks.Add(generator.GenerateBlock(new GameCoordinate(GridSize, GridSize), new GameCoordinate(x * GridSize, y1 * GridSize)));
                blocks.Add(generator.GenerateBlock(new GameCoordinate(GridSize, GridSize), new GameCoordinate(x * GridSize, (y2 - 1) * GridSize)));
            }
#if false
            for (int y = y1 + 1; y < y2 - 1; y++)
            {
                e = new Block(gen.GenerateNextIdentifier(), Color.Green);
                e.Size = new GameCoordinate(GridSize, GridSize);
                e.Location = new GameCoordinate(x1 * GridSize, y * GridSize);
                blocks.Add(e);

                e = new Block(gen.GenerateNextIdentifier(), Color.Green);
                e.Size = new GameCoordinate(GridSize, GridSize);
                e.Location = new GameCoordinate((x2-1)  * GridSize, y * GridSize);
                blocks.Add(e);
            }
#endif

            return blocks;
        }
    }
}
