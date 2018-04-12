namespace System.Data.LibCore
{
    using System;
    using System.ComponentModel;
    using System.Data.Common;

    [DefaultEvent("RowUpdated"), Designer("Microsoft.VSDesigner.Data.VS.SqlDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxItem("LibCore.Designer.LibCoreDataAdapterToolboxItem, FwNs.Core.Designer, Version=1.5.0.0, Culture=neutral, PublicKeyToken=9c147f7358eea142")]
    public sealed class UtlDataAdapter : DbDataAdapter
    {
        private static readonly object UpdatingEventPh = new object();
        private static readonly object UpdatedEventPh = new object();

        public event EventHandler<RowUpdatedEventArgs> RowUpdated
        {
            add
            {
                base.Events.AddHandler(UpdatedEventPh, value);
            }
            remove
            {
                base.Events.RemoveHandler(UpdatedEventPh, value);
            }
        }

        public event EventHandler<RowUpdatingEventArgs> RowUpdating
        {
            add
            {
                EventHandler<RowUpdatingEventArgs> mcd = (EventHandler<RowUpdatingEventArgs>) base.Events[UpdatingEventPh];
                if ((mcd != null) && (value.Target is DbCommandBuilder))
                {
                    EventHandler<RowUpdatingEventArgs> handler2 = (EventHandler<RowUpdatingEventArgs>) FindBuilder(mcd);
                    if (handler2 != null)
                    {
                        base.Events.RemoveHandler(UpdatingEventPh, handler2);
                    }
                }
                base.Events.AddHandler(UpdatingEventPh, value);
            }
            remove
            {
                base.Events.RemoveHandler(UpdatingEventPh, value);
            }
        }

        public UtlDataAdapter()
        {
        }

        public UtlDataAdapter(UtlCommand cmd)
        {
            this.SelectCommand = cmd;
        }

        public UtlDataAdapter(string commandText, UtlConnection connection)
        {
            this.SelectCommand = new UtlCommand(commandText, connection);
        }

        public UtlDataAdapter(string commandText, string connectionString)
        {
            UtlConnection connection = new UtlConnection(connectionString);
            this.SelectCommand = new UtlCommand(commandText, connection);
        }

        public static Delegate FindBuilder(MulticastDelegate mcd)
        {
            if (mcd != null)
            {
                Delegate[] invocationList = mcd.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    if (invocationList[i].Target is DbCommandBuilder)
                    {
                        return invocationList[i];
                    }
                }
            }
            return null;
        }

        protected override void OnRowUpdated(RowUpdatedEventArgs value)
        {
            EventHandler<RowUpdatedEventArgs> handler = base.Events[UpdatedEventPh] as EventHandler<RowUpdatedEventArgs>;
            if (handler != null)
            {
                handler(this, value);
            }
        }

        protected override void OnRowUpdating(RowUpdatingEventArgs value)
        {
            EventHandler<RowUpdatingEventArgs> handler = base.Events[UpdatingEventPh] as EventHandler<RowUpdatingEventArgs>;
            if (handler != null)
            {
                handler(this, value);
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public UtlCommand SelectCommand
        {
            get
            {
                return (UtlCommand) base.SelectCommand;
            }
            set
            {
                base.SelectCommand = value;
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public UtlCommand InsertCommand
        {
            get
            {
                return (UtlCommand) base.InsertCommand;
            }
            set
            {
                base.InsertCommand = value;
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public UtlCommand UpdateCommand
        {
            get
            {
                return (UtlCommand) base.UpdateCommand;
            }
            set
            {
                base.UpdateCommand = value;
            }
        }

        [DefaultValue((string) null), Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public UtlCommand DeleteCommand
        {
            get
            {
                return (UtlCommand) base.DeleteCommand;
            }
            set
            {
                base.DeleteCommand = value;
            }
        }
    }
}

