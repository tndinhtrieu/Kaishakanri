﻿//------------------------------------------------------------------------------
// <auto-generated>
//    このコードはテンプレートから生成されました。
//
//    このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//    このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kaishakanri.Mode
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class KaishakanriDatabaseEntities : DbContext
    {
        public KaishakanriDatabaseEntities()
            : base("name=KaishakanriDatabaseEntities")
        {
        }
        public KaishakanriDatabaseEntities(string connection)
            : base(connection)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Kaishajouhou> Kaishajouhous { get; set; }
    }
}
