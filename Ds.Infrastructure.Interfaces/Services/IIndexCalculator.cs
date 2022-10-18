﻿using Ds.Infrastructure.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Infrastructure.Interfaces.Services
{
    public interface IIndexCalculator
    {
        IEnumerable<IDtValue> IndexActivitiesCalculate(IEnumerable<IDtValue> datas, double deltaTime, double minValue = 0, double maxValue = double.MaxValue);
    }
}