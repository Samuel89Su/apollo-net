using NMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apollo.Test
{
    public class NMockFactory<T> where T : class
    {
        public virtual Mock<T> MockObject { get; protected set; }

        public virtual T Create()
        {
            var _factory = new MockFactory();
            MockObject = _factory.CreateMock<T>();

            return MockObject.MockObject;
        }
    }
}
