using Domain.Controllers;

namespace ReceiverWinFormsApp.Controllers
{
    class KeyboardController : IKeyboardController
    {
        public void PressKey(string key)
        {
            //SendKeys.SendWait("^+{ESC}");
        }
    }
}
