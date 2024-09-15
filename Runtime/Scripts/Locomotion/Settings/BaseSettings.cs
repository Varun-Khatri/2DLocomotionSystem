using System;
using System.Collections.Generic;
using UnityEngine;
using VK.Input;

namespace VK.Locomotion
{

    public abstract class BaseSettings : ScriptableObject
    {
        [SerializeField] private List<BaseSettings> _possibleTransitionsTo;
        protected Func<bool> _enterCondition;
        protected Func<bool> _exitCondition;

        public List<BaseSettings> PossibleTransitionsTo => _possibleTransitionsTo;
        public Func<bool> EnterCondition => _enterCondition;
        public Func<bool> ExitCondition => _exitCondition;
        public abstract BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler);
    }
}
