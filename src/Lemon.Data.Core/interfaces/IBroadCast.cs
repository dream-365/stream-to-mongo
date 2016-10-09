﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemon.Data.Core
{
    public interface IBroadCast
    {
        void AddChild(Node node);

        IEnumerable<Node> ChildrenNodes { get; }
    }
}