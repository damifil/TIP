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

        internal AplicationUser user { get; set; }
        internal Client client { get; set; }
        internal ObservableCollection<User> Users { get; set; }
        internal ObservableCollection<User> Friends { get; set; }
        internal List<ListUser> listUsers { get; set; }
        internal List<ListUser> listUserSearch { get; set; }
        internal PhoneVOIP phoneVOIP { get; set; }
        internal ObservableCollection<ItemTB> items { get; set; }
        internal Thread isOnlineThread { get; set; }
        internal string searchvalue { get; set; }
        internal MainWindow mainwindow { get; set; }
        internal bool isloged { get; set; }
        internal bool listusercompare { get; set; }
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
        public SingletoneObject()
        {
            isloged = false;
            listusercompare = false;
        }

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
            isloged = false;
            
        }
    }

}
