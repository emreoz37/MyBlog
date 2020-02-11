using System.Runtime.CompilerServices;

namespace Core.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the startup engine.
    /// </summary>
    public class StartupEngineContext
    {
        #region Methods

        /// <summary>
        /// Create a static instance of the startup engine.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IStartupEngine Create()
        {
            //create StartupEngine as engine
            return Singleton<IStartupEngine>.Instance ?? (Singleton<IStartupEngine>.Instance = new StartupEngine());
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IStartupEngine engine)
        {
            Singleton<IStartupEngine>.Instance = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton Startup engine used to access project services.
        /// </summary>
        public static IStartupEngine Current
        {
            get
            {
                if (Singleton<IStartupEngine>.Instance == null)
                {
                    Create();
                }

                return Singleton<IStartupEngine>.Instance;
            }
        }

        #endregion
    }
}
