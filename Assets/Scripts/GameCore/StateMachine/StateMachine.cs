using System;
using System.Collections.Generic;

namespace GameCore.StateMachine
{
    public class StateMachine<TStateBase, TStateType>
        where TStateBase : StateBase<TStateType>
        where TStateType : Enum 
    {
        public TStateBase currentState;
        public List<TStateBase> states;
		
        private TStateType _currentType;
        private readonly EqualityComparer<TStateType> _comparer = EqualityComparer<TStateType>.Default;

        public void Update()
        {
            currentState.Update();
        }

        public void CheckStates()
        {
            foreach (var nextState in states)
            {
                var nextType = nextState.Type;
                if (_comparer.Equals(nextType, _currentType)) continue;
				
                if (!nextState.AllowEnterFrom(_currentType) || !nextState.CanEnter(_currentType)) continue;
                if (!currentState.CanExit(nextState.Type) || !currentState.AllowExitTo(nextType)) continue;

                currentState.OnExit(nextState.Type);
                nextState.OnEnter(_currentType);
                currentState = nextState;
                _currentType = nextType;
                break;
            }
        }

        public void ForceSetState(TStateType stateType)
        {
            if (currentState != null && _comparer.Equals(currentState.Type, stateType))
                return;
			
            foreach (var state in states)
            {
                if (!_comparer.Equals(stateType, state.Type)) continue;

                if (currentState != null)
                {
                    currentState.OnExit(stateType);
                    state.OnEnter(_currentType);
                }
                else
                {
                    state.OnEnter(default);
                }
				
                currentState = state;
                _currentType = stateType;
            }
        }
    }
}