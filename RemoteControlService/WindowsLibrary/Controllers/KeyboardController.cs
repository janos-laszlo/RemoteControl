using Domain.Controllers;

namespace WindowsLibrary.Controllers
{
    class KeyboardController : IKeyboardController
    {
        public void PressKey(string key)
        {
            //SendKeys.SendWait("^+{ESC}");
        }
    }
}
