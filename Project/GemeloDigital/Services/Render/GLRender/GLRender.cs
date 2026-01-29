using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal class GLRender : Render
    {
        internal override void Initialize()
        {
            Console.WriteLine("GLRender: Initializing");

        }

        internal override void Finish()
        {
            Console.WriteLine("GLRender: Finish");
        }

    }
}
