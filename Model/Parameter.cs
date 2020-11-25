using System.Data;

namespace ffcrm.UserEmailService.Model
{
    public class Parameter
    {
        public string Name;
        public SqlDbType SqlType;
        public int Size;
        public string Column;
        public object Value;
    }
}
