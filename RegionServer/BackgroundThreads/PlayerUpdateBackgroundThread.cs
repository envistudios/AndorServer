using System;
using System.Linq;
using MMO.Framework;
using RegionServer.Model;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RegionServer.Model.ServerEvents;
using ExitGames.Logging;

namespace RegionServer.BackgroundThreads
{
    public class PlayerUpdateBackgroundThread : IBackgroundThread
    {
        public Region Region { get; set; }
        protected ILogger Log;
        private bool _isRunning = false;

        public PlayerUpdateBackgroundThread(Region region)
        {
            Region = region;
        }

        public void Setup()
        {
            Log = LogManager.GetCurrentClassLogger();
        }

        public void Run(object threadContext)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            _isRunning = true;

            while (_isRunning)
            {
                try
                {
                    if (timer.Elapsed < TimeSpan.FromMilliseconds(100))
                    {
                        if (Region.NumPlayers <= 0)
                        {
                            Thread.Sleep(1000);
                            timer.Restart();
                        }

                        if (100 - timer.ElapsedMilliseconds > 0)
                        {
                            Thread.Sleep(100 - timer.Elapsed.Milliseconds);
                        }

                        continue;
                    }

                    var updateTime = timer.Elapsed;
                    timer.Restart();
                    Update(updateTime);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(string.Format("Exception occured in PlayerUpdateBackgroundThread.Run - {0}", e.StackTrace));
                }
            }
        }

        private void Update(TimeSpan elapsed)
        {
            Parallel.ForEach(Region.AllPlayers.Values.Where(a => a.Physics.Dirty && a is CPlayerInstance).Cast<CPlayerInstance>(), SendUpdate);
        }

        public void SendUpdate(CPlayerInstance instance)
        {
            if (instance != null && instance.Physics.Dirty)
            {
                instance.BroadcastMessage(new MoveToLocation(instance));
                instance.Physics.Dirty = false;
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}