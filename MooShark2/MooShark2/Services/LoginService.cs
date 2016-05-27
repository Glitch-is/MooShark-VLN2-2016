using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MooShark2.Services
{

    public class LoginService
    {
        private VLN2_2016_H32Entities db = new VLN2_2016_H32Entities();

        static string sha256(string password)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public bool  Validate(string email, string password)
        {

            var acc = (from account in db.account
                       where account.email.Equals(email)
                       select account).FirstOrDefault();
            var hash = sha256(password);
            if (acc == null)
                return false;
            else
                return hash == acc.password;
        }

        public void Login(string email, string sessionId)
        {
            var user = db.account.Where(m => m.email == email).First();
            user.sessionId = sessionId;
            var time = DateTime.Now;
            user.sessionExp = time.AddHours(24);
            user.lastLoggedIn = time;
            db.SaveChanges();
          }
        public account Session2Account(string session)
        {
            var ret = (from account in db.account

                       where account.sessionId.Equals(session)
                       select account).FirstOrDefault();
            return ret;
        }

    }
}