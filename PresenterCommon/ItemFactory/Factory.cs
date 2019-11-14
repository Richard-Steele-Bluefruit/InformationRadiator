using System;
using System.Collections.Generic;
using System.Linq;

namespace PresenterCommon.ItemFactory
{
    public class Factory : IItemFactory
    {
        private Dictionary<string, Type> _possibleTypes;

        private Dictionary<Type, object> _possibleParameters;

        public Factory()
        {
            _possibleParameters = new Dictionary<Type, object>();
            _possibleTypes = new Dictionary<string, Type>();
        }

        public void AddItemType(string itemType, Type type)
        {
            _possibleTypes.Add(itemType, type);
        }

        private System.Reflection.ConstructorInfo FindConstructor(Type presenterType)
        {
            System.Reflection.ConstructorInfo info = null;
            var constructors = presenterType.GetConstructors();
            if (constructors.Length == 1)
                return constructors[0];

            foreach (var constructor in constructors)
            {
                var attributes = constructor.GetCustomAttributes(
                    typeof(ItemFactoryConstructorAttribute), false);

                if (attributes.Length > 0)
                {
                    info = constructor;
                    break;
                }
            }

            if (info == null)
                throw new UnableToDetermineConstructorException();

            return info;
        }

        private object[] CreateParameters(System.Reflection.ParameterInfo[] constructorParameters,
                                          object[] specificParameters)
        {
            var parameters = new object[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                object parameterValue = specificParameters.FirstOrDefault(
                    o => constructorParameters[i].ParameterType.IsInstanceOfType(o)
                    );

                if (parameterValue == null && !_possibleParameters.TryGetValue(
                    constructorParameters[i].ParameterType, out parameterValue))
                {
                    throw new UnknownParameterTypeException();
                }

                parameters[i] = parameterValue;
            }

            return parameters;
        }

        public object CreateObject(string itemType, params object[] specificParameters)
        {
            Type requiredObjectType;
            if (!_possibleTypes.TryGetValue(itemType, out requiredObjectType))
                throw new UnknownItemTypeException();

            var constructor = FindConstructor(requiredObjectType);
            var constructorParameters = constructor.GetParameters();
            object[] parameters = null;

            if (constructorParameters.Length > 0)
                parameters = CreateParameters(constructorParameters, specificParameters);

            return Activator.CreateInstance(requiredObjectType, parameters);
        }

        public void AddParameter(Type type, object parameter)
        {
            _possibleParameters.Add(type, parameter);
        }
    }
}
