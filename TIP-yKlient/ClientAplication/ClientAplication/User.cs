﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAplication
{
    class UserBase
    {
       
        public string Name { get; set; }
    }

    class User:UserBase {
        public User(bool isfriend , bool isActive)
        {
            isFriend = isfriend;
            this.isActive = isActive;
        }
        public string IcoUser { get; set; }
        public string IcoCall { get; set; }
        public string IcoColor { get; set; }
        public bool isFriend { get; set; }
        public bool isActive { get; set; }

    }


    class AplicationUser : UserBase
    {
        public AplicationUser() { }
        public string lastActivity { get; set; }
        public string password { get; set; }

    }
}
