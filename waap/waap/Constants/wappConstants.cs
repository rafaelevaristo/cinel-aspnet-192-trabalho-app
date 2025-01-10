namespace wapp
{
    public static class waapConstants
    {
         public readonly struct USERS
        {
            public readonly struct ADMIN
            {
                public static readonly string USERNAME = "admin@sms.pt";
                public static readonly string EMAIL = "admin@sms.pt";
                public static readonly string PASSWORD = "xpto1234";
            }

            public readonly struct SALESMAN
            {
                public static readonly string USERNAME = "SALESMAN@sms.pt";
                public static readonly string EMAIL = "SALESMAN@sms.pt";
                public static readonly string PASSWORD = "01012024";
            }

            public readonly struct LOGISTICS
            {
                public static readonly string USERNAME = "administrative@sms.pt";
                public static readonly string EMAIL = "administrative@sms.pt";
                public static readonly string PASSWORD = "qwerty1234";
            }
        }

        public readonly struct ROLES
        {
            public const string ADMIN = "ADMIN";
            public const string SALESMAN = "SALESMAN";
            public const string LOGISTICS = "LOGISTICS";

        }

        public readonly struct POLICIES
        {
            public readonly struct APP_POLICY
            {
                public const string NAME = "APP_POLICY";
                public static readonly string[] POLICY_ROLES = { 
                   ROLES.LOGISTICS,
                   ROLES.ADMIN,
                   ROLES.SALESMAN
                 };
            }

            public readonly struct APP_POLICY_EDITABLE_CRUD
            {
                public const string NAME = "APP_POLICY_EDITABLE_CRUD";
                public static readonly string[] POLICY_ROLES = { 
                   ROLES.SALESMAN,
                   ROLES.ADMIN
                 };
            }

            public readonly struct APP_POLICY_ADMIN
            {
                public const string NAME = "APP_POLICY_ADMIN";
                public static readonly string[] POLICY_ROLES = { 
                   ROLES.ADMIN
                   
                 };
            }

        }           
    }
}