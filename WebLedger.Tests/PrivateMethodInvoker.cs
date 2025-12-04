using System;
using System.Reflection;

namespace WebLedger.Tests
{
    /// <summary>
    /// 用于通过反射调用私有方法的工具类
    /// </summary>
    public static class PrivateMethodInvoker
    {
        /// <summary>
        /// 调用对象的私有实例方法
        /// </summary>
        public static T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var method = type.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (method == null)
                throw new ArgumentException($"Method '{methodName}' not found in type {type.Name}");

            try
            {
                return (T)method.Invoke(instance, parameters);
            }
            catch (TargetInvocationException ex)
            {
                // 重新抛出原始异常
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// 调用类型的私有静态方法
        /// </summary>
        public static T InvokePrivateStaticMethod<T>(Type type, string methodName, params object[] parameters)
        {
            var method = type.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
                throw new ArgumentException($"Static method '{methodName}' not found in type {type.Name}");

            try
            {
                // 这里需要正确处理参数数组
                return (T)method.Invoke(null, parameters);
            }
            catch (TargetInvocationException ex)
            {
                // 重新抛出原始异常
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// 调用类型的私有静态方法（修复参数传递问题）
        /// </summary>
        public static T InvokePrivateStaticMethod<T>(Type type, string methodName, object parameter)
        {
            var method = type.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
                throw new ArgumentException($"Static method '{methodName}' not found in type {type.Name}");

            try
            {
                // 确保参数正确传递
                return (T)method.Invoke(null, new object[] { parameter });
            }
            catch (TargetInvocationException ex)
            {
                // 重新抛出原始异常
                throw ex.InnerException ?? ex;
            }
        }

        /// <summary>
        /// 获取私有字段的值
        /// </summary>
        public static T GetPrivateField<T>(object instance, string fieldName)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var field = type.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
                throw new ArgumentException($"Field '{fieldName}' not found in type {type.Name}");

            return (T)field.GetValue(instance);
        }

        /// <summary>
        /// 设置私有字段的值
        /// </summary>
        public static void SetPrivateField(object instance, string fieldName, object value)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var field = type.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
                throw new ArgumentException($"Field '{fieldName}' not found in type {type.Name}");

            field.SetValue(instance, value);
        }

        /// <summary>
        /// 调用泛型私有方法
        /// </summary>
        public static T InvokePrivateGenericMethod<T>(object instance, string methodName, Type[] genericTypes, params object[] parameters)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var method = type.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                throw new ArgumentException($"Method '{methodName}' not found in type {type.Name}");

            var genericMethod = method.MakeGenericMethod(genericTypes);

            try
            {
                return (T)genericMethod.Invoke(instance, parameters);
            }
            catch (TargetInvocationException ex)
            {
                // 重新抛出原始异常
                throw ex.InnerException ?? ex;
            }
        }
    }
}