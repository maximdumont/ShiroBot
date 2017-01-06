// using System;

// namespace ShiroBot
// {
//     /// <summary>
//     /// The interface class through which patched DLLs interact with ShiroBot
//     /// </summary>
//     public static class Interface
//     {
//         /// <summary>
//         /// Gets the main ShiroBot instance
//         /// </summary>
//         public static ShiroBot ShiroBot { get; private set; }

//         /// <summary>
//         /// Gets or sets the debug callback to use
//         /// </summary>
//         public static NativeDebugCallback DebugCallback { get; set; }

//         /// <summary>
//         /// Initializes ShiroBot
//         /// </summary>
//         public static void Initialize()
//         {
//             // Create if not already created
//             if (ShiroBot != null) return;
//             ShiroBot = new ShiroBot(DebugCallback);
//             ShiroBot.Load();
//         }

//         /// <summary>
//         /// Calls the specified deprecated hook
//         /// </summary>
//         /// <param name="oldHook"></param>
//         /// <param name="newHook"></param>
//         /// <param name="expireDate"></param>
//         /// <param name="args"></param>
//         /// <returns></returns>
//         public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
//         {
//             return ShiroBot.CallDeprecatedHook(oldHook, newHook, expireDate, args);
//         }

//         /// <summary>
//         /// Calls the specified hook
//         /// </summary>
//         /// <param name="hookname"></param>
//         /// <param name="args"></param>
//         /// <returns></returns>
//         public static object CallHook(string hookname, params object[] args) => ShiroBot?.CallHook(hookname, args);

//         /// <summary>
//         /// Calls the specified hook
//         /// </summary>
//         /// <param name="hookname"></param>
//         /// <param name="args"></param>
//         /// <returns></returns>
//         public static object Call(string hookname, params object[] args) => CallHook(hookname, args);

//         /// <summary>
//         /// Calls the specified hook and converts the return value to the specified type
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <param name="hookname"></param>
//         /// <param name="args"></param>
//         /// <returns></returns>
//         public static T Call<T>(string hookname, params object[] args) => (T)Convert.ChangeType(CallHook(hookname, args), typeof(T));

//         /// <summary>
//         /// Gets the ShiroBot mod
//         /// </summary>
//         /// <returns></returns>
//         public static ShiroBot GetMod() => ShiroBot;
//     }
// }