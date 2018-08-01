using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LQPackStat {
  public class ColorConverter : IValueConverter {
    public static readonly IValueConverter Instance = new ColorConverter();
    public object Convert(object value,Type t, object o, CultureInfo c)
    {
      int foo = (int)value;
      return
        foo == 0 ? Brushes.Red :
          Brushes.Transparent;  // For foo<1
    }
    public object ConvertBack(object value, Type t, object o, CultureInfo c)

    {
      throw new NotImplementedException();
    }
  }
}
