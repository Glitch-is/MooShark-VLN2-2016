using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MooShark2.Services
{
    class UserService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();

        public List<account> GetAllUsers()
        {
            var ret = (from usr in db.account
                       select usr).ToList();
            return ret;
        }

        public List<account> GetAllTeachers()
        {
            var ret = (from usr in db.account
                       where usr.roleId.Equals(2)
                       select usr).ToList();
            return ret;
        }

        public account getSpecificUser(int id)
        {
            var ret = (from usr in db.account
                      where usr.id.Equals(id)
                      select usr).FirstOrDefault();
            return ret;
        }
    }
}
