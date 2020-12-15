using ffcrm.UserEmailService.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffcrm.UserEmailService
{
    public class Emailer
    {
        public Emailer()
        {
            RunEmailer();
        }

        private void RunEmailer()
        {
            var loginDb = new DbLoginDataContext(new Utils().GetLoginConnection());

            //TODO - where EmailDigest = Weekly
            var globalUsers = loginDb.GlobalUsers.OrderBy(x => x.FullName);

            foreach(var globalUser in globalUsers)
            {

            }
        }

        private List<>
    }
}
