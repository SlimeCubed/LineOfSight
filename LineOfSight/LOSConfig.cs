using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using OptionalUI;
using Partiality.Modloader;
using UnityEngine;

namespace LineOfSight
{
    public partial class LineOfSightMod
    {
        public OptionInterface LoadOI()
        {
            return (OptionInterface)Activator.CreateInstance(GetOIType(), new object[] { this });
        }

        // Create a class inheriting from OptionInterface that redirects some methods to LOSConfig instead
        private static Type _oiType;
        private static Type GetOIType()
        {
            if (_oiType != null) return _oiType;

            AssemblyName name = new AssemblyName("LOSConfigProxy.dll");

            // Define a dynamic assembly and module
            AssemblyBuilder asm = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder module = asm.DefineDynamicModule("LOSConfigProxy");

            // Define a class to inherit from OptionInterface
            TypeBuilder proxy = module.DefineType("LOSConfigProxy", TypeAttributes.Class, typeof(OptionInterface));

            // FIELD DEFINITIONS
            // data (LOSConfigData)
            FieldBuilder configData = proxy.DefineField("configData", typeof(LOSConfigData), FieldAttributes.Private);

            // METHOD DEFINITIONS
            // ctor (PartialityMod) => LOSConfig.Ctor (OptionInterface)
            {
                ConstructorBuilder cb = proxy.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, new Type[] { typeof(PartialityMod) });
                ILGenerator ilg = cb.GetILGenerator();

                ConstructorInfo baseCtor = typeof(OptionInterface).GetConstructor(new Type[] { typeof(PartialityMod) });

                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Call, baseCtor); // Call base.ctor
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Newobj, typeof(LOSConfigData).GetConstructor(Type.EmptyTypes)); // Assign a new instance to configData
                ilg.Emit(OpCodes.Stfld, configData);
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Call, typeof(LOSConfig).GetMethod("Ctor")); // Call proxy
                ilg.Emit(OpCodes.Ret);
            }

            // Initialize () => LOSConfig.Initialize (OptionInterface)
            {
                MethodBuilder mb = proxy.DefineMethod("Initialize", MethodAttributes.Public | MethodAttributes.Virtual);
                ILGenerator ilg = mb.GetILGenerator();

                MethodInfo baseInitialize = typeof(OptionInterface).GetMethod("Initialize", BindingFlags.Public | BindingFlags.Instance);

                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Call, baseInitialize); // Call base.Initialize
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Call, typeof(LOSConfig).GetMethod("Initialize")); // Call proxy
                ilg.Emit(OpCodes.Ret);
            }

            // ConfigOnChange () => LOSConfig.ConfigOnChange (OptionInterface)
            {
                MethodBuilder mb = proxy.DefineMethod("ConfigOnChange", MethodAttributes.Public | MethodAttributes.Virtual);
                ILGenerator ilg = mb.GetILGenerator();

                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Call, typeof(LOSConfig).GetMethod("ConfigOnChange")); // Call proxy
                ilg.Emit(OpCodes.Ret);
            }

            // Update (float) => LOSConfig.Update (OptionInterface, float)
            {
                MethodBuilder mb = proxy.DefineMethod("Update", MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { typeof(float) });
                ILGenerator ilg = mb.GetILGenerator();

                MethodInfo baseUpdate = typeof(OptionInterface).GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);

                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Call, baseUpdate); // Call base.Update
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldarg_1);
                ilg.Emit(OpCodes.Call, typeof(LOSConfig).GetMethod("Update")); // Call proxy
                ilg.Emit(OpCodes.Ret);
            }

            // GetConfigData (OptionInterface)
            {
                MethodBuilder mb = proxy.DefineMethod("GetConfigData", MethodAttributes.Assembly | MethodAttributes.Static,
                    typeof(LOSConfigData), new Type[] { typeof(OptionInterface) });
                ILGenerator ilg = mb.GetILGenerator();

                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Ldfld, configData);
                ilg.Emit(OpCodes.Ret);
            }

            _oiType = proxy.CreateType();
            return _oiType;
        }
    }

    public static class LOSConfig
    {
        private static Func<OptionInterface, LOSConfigData> getConfigData;

        public static void Ctor(OptionInterface oi)
        {
            
        }

        public static void Initialize(OptionInterface oi)
        {
            MethodInfo m = oi.GetType().GetMethod("GetConfigData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            getConfigData = Delegate.CreateDelegate(typeof(Func<OptionInterface, LOSConfigData>), m) as Func<OptionInterface, LOSConfigData>;
            LOSConfigData data = getConfigData(oi);

            oi.Tabs = new OpTab[1];
            oi.Tabs[0] = new OpTab("Config");
            oi.Tabs[0].AddItems(
                data.modeName = new OpLabel(new Vector2(250f, 310f), new Vector2(100f, 30f), string.Empty),
                data.toggle = new OpCheckBox(new Vector2(300f - 12f, 360f - 22f), "toggleLOSClassic"),
                data.modeDesc = new OpLabelLong(new Vector2(300f - 150f, 310f - 200f), new Vector2(300f, 200f), string.Empty, true, FLabelAlignment.Center) { verticalAlignment = OpLabel.LabelVAlignment.Top }
            );

            UpdateTexts(oi, true);
        }

        public static void ConfigOnChange(OptionInterface oi)
        {
            LineOfSightMod.classic = OptionInterface.config["toggleLOSClassic"] == "true";

            UpdateTexts(oi, true);
        }

        public static void Update(OptionInterface oi, float dt)
        {
            UpdateTexts(oi);
        }

        public static void UpdateTexts(OptionInterface oi, bool force = false)
        {

            LOSConfigData data = getConfigData(oi);
            if (data.toggle.valueBool == data.isClassic && !force) return;
            if (data.toggle.valueBool)
            {
                data.isClassic = true;
                data.modeName.text = "Classic Mode";
                data.modeDesc.text = "Everything, including level geometry, is hidden when outside " +
                                     "of the player's field of view. Looks worse and requires more " +
                                     "memorization than Standard Mode, so play at your own risk.";
            }
            else
            {
                data.isClassic = false;
                data.modeName.text = "Standard Mode";
                data.modeDesc.text = "Creatures and items outside of the player's field of view are " +
                                     "hidden, but the level is still be partially visible. Shortcut " +
                                     "exits flash when the player is near the entrance.";
            }
        }
    }

    public class LOSConfigData
    {
        public bool isClassic;
        public OpLabel modeName;
        public OpCheckBox toggle;
        public OpLabelLong modeDesc;
    }
}
