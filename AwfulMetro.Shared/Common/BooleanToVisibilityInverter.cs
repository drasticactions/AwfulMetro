﻿#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AwfulMetro.Common
{
    /// <summary>
    ///     Value converter that translates true to <see cref="Visibility.Visible" /> and false to
    ///     <see cref="Visibility.Collapsed" />.
    /// </summary>
    public sealed class BooleanToVisibilityInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool) value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility) value == Visibility.Visible;
        }
    }
}