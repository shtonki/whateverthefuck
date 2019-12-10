using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    class LineOfSight
    {
        private const bool ShowOutlines = false;

        public static IEnumerable<GameEntity> CheckLOS(GameEntity looker, IEnumerable<GameEntity> looked)
        {
            var extras = new List<Drawable>();

            var rt = new List<GameEntity>();

            if (looker == null)
            {
                return rt;
            }

            rt.Add(looker);

            var blockers = looked.Where(e => e.BlocksLOS);

            PointF lookerPOV = new PointF(looker.Center.X, looker.Center.Y);

            foreach (var target in looked)
            {
                if (ShowOutlines)
                {
                    extras.Add(new view.Rectangle(target, Color.White));
                }

                if (target == looker) { continue; }

                PointF p1 = new PointF(target.Left, target.Top);
                PointF p2 = new PointF(target.Right, target.Top);
                PointF p3 = new PointF(target.Right, target.Bottom);
                PointF p4 = new PointF(target.Left, target.Bottom);

                bool visionBlockedP1 = false;
                bool visionBlockedP2 = false;
                bool visionBlockedP3 = false;
                bool visionBlockedP4 = false;

                foreach (var blocker in blockers)
                {
                    if (blocker == target || blocker == looker
                        || target.Height >= blocker.Height
                        )
                    {
                        continue;
                    }

                    visionBlockedP1 |= LineIntersectsRect(lookerPOV, p1, blocker);
                    visionBlockedP2 |= LineIntersectsRect(lookerPOV, p2, blocker);
                    visionBlockedP3 |= LineIntersectsRect(lookerPOV, p3, blocker);
                    visionBlockedP4 |= LineIntersectsRect(lookerPOV, p4, blocker);
                }


                if (!visionBlockedP1 ||
                    !visionBlockedP2   ||
                    !visionBlockedP3   ||
                    !visionBlockedP4)
                {
                    rt.Add(target);
                }
            }
            GUI.DebugInfo = extras;
            return rt;
        }

        private const float Spacing = -0.00005f;

        private static bool LineIntersectsRect(PointF p1, PointF p2, GameEntity ge)
        {
            RectangleF r = new RectangleF(ge.Location.X + Spacing, ge.Location.Y + Spacing, ge.Size.X - Spacing, ge.Size.Y - Spacing); ;
            return LineIntersectsLine(p1, p2, new PointF(r.X, r.Y), new PointF(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new PointF(r.X + r.Width, r.Y), new PointF(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new PointF(r.X + r.Width, r.Y + r.Height), new PointF(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new PointF(r.X, r.Y + r.Height), new PointF(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        private static bool LineIntersectsLine(PointF l1p1, PointF l1p2, PointF l2p1, PointF l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if( d == 0 )
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if( r <= 0 || r >= 1 || s <= 0 || s >= 1 )
            {
                return false;
            }

            return true;
        }
    }
}
