using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distribuidora.Config
{
    public static class DatabaseConfig
    {
        public static string ConnectionString =>
            "Host=ep-silent-mountain-ad3zqehq-pooler.c-2.us-east-1.aws.neon.tech;" +
            "Port=5432;" +
            "Database=neondb;" +
            "Username=neondb_owner;" +
            "Password=npg_4EsXiql9QUua;" +
            "SSL Mode=Require;";
    }
}
