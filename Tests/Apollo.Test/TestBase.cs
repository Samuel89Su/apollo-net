using NMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo.Test
{
    public abstract class TestBase
    {
        protected MockFactory mockFactory = new MockFactory();
    }
}
