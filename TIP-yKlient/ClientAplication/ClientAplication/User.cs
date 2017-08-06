using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAplication
{
    class User
    {
        public User(bool isfriend)
        {
            isFriend = isfriend;
        }
        public string Name { get; set; }
        public string IcoUser { get; set; }
        public string IcoCall { get; set; }
        public string IcoColor { get; set; }
        public string password { get; set; }
        public bool isFriend { get; set; }
        public string lastActivity {get; set;}

    }
}
