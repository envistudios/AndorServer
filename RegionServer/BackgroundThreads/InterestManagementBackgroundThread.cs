using System;
using System.Diagnostics;
using System.Threading;
using MMO.Framework;
using RegionServer.Model;
using RegionServer.Model.Interfaces;

namespace RegionServer.BackgroundThreads
{
    public class InterestManagementBackgroundThread : IBackgroundThread
    {
        private bool _isRunning = false;
        public Region Region { get; set; }

        private const int FullUpdateTimer = 100;
        private int _fullUpdateTimer = FullUpdateTimer;
        private bool _updatePass = true;

        public InterestManagementBackgroundThread(Region region)
        {
            Region = region;
        }

        public void Setup()
        {
        }

        public void Run(object threadContext)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            _isRunning = true;

            while (_isRunning)
            {
                if (timer.Elapsed < TimeSpan.FromMilliseconds(1250))
                {
                    if (Region.NumPlayers <= 0)
                    {
                        Thread.Sleep(1000);
                        timer.Restart();
                    }

                    Thread.Sleep(1250 - timer.Elapsed.Milliseconds);

                    continue;
                }

                var updateTime = timer.Elapsed;
                timer.Restart();
                Update(updateTime, (FullUpdateTimer == _fullUpdateTimer), _updatePass);

                if (_fullUpdateTimer > 0)
                {
                    _fullUpdateTimer--;
                }
                else
                {
                    _fullUpdateTimer = FullUpdateTimer;
                }

                _updatePass = !_updatePass;
            }
        }

        private void Update(TimeSpan elapsed, bool fullUpdate, bool forgetObjects)
        {
            Region.ForEachObject(obj =>
            {
                if (obj.IsVisible)
                {
                    var aggressive = false; //@TODO: (GuardAttackAgroMob && obj is CGuardInstance) // Guards which dont exist yet
                    obj.KnownList.ForgetObjects(forgetObjects);

                    if (obj is IPlayable || aggressive || fullUpdate)
                    {
                        foreach (var visible in Region.VisibleObjects)
                        {
                            if (visible != obj)
                            {
                                obj.KnownList.AddKnownObject(visible);
                            }
                        }
                    }
                    else if (obj is ICharacter)
                    {
                        foreach (IObject visible in Region.GetVisiblePlayable(obj))
                        {
                            if (visible != obj)
                            {
                                obj.KnownList.AddKnownObject(visible);
                            }
                        }
                    }
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}