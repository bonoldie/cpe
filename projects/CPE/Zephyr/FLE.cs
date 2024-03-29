using System.Runtime.InteropServices;
using System.Reflection;

namespace CPE.Zephyr
{
    public class FLE_DLL
    {

        [DllImport("FlowEngineFree")]
        unsafe public static extern void* CreateSparsePointCloud();

/*          public static void printInfo()
        {
            Type[] types = DLLAssembly.GetTypes();
            foreach (Type type in types)
            {
                if (!type.IsPublic)
                {
                    continue;
                }

                MemberInfo[] members = type.GetMethods();
                foreach (MemberInfo member in members)
                {
                    Console.WriteLine(type.Name + "." + member.Name);
                }
            }
        } */
    }
}