using System;
using System.Data;
using BasicData.DataLayer.Models;
using Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicData.Interfaces
{
    public interface IMeasurementUnitsRepository : IRepository
    {
        IEnumerable<MeasurementUnit> MeasurementUnits { get; }
        MeasurementUnit GetUnit(string name);
        MeasurementUnit GetUnit(int id);
    }
}
