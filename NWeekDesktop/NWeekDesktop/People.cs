using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWeekDesktop
{
    class People
    {
        public People(string uID, string name,  string email, string allInfo)
        {
            this.UID = uID;
            this.AllInfo = allInfo;
            this.Name = name;
            this.Email = email;
        }

        public string UID { get; set; }

        public string AllInfo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        
        //////// METHODS
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                People p = (People)obj;
                return UID.Equals(p.UID);
            }
        }

        public override string ToString()
        {
            if(Name != "")
            {
                return Name + " " + Email;
            }
            else
            {
                return UID + "_" + Name + "_" + Email + "_" + AllInfo;
            }
            
        }
        public override int GetHashCode()
        {
            return UID.GetHashCode();
        }


    }
}
