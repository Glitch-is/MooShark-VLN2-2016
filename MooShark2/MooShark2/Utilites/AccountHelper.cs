using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MooShark2.Utilites
{
    public class AccountHelper
    {
        private Services.LoginService LoginService = new Services.LoginService();
        public static Dictionary<string, account> accountCache = new Dictionary<string, account>();

        public account getCurrentUser(string sessionId)
        {
            if (sessionId == null)
            {
                return null;
            }

            account cur = null;
            
      
            try
            {
                cur = accountCache[sessionId];
            }
            catch (KeyNotFoundException e)
            {
                cur = LoginService.Session2Account(sessionId);
                accountCache[sessionId] = cur;
            }
            return cur;
        }

        public void removeAccountSession(string sessionId)
        {
            if (sessionId == null)
            {
                return;
            }

            account cur = null;
      
            try
            {
                cur = accountCache[sessionId];
                accountCache.Remove(sessionId);
            }
            catch (KeyNotFoundException e)
            {
                cur = LoginService.Session2Account(sessionId);
            }
            cur.sessionId = "";
        }
    }
}