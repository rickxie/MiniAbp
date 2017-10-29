using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;  
using MiniAbp.Domain;
using MiniAbp.Runtime;
using MiniAbp.Logging;
using MiniAbp.Domain.Uow;
using Castle.Core.Logging;
using MiniAbp.Reflection;

namespace MiniAbp.EntityFramework
{
    /// <summary>
    /// Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class EfDbContext : DbContext, ITransientDependency, IShouldInitialize
    {
        /// <summary>
        /// Used to get current session values.
        /// </summary>
        public ISession Session { get; set; }

        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public Logging.ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the current UOW provider.
        /// </summary>
        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }


        /// <summary>
        /// Constructor.
        /// Uses <see cref="IAbpStartupConfiguration.DefaultNameOrConnectionString"/> as connection string.
        /// </summary>
        protected EfDbContext()
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(DbCompiledModel model)
            : base(model)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            InitializeDbContext();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected EfDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            InitializeDbContext();
        }

        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
            RegisterToChanges();
        }

        private void RegisterToChanges()
        {
            ((IObjectContextAdapter) this)
                .ObjectContext
                .ObjectStateManager
                .ObjectStateManagerChanged += ObjectStateManager_ObjectStateManagerChanged;
        }

        protected virtual void ObjectStateManager_ObjectStateManagerChanged(object sender,
            System.ComponentModel.CollectionChangeEventArgs e)
        {
            var contextAdapter = (IObjectContextAdapter) this;
            if (e.Action != CollectionChangeAction.Add)
            {
                return;
            }

            var entry = contextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntry(e.Element);
            switch (entry.State)
            {
                case EntityState.Added:
                    CheckAndSetId(entry.Entity);
                    //CheckAndSetMustHaveTenantIdProperty(entry.Entity);
                    //SetCreationAuditProperties(entry.Entity, GetAuditUserId());
                    break;
                //case EntityState.Deleted: //It's not going here at all
                //    SetDeletionAuditProperties(entry.Entity, GetAuditUserId());
                //    break;
            }
        }

        private void SetNullsForInjectedProperties()
        {
            Logger = Logging.NullLogger.Instance;
            Session = NullSession.GetInstance();
            
        }

        public virtual void Initialize()
        {
            Database.Initialize(false);
            //this.SetFilterScopedParameterValue(AbpDataFilters.MustHaveTenant, AbpDataFilters.Parameters.TenantId,
            //    Session.TenantId ?? 0);
            //this.SetFilterScopedParameterValue(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId,
            //    Session.TenantId);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //    modelBuilder.Filter(AbpDataFilters.SoftDelete, (ISoftDelete d) => d.IsDeleted, false);
            //    modelBuilder.Filter(AbpDataFilters.MustHaveTenant,
            //        (IMustHaveTenant t, int tenantId) => t.TenantId == tenantId || (int?) t.TenantId == null,
            //        0); //While "(int?)t.TenantId == null" seems wrong, it's needed. See https://github.com/jcachat/EntityFramework.DynamicFilters/issues/62#issuecomment-208198058
            //    modelBuilder.Filter(AbpDataFilters.MayHaveTenant,
            //        (IMayHaveTenant t, int? tenantId) => t.TenantId == tenantId, 0);
        }

        public override int SaveChanges()
        {
            try
            {
                //var changedEntities = ApplyAbpConcepts();
                var result = base.SaveChanges();
                //EntityChangeEventHelper.TriggerEvents(changedEntities);
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                //var changeReport = ApplyAbpConcepts();
                var result = await base.SaveChangesAsync(cancellationToken);
                //await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }

        //protected virtual EntityChangeReport ApplyAbpConcepts()
        //{
        //    var changeReport = new EntityChangeReport();

        //    var userId = GetAuditUserId();

        //    foreach (var entry in ChangeTracker.Entries().ToList())
        //    {
        //        ApplyAbpConcepts(entry, userId, changeReport);
        //    }

        //    return changeReport;
        //}

        //protected virtual void ApplyAbpConcepts(DbEntityEntry entry, long? userId, EntityChangeReport changeReport)
        //{
        //    switch (entry.State)
        //    {
        //        case EntityState.Added:
        //            ApplyAbpConceptsForAddedEntity(entry, userId, changeReport);
        //            break;
        //        case EntityState.Modified:
        //            ApplyAbpConceptsForModifiedEntity(entry, userId, changeReport);
        //            break;
        //        case EntityState.Deleted:
        //            ApplyAbpConceptsForDeletedEntity(entry, userId, changeReport);
        //            break;
        //    }

        //    AddDomainEvents(changeReport.DomainEvents, entry.Entity);
        //}

        //protected virtual void ApplyAbpConceptsForAddedEntity(DbEntityEntry entry, long? userId, EntityChangeReport changeReport)
        //{
        //    CheckAndSetId(entry.Entity);
        //    CheckAndSetMustHaveTenantIdProperty(entry.Entity);
        //    CheckAndSetMayHaveTenantIdProperty(entry.Entity);
        //    SetCreationAuditProperties(entry.Entity, userId);
        //    changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Created));
        //}

        //protected virtual void ApplyAbpConceptsForModifiedEntity(DbEntityEntry entry, long? userId, EntityChangeReport changeReport)
        //{
        //    SetModificationAuditProperties(entry.Entity, userId);

        //    if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
        //    {
        //        SetDeletionAuditProperties(entry.Entity, userId);
        //        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
        //    }
        //    else
        //    {
        //        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Updated));
        //    }
        //}

        protected virtual void ApplyAbpConceptsForDeletedEntity(DbEntityEntry entry, long? userId)
        {
            CancelDeletionForSoftDelete(entry);
            SetDeletionAuditProperties(entry.Entity, userId);
            //changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
        }

        

        protected virtual void CheckAndSetId(object entityAsObj)
        {
            //Set GUID Ids
            var entity = entityAsObj as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                var entityType = ObjectContext.GetObjectType(entityAsObj.GetType());
                var idProperty = entityType.GetProperty("Id");
                var dbGeneratedAttr =
                    ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty);
                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    //entity.Id = GuidGenerator.Create();
                }
            }
        }

       
        protected virtual void SetCreationAuditProperties(object entityAsObj, long? userId)
        {
            //EntityAuditingHelper.SetCreationAuditProperties(MultiTenancyConfig, entityAsObj, Session.TenantId,
            //    userId);
        }

        protected virtual void SetModificationAuditProperties(object entityAsObj, long? userId)
        {
            //EntityAuditingHelper.SetModificationAuditProperties(MultiTenancyConfig, entityAsObj, Session.TenantId,
            //    userId);
        }

        protected virtual void CancelDeletionForSoftDelete(DbEntityEntry entry)
        {
            //if (!(entry.Entity is ISoftDelete))
            //{
            //    return;
            //}

            //var softDeleteEntry = entry.Cast<ISoftDelete>();
            //softDeleteEntry.Reload();
            //softDeleteEntry.State = EntityState.Modified;
            //softDeleteEntry.Entity.IsDeleted = true;
        }

        protected virtual void SetDeletionAuditProperties(object entityAsObj, long? userId)
        {
            //if (entityAsObj is IHasDeletionTime)
            //{
            //    var entity = entityAsObj.As<IHasDeletionTime>();

            //    if (entity.DeletionTime == null)
            //    {
            //        entity.DeletionTime = Clock.Now;
            //    }
            //}

            //if (entityAsObj is IDeletionAudited)
            //{
            //    var entity = entityAsObj.As<IDeletionAudited>();

            //    if (entity.DeleterUserId != null)
            //    {
            //        return;
            //    }

            //    if (userId == null)
            //    {
            //        entity.DeleterUserId = null;
            //        return;
            //    }

            //    //Special check for multi-tenant entities
            //    if (entity is IMayHaveTenant || entity is IMustHaveTenant)
            //    {
            //        //Sets LastModifierUserId only if current user is in same tenant/host with the given entity
            //        if ((entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == Session.TenantId) ||
            //            (entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == Session.TenantId))
            //        {
            //            entity.DeleterUserId = userId;
            //        }
            //        else
            //        {
            //            entity.DeleterUserId = null;
            //        }
            //    }
            //    else
            //    {
            //        entity.DeleterUserId = userId;
            //    }
            //}
        }

        protected virtual void LogDbEntityValidationException(DbEntityValidationException exception)
        {
            Logger.Error("There are some validation errors while saving changes in EntityFramework:");
            foreach (var ve in exception.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
            {
                Logger.Error(" - " + ve.PropertyName + ": " + ve.ErrorMessage);
            }
        }

        protected virtual string GetAuditUserId()
        {
            if (string.IsNullOrWhiteSpace(Session.UserId) &&
                CurrentUnitOfWorkProvider != null &&
                CurrentUnitOfWorkProvider.Current != null)
            {
                return Session.UserId;
            }

            return null;
        }
    }
}