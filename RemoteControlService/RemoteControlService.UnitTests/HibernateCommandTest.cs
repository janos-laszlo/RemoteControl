﻿using Domain.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteControlService.UniTests.Mocks;

namespace RemoteControlService.UniTests
{
    [TestClass]
    public class HibernateCommandTest
    {
        [TestMethod]
        public void Execute_Hibernation_WillHibernate()
        {
            // Arrange
            ICommand cmd = new HibernateCommand(new CmdLinePowerControllerMock());

            // Act
            cmd.Execute();
        }
    }
}
