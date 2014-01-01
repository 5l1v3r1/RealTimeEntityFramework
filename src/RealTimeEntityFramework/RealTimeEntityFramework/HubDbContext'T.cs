﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace RealTimeEntityFramework
{
    /// <summary>
    /// A DbContext base class the sends notifications to SignalR clients via implied Hub groups whenever any tracked entities are added, updated or deleted.
    /// </summary>
    /// <typeparam name="THub"></typeparam>
    public abstract class HubDbContext<THub> : NotifyingDbContext where THub : IHub
    {
        private IDisposable _subscription;
        private IHubContext _hubContext;

        public HubDbContext()
            : base()
        {
            Initialize(GlobalHost.ConnectionManager);
        }

        public HubDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Initialize(GlobalHost.ConnectionManager);
        }

        public HubDbContext(DbCompiledModel model)
            : base(model)
        {
            Initialize(GlobalHost.ConnectionManager);
        }

        public HubDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            Initialize(GlobalHost.ConnectionManager);
        }

        public HubDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Initialize(GlobalHost.ConnectionManager);
        }

        public HubDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            Initialize(GlobalHost.ConnectionManager);
        }

        public HubDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            Initialize(GlobalHost.ConnectionManager);
        }

        public HubDbContext(IConnectionManager connectionManager)
            : base()
        {
            Initialize(connectionManager);
        }

        public HubDbContext(IConnectionManager connectionManager, string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Initialize(connectionManager);
        }

        public HubDbContext(IConnectionManager connectionManager, DbCompiledModel model)
            : base(model)
        {
            Initialize(connectionManager);
        }

        public HubDbContext(IConnectionManager connectionManager, string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            Initialize(connectionManager);
        }

        public HubDbContext(IConnectionManager connectionManager, DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            Initialize(connectionManager);
        }

        public HubDbContext(IConnectionManager connectionManager, ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            Initialize(connectionManager);
        }

        public HubDbContext(IConnectionManager connectionManager, DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            Initialize(connectionManager);
        }

        public string ClientEntityUpdatedMethodName { get; set; }

        protected override void Dispose(bool disposing)
        {
            _subscription.Dispose();

            base.Dispose(disposing);
        }

        private void Initialize(IConnectionManager connectionManager)
        {
            ClientEntityUpdatedMethodName = "entityUpdated";

            _hubContext = connectionManager.GetHubContext<THub>();

            _subscription = Subscribe(GetType(), Notify);
        }

        private void Notify(ChangeDetails details)
        {
            var payload = new
            {
                changeType = details.EntityState.ToString(),
                keyNames = details.EntityKey.EntityKeyValues.Select(k => k.Key).ToList(),
                entity = details.Entity
            };

            ((IClientProxy)_hubContext.Clients.All).Invoke(ClientEntityUpdatedMethodName, payload);
        }
    }
}