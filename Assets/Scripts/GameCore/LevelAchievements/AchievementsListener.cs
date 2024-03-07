using System;
using System.Collections.Generic;

namespace GameCore.LevelAchievements
{
    public class AchievementsListener : IDisposable
    {
        private readonly List<AchievementListenerBase> _listeners = new();

        public void AddAchievementListener(AchievementListenerBase listener)
        {
            listener.Initialize();
            _listeners.Add(listener);
        }
        
        public void Dispose()
        {
            foreach (var listener in _listeners)
            {
                listener.Dispose();
            }
        }
    }
}