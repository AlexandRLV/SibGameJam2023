using System;
using System.Collections.Generic;
using GameCore.Character.Visuals;

namespace GameCore.StateMachine
{
    public abstract class StateBase<TStateType> where TStateType : Enum
    {
        public abstract AnimationType AnimationType { get; }
        public abstract TStateType Type { get; }

        public HashSet<TStateType> whiteListOnEnter = new();
        public HashSet<TStateType> whiteListOnExit = new();
        
        public HashSet<TStateType> blackListOnEnter = new();
        public HashSet<TStateType> blackListOnExit = new();

        public bool AllowEnterFrom(TStateType prevState)
        {
            bool inWhiteList = whiteListOnEnter.Count == 0 || whiteListOnEnter.Contains(prevState);
            bool inBlackList = blackListOnEnter.Count > 0 && blackListOnEnter.Contains(prevState);
            return inWhiteList && !inBlackList;
        }
        
        public bool AllowExitTo(TStateType nextState)
        {
            bool inWhiteList = whiteListOnExit.Count == 0 || whiteListOnExit.Contains(nextState);
            bool inBlackList = blackListOnExit.Count > 0 && blackListOnExit.Contains(nextState);
            return inWhiteList && !inBlackList;
        }

        public virtual bool CanEnter(TStateType prevState) => true;
        public virtual bool CanExit(TStateType nextState) => true;

        public virtual void OnEnter(TStateType prevState) { }
        public virtual void OnExit(TStateType nextState) { }
		
        public virtual void Update() { }
    }
}