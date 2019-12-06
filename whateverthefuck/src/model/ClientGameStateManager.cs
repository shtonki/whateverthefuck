using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using whateverthefuck.src.control;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.network.messages;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    class ClientGameStateManager
    {
        private Timer TickTimer;

        private PlayerCharacter Hero;

        public GameState GameState { get; set; }

        public ClientGameStateManager()
        {
            GameState = new GameState();

            TickTimer = new Timer(_ => Tick(), null, 0, 10);
        }

        private void Tick()
        {
            if (Hero == null)
            {
                return;
            }

            Program.ServerConnection.SendMessage(new UpdatePlayerControlMessage(Hero));
        }

        public void AddPlayerCharacter(EntityLocationInfo info)
        {
            PlayerCharacter pc = new PlayerCharacter(new EntityIdentifier(info.Identifier));
            pc.Location = new GameCoordinate(info.X, info.Y);
            GameState.AddEntity(pc);
        }

        public void TakeControl(int identifier)
        {
            Hero = (PlayerCharacter)GameState.GetEntityById(identifier);
        }

        public void UpdateLocations(IEnumerable<EntityLocationInfo> infos)
        {
            foreach (var info in infos)
            {
                var updatee = GameState.GetEntityById(info.Identifier);

                if (updatee == null)
                {
                    Logging.Log("Got position of Entity we don't think exists.", Logging.LoggingLevel.Warning);
                }
                else
                {
                    updatee.Location = new GameCoordinate(info.X, info.Y);
                }
            }
        }

        public void ActivateAction(GameAction gameAction)
        {
            switch (gameAction)
            {
                case GameAction.HeroWalkUpwards:
                    {
                        Hero.SetMovementUpwards(true);
                    }
                    break;

                case GameAction.HeroWalkUpwardsStop:
                    {
                        Hero.SetMovementUpwards(false);
                    }
                    break;

                case GameAction.HeroWalkDownwards:
                    {
                        Hero.SetMovementDownwards(true);
                    }
                    break;

                case GameAction.HeroWalkDownwardsStop:
                    {
                        Hero.SetMovementDownwards(false);
                    }
                    break;

                case GameAction.HeroWalkLeftwards:
                    {
                        Hero.SetMovementLeftwards(true);
                    }
                    break;

                case GameAction.HeroWalkLeftwardsStop:
                    {
                        Hero.SetMovementLeftwards(false);
                    }
                    break;

                case GameAction.HeroWalkRightwards:
                    {
                        Hero.SetMovementRightwards(true);
                    }
                    break;

                case GameAction.HeroWalkRightwardsStop:
                    {
                        Hero.SetMovementRightwards(false);
                    }
                    break;

                case GameAction.CameraZoomIn:
                    {
                        GUI.Camera.Zoom.ZoomIn();
                    }
                    break;

                case GameAction.CameraZoomOut:
                    {
                        GUI.Camera.Zoom.ZoomOut();
                    }
                    break;

                default: throw new Exception("Can't be fucked making a proper message so if you see this someone fucked up bad.");
            }
        }

    }
}
