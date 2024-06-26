﻿using BSP.BL.Geometries;
using BSP.Geometries.SDK;

namespace BSP.BL.Services
{
    public class GeometryService
    {
        private static Dictionary<Type, string> availableGeometries = new Dictionary<Type, string>()
        {
            {typeof(CylinderRadial), "Cylinder Radial" },
            {typeof(CylinderAxial), "Cylinder Axial" },
            {typeof(Parallelepiped), "Parallelepiped" },
            {typeof(PointGeometry), "Point" },
        };

        public static Dictionary<Type, string> Geometries => availableGeometries;


        public static IGeometry GetGeometryInstance(Type geometryType)
        {
            return Activator.CreateInstance(geometryType) as IGeometry;
        }

        public static IEnumerable<DimensionsInfo> GetDimensionsInfo(Type geometryType)
        {
            if (geometryType == typeof(CylinderRadial) || geometryType == typeof(CylinderAxial))
            {
                return CylinderForm.GetDimensionsInfo();
            }
            if (geometryType == typeof(Parallelepiped))
            {
                return ParallelepipedForm.GetDimensionsInfo();
            }
            return EmptyForm.GetDimensionsInfo();
        }

        /// <summary>
        /// Возвращает слагаемое для расчета толщины воздушного зазора между источником и точкой измерения
        /// </summary>
        /// <param name="geometryType"></param>
        /// <param name="dimensions"></param>
        /// <returns></returns>
        public static float GetSubstractionTermForAirgapCalculation(Type geometryType, float[] dimensions)
        {
            return geometryType switch
            {
                Type e when e == typeof(CylinderRadial) => dimensions[0],
                Type e when e == typeof(CylinderAxial) => dimensions[1],
                Type e when e == typeof(Parallelepiped) => dimensions[0],
                Type e when e == typeof(PointGeometry) => 0,
                _ => 0
            };
        }

        /// <summary>
        /// Возвращает ключ словаря, в котором содержится название для текущей локализации приложения
        /// </summary>
        /// <param name="geometryType"></param>
        /// <returns></returns>
        public static string GetTranslationKey(Type geometryType)
        {
            return geometryType switch
            {
                Type e when e == typeof(CylinderRadial) => "SourceFormCylinderRadial",
                Type e when e == typeof(CylinderAxial) => "SourceFormCylinderAxial",
                Type e when e == typeof(Parallelepiped) => "SourceFormParallelepiped",
                Type e when e == typeof(PointGeometry) => "SourceFormPoint",
                _ => null
            };
        }
    }
}
