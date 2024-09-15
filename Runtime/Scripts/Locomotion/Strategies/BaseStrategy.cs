using System;
using System.Collections.Generic;
using VK.Input;

namespace VK.Locomotion
{
    public abstract class BaseStrategy
    {
        protected LocomotionController _locomotionController;
        protected BaseSettings _settings;
        protected InputHandler _inputHandler;
        protected Func<bool> _enterCondition;
        protected List<BaseSettings> _transitionsTo = new();
        public BaseSettings Settings => _settings;
        public Func<bool> EnterCondition => _enterCondition;
        public List<BaseSettings> TransitionsTo => _transitionsTo;
        public BaseStrategy(LocomotionController locomotionController, InputHandler inputHandler, BaseSettings settings)
        {
            _locomotionController = locomotionController;
            _inputHandler = inputHandler;
            _settings = settings;

            _enterCondition = _settings.EnterCondition;
            foreach (var strategyContainer in _settings.PossibleTransitionsTo)
            {
                _transitionsTo.Add(strategyContainer);
            }
        }
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Execute() { }
        public virtual void PhysicsExecute() { }
    }
}