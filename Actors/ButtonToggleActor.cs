using System.Windows.Forms;
using Akka.Actor;

namespace ChartApp.Actors
{
    /// <summary>
    /// Actor responsible for managing button toggles
    /// </summary>
    public class ButtonToggleActor : UntypedActor
    {
        private readonly IActorRef _coordinatorActor;
        private readonly Button _myButton;
        private readonly CounterType _myCounterType;
        private bool _isToggleOn;

        #region Message types

        /// <summary>
        /// Toggles this button on or off and sends an appropriate messages to the
        /// <see cref="PerformanceCounterCoordinatorActor"/>
        /// </summary>
        public class Toggle
        {
        }

        #endregion

        public ButtonToggleActor(IActorRef coordinatorActor, Button myButton, CounterType myCounterType,
            bool isToggleOn = false)
        {
            _coordinatorActor = coordinatorActor;
            _myButton = myButton;
            _myCounterType = myCounterType;
            _isToggleOn = isToggleOn;
        }

        protected override void OnReceive(object message)
        {
            if (message is Toggle && _isToggleOn)
            {
                // toggle is currently on
                // stop watching
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Unwatch(_myCounterType));
                FlipToggle();
            }
            else if (message is Toggle && !_isToggleOn)
            {
                // toggle is currently off
                // start watching
                _coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(_myCounterType));
                FlipToggle();
            }
            else
            {
                Unhandled(message);
            }
        }

        private void FlipToggle()
        {
            _isToggleOn = !_isToggleOn;

            _myButton.Text = string.Format("{0} ({1})", _myCounterType.ToString().ToUpperInvariant(),
                _isToggleOn ? "ON" : "OFF");
        }
    }
}