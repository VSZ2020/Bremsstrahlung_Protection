﻿using BSP.Common;
using BSP.ViewModels.Tabs;
using System.ComponentModel;
using System.Windows;

namespace BSP.ViewModels
{
    public class BremsstrahlungEnergyYieldVM : BaseViewModel, IDataErrorInfo
    {
        public BremsstrahlungEnergyYieldVM(SourceTabVM vm)
        {
            this.sourceTabVM = vm;
        }

        private readonly SourceTabVM sourceTabVM;

        private float energy;
        private double energyYield;
        private double energyFlux;
        private double photonsFlux;

        /// <summary>
        /// Средняя энергия группы в [МэВ]
        /// </summary>
        public float Energy
        {
            get => energy; set
            {
                if (!sourceTabVM.IsAutoGeneratedModeChecked)
                    EnergyYield = EnergyYield / energy * value;
                energy = value;
                OnChanged();
            }
        }

        /// <summary>
        /// Выход энергии тормозного излучения для группы в [МэВ/распад]
        /// </summary>
        public double EnergyYield
        {
            get => energyYield; set
            {
                energyYield = value;
                OnChanged();
                if (!sourceTabVM.IsAutoGeneratedModeChecked)
                {
                    energyFlux = energyYield * sourceTabVM.SourceTotalActivity;
                    photonsFlux = energy > 0 ? EnergyFlux / energy : 0;
                    OnChanged(nameof(EnergyFlux));
                    OnChanged(nameof(PhotonsFlux));
                }
            }
        }

        /// <summary>
        /// Поток энергии тормозного излучения [МэВ*фотон/с]
        /// </summary>
        public double EnergyFlux
        {
            get => energyFlux;
            set
            {
                energyFlux = value;
                OnChanged();
                if (!sourceTabVM.IsAutoGeneratedModeChecked)
                {
                    energyYield = sourceTabVM.SourceTotalActivity > 0 ? energyFlux / sourceTabVM.SourceTotalActivity : 0;
                    photonsFlux = energy > 0 ? energyFlux / energy : 0;
                    OnChanged(nameof(EnergyYield));
                    OnChanged(nameof(PhotonsFlux));
                }
            }
        }

        /// <summary>
        /// Поток фотонов тормозного излучения [фотон/с]
        /// </summary>
        public double PhotonsFlux
        {
            get => photonsFlux; set
            {
                photonsFlux = value;
                OnChanged();
                if (!sourceTabVM.IsAutoGeneratedModeChecked)
                {
                    energyFlux = photonsFlux * energy;
                    energyYield = sourceTabVM.SourceTotalActivity > 0 ? energyFlux / sourceTabVM.SourceTotalActivity : 0;
                    OnChanged(nameof(EnergyFlux));
                    OnChanged(nameof(EnergyYield));
                }
            }
        }


        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(Energy):
                        if (Energy < 0)
                            error = (Application.Current.TryFindResource("msg_ValidationEnergy") as string) ?? "Incorrect value";
                        break;
                    case nameof(EnergyYield):
                        if (EnergyYield < 0)
                            error = (Application.Current.TryFindResource("msg_ValidationEnergyYield") as string) ?? "Incorrect value";
                        break;
                    case nameof(EnergyFlux):
                        if (EnergyYield < 0)
                            error = (Application.Current.TryFindResource("msg_ValidationEnergyYield") as string) ?? "Incorrect value";
                        break;
                }
                return error;
            }
        }
        public string Error => "";
    }
}
