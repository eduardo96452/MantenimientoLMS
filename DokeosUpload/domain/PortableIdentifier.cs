using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lmsda.domain
{
    class PortableIdentifier
    {
        private static PortableIdentifier _instance;
        private static object syncLock = new object();

        private Boolean _isPortable;

        public Boolean isPortable { get { return _isPortable; } }

        protected PortableIdentifier(Boolean isPortable)
        {
            this._isPortable = isPortable;
        }

        /// <summary>
        ///     Singleton Pattern.
        ///     Double Checked Locking Pattern. (support for multithreaded programs.)
        ///     Returns the object of this class.
        /// </summary>
        /// <returns>The object of the DomainController class.</returns>
        /// 
        public static PortableIdentifier Instance()
        {
            return Instance(false);
        }

        private static PortableIdentifier Instance(Boolean isPortable)
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new PortableIdentifier(isPortable);
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        ///     This only works once before Instance() is ever called.
        /// </summary>
        /// <returns></returns>
        public static PortableIdentifier CreatePortableInstance()
        {
            return Instance(true);
        }

    }
}
