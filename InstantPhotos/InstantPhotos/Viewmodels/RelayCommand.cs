using System;
using System.Windows.Input;

namespace InstantPhotos
{
    class RelayCommand<T> : ICommand
    {

        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// The action to execute
        /// </summary>
        private Action<T> executeAction;

        /// <summary>
        /// Defines the behavior of execute command
        /// </summary>
        private Func<bool> canExecuteAction;

        /// <summary>
        /// CreateCommand constructor
        /// </summary>
        /// <param name="action"></param>
        public RelayCommand(Action<T> executeAction, Func<bool> canExecuteAction)
        {
            this.executeAction = executeAction ?? throw new ArgumentNullException("executeAction");
            this.canExecuteAction = canExecuteAction;
        }

        /// <summary>
        /// Returns true if SelectedComponent != null
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {

            return canExecuteAction == null || canExecuteAction();
        }

        /// <summary>
        /// Specifies what should be executed on command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this.executeAction((T)parameter);
        }
    }
}
