using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static We_have_doom_at_home.Technical.Common;

namespace We_have_doom_at_home.Entities.Interfaces;

public interface ICurrency : IItem
{
    CurrencyType _CurrencyType { get; }
}
