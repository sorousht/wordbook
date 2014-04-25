using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordbook.Controllers
{
    public static class ControllerLocator
    {
        static ControllerLocator()
        {
            ControllerLocator.EditController = new EditController();
            ControllerLocator.ListController = new ListController();
            ControllerLocator.MainController = new MainController();
        }
        public static EditController EditController { get; set; }

        public static ListController ListController { get; set; }

        public static MainController MainController { get; set; }

    }
}
