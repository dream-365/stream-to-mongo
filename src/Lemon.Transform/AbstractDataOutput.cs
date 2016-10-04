﻿using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Lemon.Transform.Models;

namespace Lemon.Transform
{
    public abstract class AbstractDataOutput : LinkObject
    {
        private ActionBlock<DataRowTransformWrapper<BsonDataRow>> _actionBlock;

        public Action<BsonDataRow> OnError;

        public Action<BsonDataRow> BeforeWrite;

        public Action<BsonDataRow> AfterWrite;

        protected Func<BsonDataRow, bool> DetermineWriteOrNot;

        public AbstractDataOutput()
        {
            _actionBlock = new ActionBlock<DataRowTransformWrapper<BsonDataRow>>(new Action<DataRowTransformWrapper<BsonDataRow>>(OnReceive), new ExecutionDataflowBlockOptions {
                BoundedCapacity = GlobalConfiguration.TransformConfiguration.BoundedCapacity ?? ExecutionDataflowBlockOptions.Unbounded
            });

            BeforeWrite = Dummy;

            AfterWrite = Dummy;

            DetermineWriteOrNot = (row) => { return true; };
        }

        private static void Dummy(BsonDataRow row) { }

        protected Func<BsonDataRow, bool> BuildDetermineWriteOrNotFunction(WriteOnChangeConfiguration writeOnChange)
        {
            Func<BsonDataRow, bool> func = (row) => { return true; };

            if (writeOnChange != null && writeOnChange.Enabled)
            {
                var context = BuildDataRowStatusContext(writeOnChange.ExcludedColumNames);

                func = (row) => {

                    var status = context.Compare(row);

                    LogService.Default.Info(string.Format("status: {0}", status));

                    return status != DataRowCompareStatus.NoChange;
                };
            }

            return func;
        }

        protected virtual DataRowStatusContext BuildDataRowStatusContext(string[] excludes)
        {
            throw new NotSupportedException();
        }

        internal override ISourceBlock<DataRowTransformWrapper<BsonDataRow>> AsSource()
        {
            throw new NotSupportedException();
        }

        internal override ITargetBlock<DataRowTransformWrapper<BsonDataRow>> AsTarget()
        {
            return _actionBlock as ITargetBlock<DataRowTransformWrapper<BsonDataRow>>;
        }

        public override Task Compltetion
        {
            get
            {
                return _actionBlock.Completion;
            }
        }

        protected void OnReceive(DataRowTransformWrapper<BsonDataRow> data)
        {
            Context.ProgressIndicator.Increment(string.Format("{0}.process", Name));

            try
            {
                if(DetermineWriteOrNot(data.Row))
                {
                    BeforeWrite(data.Row);

                    OnReceive(data.Row);

                    Context.ProgressIndicator.Increment(string.Format("{0}.output", Name));

                    AfterWrite(data.Row);
                }
                else
                {
                    Context.ProgressIndicator.Increment(string.Format("{0}.nochange", Name));
                }  
            }
            catch (Exception ex)
            {
                OnError(data.Row);

                Context.ProgressIndicator.Increment(string.Format("{0}.error", Name));

                LogService.Default.Error(string.Format("{0} - failed", Name), ex);
            }
        }

        protected abstract void OnReceive(BsonDataRow row);
    }
}
