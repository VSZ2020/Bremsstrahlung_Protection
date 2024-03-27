﻿using BSP.BL.Buildups.Common;
using BSP.BL.Services;
using System.Numerics;
using System.Text;
using BSP.Geometries.SDK;

namespace BSP.BL.Calculation
{
    public class InputDataBuilder
    {
        public InputDataBuilder(MaterialsService materialsService, BuildupService buildupService, DoseFactorsService doseFactorsService)
        {
            this.materialsService = materialsService;
            this.buildupService = buildupService;
            this.doseFactorsService = doseFactorsService;

            inputClass = new InputData();
        }
        
        private readonly MaterialsService materialsService;
        private readonly BuildupService buildupService;
        private readonly DoseFactorsService doseFactorsService;

        private InputData inputClass;

        //public double[] Energies;
        //public double[] massEnvironmentAbsorptionFactors;
        //public double[][] massAttenuationFactors;
        //public double[][][] BuildupFactors;

        //public List<ShieldLayer> Layers = new List<ShieldLayer>();

        //public float SourceDensity = 0;

        //public double SourceActivity = 0;
        
        ///// <summary>
        ///// Флаг учета самопоглощения в материале источника
        ///// </summary>
        //public bool IsSelfAbsorptionAllowed = true;

        ///// <summary>
        ///// Расстояние от точечного источника до точки регистрации излучения
        ///// </summary>
        //public Vector3 CalculationPoint;

        ///// <summary>
        ///// Рассчитанные потоки энергий тормозного излучения [МэВ/с]
        ///// </summary>
        //public double[] BremsstrahlungEnergyFluxes;

        ///// <summary>
        ///// Класс, содержащий метод расчета фактора накопления для гетерогенной защиты. Внутри него хранится ссылка на метод расчета фактора накопления для гомогенной защиты
        ///// </summary>
        //public BaseHeterogeneousBuildup BuildupProcessor;

        //public CancellationToken CancellationToken;

        //public IProgress<double> Progress;
        

        public InputDataBuilder WithDimensions(float[] dims, int[] discreteness)
        {
            inputClass.Dimensions = dims;
            inputClass.Discreteness = discreteness;
            return this; 
        }
        
        public InputDataBuilder WithEnergies(double[] energies)
        {
            //this.Energies = energies;
            inputClass.Energies = energies;
            return this; 
        }

        public InputDataBuilder WithBremsstrahlungEnergyFluxes(double[] fluxes)
        {
            //this.BremsstrahlungEnergyFluxes = fluxes;
            inputClass.PhotonsFluxes = fluxes;
            return this;
        }

        public InputDataBuilder WithEnvironmentAbsorptionFactors(double[] energies, int environmentMaterialId)
        {
            //this.massEnvironmentAbsorptionFactors = materialsService.GetMassAbsorptionFactors(environmentMaterialId, energies);
            inputClass.massEnvironmentAbsorptionFactors = materialsService.GetMassAbsorptionFactors(environmentMaterialId, energies);
            return this;
        }

        public InputDataBuilder WithAttenuationFactors(int sourceMaterialId, int[] shieldMaterialsIds, double[] energies)
        {
            //this.massAttenuationFactors = materialsService.GetMassAttenuationFactors(CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            var massAttenuationFactors = materialsService.GetMassAttenuationFactors(CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            inputClass.massAttenuationFactors = massAttenuationFactors;
            return this;
        }

        public InputDataBuilder WithBuildup(Type? homogeneousBuildup, Type? heterogeneousBuildup, int sourceMaterialId, int[] shieldMaterialsIds, double[] energies)
        {
            if (homogeneousBuildup != null && heterogeneousBuildup != null)
            {
                //this.BuildupProcessor = BuildupService.GetHeterogeneousBuildupInstance(heterogeneousBuildup, homogeneousBuildup);
                //this.BuildupFactors = buildupService.GetInterpolatedBuildupFactors(homogeneousBuildup, CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
                inputClass.BuildupProcessor = BuildupService.GetHeterogeneousBuildupInstance(heterogeneousBuildup, homogeneousBuildup);
                inputClass.BuildupFactors = buildupService.GetInterpolatedBuildupFactors(homogeneousBuildup, CombineIds(sourceMaterialId, shieldMaterialsIds), energies);
            }
            return this;
        }

