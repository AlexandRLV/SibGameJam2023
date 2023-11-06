using System;
using System.Collections.Generic;

namespace GameCore.StateMachine
{
    public class StateMachine<TStateBase, TStateType>
        where TStateBase : StateBase<TStateType>
        where TStateType : Enum 
    {
        internal TStateBase CurrentState;
        internal List<TStateBase> States;
		
        private TStateType _currentType;
        private readonly EqualityComparer<TStateType> _comparer = EqualityComparer<TStateType>.Default;

        internal void Update()
        {
            CurrentState.Update();
        }

        internal void CheckStates()
        {
            foreach (var nextState in States)
            {
                var nextType = nextState.Type;
                if (_comparer.Equals(nextType, _currentType)) continue;
				
                if (!nextState.AllowEnterFrom(_currentType) || !nextState.CanEnter(_currentType)) continue;
                if (!CurrentState.CanExit(nextState.Type) || !CurrentState.AllowExitTo(nextType)) continue;

                CurrentState.OnExit(nextState.Type);
                nextState.OnEnter(_currentType);
                CurrentState = nextState;
                _currentType = nextType;
                break;
            }
        }

        internal void ForceSetState(TStateType stateType)
        {
            if (CurrentState != null && _comparer.Equals(CurrentState.Type, stateType))
                return;
			
            foreach (var state in States)
            {
                if (!_comparer.Equals(stateType, state.Type)) continue;

                if (CurrentState != null)
                {
                    CurrentState.OnExit(stateType);
                    state.OnEnter(_currentType);
                }
                else
                {
                    state.OnEnter(default);
                }
				
                CurrentState = state;
                _currentType = stateType;
            }
        }
    }
}