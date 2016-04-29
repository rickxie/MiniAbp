using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniAbp.Dependency
{
    /// <summary>
    /// Used to pass needed objects on conventional registration process.
    /// </summary>
    public interface IConventionalRegistrationContext
    {
        /// <summary>
        /// Gets the registering Assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Reference to the IOC Container to register types.
        /// </summary>
        IocManager IocManager { get; }

        ///// <summary>
        ///// Registration configuration.
        ///// </summary>
        ConventionalRegistrationConfig Config { get; }
    }
}
