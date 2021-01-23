using Domain.CommandFactories;
using Domain.Common.Utilities;
using Domain.Controllers;

namespace Domain.Commands
{
    public class HibernateCommand : ICommand
    {
        private readonly IPowerController powerController;

        public HibernateCommand(IPowerController powerController)
        {
            this.powerController = powerController;
        }

        public Maybe<string> Execute()
        {
            powerController.Hibernate();
            return Maybe<string>.None();
        }
    }
}
