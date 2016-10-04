﻿using System.Collections.Generic;
using Lemon.Transform;
using System.Threading.Tasks;

namespace Demo001
{
    public class ListDataInput : AbstractDataInput
    {
        private IEnumerable<BsonDataRow> _rows;

        public ListDataInput(IEnumerable<BsonDataRow> rows)
        {
            _rows = rows;
        }

        public override async Task StartAsync(IDictionary<string, object> parameters = null)
        {
            foreach(var row in _rows)
            {
                await SendAsync(row);
            }

            Complete();
        }
    }
}
