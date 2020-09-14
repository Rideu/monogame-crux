using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Crux.Simplex;
using static Crux.CoreTests;
using static System.Math;

namespace Crux
{
    public struct Timer
    {
        public float Period;


        /// <summary>
        /// Returns passed percent of time.
        /// </summary>
        public float Progress { get { return Elapse / Period; } }
        public bool IsRunning { get { return Run; } }

        bool Run;

        float Elapse;

        public event Action OnFinish;

        public Timer(float periodmillis)
        {
            Period = periodmillis;
            Run = false;
            Elapse = 0f;
            OnFinish = new Action(() => { });
            //Timing.Timers.Add(this);
        }

        public void Start()
        {
            Run = true;
        }

        public void Stop()
        {
            Run = !true;
        }


        public void Reset()
        {
            Elapse = 0;
        }

        public void Restart()
        {
            Reset(); Start();
        }

        public void Update(GameTime gt)
        {
            Update((float)gt.ElapsedGameTime.TotalMilliseconds);
        }

        public void Update(float elapse)
        {
            if (Run)
            {
                Elapse += elapse;
                if (Elapse >= Period)
                {
                    Elapse = Period;
                    Run = !true;
                    OnFinish?.Invoke();
                }
            }
        }


        public static implicit operator float(Timer t)
        {
            return t.Progress;
        }
    }

    public static class Timing
    {
        internal static List<Timer> Timers = new List<Timer>();

        public static void Update(GameTime gt)
        {
            foreach (var n in Timers)
            {
                n.Update((float)gt.ElapsedGameTime.TotalMilliseconds);
            }
        }

    }
}
