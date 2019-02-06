using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiwits.Authorization.Cache.SqlServer
{
    public class UserTokenRelationshipSvc
    {

        private static UserTokenRelationshipSvc _instance { get; set; }
        public static UserTokenRelationshipSvc Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserTokenRelationshipSvc();

                return _instance;
            }
        }

        private Hiwits_Authorization_db _dbContext { get; set; }

        public UserTokenRelationshipSvc()
        {
            this._dbContext = new Hiwits_Authorization_db();
        }

        public UserTokenRelationship Get(string name)
        {
            return this._dbContext.UserTokenRelationship.Where(u => u.Name.Equals(name)).FirstOrDefault();
        }

        public void Update(UserTokenRelationship model)
        {
            var entry  = this._dbContext.Entry<UserTokenRelationship>(model);

            entry.Property("RefreshToken").IsModified = true;
            entry.Property("AccessToken").IsModified = true;

            this._dbContext.SaveChanges();
        }


        public void Add(UserTokenRelationship model)
        {
            this._dbContext.UserTokenRelationship.Add(model);
            this._dbContext.SaveChanges();
        }

    }
}
