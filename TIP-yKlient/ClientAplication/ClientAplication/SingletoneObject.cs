using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientAplication
{
    public sealed class SingletoneObject
    {

        internal User user;
        internal Client client;
        internal ObservableCollection<User> Users;
        internal ObservableCollection<User> Friends;
        internal List<ListUser> listUsers;
        internal PhoneVOIP phoneVOIP;
        internal ObservableCollection<itemTB> items;
        internal Thread isOnlineThread;

        private static SingletoneObject instance = null;
        private static readonly object PadLock = new object();
        public static SingletoneObject GetInstance
        {
            get
            {
                lock (PadLock)
                {
                    if (instance == null)
                    {
                        instance = new SingletoneObject();
                    }
                    return instance;
                }
            }
        }
        internal string searchvalue;

        public void setdefaultvalue()
        {
            client.destroyfunction();
            user = null;
            client = null;
            Users = null;
            Friends = null;
            listUsers = null;
            phoneVOIP = null;
            items = null;
            isOnlineThread=null;
            searchvalue = "";
        }
    }

}
