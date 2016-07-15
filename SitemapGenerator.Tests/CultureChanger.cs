using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SitemapGenerator.Tests
{

    public class CultureChanger : IDisposable
    {
        private CultureInfo _oldCulture;

        public CultureChanger(string newCultureString) : this(new CultureInfo(newCultureString))
        {
        }

        public CultureChanger(CultureInfo newCulture)
        {
            _oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = _oldCulture;
            Thread.CurrentThread.CurrentUICulture = _oldCulture;
        }
    }

}
