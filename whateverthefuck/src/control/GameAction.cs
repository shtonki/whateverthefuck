using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.control
{
    public enum GameAction
    {
        Undefined,

        HeroWalkUpwards,
        HeroWalkUpwardsStop,
        HeroWalkDownwards,
        HeroWalkDownwardsStop,
        HeroWalkLeftwards,
        HeroWalkLeftwardsStop,
        HeroWalkRightwards,
        HeroWalkRightwardsStop,

        CastAbility1,

        CameraZoomOut,
        CameraZoomIn,

        TogglePanel,
    }
}
