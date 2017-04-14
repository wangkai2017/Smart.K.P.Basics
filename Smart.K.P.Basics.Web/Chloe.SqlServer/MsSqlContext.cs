using Chloe.Core;
using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Entity;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.SqlServer
{
    public class MsSqlContext : DbContext
    {
        DbContextServiceProvider _dbContextServiceProvider;
        public MsSqlContext(string connString, string dataBaseName)
            : this(new DefaultDbConnectionFactory(connString, dataBaseName))
        {
        }

        public MsSqlContext(IDbConnectionFactory dbConnectionFactory)
        {
            Utils.CheckNull(dbConnectionFactory);

            this.PagingMode = PagingMode.ROW_NUMBER;
            this._dbContextServiceProvider = new DbContextServiceProvider(dbConnectionFactory, this);
        }

        public PagingMode PagingMode { get; set; }
        public override IDbContextServiceProvider DbContextServiceProvider
        {
            get { return this._dbContextServiceProvider; }
        }
    }
}
