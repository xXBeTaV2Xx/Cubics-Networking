using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubicsGamesNetworking
{
    [ProtoContract]
    public class ProtoTestClass
    {
        protected ProtoTestClass() { }

        public ProtoTestClass(string name, ulong uid)
        {
            this.Name = name;
            this.UID = uid;
        }

        [ProtoMember(1)]
        public string Name { get; private set; }

        [ProtoMember(2)]
        public ulong UID { get; private set; }
    }
}
