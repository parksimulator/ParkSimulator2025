using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal abstract class Resources
    {
        internal abstract void Initialize();
        internal abstract void Finish();

        internal abstract bool ExistsResource(string id);
        internal abstract byte[] GetBinaryResource(string id);
        internal abstract string GetTextResource(string id, Encoding encoding);


    }
}
