using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    class LineOfSight
    {
        public static IEnumerable<GameEntity> CheckLOS(GameEntity looker, IEnumerable<GameEntity> looked)
        {
            var rt = new List<GameEntity>();
            rt.Add(looker);

            var blockers = looked.Where(e => e.BlocksLOS);

            PointF lookerPOV = new PointF(looker.Center.X, looker.Center.Y);

            foreach (var target in looked)
            {
                if (target == looker) { continue; }

                PointF p1 = new PointF(target.Left, target.Top);
                PointF p2 = new PointF(target.Right, target.Top);
                PointF p3 = new PointF(target.Right, target.Bottom);
                PointF p4 = new PointF(target.Left, target.Bottom);

                var blocked = blockers.Any(blocker => blocker != target && blocker != looker && 
                (
                LineIntersectsRect(lookerPOV, p1, blocker) &&
                LineIntersectsRect(lookerPOV, p2, blocker) && 
                LineIntersectsRect(lookerPOV, p3, blocker) && 
                LineIntersectsRect(lookerPOV, p4, blocker) 
                ));
#if true

#else
                (!LineIntersectsRect(lookerPOV, p1, blocker) ||
                 !LineIntersectsRect(lookerPOV, p2, blocker) ||
                 !LineIntersectsRect(lookerPOV, p3, blocker) ||
                 !LineIntersectsRect(lookerPOV, p4, blocker))
#endif

                if (!blocked)
                {
                    rt.Add(target);
                }
            }

            return rt;
        }



        private static bool LineIntersectsRect(PointF p1, PointF p2, GameEntity ge)
        {
            RectangleF r = new RectangleF(ge.Location.X, ge.Location.Y, ge.Size.X, ge.Size.Y);
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

            if( r < 0 || r > 1 || s < 0 || s > 1 )
            {
                return false;
            }

            return true;
        }
    }
}
