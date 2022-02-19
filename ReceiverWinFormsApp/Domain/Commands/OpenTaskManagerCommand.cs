using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;

namespace Domain.Commands
{
    public class OpenTaskManagerCommand : ICommand
    {
        private readonly IKeyboardController keyboardController;

        public OpenTaskManagerCommand(IKeyboardController keyboardController)
        {
            this.keyboardController = keyboardController;
        }

        public Maybe<string> Execute()
        {
            keyboardController.PressKey("bla");
            return Maybe<string>.None();
        }
    }
}
