using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GDC.PH.AIDE.ServiceFactory
{
    /// <summary>
    /// Singleton factory class for WCF Services.
    /// Creates service clients based on interface type and automatically
    /// resets faulted channels.
    /// </summary>
    public class ServiceClientFactory
    {
        private readonly Dictionary<Type, object> _factories;
        private static ServiceClientFactory _instance;

        private ServiceClientFactory()
        {
            _factories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Retrieves a service client for the interface specified in generic parameter.
        /// </summary>
        /// <typeparam name="T">Interface type to use for Service Client creation.</typeparam>
        /// <returns>Service client instance for specified interface.</returns>
        public T GetClient<T>()
        {
            var genericType = typeof(T);
            Type serviceClientType;
            if (genericType.IsInterface)
            {
                serviceClientType = GetClientType(genericType);

                if (serviceClientType == null)
                    return default(T);

                var client = Activator.CreateInstance(serviceClientType);
                if (!(client is ICommunicationObject))
                {
                    client = null;
                    return (T)client;

                }
                (client as ICommunicationObject).Faulted += Channel_Faulted<T>;

                if (!_factories.ContainsKey(typeof(T)))
                {
                    var prop = serviceClientType.GetProperty("ChannelFactory");
                    var factory = prop.GetValue(client, null);
                    _factories.Add(typeof(T), factory);
                }
                return (T)client;
            }
            return default(T);
        }

        #region Reflection Utilities
        /// <summary>
        /// Retrieves a service client for the interface specified in generic parameter.
        /// </summary>
        /// <typeparam name="T">Interface type to use for Service Client creation.</typeparam>
        /// <returns>Service client instance for specified interface.</returns>
        private static Type GetClientType(Type type)
        {
            var assy = type.Assembly;
            var serviceModelAssy = typeof(ChannelFactory).Assembly;
            var clientBaseType = serviceModelAssy.GetType("System.ServiceModel.ClientBase`1").MakeGenericType(type);

            foreach (var _classType in assy.GetTypes())
            {
                if (_classType.IsClass && type.IsAssignableFrom(_classType))
                {
                    if (_classType.IsSubclassOf(clientBaseType))
                        return _classType;
                }
            }

            return null;
        }
        /// <summary>
        /// Retrieves a service client for the interface specified in generic parameter.
        /// </summary>
        /// <typeparam name="T">Interface type to use for Service Client creation.</typeparam>
        /// <typeparam name="params object[] paramArray">parameters for the constructor.</typeparam>
        /// <returns>Service client instance for specified interface.</returns>
        public T GetClient<T>(params object[] paramArray)
        {
            var genericType = typeof(T);
            Type serviceClientType;
            if (genericType.IsInterface)
            {
                serviceClientType = GetClientType(genericType);

                if (serviceClientType == null)
                    return default(T);

                var client = Activator.CreateInstance(serviceClientType, args: paramArray);
                if (!(client is ICommunicationObject))
                {
                    client = null;
                    return (T)client;

                }
                (client as ICommunicationObject).Faulted += Channel_Faulted<T>;

                if (!_factories.ContainsKey(typeof(T)))
                {
                    var prop = serviceClientType.GetProperty("ChannelFactory");
                    var factory = prop.GetValue(client, null);
                    _factories.Add(typeof(T), factory);
                }
                return (T)client;
            }
            return default(T);
        }


        #endregion

        /// <summary>
        /// Event handler for ClientBase.Faulted event.
        /// </summary>
        /// <typeparam name="T">Interface type of service</typeparam>
        /// <param name="sender">ClientBase instance</param>
        /// <param name="e">Event Args</param>
        private void Channel_Faulted<T>(object sender, EventArgs e)
        {
            ((ICommunicationObject)sender).Abort();
            var factory = (ChannelFactory<T>)_factories[typeof(T)];
            factory.CreateChannel();
        }

        /// <summary>
        /// Returns the singleton instance of ServiceClientFactory.
        /// </summary>
        /// <returns>Singleton instance of ServiceClientFactory</returns>
        public static ServiceClientFactory GetFactory()
        {
            if (_instance == null)
            {
                _instance = new ServiceClientFactory();
            }
            return _instance;
        }
    }
}
