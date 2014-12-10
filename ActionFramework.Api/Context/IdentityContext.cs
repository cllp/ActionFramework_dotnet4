using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Api.Context
{
    public class IdentityContext : IDisposable
    {
        private static IIdentity identity;

        public static void Initialize(IIdentity identityProvider)
        {
            identity = identityProvider;
        }

        /// <summary>
        /// Returns a identity context that is applicable for this context.
        /// </summary>
        public static IIdentity Current
        {
            get
            {
                EnsureInitialized();
                return identity;
            }
        }

        public static bool IsInitialized
        {
            get { return identity != null; }
        }

        private static void EnsureInitialized()
        {
            if (identity == null)
            {
                throw new InvalidOperationException("Identity provider has not been initialized.");
            }
        }

        public void Dispose()
        {
            identity = null;
        }
    }
}