        public InputDataBuilder WithShieldLayers(List<ShieldLayer> layers)
        {
            //this.Layers = layers;
            inputClass.Layers = layers;
            return this;
        }

        public InputDataBuilder WithCalculationPoint(Vector3 vector)
        {
            //this.CalculationPoint = vector;
            inputClass.CalculationPoint = vector;
            return this;
        }

        public InputDataBuilder WithCalculationPoint(float X, float Y, float Z)
        {
            //this.CalculationPoint = new Vector3(X, Y, Z);
            inputClass.CalculationPoint = new Vector3(X, Y, Z);
            return this;
        }

        public InputDataBuilder WithCancellationToken(CancellationToken token)
        {
            //this.CancellationToken = token;
            inputClass.CancellationToken = token;
            return this;
        }


        public InputDataBuilder WithSourceDensity(float density)
        {
            //this.SourceDensity = density;
            inputClass.SourceDensity = density;
            return this;
        }
        
        public InputDataBuilder WithSourceActivity(double activity)
        {
            //this.SourceActivity = activity;
            inputClass.SourceActivity = activity;
            return this;
        }

        public InputDataBuilder WithProgress(IProgress<double> prg)
        {
            //this.Progress = prg;
            inputClass.Progress = prg;
            return this;
        }

        public InputDataBuilder WithSelfabsorption(bool isSelfAbsorptionAllowed)
        {
            //this.IsSelfAbsorptionAllowed = isSelfAbsorptionAllowed;
            inputClass.IsSelfAbsorptionAllowed = isSelfAbsorptionAllowed; 
            return this;
        }

        public InputData Build()
        {
            return new InputData()
            {
                Dimensions = inputClass.Dimensions,
                Discreteness = inputClass.Discreteness,
                Energies = inputClass.Energies,
                massEnvironmentAbsorptionFactors = inputClass.massEnvironmentAbsorptionFactors,
                massAttenuationFactors = inputClass.massAttenuationFactors,
                PhotonsFluxes = inputClass.PhotonsFluxes,
                BuildupFactors = inputClass.BuildupFactors,
                BuildupProcessor = inputClass.BuildupProcessor,
                CalculationPoint = inputClass.CalculationPoint,
                CancellationToken = inputClass.CancellationToken,
                SourceDensity = inputClass.SourceDensity,
                SourceActivity = inputClass.SourceActivity,
                Layers = inputClass.Layers ?? new List<ShieldLayer>(),
                Progress = inputClass.Progress,
                IsSelfAbsorptionAllowed = inputClass.IsSelfAbsorptionAllowed
            };
        }

        private int[] CombineIds(int sourceId, int[] layersIds)
        {
            var ids = new int[layersIds.Length + 1];
            ids[0] = sourceId;
            for (var i = 0; i < layersIds.Length; i++)
                ids[i + 1] = layersIds[i];
            return ids;
        }

