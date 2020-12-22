using System;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Netnr.SharedFast;
using System.Collections.Generic;
using System.Linq;

namespace Netnr.Blog.Data
{
    /// <summary>
    /// 扩展
    /// </summary>
    public partial class ContextBase : DbContext
    {
        /// <summary>
        /// 填充表
        /// </summary>
        /// <param name="args"></param>
        public static Dictionary<string, object> GetDicDbSet(ContextBase db)
        {
            var dic = GetDicDbSet(db.DocSet, db.DocSetDetail, db.Draw, db.GiftRecord, db.GiftRecordDetail, db.Gist, db.GistSync, db.KeyValues, db.KeyValueSynonym, db.Notepad, db.OperationRecord, db.Run, db.Tags, db.UserConnection, db.UserInfo, db.UserMessage, db.UserReply, db.UserWriting, db.UserWritingTags);
            return dic;
        }

        /// <summary>
        /// 填充表
        /// </summary>
        /// <param name="args"></param>
        public static Dictionary<string, object> GetDicDbSet(params object[] args)
        {
            var DicDbSet = new Dictionary<string, object>();
            foreach (var arg in args)
            {
                var name = arg.GetType().FullName.Split(',')[0].Split('.').Last();
                DicDbSet.Add(name, arg);
            }
            return DicDbSet;
        }

        /// <summary>
        /// 只读（1分钟后生效，为初始化数据预留时间）
        /// </summary>
        public static void IsReadOnly()
        {
            if (GlobalTo.GetValue<bool>("ReadOnly") && Process.GetCurrentProcess().StartTime.AddMinutes(1) < DateTime.Now)
            {
                throw new Exception("The database is read-only");
            }
        }

        public override int SaveChanges()
        {
            IsReadOnly();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            IsReadOnly();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            IsReadOnly();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IsReadOnly();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}