        public string ExportToString(double[] energies, int environmentMaterialId, int sourceMaterialId, int[] selectedMaterialsIds, Type? homogeneousBuildupType = null, Type? doseFactorType = null, int exposureGeometryId = 1, int organId = 1,  bool exportAllowed = false, int precision = 3)
        {
            if (exportAllowed)
            {
                char colDelimeter = '\t';

                string envMaterialName = materialsService.GetMaterialById(environmentMaterialId).Name;
                int[] materialsIds = CombineIds(sourceMaterialId, selectedMaterialsIds);
                var materialsNames = materialsService.GetMaterialsById(materialsIds).Select(m => m.Name).ToArray();
                var buildupCoefficientsNames = BuildupService.GetBuildupCoefficientsNames(homogeneousBuildupType);

                StringBuilder builder = new();
                builder.AppendLine("\n===== Interpolated Input Data ====="); 
                builder.AppendLine("Designations: BEF - Bremsstrahlung energy flux; MEAF - mass environments absorption factor; MAF - mass attenuation factor.");

                //Заголовок данных по тормозному излучению
                builder.AppendLine(string.Format("{0}{1}{2}", "Energy (MeV)", colDelimeter, "BEF (MeV/s)"));
                //Записываем данные по тормозному излучению
                for (var i = 0; i < energies.Length; i++)
                {
                    builder.AppendLine(string.Format("{0:e" + precision + "}{1}{2:e" + precision + "}", energies[i], colDelimeter, inputClass.PhotonsFluxes[i]));
                }


                builder.AppendLine();
                //Записываем заголовок для коэффициента поглощения в среде
                builder.Append(string.Format("{0}{1}{2}-{3}{4}", "Energy (MeV)", colDelimeter, "MEAF (cm2/g)", envMaterialName, colDelimeter));

                //Записываем заголовки данных по коэффициентам ослабления для каждого материала
                for (var j = 0; j < materialsNames.Length; j++)
                {
                    builder.Append(string.Format("MAF (cm2/g)-{0}{1}", materialsNames[j], colDelimeter));
                }
                builder.Append("\n");

                //Записываем данные по коэффициентам ослабления для каждого материала
                for (var i = 0; i < energies.Length; i++)
                {
                    //Записываем значение для коэффициента поглощения в среде
                    builder.Append(string.Format("{0:e" + precision + "}{1}{2:e" + precision + "}{3}", energies[i], colDelimeter, inputClass.massEnvironmentAbsorptionFactors[i], colDelimeter));

                    //Записываем значения коэффициентов ослабления для каждого материала
                    for (var j = 0; j < materialsNames.Length; j++)
                        builder.Append(string.Format("{0:e" + precision + "}{1}", inputClass.massAttenuationFactors[i][j], colDelimeter));

                    builder.Append("\n");
                }


                if (inputClass.BuildupProcessor != null)
                {
                    builder.AppendLine();

                    builder.Append(string.Format("{0}{1}", "Energy (MeV)", colDelimeter));
                    //Записываем заголовки для коэффициентов формулы фактора накопления для каждого материала
                    for (var j = 0; j < materialsNames.Length; j++)
                        for (var k = 0; k < buildupCoefficientsNames.Length; k++)
                            builder.Append(string.Format("{0}({1}){2}", buildupCoefficientsNames[k], materialsNames[j], colDelimeter));
                    builder.Append("\n");

                    //Записываем значения коэффициентов формулы расчета фактора накопления для каждой энергии
                    for (var i = 0; i < energies.Length; i++)
                    {
                        builder.Append(string.Format("{0:e" + precision + "}{1}", energies[i], colDelimeter));

                        //Записываем значения коэффициентов расчета фактора накопления для каждого материала
                        for (var j = 0; j < materialsNames.Length; j++)
                            for (var k = 0; k < buildupCoefficientsNames.Length; k++)
                                builder.Append(string.Format("{0:e" + precision + "}{1}", inputClass.BuildupFactors[i][j][k], colDelimeter));

                        builder.Append("\n");
                    }
                }

                if (doseFactorType != null)
                {
                    builder.AppendLine();
                    var doseFactors = doseFactorsService.GetDoseConversionFactors(doseFactorType, energies, exposureGeometryId, organId);
                    var doseFactorName = DoseFactorsService.DoseFactors[doseFactorType];
                    var units = DoseFactorsService.GetDoseConversionFactorUnits(doseFactorType);

                    builder.Append(string.Format("{0}{1}{2}\n", "Energy (MeV)", colDelimeter, $"{doseFactorName} ({units})"));
                    for (var i = 0; i < energies.Length; i++)
                    {
                        builder.AppendLine(string.Format("{0:e3}{1}{2:e" + precision + "}{3}", energies[i], colDelimeter, doseFactors[i], colDelimeter));
                    }
                }

                builder.AppendLine("\n===== End of Interpolated Input Data =====");
                builder.AppendLine();

                return builder.ToString();
            }
            return "";
        }
    }
}
